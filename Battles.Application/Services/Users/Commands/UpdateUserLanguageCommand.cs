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
    public class UpdateUserLanguageCommand : IRequest<BaseResponse>
    {
        [Required] public string Language { get; set; }

        public string UserId { get; set; }
    }

    public class UpdateUserLanguageCommandHandler : IRequestHandler<UpdateUserLanguageCommand, BaseResponse>
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

        public async Task<BaseResponse> Handle(UpdateUserLanguageCommand command, CancellationToken cancellationToken)
        {
            var translationContext = await _library.GetContext();

            var user = _dbContext.UserInformation.FirstOrDefault(x => x.Id == command.UserId);

            if (user == null)
                return BaseResponse.Fail(translationContext.Read("User", "NotFound"));

            user.Language = command.Language;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return BaseResponse.Ok(translationContext.Read("User", "LanguageUpdated"));
        }
    }
}