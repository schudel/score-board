using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScoreBoard.Domain.Models;
using ScoreBoard.Services.Adapters;

namespace ScoreBoard.Fakes.Infrastructure.Repositories
{
    public class MatchRepositoryFake : IMatchRepository
    {
        private FakeData fakeData;

        public void Initialize(string connectionString)
        {
            fakeData = new FakeData();
        }

        public async Task<Match> GetById(Guid id, bool slim = false)
        {
            await Task.Delay(0);
            return fakeData.FakeMatches.SingleOrDefault(m => m.Id == id);
        }

        public async Task<ICollection<Match>> GetAll(bool slim = false)
        {
            await Task.Delay(0);
            return fakeData.FakeMatches;
        }

        public async Task Add(Match match)
        {
            await Task.Delay(0);
            fakeData.FakeMatches.Add(match);
        }

        public async Task Update(Match match)
        {
            await Task.Delay(0);
            foreach (Match fakeMatch in fakeData.FakeMatches)
            {
                if (fakeMatch.Id == match.Id)
                {
                    fakeMatch.Game = match.Game;
                    fakeMatch.MatchQuality = match.MatchQuality;
                    fakeMatch.Score1 = match.Score1;
                    fakeMatch.Score2 = match.Score2;
                    fakeMatch.StartTime = match.StartTime;
                    fakeMatch.State = match.State;
                    fakeMatch.StopTime = match.StopTime;
                    fakeMatch.Team1 = match.Team1;
                    fakeMatch.Team2 = match.Team2;
                }
            }
        }

        public async Task Remove(Match match)
        {
            await Task.Delay(0);
            fakeData.FakeMatches.Remove(match);
        }

        public async Task<ICollection<Match>> GetMatchesByGameId(Guid id, bool slim = false)
        {
            await Task.Delay(0);
            return fakeData.FakeMatches.Where(m => m.Game.Id == id).ToList();
        }

        public async Task<ICollection<Match>> GetMatchesByPlayerId(Guid id, bool slim = false)
        {
            await Task.Delay(0);
            IList<Match> matches = new List<Match>();
            foreach (Match fakeMatch in fakeData.FakeMatches)
            {
                if (fakeMatch.Team1.Player1.Id == id)
                {
                    matches.Add(fakeMatch);
                }
                else if (fakeMatch.Team1.Player2?.Id == id)
                {
                    matches.Add(fakeMatch);
                }
                else if (fakeMatch.Team2.Player1.Id == id)
                {
                    matches.Add(fakeMatch);
                }
                else if (fakeMatch.Team2.Player2?.Id == id)
                {
                    matches.Add(fakeMatch);
                }
            }
            return matches;
        }

        public async Task<long> Count()
        {
            await Task.Delay(0);
            return fakeData.FakeMatches.Count;
        }

        public async Task<long> CountByGame(Guid id)
        {
            await Task.Delay(0);
            return fakeData.FakeMatches.Count(m => m.Game.Id == id);
        }

        public async Task<long> CountByPlayer(Guid id)
        {
            await Task.Delay(0);
            return fakeData.FakeMatches.Count(m => m.Team1.Player1.Id == id || m.Team1.Player2.Id == id || m.Team2.Player1.Id == id || m.Team2.Player2.Id == id);
        }
    }
}
