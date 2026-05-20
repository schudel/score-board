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
    public class MatchRepository : IMatchRepository
    {
        private string dbConnectionString;

        public void Initialize(string connectionString)
        {
            dbConnectionString = connectionString;
        }

        public async Task<Match> GetById(Guid id, bool slim = false)
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            MatchEntity matchEntity = await session.GetAsync<MatchEntity>(id).ConfigureAwait(false);
            return matchEntity?.GetMatch(slim);
        }
        
        public async Task<ICollection<Match>> GetAll(bool slim = false)
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            ICollection<MatchEntity> matchEntities = await session.Query<MatchEntity>().ToListAsync().ConfigureAwait(false);
            return ConvertMatchEntities(matchEntities, slim);
        }
        
        public async Task Add(Match match)
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            using ITransaction transaction = session.BeginTransaction();
            await session.SaveAsync(MatchEntity.Create(match)).ConfigureAwait(false);
            await transaction.CommitAsync().ConfigureAwait(false);
        }

        public async Task Update(Match match)
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            using ITransaction transaction = session.BeginTransaction();
            await session.UpdateAsync(MatchEntity.Create(match)).ConfigureAwait(false);
            await transaction.CommitAsync().ConfigureAwait(false);
        }

        public async Task Remove(Match match)
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            using ITransaction transaction = session.BeginTransaction();
            await session.DeleteAsync(MatchEntity.Create(match)).ConfigureAwait(false);
            await transaction.CommitAsync().ConfigureAwait(false);
        }

        public async Task<ICollection<Match>> GetMatchesByGameId(Guid id, bool slim = false)
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            string sql = "from MatchEntity m where m.Game.Id = '" + id + "'";
            ICollection<MatchEntity> matchEntities = await session.CreateQuery(sql).ListAsync<MatchEntity>().ConfigureAwait(false);
            if (matchEntities == null || matchEntities.Count == 0)
            {
                return null;
            }
            return ConvertMatchEntities(matchEntities, slim);
        }

        public async Task<ICollection<Match>> GetMatchesByPlayerId(Guid id, bool slim = false)
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            string sql = "from MatchEntity m where m.Team1.Player1.Id = '" + id + "' OR ";
            sql += "m.Team1.Player2.Id = '" + id + "' OR ";
            sql += "m.Team2.Player1.Id = '" + id + "' OR ";
            sql += "m.Team2.Player2.Id = '" + id + "'";
            ICollection<MatchEntity> matchEntities = await session.CreateQuery(sql).ListAsync<MatchEntity>().ConfigureAwait(false);
            if (matchEntities == null || matchEntities.Count == 0)
            {
                return null;
            }
            return ConvertMatchEntities(matchEntities, slim);
        }

        public async Task<long> Count()
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            const string sql = "select count(distinct m.Id) from MatchEntity m";
            long count = await session.CreateQuery(sql).UniqueResultAsync<long>().ConfigureAwait(false);
            return count;
        }

        public async Task<long> CountByGame(Guid id)
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            string sql = "select count(distinct m.Id) from MatchEntity m where m.Game.Id ='" + id + "'";
            long count = await session.CreateQuery(sql).UniqueResultAsync<long>().ConfigureAwait(false);
            return count;
        }

        public async Task<long> CountByPlayer(Guid id)
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            string sql = "select count(distinct m.Id) from MatchEntity m where m.Team1.Player1.Id ='" + id + "' OR";
            sql += "m.Team1.Player2.Id = '" + id + "' OR ";
            sql += "m.Team2.Player1.Id = '" + id + "' OR ";
            sql += "m.Team2.Player2.Id = '" + id + "'";
            long count = await session.CreateQuery(sql).UniqueResultAsync<long>().ConfigureAwait(false);
            return count;
        }

        private static ICollection<Match> ConvertMatchEntities(ICollection<MatchEntity> matchEntities, bool slim = false)
        {
            if (matchEntities == null)
            {
                return null;
            }

            ICollection<Match> matches = new List<Match>();
            foreach (MatchEntity matchEntity in matchEntities)
            {
                matches.Add(matchEntity.GetMatch(slim));
            }
            return matches;
        }
    }
}
