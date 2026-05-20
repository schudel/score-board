using System;
using System.Linq;
using System.Threading.Tasks;
using ScoreBoard.Domain.Models;
using ScoreBoard.Services.Adapters;

namespace ScoreBoard.Fakes.Infrastructure.Repositories
{
    public class PasswordResetRequestRepositoryFake : IPasswordResetRequestRepository
    {
        private FakeData fakeData;

        public void Initialize(string connectionString)
        {
            fakeData = new FakeData();
        }

        public async Task<PasswordResetRequest> GetById(Guid id)
        {
            await Task.Delay(0);
            return fakeData.FakePasswordResetRequests.SingleOrDefault(r => r.Id == id);
        }

        public async Task Add(PasswordResetRequest passwordResetRequest)
        {
            await Task.Delay(0);
            fakeData.FakePasswordResetRequests.Add(passwordResetRequest);
        }

        public async Task Remove(PasswordResetRequest passwordResetRequest)
        {
            await Task.Delay(0);
            fakeData.FakePasswordResetRequests.Remove(passwordResetRequest);
        }
    }
}
