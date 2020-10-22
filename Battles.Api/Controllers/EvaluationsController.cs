using System.Collections;
using System.Collections.Generic;
using Battles.Api.Infrastructure;
using Battles.Application.Services.Evaluations.Commands;
using Battles.Application.Services.Evaluations.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Battles.Application.ViewModels;
using MediatR;

namespace Battles.Api.Controllers
{
    [Authorize]
    public class EvaluationsController : BaseController
    {
        public EvaluationsController(IMediator mediator)
            : base(mediator) { }

        [HttpGet]
        public Task<IEnumerable<EvaluationViewModel>> GetEvaluations([FromQuery] GetEvaluationsQuery query)
        {
            return Mediator.Send(query);
        }

        [HttpGet("{evaluationId}/results")]
        public Task<DecisionResultViewModel> GetResult([FromRoute] GetVoteResultsQuery query)
        {
            return Mediator.Send(query);
        }

        [HttpGet("count")]
        public Task<int> GetEvaluationCount()
        {
            return Mediator.Send(new GetEvaluationCountQuery());
        }

        [HttpPost("{evaluationId}")]
        public Task<Response> MakeDecision(int id, [FromBody] VoteCommand command)
        {
            command.EvaluationId = id;
            return Mediator.Send(command);
        }
    }
}