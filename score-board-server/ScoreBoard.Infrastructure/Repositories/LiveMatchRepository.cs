using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Linq;
using ScoreBoard.Domain.Models;
using ScoreBoard.Infrastructure.Models;
using ScoreBoard.Services.Adapters;

namespace ScoreBoard.Infrastructure.Repositories
{
    public class LiveMatchRepository : ILiveMatchRepository
    {
        private string dbConnectionString;

        public void Initialize(string connectionString)
        {
            dbConnectionString = connectionString;
        }

        public async Task<LiveMatch> GetById(Guid id)
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            LiveMatchEntity liveMatchEntity = await session.GetAsync<LiveMatchEntity>(id).ConfigureAwait(false);
            return liveMatchEntity?.GetLiveMatch();
        }
        
        public async Task<ICollection<LiveMatch>> GetAll()
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            ICollection<LiveMatchEntity> liveMatchEntities = await session.Query<LiveMatchEntity>().ToListAsync().ConfigureAwait(false);
            return ConvertLiveMatchEntities(liveMatchEntities);
        }

        public async Task<ICollection<LiveMatch>> GetAllDistinct()
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            //ICriteria criteria = session.CreateCriteria(typeof(LiveMatchEntity));
            //criteria.SetProjection(Projections.Distinct(Projections.ProjectionList().Add(Projections.Property("MatchId").As("MatchId"))));
            //criteria.SetResultTransformer(new AliasToBeanResultTransformer(typeof(LiveMatchEntity)));
            ////criteria.AddOrder(Order.Desc("TimeStamp"));
            //IList<LiveMatchEntity> liveMatches = await criteria.ListAsync<LiveMatchEntity>().ConfigureAwait(false);
            // TODO: get distinct query to work
            ICollection<LiveMatchEntity> liveMatchEntities = await session.Query<LiveMatchEntity>().ToListAsync().ConfigureAwait(false);
            IList<LiveMatchEntity> distinctList = new List<LiveMatchEntity>();
            foreach (LiveMatchEntity liveMatchEntity in liveMatchEntities)
            {
                distinctList = AddOrUpdateLiveMatch(distinctList, liveMatchEntity);
            }
            return ConvertLiveMatchEntities(distinctList);
        }

        public async Task Add(LiveMatch liveMatch)
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            using ITransaction transaction = session.BeginTransaction();
            await session.SaveAsync(LiveMatchEntity.Create(liveMatch)).ConfigureAwait(false);
            await transaction.CommitAsync().ConfigureAwait(false);
        }

        public async Task Update(LiveMatch liveMatch)
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            using ITransaction transaction = session.BeginTransaction();
            await session.UpdateAsync(LiveMatchEntity.Create(liveMatch)).ConfigureAwait(false);
            await transaction.CommitAsync().ConfigureAwait(false);
        }

        public async Task Remove(LiveMatch liveMatch)
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            using ITransaction transaction = session.BeginTransaction();
            await session.DeleteAsync(LiveMatchEntity.Create(liveMatch)).ConfigureAwait(false);
            await transaction.CommitAsync().ConfigureAwait(false);
        }

        public async Task<long> Count()
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            const string sql = "select count(distinct lm.Id) from LiveMatchEntity lm";
            return await session.CreateQuery(sql).UniqueResultAsync<long>().ConfigureAwait(false);
        }

        public async Task<ICollection<LiveMatch>> GetByMatchId(Guid matchId)
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            string sql = "from LiveMatchEntity lm where lm.MatchId = '" + matchId + "'";
            ICollection<LiveMatchEntity> liveMatchEntities = await session.CreateQuery(sql).ListAsync<LiveMatchEntity>().ConfigureAwait(false);
            if (liveMatchEntities == null || liveMatchEntities.Count == 0)
            {
                return null;
            }
            return ConvertLiveMatchEntities(liveMatchEntities);
        }

        private static ICollection<LiveMatch> ConvertLiveMatchEntities(ICollection<LiveMatchEntity> liveMatchEntities)
        {
            if (liveMatchEntities == null)
            {
                return null;
            }

            ICollection<LiveMatch> liveMatches = new List<LiveMatch>();
            foreach (LiveMatchEntity liveMatchEntity in liveMatchEntities)
            {
                liveMatches.Add(liveMatchEntity.GetLiveMatch());
            }
            return liveMatches;
        }

        private static IList<LiveMatchEntity> AddOrUpdateLiveMatch(IList<LiveMatchEntity> liveMatchEntities, LiveMatchEntity liveMatchEntity)
        {
            bool newMatch = true;
            for (int i = 0; i < liveMatchEntities.Count; i++)
            {
                int dateCompare = DateTime.Compare(liveMatchEntities[i].TimeStamp, liveMatchEntity.TimeStamp);
                if (liveMatchEntities[i].MatchId != liveMatchEntity.MatchId)
                {
                    continue;
                }
                newMatch = false;
                if (dateCompare >= 0)
                {
                    continue;
                }
                liveMatchEntities[i] = liveMatchEntity;
                break;
            }
            if (newMatch)
            {
                liveMatchEntities.Add(liveMatchEntity);
            }
            return liveMatchEntities;
        }
    }
}
