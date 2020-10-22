using System.Collections.Generic;
using System.Linq;
using Battles.Application.Extensions;
using Battles.Application.ViewModels.Matches;
using Battles.Enums;
using Battles.Interfaces;
using Battles.Models;
using Battles.Rules.Matches.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TrickingRoyal.Database;
using TrickingRoyal.Database.Extensions;

namespace Battles.Application.Services.Matches.Queries
{
    public class GetMatchesQuery : BaseRequest, IRequest<IEnumerable<object>>
    {
        public string Filter { get; set; }
        public int Index { get; set; }
        public string DisplayName { get; set; }
    }

    public class GetMatchesQueryHandler : RequestHandler<GetMatchesQuery, IEnumerable<object>>
    {
        private readonly AppDbContext _ctx;
        private readonly IOpponentChecker _opponentChecker;

        public GetMatchesQueryHandler(AppDbContext ctx, IOpponentChecker opponentChecker)
        {
            _ctx = ctx;
            _opponentChecker = opponentChecker;
            _ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        protected override IEnumerable<object> Handle(GetMatchesQuery request)
        {
            switch (request.Filter)
            {
                case "hosted":
                    return GetHostedMatches(request.UserId);
                case "open":
                    return GetOpenMatches(request.UserId, request.DisplayName);
                case "history":
                    return GetCompleteMatches(request.UserId, request.DisplayName, request.Index);
                case "active":
                    return GetActiveMatches(request.UserId, request.DisplayName);
                case "spectate":
                    return GetSpectateMatches(request.UserId, request.Index);
                default:
                    return null;
            }
        }

        private IEnumerable<OpenMatchViewModel> GetHostedMatches(string userId)
        {
            return OpenMatchQuery()
                .Where(x => x.MatchUsers
                    .Any(y => y.Role == MatchRole.Host && y.UserId == userId))
                .ToList()
                .Select(x => OpenMatchViewModel.GetOpenMatch(x, userId));
        }

        private IEnumerable<OpenMatchViewModel> GetOpenMatches(string userId, string displayName)
        {
            var matches = OpenMatchQuery()
                .FilterByUser(displayName)
                .ToList();

            _opponentChecker.LoadOpponents(userId);

            return matches.Select(x =>
            {
                var host = x.GetHost();
                var vm = OpenMatchViewModel.GetOpenMatch(x, userId);
                vm.CanJoin = !vm.CanClose && !_opponentChecker.AreOpponents(host.UserId, userId);
                return vm;
            });
        }

        private IQueryable<Match> OpenMatchQuery()
        {
            return _ctx.Matches
                .Include(x => x.MatchUsers)
                .ThenInclude(x => x.User)
                .Where(x => x.Status == Status.Open);
        }

        private IEnumerable<MatchViewModel> GetCompleteMatches(string userId, string displayName, int index)
        {
            return MatchQueryByStatus(Status.Complete)
                .Where(x => x.MatchUsers.Select(y => y.UserId).Contains(userId))
                .FilterByUser(displayName)
                .OrderByDate()
                .GrabSegment(index)
                .ToList()
                .Select(x => MatchViewModel.GetMatch(x, userId));
        }

        private IEnumerable<MatchViewModel> GetActiveMatches(string userId, string displayName)
        {
            return MatchQueryByStatus(Status.Active)
                .Where(x => x.MatchUsers.Select(y => y.UserId).Contains(userId))
                .FilterByUser(displayName)
                .OrderByDate()
                .ToList()
                .Select(x => MatchViewModel.GetMatch(x, userId));
        }

        private IEnumerable<MatchViewModel> GetSpectateMatches(string userId, int index)
        {
            return MatchQuery()
                .Where(x => x.Status != Status.Open && x.Status != Status.Pending)
                .OrderByDate()
                .GrabSegment(index)
                .ToList()
                .Select(x => MatchViewModel.GetMatch(x, userId));
        }

        private IQueryable<Match> MatchQueryByStatus(Status status)
        {
            return MatchQuery()
                .Where(x => x.Status == status);
        }

        private IQueryable<Match> MatchQuery()
        {
            return _ctx.Matches
                .Include(x => x.Videos)
                .Include(x => x.MatchUsers)
                .ThenInclude(x => x.User)
                .Include(x => x.Likes);
        }
    }
}