using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Transform;
using ScoreBoard.Domain.Models;
using ScoreBoard.Infrastructure.Models;
using ScoreBoard.Services.Adapters;

namespace ScoreBoard.Infrastructure.Repositories
{
    public class GameRepository : IGameRepository
    {
        private  string dbConnectionString;
        
        public void Initialize(string connectionString)
        {
            dbConnectionString = connectionString;
        }

        public async Task<Game> GetById(Guid id)
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            GameEntity gameEntity = await session.GetAsync<GameEntity>(id).ConfigureAwait(false);
            return gameEntity?.GetGame();
        }
        
        public async Task<ICollection<Game>> GetAll()
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            ICollection<GameEntity> gameEntities = await session.Query<GameEntity>().ToListAsync().ConfigureAwait(false);
            return ConvertGameEntities(gameEntities);
        }

        public async Task Add(Game game)
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            using ITransaction transaction = session.BeginTransaction();
            await session.SaveAsync(GameEntity.Create(game)).ConfigureAwait(false);
            await transaction.CommitAsync().ConfigureAwait(false);
        }

        public async Task Update(Game game)
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            using ITransaction transaction = session.BeginTransaction();
            await session.UpdateAsync(GameEntity.Create(game)).ConfigureAwait(false);
            await transaction.CommitAsync().ConfigureAwait(false);
        }

        public async Task Remove(Game game)
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            using ITransaction transaction = session.BeginTransaction();
            await session.DeleteAsync(GameEntity.Create(game)).ConfigureAwait(false);
            await transaction.CommitAsync().ConfigureAwait(false);
        }

        public async Task<long> Count()
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            const string sql = "select count(distinct g.Id) from GameEntity g";
            return await session.CreateQuery(sql).UniqueResultAsync<long>().ConfigureAwait(false);
        }

        public async Task<ICollection<string>> GetGenres()
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            ICriteria criteria = session.CreateCriteria(typeof(GameEntity));
            criteria.SetProjection(Projections.Distinct(Projections.ProjectionList().Add(Projections.Property("Genre").As("Genre"))));
            criteria.SetResultTransformer(new AliasToBeanResultTransformer(typeof(GameEntity)));
            IList<GameEntity> games = await criteria.ListAsync<GameEntity>().ConfigureAwait(false);
            ICollection<string> genres = new List<string>();
            if (games == null)
            {
                return genres;
            }
            foreach (GameEntity gameEntity in games)
            {
                genres.Add(gameEntity.Genre);
            }
            return genres;
        }

        public async Task<ICollection<string>> GetTypes()
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            ICriteria criteria = session.CreateCriteria(typeof(GameEntity));
            criteria.SetProjection(Projections.Distinct(Projections.ProjectionList().Add(Projections.Property("Type").As("Type"))));
            criteria.SetResultTransformer(new AliasToBeanResultTransformer(typeof(GameEntity)));
            IList<GameEntity> games = await criteria.ListAsync<GameEntity>().ConfigureAwait(false);
            ICollection<string> types = new List<string>();
            if (games == null)
            {
                return types;
            }
            foreach (GameEntity gameEntity in games)
            {
                types.Add(gameEntity.Type);
            }
            return types;
        }

        private static ICollection<Game> ConvertGameEntities(ICollection<GameEntity> gameEntities)
        {
            if (gameEntities == null)
            {
                return null;
            }

            ICollection<Game> games = new List<Game>();
            foreach (GameEntity gameEntity in gameEntities)
            {
                games.Add(gameEntity.GetGame());
            }
            return games;
        }
    }
}
