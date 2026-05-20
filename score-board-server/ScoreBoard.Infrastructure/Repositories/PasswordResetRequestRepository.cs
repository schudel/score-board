using System;
using System.Threading.Tasks;
using NHibernate;
using ScoreBoard.Domain.Models;
using ScoreBoard.Infrastructure.Models;
using ScoreBoard.Services.Adapters;

namespace ScoreBoard.Infrastructure.Repositories
{
    public class PasswordResetRequestRepository : IPasswordResetRequestRepository
    {
        private string dbConnectionString;
        
        public void Initialize(string connectionString)
        {
            dbConnectionString = connectionString;
        }

        public async Task<PasswordResetRequest> GetById(Guid id)
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            PasswordResetRequestEntity passwordResetRequestEntity = await session.GetAsync<PasswordResetRequestEntity>(id).ConfigureAwait(false);
            return passwordResetRequestEntity?.GetPasswordResetRequest();
        }

        public async Task Add(PasswordResetRequest passwordResetRequest)
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            using ITransaction transaction = session.BeginTransaction();
            await session.SaveAsync(PasswordResetRequestEntity.Create(passwordResetRequest)).ConfigureAwait(false);
            await transaction.CommitAsync().ConfigureAwait(false);
        }

        public async Task Remove(PasswordResetRequest passwordResetRequest)
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            using ITransaction transaction = session.BeginTransaction();
            await session.DeleteAsync(PasswordResetRequestEntity.Create(passwordResetRequest)).ConfigureAwait(false);
            await transaction.CommitAsync().ConfigureAwait(false);
        }
    }
}
