using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Linq;
using ScoreBoard.Domain.Models;
using ScoreBoard.Infrastructure.Models;
using ScoreBoard.Services.Adapters;

namespace ScoreBoard.Infrastructure.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        private string dbConnectionString;
        
        public void Initialize(string connectionString)
        {
            dbConnectionString = connectionString;
        }

        public async Task<Player> GetById(Guid id, bool slim = false)
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            PlayerEntity playerEntity = await session.GetAsync<PlayerEntity>(id).ConfigureAwait(false);
            return playerEntity?.GetPlayer(slim);
        }

        public async Task<ICollection<Player>> GetAll()
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            ICollection<PlayerEntity> playerEntities = await session.Query<PlayerEntity>().ToListAsync().ConfigureAwait(false);
            return ConvertPlayerEntities(playerEntities);
        }

        public async Task Add(Player player)
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            using ITransaction transaction = session.BeginTransaction();
            await session.SaveAsync(PlayerEntity.Create(player)).ConfigureAwait(false);
            await transaction.CommitAsync().ConfigureAwait(false);
        }

        public async Task Update(Player player)
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            using ITransaction transaction = session.BeginTransaction();
            await session.UpdateAsync(PlayerEntity.Create(player)).ConfigureAwait(false);
            await transaction.CommitAsync().ConfigureAwait(false);
        }

        public async Task Remove(Player player)
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            using ITransaction transaction = session.BeginTransaction();
            await session.DeleteAsync(PlayerEntity.Create(player)).ConfigureAwait(false);
            await transaction.CommitAsync().ConfigureAwait(false);
        }

        public async Task<Player> GetPlayerByPlayerName(string playerName)
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            string sql = "from PlayerEntity p where p.PlayerName = '" + playerName + "'";
            ICollection<PlayerEntity> playerEntities = await session.CreateQuery(sql).ListAsync<PlayerEntity>().ConfigureAwait(false);
            if (playerEntities == null || playerEntities.Count == 0)
            {
                return null;
            }
            return playerEntities.ElementAt(0).GetPlayer();
        }

        public async Task<long> Count()
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            const string sql = "select count(distinct p.Id) from PlayerEntity p";
            return await session.CreateQuery(sql).UniqueResultAsync<long>().ConfigureAwait(false);
        }

        public async Task<Player> GetByEmail(string email)
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            string sql = "from PlayerEntity p where p.Email = '" + email + "'";
            ICollection<PlayerEntity> playerEntities = await session.CreateQuery(sql).ListAsync<PlayerEntity>().ConfigureAwait(false);
            if (playerEntities == null || playerEntities.Count == 0)
            {
                return null;
            }
            return playerEntities.ElementAt(0).GetPlayer(true);
        }

        private static ICollection<Player> ConvertPlayerEntities(ICollection<PlayerEntity> playerEntities)
        {
            if (playerEntities == null)
            {
                return null;
            }

            ICollection<Player> players = new List<Player>();
            foreach (PlayerEntity playerEntity in playerEntities)
            {
                players.Add(playerEntity?.GetPlayer());
            }
            return players;
        }
    }
}
