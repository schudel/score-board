using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScoreBoard.Domain.Models;
using ScoreBoard.Services.Adapters;

namespace ScoreBoard.Fakes.Infrastructure.Repositories
{
    public class PlayerRepositoryFake : IPlayerRepository
    {
        private FakeData fakeData;

        public void Initialize(string connectionString)
        {
            fakeData = new FakeData();
        }

        public async Task<Player> GetById(Guid id, bool slim = false)
        {
            await Task.Delay(0);
            return fakeData.FakePlayers.SingleOrDefault(p => p.Id == id);
        }

        public async Task<ICollection<Player>> GetAll()
        {
            await Task.Delay(0);
            return fakeData.FakePlayers;
        }

        public async Task Add(Player player)
        {
            await Task.Delay(0);
            fakeData.FakePlayers.Add(player);
        }

        public async Task Update(Player player)
        {
            await Task.Delay(0);
            foreach (Player fakePlayer in fakeData.FakePlayers)
            {
                if (fakePlayer.Id == player.Id)
                {
                    fakePlayer.ActivationDate = player.ActivationDate;
                    fakePlayer.Email = player.Email;
                    fakePlayer.FirstName = player.FirstName;
                    fakePlayer.Image = player.Image;
                    fakePlayer.IsActive = player.IsActive;
                    fakePlayer.LastLogin = player.LastLogin;
                    fakePlayer.LastName = player.LastName;
                    fakePlayer.MustChangePassword = player.MustChangePassword;
                    fakePlayer.PasswordHash = player.PasswordHash;
                    fakePlayer.PasswordSalt = player.PasswordSalt;
                    fakePlayer.PlayerName = player.PlayerName;
                    fakePlayer.RegistrationDate = player.RegistrationDate;
                    fakePlayer.Role = player.Role;
                    fakePlayer.Settings = player.Settings;
                    fakePlayer.RegistrationDate = player.RegistrationDate;
                }
            }
        }

        public async Task Remove(Player player)
        {
            await Task.Delay(0);
            fakeData.FakePlayers.Remove(player);
        }

        public async Task<Player> GetPlayerByPlayerName(string playerName)
        {
            await Task.Delay(0);
            return fakeData.FakePlayers.SingleOrDefault(p => p.PlayerName == playerName);
        }

        public async Task<long> Count()
        {
            await Task.Delay(0);
            return fakeData.FakePlayers.Count;
        }

        public async Task<Player> GetByEmail(string email)
        {
            await Task.Delay(0);
            return fakeData.FakePlayers.SingleOrDefault(p => p.Email == email);
        }
    }
}
