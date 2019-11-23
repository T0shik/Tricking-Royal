using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Battles.Api.Workers
{
    public class WorkerManager : BackgroundService
    {
        private readonly IEnumerable<IWorker> _backgroundServices;

        public WorkerManager(IEnumerable<IWorker> backgroundServices)
        {
            _backgroundServices = backgroundServices;
        }
        
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var services = _backgroundServices.Select(service => LoopService(service, cancellationToken));
            await Task.WhenAll(services);
        }

        public static async Task LoopService(IWorker worker, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await worker.StartAsync(cancellationToken);
            }
        }
    }
}