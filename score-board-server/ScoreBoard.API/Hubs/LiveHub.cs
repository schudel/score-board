using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ScoreBoard.API.Dtos;
using ScoreBoard.API.Models;
using ScoreBoard.Domain.Enums;
using ScoreBoard.Domain.Models;
using ScoreBoard.Services.UseCases;

namespace ScoreBoard.API.Hubs
{
    public class LiveHub : Hub
    {
        private readonly ILogger<LiveHub> logger;
        private readonly ILiveMatchService liveMatchService;

        public LiveHub(IOptions<AppSettings> appSettings, ILogger<LiveHub> logger, ILiveMatchService liveMatchService)
        {
            AppSettings settings = appSettings.Value;
            this.logger = logger;
            this.liveMatchService = liveMatchService;
            liveMatchService.Initialize(settings.DbConnectionString);
        }

        // ReSharper disable once UnusedMember.Global
        public async Task UpdateMatch(LiveMatchDto liveMatch)
        {
            try
            {
                // send message
                await Clients.All.SendAsync("ReceiveMatchUpdate", liveMatch).ConfigureAwait(false);
                if (MatchState.GetMatchState(liveMatch.State) == MatchState.Done ||
                    MatchState.GetMatchState(liveMatch.State) == MatchState.Aborted)
                {
                    // get all live match entries for this match
                    ICollection<LiveMatch> liveMatches = await liveMatchService.GetByMatchId(liveMatch.MatchId).ConfigureAwait(false);
                    // delete all live match entries for this match
                    foreach (LiveMatch match in liveMatches)
                    {
                        await liveMatchService.Remove(match).ConfigureAwait(false);
                    }
                }
                else
                {
                    await liveMatchService.Add(liveMatch.GetLiveMatch()).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
        }
    }
}
