using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Battles.Application.Services.Matches.Queues;
using Battles.Application.ViewModels;
using Battles.Rules.Matches.Actions.Update;
using Battles.Rules.Matches.Extensions;
using Battles.Shared;
using IdentityModel.Client;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TrickingRoyal.Database;

namespace Battles.Application.Services.Matches.Commands
{
    public class StartMatchUpdateCommand : UpdateSettings, IRequest<BaseResponse>
    {
        public double Start { get; set; }
        public double End { get; set; }
    }

    public class StartMatchUpdateCommandHandler : IRequestHandler<StartMatchUpdateCommand, BaseResponse>
    {
        private readonly OAuth _oAuth;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMediator _mediator;
        private readonly AppDbContext _ctx;
        private readonly UpdateMatchQueue _matchQueue;

        public StartMatchUpdateCommandHandler(
            OAuth oAuth,
            IHttpClientFactory httpClientFactory,
            IMediator mediator,
            AppDbContext ctx,
            UpdateMatchQueue matchQueue)
        {
            _oAuth = oAuth;
            _httpClientFactory = httpClientFactory;
            _mediator = mediator;
            _ctx = ctx;
            _matchQueue = matchQueue;
        }

        public async Task<BaseResponse> Handle(
            StartMatchUpdateCommand command,
            CancellationToken cancellationToken)
        {
            var match = await _ctx.Matches
                                  .Include(x => x.MatchUsers)
                                  .ThenInclude(x => x.User)
                                  .FirstOrDefaultAsync(x => x.Id == command.MatchId,
                                                       cancellationToken: cancellationToken);

            if (match == null)
            {
                return new BaseResponse("Match not found.", false);
            }

            if (!match.CanGo(command.UserId))
            {
                return new BaseResponse("Not allowed to go.", false);
            }

            _matchQueue.QueueUpdate(ct => UpdateMatch(command, ct));

            return new BaseResponse("Match Update started", true);
        }

        private async Task UpdateMatch(StartMatchUpdateCommand command, CancellationToken cancellationToken)
        {
            await TrimVideo();
            // update match
        }

        private async Task TrimVideo()
        {
            var serverClient = _httpClientFactory.CreateClient();
            var discoveryDocument = await serverClient.GetDiscoveryDocumentAsync(_oAuth.Routing.Server);

            var tokenResponse = await serverClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = discoveryDocument.TokenEndpoint,

                ClientId = "client_id",
                ClientSecret = "client_secret",

                Scope = "ApiOne",
            });

            //retrieve secret data
            var apiClient = _httpClientFactory.CreateClient();

            apiClient.SetBearerToken(tokenResponse.AccessToken);

            var response = await apiClient.GetAsync("https://localhost:44337/secret");

            var content = await response.Content.ReadAsStringAsync();
        }
    }
}