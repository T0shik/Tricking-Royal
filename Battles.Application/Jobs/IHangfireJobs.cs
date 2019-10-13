using System.Threading.Tasks;

namespace Battles.Application.Jobs
{
    public interface IHangfireJobs
    {
        Task CloseExpiredMatches();
        Task CloseExpiredEvaluations();
    }
}