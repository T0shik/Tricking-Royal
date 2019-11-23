using System.Threading;
using System.Threading.Tasks;

namespace Battles.Api.Workers
{
    public interface IWorker
    {
        Task StartAsync(CancellationToken cancellationToken);
    }
}