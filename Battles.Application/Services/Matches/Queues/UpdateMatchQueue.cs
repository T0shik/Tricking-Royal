using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Battles.Application.Services.Matches.Queues
{
    public class UpdateMatchQueue
    {
        private readonly ILogger<UpdateMatchQueue> _logger;
        private readonly ConcurrentQueue<Func<CancellationToken, Task>> _updateMatchQueue;
        private readonly SemaphoreSlim _signal;

        public UpdateMatchQueue(ILogger<UpdateMatchQueue> logger)
        {
            _logger = logger;
            _updateMatchQueue = new ConcurrentQueue<Func<CancellationToken, Task>>();
            _signal = new SemaphoreSlim(0);
        }

        public void QueueUpdate(Func<CancellationToken, Task> task)
        {
            _updateMatchQueue.Enqueue(task);
            _signal.Release();
        }

        public async Task UpdateMatch(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _updateMatchQueue.TryDequeue(out var task);

            if (!cancellationToken.IsCancellationRequested)
            {
                await task(cancellationToken);
            }
        }
    }
}