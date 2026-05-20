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
    public class RatingRepository : IRatingRepository
    {
        private string dbConnectionString;
        
        public void Initialize(string connectionString)
        {
            dbConnectionString = connectionString;
        }

        public async Task<Rating> GetById(Guid id, bool slim = false)
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            RatingEntity ratingEntity = await session.GetAsync<RatingEntity>(id).ConfigureAwait(false);
            return ratingEntity?.GetRating(slim);
        }
        
        public async Task<ICollection<Rating>> GetAll(bool slim = false)
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            ICollection<RatingEntity> ratingEntities = await session.Query<RatingEntity>().ToListAsync().ConfigureAwait(false);
            return ConvertRatingEntities(ratingEntities, slim);
        }

        public async Task Add(Rating rating)
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            using ITransaction transaction = session.BeginTransaction();
            await session.SaveAsync(RatingEntity.Create(rating)).ConfigureAwait(false);
            await transaction.CommitAsync().ConfigureAwait(false);
        }

        public async Task Update(Rating rating)
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            using ITransaction transaction = session.BeginTransaction();
            await session.UpdateAsync(RatingEntity.Create(rating)).ConfigureAwait(false);
            await transaction.CommitAsync().ConfigureAwait(false);
        }

        public async Task Remove(Rating rating)
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            using ITransaction transaction = session.BeginTransaction();
            await session.DeleteAsync(RatingEntity.Create(rating)).ConfigureAwait(false);
            await transaction.CommitAsync().ConfigureAwait(false);
        }

        public async Task<ICollection<Rating>> GetByGameId(Guid gameId)
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            string sql = "from RatingEntity r where r.GameId = '" + gameId.ToString().TrimStart('{').TrimEnd('}') + "'";
            ICollection<RatingEntity> ratingEntities = await session.CreateQuery(sql).ListAsync<RatingEntity>().ConfigureAwait(false);
            if (ratingEntities == null || ratingEntities.Count == 0)
            {
                return null;
            }
            return ConvertRatingEntities(ratingEntities);
        }

        public async Task<ICollection<Rating>> GetByPlayerId(Guid playerId)
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            string sql = "from RatingEntity r where r.Player.Id = '" + playerId.ToString().TrimStart('{').TrimEnd('}') + "'";
            ICollection<RatingEntity> ratingEntities = await session.CreateQuery(sql).ListAsync<RatingEntity>().ConfigureAwait(false);
            if (ratingEntities == null || ratingEntities.Count == 0)
            {
                return null;
            }
            return ConvertRatingEntities(ratingEntities);
        }

        public async Task<Rating> GetByGameIdAndPlayerId(Guid gameId, Guid playerId, bool slim = false)
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            string sql = "from RatingEntity r where r.Game.Id = '" + gameId.ToString().TrimStart('{').TrimEnd('}') + "' AND r.Player.Id = '" + playerId.ToString().TrimStart('{').TrimEnd('}') + "'";
            ICollection<RatingEntity> ratingEntities = await session.CreateQuery(sql).ListAsync<RatingEntity>().ConfigureAwait(false);
            if (ratingEntities == null || ratingEntities.Count == 0)
            {
                return null;
            }
            return ratingEntities.ElementAt(0).GetRating(slim);
        }

        private static ICollection<Rating> ConvertRatingEntities(ICollection<RatingEntity> ratingEntities, bool slim = false)
        {
            if (ratingEntities == null)
            {
                return null;
            }
            ICollection<Rating> ratings = new List<Rating>();
            foreach (RatingEntity ratingEntity in ratingEntities)
            {
                ratings.Add(ratingEntity.GetRating(slim));
            }
            return ratings;
        }
    }
}
