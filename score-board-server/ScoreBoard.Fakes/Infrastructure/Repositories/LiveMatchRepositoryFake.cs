using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScoreBoard.Domain.Models;
using ScoreBoard.Services.Adapters;

namespace ScoreBoard.Fakes.Infrastructure.Repositories
{
    public class LiveMatchRepositoryFake : ILiveMatchRepository
    {
        private FakeData fakeData;

        public void Initialize(string connectionString)
        {
            fakeData = new FakeData();
        }

        public async Task<LiveMatch> GetById(Guid id)
        {
            await Task.Delay(0);
            return fakeData.FakeLiveMatches.SingleOrDefault(l => l.Id == id);
        }

        public async Task<ICollection<LiveMatch>> GetAll()
        {
            await Task.Delay(0);
            return fakeData.FakeLiveMatches;
        }

        public async Task Add(LiveMatch liveMatch)
        {
            await Task.Delay(0);
            fakeData.FakeLiveMatches.Add(liveMatch);
        }

        public async Task Update(LiveMatch liveMatch)
        {
            await Task.Delay(0);
            foreach (LiveMatch fakeLiveMatch in fakeData.FakeLiveMatches)
            {
                if (fakeLiveMatch.Id == liveMatch.Id)
                {
                    fakeLiveMatch.MatchId = liveMatch.MatchId;
                    fakeLiveMatch.Score1 = liveMatch.Score1;
                    fakeLiveMatch.Score2 = liveMatch.Score2;
                    fakeLiveMatch.State = liveMatch.State;
                    fakeLiveMatch.TimeStamp = liveMatch.TimeStamp;
                }
            }
        }

        public async Task Remove(LiveMatch liveMatch)
        {
            await Task.Delay(0);
            fakeData.FakeLiveMatches.Remove(liveMatch);
        }

        public async Task<long> Count()
        {
            await Task.Delay(0);
            return fakeData.FakeLiveMatches.Count;
        }

        public async Task<ICollection<LiveMatch>> GetByMatchId(Guid matchId)
        {
            await Task.Delay(0);
            return fakeData.FakeLiveMatches.Where(l => l.MatchId == matchId).ToList();
        }

        public async Task<ICollection<LiveMatch>> GetAllDistinct()
        {
            await Task.Delay(0);
            return fakeData.FakeLiveMatches.Distinct().ToList();
        }
    }
}
