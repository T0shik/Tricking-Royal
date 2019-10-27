using System.Threading.Tasks;

namespace TrickingRoyal.Services.Email
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}
