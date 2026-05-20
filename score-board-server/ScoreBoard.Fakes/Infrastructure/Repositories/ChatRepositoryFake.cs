using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScoreBoard.Domain.Models;
using ScoreBoard.Services.Adapters;

namespace ScoreBoard.Fakes.Infrastructure.Repositories
{
    public class ChatRepositoryFake : IChatRepository
    {
        private FakeData fakeData;

        public void Initialize(string connectionString)
        {
            fakeData = new FakeData();
        }

        public async Task<Chat> GetById(Guid id)
        {
            await Task.Delay(0);
            return fakeData.FakeChats.SingleOrDefault(c => c.Id == id);
        }

        public async Task<ICollection<Chat>> GetAll()
        {
            await Task.Delay(0);
            return fakeData.FakeChats;
        }

        public async Task Add(Chat chat)
        {
            await Task.Delay(0);
            fakeData.FakeChats.Add(chat);
        }

        public async Task Update(Chat chat)
        {
            await Task.Delay(0);
            foreach (Chat fakeChat in fakeData.FakeChats)
            {
                if (fakeChat.Id == chat.Id)
                {
                    fakeChat.Message = chat.Message;
                    fakeChat.PlayerId = chat.PlayerId;
                    fakeChat.Room = chat.Room;
                    fakeChat.TimeStamp = chat.TimeStamp;
                    fakeChat.UserName = chat.UserName;
                }
            }
        }

        public async Task Remove(Chat chat)
        {
            await Task.Delay(0);
            fakeData.FakeChats.Remove(chat);
        }

        public async Task<long> Count()
        {
            await Task.Delay(0);
            return fakeData.FakeChats.Count;
        }

        public async Task<ICollection<Chat>> GetByPlayerId(Guid playerId)
        {
            await Task.Delay(0);
            return fakeData.FakeChats.Where(c => c.PlayerId == playerId).ToList();
        }

        public async Task<ICollection<Chat>> GetSpecificEntries(int amount, int skip = 0)
        {
            await Task.Delay(0);
            return fakeData.FakeChats.OrderByDescending(c => c.TimeStamp).Skip(skip).Take(amount).ToList();
        }
    }
}
