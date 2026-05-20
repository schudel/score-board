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
    public class ChatRepository : IChatRepository
    {
        private string dbConnectionString;

        public void Initialize(string connectionString)
        {
            dbConnectionString = connectionString;
        }

        public async Task<Chat> GetById(Guid id)
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            ChatEntity chatEntity = await session.GetAsync<ChatEntity>(id).ConfigureAwait(false);
            return chatEntity?.GetChat();
        }
        
        public async Task<ICollection<Chat>> GetAll()
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            ICollection<ChatEntity> chatEntities = await session.Query<ChatEntity>().ToListAsync().ConfigureAwait(false);
            return ConvertChatEntities(chatEntities);
        }
        
        public async Task Add(Chat chat)
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            using ITransaction transaction = session.BeginTransaction();
            await session.SaveAsync(ChatEntity.Create(chat)).ConfigureAwait(false);
            await transaction.CommitAsync().ConfigureAwait(false);
        }

        public async Task Update(Chat chat)
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            using ITransaction transaction = session.BeginTransaction();
            await session.UpdateAsync(ChatEntity.Create(chat)).ConfigureAwait(false);
            await transaction.CommitAsync().ConfigureAwait(false);
        }

        public async Task Remove(Chat chat)
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            using ITransaction transaction = session.BeginTransaction();
            await session.DeleteAsync(ChatEntity.Create(chat)).ConfigureAwait(false);
            await transaction.CommitAsync().ConfigureAwait(false);
        }

        public async Task<long> Count()
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            const string sql = "select count(distinct c.Id) from ChatEntity c";
            return await session.CreateQuery(sql).UniqueResultAsync<long>().ConfigureAwait(false);
        }

        public async Task<ICollection<Chat>> GetByPlayerId(Guid playerId)
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            string sql = "from ChatEntity c where c.PlayerId = '" + playerId + "'";
            ICollection<ChatEntity> chatEntities = await session.CreateQuery(sql).ListAsync<ChatEntity>().ConfigureAwait(false);
            if (chatEntities == null || chatEntities.Count == 0)
            {
                return null;
            }
            return ConvertChatEntities(chatEntities);
        }

        public async Task<ICollection<Chat>> GetSpecificEntries(int amount, int skip = 0)
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            ICollection<ChatEntity> chatEntities = await session.QueryOver<ChatEntity>()
                                                                .OrderBy(x => x.TimeStamp).Desc
                                                                .Skip(skip)
                                                                .Take(amount).ListAsync().ConfigureAwait(false);
            if (chatEntities == null || chatEntities.Count == 0)
            {
                return null;
            }
            return ConvertChatEntities(chatEntities);
        }

        private static ICollection<Chat> ConvertChatEntities(ICollection<ChatEntity> chatEntities)
        {
            if (chatEntities == null)
            {
                return null;
            }

            ICollection<Chat> chates = new List<Chat>();
            foreach (ChatEntity chatEntity in chatEntities)
            {
                chates.Add(chatEntity.GetChat());
            }
            return chates;
        }
    }
}
