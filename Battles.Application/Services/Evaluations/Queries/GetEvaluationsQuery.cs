using TrickingRoyal.Database;
using Battles.Application.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Battles.Enums;
using Battles.Models;

namespace Battles.Application.Services.Evaluations.Queries
{
    public class GetEvaluationsQuery : IRequest<IEnumerable<EvaluationViewModel>>
    {
        public string Type { get; set; }
        public string UserId { get; set; }
    }

    public class GetEvaluationsQueryHandler : RequestHandler<GetEvaluationsQuery, IEnumerable<EvaluationViewModel>>
    {
        private readonly AppDbContext _ctx;

        public GetEvaluationsQueryHandler(AppDbContext ctx)
        {
            _ctx = ctx;
            _ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        protected override IEnumerable<EvaluationViewModel> Handle(GetEvaluationsQuery request)
        {
            var evaluations = Query(request.Type, request.UserId)
                ?.Select(EvaluationViewModel.Projection)
                .ToList();

            //todo this is bad can't rely on type to decide if someone can vote or not check decisions'
            return evaluations?.Select(x =>
            {
                x.CanVote = request.Type != "history";
                return x;
            });
        }

        private IQueryable<Evaluation> Query(string type, string userId)
        {
            var query = _ctx.Evaluations
                .Include(e => e.Match)
                .ThenInclude(x => x.MatchUsers)
                .Include(e => e.Match)
                .ThenInclude(x => x.Videos)
                .Include(e => e.Decisions)
                .Where(x => !x.Complete);

            switch (type)
            {
                case "history":
                    return query
                        .Where(x => x.Decisions.Select(y => y.UserId).Contains(userId)
                                    || x.Match.MatchUsers.Select(y => y.UserId).Contains(userId));
                case "complete":
                    return query
                        .Where(x => x.EvaluationType == EvaluationT.Complete)
                        .Where(x => !x.Decisions.Select(y => y.UserId).Contains(userId))
                        .Where(x => !x.Match.MatchUsers.Select(y => y.UserId).Contains(userId));
                case "flag":
                    return query
                        .Where(x => x.EvaluationType == EvaluationT.Flag)
                        .Where(x => !x.Decisions.Select(y => y.UserId).Contains(userId))
                        .Where(x => !x.Match.MatchUsers.Select(y => y.UserId).Contains(userId));
                default:
                    return null;
            }

            ;
        }
    }
}