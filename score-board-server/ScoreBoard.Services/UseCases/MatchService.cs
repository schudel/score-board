using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScoreBoard.Domain.Enums;
using ScoreBoard.Domain.Models;
using ScoreBoard.Services.Adapters;

namespace ScoreBoard.Services.UseCases
{
    public class MatchService : IMatchService
    {
        private readonly IMatchRepository matchRepository;
        private readonly IRatingService ratingService;
        private readonly ITeamService teamService;

        public MatchService(IMatchRepository matchRepository, IRatingService ratingService, ITeamService teamService)
        {
            this.matchRepository = matchRepository;
            this.ratingService = ratingService;
            this.teamService = teamService;
        }

        public void Initialize(string dbConnectionString)
        {
            matchRepository.Initialize(dbConnectionString);
            ratingService.Initialize(dbConnectionString);
            teamService.Initialize(dbConnectionString);
        }

        public async Task<Match> GetById(string id, bool slim = false)
        {
            if (Guid.TryParse(id, out Guid guid))
            {
                return await matchRepository.GetById(guid, slim).ConfigureAwait(false);
            }
            throw new Exception("No valid Id: \"" + id + "\"");
        }

        public async Task<ICollection<Match>> GetAll(bool slim = false) => await matchRepository.GetAll(slim).ConfigureAwait(false);

        public async Task Add(Match match, bool calcQuality, bool updateRanking)
        {
            await teamService.SetExistingTeamsIfAvailable(match).ConfigureAwait(false);
            await matchRepository.Add(match).ConfigureAwait(false);
            IDictionary<Guid, Rating> ratings = null;
            if (updateRanking && match.State == MatchState.Done)
            {
                if (string.IsNullOrEmpty(match.Game.Name))
                {
                    // load the complete game again
                    match = await matchRepository.GetById(match.Id).ConfigureAwait(false);
                }
                ratings = await ratingService.CalcRating(match, null, true).ConfigureAwait(false);
            }
            if (calcQuality && match.State == MatchState.Done)
            {
                if (string.IsNullOrEmpty(match.Game.Name))
                {
                    // load the complete game again
                    match = await matchRepository.GetById(match.Id).ConfigureAwait(false);
                }
                if (ratings != null)
                {
                    await ratingService.CalcMatchQuality(match, ratings, true).ConfigureAwait(false);
                }
                else
                {
                    await ratingService.CalcMatchQuality(match, null, true).ConfigureAwait(false);
                }
            }
        }

        public async Task Update(string id, Match match, bool calcQuality, bool updateRanking)
        {
            if (!Guid.TryParse(id, out Guid guid))
            {
                throw new Exception("No valid Id: \"" + id + "\"");
            }
            Match m = await matchRepository.GetById(guid).ConfigureAwait(false);
            if (m == null)
            {
                throw new Exception("Match not found");
            }
            await teamService.SetExistingTeamsIfAvailable(match).ConfigureAwait(false);
            await matchRepository.Update(match).ConfigureAwait(false);
            IDictionary<Guid, Rating> ratings = null;
            if (updateRanking && match.State == MatchState.Done)
            {
                if (string.IsNullOrEmpty(match.Game.Name))
                {
                    // load the complete game again
                    match = await matchRepository.GetById(match.Id).ConfigureAwait(false);
                }
                ratings = await ratingService.CalcRating(match, null, true).ConfigureAwait(false);
            }
            if (calcQuality && match.State == MatchState.Done)
            {
                if (string.IsNullOrEmpty(match.Game.Name))
                {
                    // load the complete game again
                    match = await matchRepository.GetById(match.Id).ConfigureAwait(false);
                }
                if (ratings != null)
                {
                    await ratingService.CalcMatchQuality(match, ratings, true).ConfigureAwait(false);
                }
                else
                {
                    await ratingService.CalcMatchQuality(match, null, true).ConfigureAwait(false);
                }
            }
        }

        public async Task Update(Match match, bool calcQuality, bool updateRanking)
        {
            await teamService.SetExistingTeamsIfAvailable(match).ConfigureAwait(false);
            await matchRepository.Update(match).ConfigureAwait(false);
            IDictionary<Guid, Rating> ratings = null;
            if (updateRanking && match.State == MatchState.Done)
            {
                ratings = await ratingService.CalcRating(match, null, true).ConfigureAwait(false);
            }
            if (calcQuality && match.State == MatchState.Done)
            {
                if (ratings != null)
                {
                    await ratingService.CalcMatchQuality(match, ratings, true).ConfigureAwait(false);
                }
                else
                {
                    await ratingService.CalcMatchQuality(match, null, true).ConfigureAwait(false);
                }
            }
        }

        public async Task Remove(Match match)
        {
            await matchRepository.Remove(match).ConfigureAwait(false);
        }

        public async Task Remove(string id)
        {
            if (!Guid.TryParse(id, out Guid guid))
            {
                throw new Exception("No valid Id: \"" + id + "\"");
            }
            Match match = await matchRepository.GetById(guid).ConfigureAwait(false);
            await matchRepository.Remove(match).ConfigureAwait(false);
            throw new Exception("No valid Id: \"" + id + "\"");
        }

        public async Task<ICollection<Match>> GetMatchesByGameId(string id, bool slim = false)
        {
            if (Guid.TryParse(id, out Guid guid))
            {
                return await matchRepository.GetMatchesByGameId(guid, slim).ConfigureAwait(false);
            }
            throw new Exception("No valid Id: \"" + id + "\"");
        }

        public async Task<ICollection<Match>> GetMatchesByPlayerId(string id, bool slim = false)
        {
            if (Guid.TryParse(id, out Guid guid))
            {
                return await matchRepository.GetMatchesByPlayerId(guid, slim).ConfigureAwait(false);
            }
            throw new Exception("No valid Id: \"" + id + "\"");
        }

        public async Task<long> Count() => await matchRepository.Count().ConfigureAwait(false);

        public async Task<long> CountByGame(string id)
        {
            if (Guid.TryParse(id, out Guid guid))
            {
                return await matchRepository.CountByGame(guid).ConfigureAwait(false);
            }
            throw new Exception("No valid Id: \"" + id + "\"");
        }

        public async Task<long> CountByPlayer(string id)
        {
            if (Guid.TryParse(id, out Guid guid))
            {
                return await matchRepository.CountByPlayer(guid).ConfigureAwait(false);
            }
            throw new Exception("No valid Id: \"" + id + "\"");
        }
    }
}
