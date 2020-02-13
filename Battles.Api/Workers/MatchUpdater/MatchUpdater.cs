using System.Threading;
using System.Threading.Tasks;
using Battles.Application.Services.Matches;

namespace Battles.Api.Workers.MatchUpdater
{
    public class MatchUpdater : IWorker
    {
        private readonly UpdateMatchQueue _queue;

        public MatchUpdater(UpdateMatchQueue queue)
        {
            _queue = queue;
        }

        public Task StartAsync(CancellationToken cancellationToken) => _queue.UpdateMatch(cancellationToken);
    }
}