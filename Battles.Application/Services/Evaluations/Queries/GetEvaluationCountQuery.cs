﻿using TrickingRoyal.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Battles.Application.Services.Evaluations.Queries
{
    public class GetEvaluationCountQuery : IRequest<int>
    {
        public string UserId { get; set; }
    }

    public class GetEvaluationCountQueryHandler : RequestHandler<GetEvaluationCountQuery, int>
    {
        private AppDbContext _ctx;

        public GetEvaluationCountQueryHandler(AppDbContext ctx)
        {
            _ctx = ctx;
            _ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        protected override int Handle(GetEvaluationCountQuery request) =>
            _ctx.Evaluations
               .Include(e => e.Decisions)
               .Where(x => !x.Complete)
               .Where(x => !x.Decisions.Select(y => y.UserId).Contains(request.UserId))
               .Count();
    }
}
