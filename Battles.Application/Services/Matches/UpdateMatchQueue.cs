using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Battles.Application.Services.Matches.Commands;
using Battles.Application.SubServices.VideoConversion;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Battles.Application.Services.Matches
{
    public class UpdateMatchQueue
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly VideoConverter _videoConverter;
        private readonly ILogger<UpdateMatchQueue> _logger;
        private readonly ConcurrentQueue<Func<CancellationToken, Task>> _updateMatchQueue;
        private readonly SemaphoreSlim _signal;

        public UpdateMatchQueue(
            IServiceProvider serviceProvider,
            VideoConverter videoConverter,
            ILogger<UpdateMatchQueue> logger)
        {
            _serviceProvider = serviceProvider;
            _videoConverter = videoConverter;
            _logger = logger;
            _updateMatchQueue = new ConcurrentQueue<Func<CancellationToken, Task>>();
            _signal = new SemaphoreSlim(0);
        }

        public void QueueUpdate(StartMatchUpdateCommand command)
        {
            _updateMatchQueue.Enqueue(ct => UpdateMatch(command, ct));
            _signal.Release();
        }

        private async Task UpdateMatch(StartMatchUpdateCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting video conversion for match ({0})", command.MatchId);

            using (var scope = _serviceProvider.CreateScope())
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                try
                {
                    var trimResult = await _videoConverter.TrimVideoAsync(command.MatchId.ToString(),
                                                                          command.Video,
                                                                          command.Start,
                                                                          command.End);

                    var updateCommand = new UpdateMatchCommand
                    {
                        MatchId = command.MatchId,
                        UserId = command.UserId,
                        Thumb = trimResult.Thumb,
                        Video = trimResult.Video,
                        Move = command.Move,
                    };

                    await mediator.Send(updateCommand, cancellationToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed to update match ({0}), stopping update.", command.MatchId);
                    var stopCommand = new StopMatchUpdateCommand {UserId = command.UserId, MatchId = command.MatchId};
                    await mediator.Send(stopCommand, cancellationToken);
                }

                var realtimeNotifications = scope.ServiceProvider.GetRequiredService<IMatchUpdaterNotifications>();
                await realtimeNotifications.NotifyMatchUpdated(command.UserId, command.MatchId);
            }
        }

        public async Task UpdateMatch(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _updateMatchQueue.TryDequeue(out var task);

            if (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await task(cancellationToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Match Update Task Failed, Preserving Queue Service");
                }
            }
        }
    }
}