using TrickingRoyal.Database;
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Battles.Application.ViewModels;
using Battles.Helpers;
using Battles.Shared;
using Transmogrify;

namespace Battles.Application.Services.Users.Commands
{
    public class UpdateUserLanguageCommand : BaseRequest, IRequest<Response>
    {
        [Required] public string Language { get; set; }
    }

    public class UpdateUserLanguageCommandHandler : IRequestHandler<UpdateUserLanguageCommand, Response>
    {
        private readonly AppDbContext _dbContext;
        private readonly Routing _routing;
        private readonly Library _library;

        public UpdateUserLanguageCommandHandler(
            AppDbContext dbContext,
            Routing routing,
            Library library)
        {
            _dbContext = dbContext;
            _routing = routing;
            _library = library;
        }

        public async Task<Response> Handle(UpdateUserLanguageCommand command, CancellationToken cancellationToken)
        {
            var translationContext = await _library.GetContext();

            var user = _dbContext.UserInformation.FirstOrDefault(x => x.Id == command.UserId);

            if (user == null)
                return Response.Fail(translationContext.Read("User", "NotFound"));

            user.Language = command.Language;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Response.Ok(translationContext.Read("User", "LanguageUpdated"));
        }
    }
}