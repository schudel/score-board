using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScoreBoard.Domain.Models;
using ScoreBoard.Services.Adapters;

namespace ScoreBoard.Services.UseCases
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository chatRepository;


        public ChatService(IChatRepository chatRepository)
        {
            this.chatRepository = chatRepository;
        }

        public void Initialize(string dbConnectionString)
        {
            chatRepository.Initialize(dbConnectionString);
        }

        public async Task<Chat> GetById(string id)
        {
            if (Guid.TryParse(id, out Guid guid))
            {
                return await chatRepository.GetById(guid).ConfigureAwait(false);
            }
            throw new Exception("No valid Id: \"" + id + "\"");
        }

        public async Task<ICollection<Chat>> GetAll() => await chatRepository.GetAll().ConfigureAwait(false);

        public async Task Add(Chat chat)
        {
            if (chat == null)
            {
                throw new Exception("Chat is required.");
            }
            await chatRepository.Add(chat).ConfigureAwait(false);
        }

        public async Task Update(string id, Chat chat)
        {
            if (!Guid.TryParse(id, out Guid guid))
            {
                throw new Exception("No valid Id: \"" + id + "\"");
            }
            Chat lm = await chatRepository.GetById(guid).ConfigureAwait(false);
            if (lm == null)
            {
                throw new Exception("Chat not found");
            }
            await chatRepository.Update(chat).ConfigureAwait(false);
        }
        
        public async Task Remove(Chat chat)
        {
            if (chat == null)
            {
                throw new Exception("Chat is required.");
            }
            await chatRepository.Remove(chat).ConfigureAwait(false);
        }

        public async Task Remove(string id)
        {
            if (!Guid.TryParse(id, out Guid guid))
            {
                throw new Exception("No valid Id: \"" + id + "\"");
            }
            Chat chat = await chatRepository.GetById(guid).ConfigureAwait(false);
            await chatRepository.Remove(chat).ConfigureAwait(false);
        }

        public async Task<long> Count() => await chatRepository.Count().ConfigureAwait(false);

        public async Task<ICollection<Chat>> GetByPlayerId(string playerId)
        {
            if (Guid.TryParse(playerId, out Guid guid))
            {
                return await chatRepository.GetByPlayerId(guid).ConfigureAwait(false);
            }
            throw new Exception("No valid Id: \"" + playerId + "\"");
        }

        public async Task<ICollection<Chat>> GetSpecificEntries(int amount, int skip = 0) => await chatRepository.GetSpecificEntries(amount, skip).ConfigureAwait(false);
    }
}
