using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScoreBoard.Domain.Enums;
using ScoreBoard.Domain.Models;
using ScoreBoard.Services.Adapters;
using ScoreBoard.Skills;
using Player = ScoreBoard.Domain.Models.Player;
using Rating = ScoreBoard.Domain.Models.Rating;
using Team = ScoreBoard.Domain.Models.Team;

namespace ScoreBoard.Services.UseCases
{
    public class RatingService : IRatingService
    {
        private readonly IRatingRepository ratingRepository;
        private readonly IMatchRepository matchRepository;
        private readonly ITeamService teamService;
        private readonly IRatingHistoryService ratingHistoryService;

        public RatingService(IRatingRepository ratingRepository, IMatchRepository matchRepository, ITeamService teamService, IRatingHistoryService ratingHistoryService)
        {
            this.ratingRepository = ratingRepository;
            this.matchRepository = matchRepository;
            this.teamService = teamService;
            this.ratingHistoryService = ratingHistoryService;
        }

        public void Initialize(string dbConnectionString)
        {
            ratingRepository.Initialize(dbConnectionString);
            matchRepository.Initialize(dbConnectionString);
            teamService.Initialize(dbConnectionString);
            ratingHistoryService.Initialize(dbConnectionString);
        }

        public async Task<Rating> GetById(string id, bool slim = false)
        {
            if (Guid.TryParse(id, out Guid guid))
            {
                return await ratingRepository.GetById(guid, slim).ConfigureAwait(false);
            }
            throw new Exception("No valid Id: \"" + id + "\"");
        }

        public async Task<ICollection<Rating>> GetAll(bool slim = false) => await ratingRepository.GetAll(slim).ConfigureAwait(false);

        public async Task Add(Rating rating)
        {
            await ratingRepository.Add(rating).ConfigureAwait(false);
        }

        public async Task Update(string id, Rating rating)
        {
            if (!Guid.TryParse(id, out Guid guid))
            {
                throw new Exception("No valid Id: \"" + id + "\"");
            }
            Rating r = await ratingRepository.GetById(guid).ConfigureAwait(false);
            if (r == null)
            {
                throw new Exception("Rating not found");
            }
            await ratingRepository.Update(r).ConfigureAwait(false);
        }

        public async Task CalcAllRatings(bool saveToDb = false)
        {
            ICollection<Match> matches = await matchRepository.GetAll().ConfigureAwait(false);
            // Order by start time
            matches = matches.OrderBy(m => m.StartTime).ToList();
            // Remove all existing Ratings
            ICollection<Rating> extRatings = await ratingRepository.GetAll(true).ConfigureAwait(false);
            foreach (Rating rating in extRatings)
            {
                await ratingRepository.Remove(rating).ConfigureAwait(false);
            }
            // Remove all Rating Histories
            ICollection<RatingHistory> ratingHistories = await ratingHistoryService.GetAll(true).ConfigureAwait(false);
            foreach (RatingHistory ratingHistory in ratingHistories)
            {
                await ratingHistoryService.Remove(ratingHistory).ConfigureAwait(false);
            }

            IDictionary<Guid, Rating> existingRatings = new Dictionary<Guid, Rating>();
            foreach (Match match in matches)
            {
                // only matches which are done should be considered
                if (match.State != MatchState.Done)
                {
                    continue;
                }

                if (!existingRatings.ContainsKey(match.Team1.Player1.Id))
                {
                    existingRatings.Add(match.Team1.Player1.Id, GetInitialRating(match.Game, match.Team1.Player1));
                }
                if (match.Team1.Player2 != null && !existingRatings.ContainsKey(match.Team1.Player2.Id))
                {
                    existingRatings.Add(match.Team1.Player2.Id, GetInitialRating(match.Game, match.Team1.Player2));
                }
                if (!existingRatings.ContainsKey(match.Team2.Player1.Id))
                {
                    existingRatings.Add(match.Team2.Player1.Id, GetInitialRating(match.Game, match.Team2.Player1));
                }
                if (match.Team2.Player2 != null && !existingRatings.ContainsKey(match.Team2.Player2.Id))
                {
                    existingRatings.Add(match.Team2.Player2.Id, GetInitialRating(match.Game, match.Team2.Player2));
                }

                // Calc Rating for each Match
                IDictionary<Guid, Rating> ratings = await CalcRating(match, existingRatings, saveToDb).ConfigureAwait(false);
                foreach (var (key, rating) in ratings)
                {
                    existingRatings[key] = rating;
                }
            }
        }

        public async Task<IDictionary<Guid, Rating>> CalcRating(Match match, IDictionary<Guid, Rating> existingRatings = null, bool saveToDb = false)
        {
            // Get Game Info from Game
            GameInfo gameInfo = GetTrueSkillGameInfo(match.Game);

            // Team 1
            Team<Guid> team1 = await GetTrueSkillTeam(match.Team1, match.Game, existingRatings).ConfigureAwait(false);

            // Team 2
            Team<Guid> team2 = await GetTrueSkillTeam(match.Team2, match.Game, existingRatings).ConfigureAwait(false);
            
            IEnumerable<IDictionary<Guid, Skills.Rating>> teams = Teams.Concat(team1, team2);
            int rank1 = 1;
            int rank2 = 1;
            if (match.Winner == match.Team1)
            {
                rank1 = 1;
                rank2 = 2;
            }
            else if (match.Winner == match.Team2)
            {
                rank1 = 2;
                rank2 = 1;
            }
            // This is the key line. We ask the calculator to calculate new ratings
            // Pay careful attention to the numbers at the end. This indicates that
            // team1 came in first place and team2 came in second place. TrueSkill
            // is flexible and allows scenarios such as team1 and team2 drawing which
            // could be represented as "1,1" since they both came in first place.
            IDictionary<Guid, Skills.Rating> newRatings = TrueSkillCalculator.CalculateNewRatings(gameInfo, teams, rank1, rank2);

            IDictionary<Guid, Rating> ratings = new Dictionary<Guid, Rating>();
            // The result of the calculation is a dictionary mapping the players to
            // their new rating. Here we get the ratings out for each player
            if (match.Team1.Player1 != null)
            {
                Guid id = match.Team1.Player1.Id;
                Skills.Rating r = newRatings[id];
                (Rating rating, bool isNew) = await GetRating(r, match.Game, match.Team1.Player1).ConfigureAwait(false);
                ratings.Add(id, rating);
                if (saveToDb)
                {
                    if (isNew)
                    {
                        await ratingRepository.Add(rating).ConfigureAwait(false);
                    }
                    else
                    {
                        await ratingRepository.Update(rating).ConfigureAwait(false);
                    }
                }
            }
            if (match.Team1.Player2 != null)
            {
                Guid id = match.Team1.Player2.Id;
                Skills.Rating r = newRatings[id];
                (Rating rating, bool isNew) = await GetRating(r, match.Game, match.Team1.Player2).ConfigureAwait(false);
                ratings.Add(id, rating);
                if (saveToDb)
                {
                    if (isNew)
                    {
                        await ratingRepository.Add(rating).ConfigureAwait(false);
                    }
                    else
                    {
                        await ratingRepository.Update(rating).ConfigureAwait(false);
                    }
                }
            }

            // Ratings Team 2
            if (match.Team2.Player1 != null)
            {
                Guid id = match.Team2.Player1.Id;
                Skills.Rating r = newRatings[id];
                (Rating rating, bool isNew) = await GetRating(r, match.Game, match.Team2.Player1).ConfigureAwait(false);
                ratings.Add(id, rating);
                if (saveToDb)
                {
                    if (isNew)
                    {
                        await ratingRepository.Add(rating).ConfigureAwait(false);
                    }
                    else
                    {
                        await ratingRepository.Update(rating).ConfigureAwait(false);
                    }
                }
            }
            if (match.Team2.Player2 != null)
            {
                Guid id = match.Team2.Player2.Id;
                Skills.Rating r = newRatings[id];
                (Rating rating, bool isNew) = await GetRating(r, match.Game, match.Team2.Player2).ConfigureAwait(false);
                ratings.Add(id, rating);
                if (saveToDb)
                {
                    if (isNew)
                    {
                        await ratingRepository.Add(rating).ConfigureAwait(false);
                    }
                    else
                    {
                        await ratingRepository.Update(rating).ConfigureAwait(false);
                    }
                }
            }
            foreach (KeyValuePair<Guid, Rating> keyValuePair in ratings)
            {
                if (!saveToDb)
                {
                    continue;
                }
                // save Rating History
                RatingHistory ratingHistory = GetRatingHistory(keyValuePair.Value, match);
                await ratingHistoryService.Add(ratingHistory).ConfigureAwait(false);
            }
            return ratings;
        }

        public async Task<double> CalcMatchQuality(Match match, IDictionary<Guid, Rating> existingRatings = null, bool saveToDb = false)
        {
            GameInfo gameInfo = GameInfo.DefaultGameInfo;

            // Team 1
            Team<Guid> team1 = await GetTrueSkillTeam(match.Team1, match.Game, existingRatings).ConfigureAwait(false);

            // Team 2
            Team<Guid> team2 = await GetTrueSkillTeam(match.Team2, match.Game, existingRatings).ConfigureAwait(false);

            // Before we know the actual results of the game, we can ask the 
            // calculator for what it perceives as the quality of the match (higher
            // means more fair/equitable)
            IEnumerable<IDictionary<Guid, Skills.Rating>> teams = Teams.Concat(team1, team2);
            double matchQuality = TrueSkillCalculator.CalculateMatchQuality(gameInfo, teams);
            if (!saveToDb)
            {
                return matchQuality;
            }
            match.MatchQuality = matchQuality;
            await teamService.SetExistingTeamsIfAvailable(match).ConfigureAwait(false);
            await matchRepository.Update(match).ConfigureAwait(false);
            return matchQuality;
        }
        
        public async Task AddDefaultRating(Game game, Player player)
        {
            if (game == null || player == null)
            {
                return;
            }
            var (rating, isNew) = await GetRating(game, player).ConfigureAwait(false);
            if (isNew)
            {
                await ratingRepository.Add(rating).ConfigureAwait(false);
            }
        }

        public void SetDefaultGameRatingValues(Game game)
        {
            GameInfo gameInfo = GameInfo.DefaultGameInfo;
            game.Beta = gameInfo.Beta;
            game.DrawProbability = gameInfo.DrawProbability;
            game.DynamicsFactor = gameInfo.DynamicsFactor;
            game.InitialMean = gameInfo.InitialMean;
            game.InitialStandardDeviation = gameInfo.InitialStandardDeviation;
            game.InitialConservativeRating = gameInfo.DefaultRating.ConservativeRating;
        }

        public async Task<ICollection<Rating>> GetRatingsByPlayerId(string id)
        {
            if (Guid.TryParse(id, out Guid guid))
            {
                return await ratingRepository.GetByPlayerId(guid).ConfigureAwait(false);
            }
            throw new Exception("No valid Id: \"" + id + "\"");
        }

        public async Task CalcAllMatchQualities()
        {
            ICollection<Match> matches = await matchRepository.GetAll(true).ConfigureAwait(false);
            // Order by start time
            matches = matches.OrderBy(m => m.StartTime).ToList();

            IDictionary<Guid, Rating> existingRatings = new Dictionary<Guid, Rating>();
            foreach (Match match in matches)
            {
                // only matches which are done should be considered
                if (match.State != MatchState.Done)
                {
                    continue;
                }

                if (!existingRatings.ContainsKey(match.Team1.Player1.Id))
                {
                    existingRatings.Add(match.Team1.Player1.Id, GetInitialRating(match.Game, match.Team1.Player1));
                }
                if (match.Team1.Player2 != null && !existingRatings.ContainsKey(match.Team1.Player2.Id))
                {
                    existingRatings.Add(match.Team1.Player2.Id, GetInitialRating(match.Game, match.Team1.Player2));
                }
                if (!existingRatings.ContainsKey(match.Team2.Player1.Id))
                {
                    existingRatings.Add(match.Team2.Player1.Id, GetInitialRating(match.Game, match.Team2.Player1));
                }
                if (match.Team2.Player2 != null && !existingRatings.ContainsKey(match.Team2.Player2.Id))
                {
                    existingRatings.Add(match.Team2.Player2.Id, GetInitialRating(match.Game, match.Team2.Player2));
                }

                // Calc Rating for each Match
                IDictionary<Guid, Rating> ratings = await CalcRating(match, existingRatings).ConfigureAwait(false);
                foreach (var (guid, rating) in ratings)
                {
                    existingRatings[guid] = rating;
                }

                // Calc Quality for each Match
                await CalcMatchQuality(match, ratings, true).ConfigureAwait(false);
            }
        }

        public async Task<ICollection<RatingHistory>> CalcAllRatingHistories(bool saveToDb = false)
        {
            ICollection<Match> matches = await matchRepository.GetAll(true).ConfigureAwait(false);
            // Order by start time
            matches = matches.OrderBy(m => m.StartTime).ToList();

            // Remove all existing Rating Histories
            if (saveToDb)
            {
                ICollection<RatingHistory> allRatingHistories = await ratingHistoryService.GetAll().ConfigureAwait(false);
                foreach (RatingHistory ratingHistory in allRatingHistories)
                {
                    await ratingHistoryService.Remove(ratingHistory).ConfigureAwait(false);
                }
            }

            IDictionary<Guid, Rating> existingRatings = new Dictionary<Guid, Rating>();
            ICollection<RatingHistory> ratingHistories = new List<RatingHistory>();
            foreach (Match match in matches)
            {
                // only matches which are done should be considered
                if (match.State != MatchState.Done)
                {
                    continue;
                }
                // Calc Rating for each Match
                if (!existingRatings.ContainsKey(match.Team1.Player1.Id))
                {
                    existingRatings.Add(match.Team1.Player1.Id, GetInitialRating(match.Game, match.Team1.Player1));
                }
                if (match.Team1.Player2 != null && !existingRatings.ContainsKey(match.Team1.Player2.Id))
                {
                    existingRatings.Add(match.Team1.Player2.Id, GetInitialRating(match.Game, match.Team1.Player2));
                }
                if (!existingRatings.ContainsKey(match.Team2.Player1.Id))
                {
                    existingRatings.Add(match.Team2.Player1.Id, GetInitialRating(match.Game, match.Team2.Player1));
                }
                if (match.Team2.Player2 != null && !existingRatings.ContainsKey(match.Team2.Player2.Id))
                {
                    existingRatings.Add(match.Team2.Player2.Id, GetInitialRating(match.Game, match.Team2.Player2));
                }
                IDictionary<Guid, Rating> ratings = await CalcRating(match, existingRatings).ConfigureAwait(false);
                foreach (var (guid, rating) in ratings)
                {
                    existingRatings[guid] = rating;

                    RatingHistory ratingHistory = GetRatingHistory(rating, match);
                    ratingHistories.Add(ratingHistory);
                    if (saveToDb)
                    {
                        await ratingHistoryService.Add(ratingHistory).ConfigureAwait(false);
                    }
                }
            }
            return ratingHistories;
        }

        private static Skills.Rating GetTrueSkillRating(Game game, Rating rating = null)
        {
            if (rating == null)
            {
                return game == null ? GameInfo.DefaultGameInfo.DefaultRating : new Skills.Rating(game.InitialMean, game.InitialStandardDeviation, game.InitialConservativeRating);
            }
            return new Skills.Rating(rating.Mean, rating.StandardDeviation, rating.ConservativeRating);
        }

        private static GameInfo GetTrueSkillGameInfo(Game game = null)
        {
            return game == null ? 
                GameInfo.DefaultGameInfo : 
                new GameInfo(game.InitialMean, game.InitialStandardDeviation, game.Beta, game.DynamicsFactor, game.DrawProbability);
        }

        private async Task<Team<Guid>> GetTrueSkillTeam(Team team, Game game = null, IDictionary<Guid, Rating> existingRatings = null)
        {
            if (team == null)
            {
                return null;
            }
            // Players
            Player<Guid> p1 = null;
            Player<Guid> p2 = null;
            if (team.Player1 != null)
            {
                p1 = new Player<Guid>(team.Player1.Id);
            }
            if (team.Player2 != null)
            {
                p2 = new Player<Guid>(team.Player2.Id);
            }
            // Team
            Team<Guid> t = new Team<Guid>();
            if (p1 != null)
            {
                if (existingRatings == null)
                {
                    Tuple<Rating, bool> tuple = await GetRating(game, team.Player1).ConfigureAwait(false);
                    t.AddPlayer(p1.Id, GetTrueSkillRating(game, tuple.Item1));
                }
                else
                {
                    Rating existingRating = existingRatings[team.Player1.Id];
                    t.AddPlayer(p1.Id, GetTrueSkillRating(game, existingRating));
                }
            }
            if (p2 != null)
            {
                if (existingRatings == null)
                {
                    Tuple<Rating, bool> tuple = await GetRating(game, team.Player2).ConfigureAwait(false);
                    t.AddPlayer(p2.Id, GetTrueSkillRating(game, tuple.Item1));
                }
                else
                {
                    Rating existingRating = existingRatings[team.Player2.Id];
                    t.AddPlayer(p2.Id, GetTrueSkillRating(game, existingRating));
                }
            }
            return t;
        }

        private async Task<Tuple<Rating, bool>> GetRating(Skills.Rating r, Game game, Player player)
        {
            bool isNewRating = false;
            if (r == null || game == null || player == null)
            {
                return null;
            }
            Rating existingRating = await ratingRepository.GetByGameIdAndPlayerId(game.Id, player.Id, true).ConfigureAwait(false);
            Rating rating = existingRating;
            if (rating == null)
            {
                isNewRating = true;
                rating = new Rating
                {
                    Game = game,
                    Player = player
                };
            }
            rating.ConservativeRating = r.ConservativeRating;
            rating.Mean = r.Mean;
            rating.StandardDeviation = r.StandardDeviation;
            return new Tuple<Rating, bool>(rating, isNewRating);
        }

        private async Task<Tuple<Rating, bool>> GetRating(Game game, Player player)
        {
            if (game == null || player == null)
            {
                return null;
            }
            Rating existingRating = await ratingRepository.GetByGameIdAndPlayerId(game.Id, player.Id, true).ConfigureAwait(false);
            if (existingRating != null)
            {
                return new Tuple<Rating, bool>(existingRating, false);
            }
            Rating rating = GetInitialRating(game, player);
            return new Tuple<Rating, bool>(rating, true);
        }

        private static Rating GetInitialRating(Game game, Player player)
        {
            Rating rating = new Rating
            {
                Game = game,
                Player = player,
                ConservativeRating = game.InitialConservativeRating,
                Mean = game.InitialMean,
                StandardDeviation = game.InitialStandardDeviation
            };
            return rating;
        }

        private static RatingHistory GetRatingHistory(Rating rating, Match match)
        {
            RatingHistory ratingHistory = new RatingHistory
            {
                Player = rating.Player,
                MatchId = match.Id,
                GameId = match.Game.Id,
                ConservativeRating = rating.ConservativeRating,
                Mean = rating.Mean,
                StandardDeviation = rating.StandardDeviation
            };
            if (match.StartTime != null)
            {
                ratingHistory.DateTime = (DateTime)match.StartTime;
            }
            return ratingHistory;
        }
    }
}
