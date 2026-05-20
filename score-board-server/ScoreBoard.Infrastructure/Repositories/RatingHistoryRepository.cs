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
    public class RatingHistoryRepository : IRatingHistoryRepository
    {
        private string dbConnectionString;
        
        public void Initialize(string connectionString)
        {
            dbConnectionString = connectionString;
        }

        public async Task<RatingHistory> GetById(Guid id, bool slim = false)
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            RatingHistoryEntity ratingHistoryEntity = await session.GetAsync<RatingHistoryEntity>(id).ConfigureAwait(false);
            return ratingHistoryEntity?.GetRatingHistory(slim);
        }
        
        public async Task<ICollection<RatingHistory>> GetAll(bool slim = false)
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            ICollection<RatingHistoryEntity> ratingHistoryEntities = await session.Query<RatingHistoryEntity>().ToListAsync().ConfigureAwait(false);
            return ConvertRatingHistoryEntities(ratingHistoryEntities, slim);
        }

        public async Task Add(RatingHistory ratingHistory)
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            using ITransaction transaction = session.BeginTransaction();
            await session.SaveAsync(RatingHistoryEntity.Create(ratingHistory)).ConfigureAwait(false);
            await transaction.CommitAsync().ConfigureAwait(false);
        }

        public async Task Update(RatingHistory ratingHistory)
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            using ITransaction transaction = session.BeginTransaction();
            await session.UpdateAsync(RatingHistoryEntity.Create(ratingHistory)).ConfigureAwait(false);
            await transaction.CommitAsync().ConfigureAwait(false);
        }

        public async Task Remove(RatingHistory ratingHistory)
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            using ITransaction transaction = session.BeginTransaction();
            await session.DeleteAsync(RatingHistoryEntity.Create(ratingHistory)).ConfigureAwait(false);
            await transaction.CommitAsync().ConfigureAwait(false);
        }
        
        private static ICollection<RatingHistory> ConvertRatingHistoryEntities(ICollection<RatingHistoryEntity> ratingHistoryEntities, bool slim = false)
        {
            if (ratingHistoryEntities == null)
            {
                return null;
            }
            ICollection<RatingHistory> ratingHistories = new List<RatingHistory>();
            foreach (RatingHistoryEntity ratingHistoryEntity in ratingHistoryEntities)
            {
                ratingHistories.Add(ratingHistoryEntity.GetRatingHistory(slim));
            }
            return ratingHistories;
        }
    }
}
