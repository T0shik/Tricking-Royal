using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Battles.Application.ViewModels;
using Battles.Enums;
using Battles.Helpers;
using Battles.Models;
using Battles.Rules.Matches.Extensions;
using Battles.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Transmogrify;
using TrickingRoyal.Database;

namespace Battles.Application.Services.Matches.Commands
{
    public class UpdateMatchVideoCommand : IRequest<BaseResponse>
    {
        public int MatchId { get; set; }
        public string UserId { get; set; }
        public string Video { get; set; }
        public string Thumb { get; set; }
        public int Index { get; set; }
    }

    public class UpdateMatchVideoHandler : IRequestHandler<UpdateMatchVideoCommand, BaseResponse>
    {
        private readonly AppDbContext _ctx;
        private readonly Routing _routing;
        private readonly ITranslator _translator;

        public UpdateMatchVideoHandler(
            AppDbContext ctx,
            Routing routing,
            ITranslator translator)
        {
            _ctx = ctx;
            _routing = routing;
            _translator = translator;
        }

        public async Task<BaseResponse> Handle(UpdateMatchVideoCommand request, CancellationToken cancellationToken)
        {
            var match = _ctx.Matches
                            .Include(x => x.MatchUsers)
                            .Include(x => x.Videos)
                            .FirstOrDefault(x => x.Id == request.MatchId);

            if (match == null)
                return BaseResponse.Fail(await _translator.GetTranslation("Match", "NotFound"));

            if (!match.CanUpdate(request.UserId))
                return BaseResponse.Fail(await _translator.GetTranslation("Match", "CantUpdate"));

            var videoToUpdate = GetVideoToUpdate(match, request);

            videoToUpdate.VideoPath =
                CdnUrlHelper.CreateVideoUrl(_routing.Cdn, request.MatchId.ToString(), request.Video);
            videoToUpdate.ThumbPath =
                CdnUrlHelper.CreateThumbUrl(_routing.Cdn, request.MatchId.ToString(), request.Thumb);

            await _ctx.SaveChangesAsync(cancellationToken);

            return BaseResponse.Ok(await _translator.GetTranslation("Match", "Updated"));
        }

        private static Video GetVideoToUpdate(Match match, UpdateMatchVideoCommand request)
        {
            if (match.Mode == Mode.ThreeRoundPass && match.TurnType == TurnType.Blitz)
            {
                return match.Videos
                            .Where(x => x.UserId == request.UserId)
                            .OrderBy(x => x.VideoIndex)
                            .Skip(request.Index)
                            .First();
            }

            var latestIndex = match.Videos.Where(x => x.UserId == request.UserId).Max(x => x.VideoIndex);
            return match.Videos.First(x => x.UserId == request.UserId && x.VideoIndex == latestIndex);
        }
    }
}