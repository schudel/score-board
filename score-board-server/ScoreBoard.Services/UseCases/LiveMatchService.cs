using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScoreBoard.Domain.Models;
using ScoreBoard.Services.Adapters;
using ScoreBoard.Services.Helpers;

namespace ScoreBoard.Services.UseCases
{
    public class LiveMatchService : ILiveMatchService
    {
        private readonly ILiveMatchRepository liveMatchRepository;
        private readonly IPlayerRepository playerRepository;
        private readonly IEmailService emailService;

        public LiveMatchService(ILiveMatchRepository liveMatchRepository, IPlayerRepository playerRepository, IEmailService emailService)
        {
            this.liveMatchRepository = liveMatchRepository;
            this.playerRepository = playerRepository;
            this.emailService = emailService;
        }

        public void Initialize(string dbConnectionString)
        {
            liveMatchRepository.Initialize(dbConnectionString);
            playerRepository.Initialize(dbConnectionString);
        }

        public async Task<LiveMatch> GetById(string id)
        {
            if (Guid.TryParse(id, out Guid guid))
            {
                return await liveMatchRepository.GetById(guid).ConfigureAwait(false);
            }
            throw new Exception("No valid Id: \"" + id + "\"");
        }

        public async Task<ICollection<LiveMatch>> GetAll() => await liveMatchRepository.GetAll().ConfigureAwait(false);

        public async Task<ICollection<LiveMatch>> GetAllDistinct() => await liveMatchRepository.GetAllDistinct().ConfigureAwait(false);

        public async Task<bool> InvitePlayer(Invitation invitation)
        {
            Player sender = await playerRepository.GetById(invitation.SenderId, true).ConfigureAwait(false);
            if (sender == null)
            {
                return false;
            }

            string playerNames = "";
            string[] emails = new string[invitation.ReceiverIdList.Count];
            int counter = 0;
            foreach (Guid receiverId in invitation.ReceiverIdList)
            {
                Player receiver = await playerRepository.GetById(receiverId).ConfigureAwait(false);
                if (receiver == null)
                {
                    continue;
                }
                playerNames = playerNames + receiver.PlayerName + ", ";
                emails[counter] = receiver.Email;
                counter++;
            }
            return await emailService.SendInvitation(sender.PlayerName, playerNames.TrimEnd().TrimEnd(','), emails, invitation.MatchId).ConfigureAwait(false);
        }

        public async Task Add(LiveMatch liveMatch)
        {
            await liveMatchRepository.Add(liveMatch).ConfigureAwait(false);
        }

        public async Task Update(string id, LiveMatch liveMatch)
        {
            if (!Guid.TryParse(id, out Guid guid))
            {
                throw new Exception("No valid Id: \"" + id + "\"");
            }
            LiveMatch lm = await liveMatchRepository.GetById(guid).ConfigureAwait(false);
            if (lm == null)
            {
                throw new Exception("Live Match not found");
            }
            await liveMatchRepository.Update(liveMatch).ConfigureAwait(false);
        }

        public async Task Update(LiveMatch liveMatch)
        {
            await liveMatchRepository.Update(liveMatch).ConfigureAwait(false);
        }

        public async Task Remove(LiveMatch liveMatch)
        {
            await liveMatchRepository.Remove(liveMatch).ConfigureAwait(false);
        }

        public async Task Remove(string id)
        {
            if (!Guid.TryParse(id, out Guid guid))
            {
                throw new Exception("No valid Id: \"" + id + "\"");
            }
            LiveMatch liveMatch = await liveMatchRepository.GetById(guid).ConfigureAwait(false);
            await liveMatchRepository.Remove(liveMatch).ConfigureAwait(false);
            throw new Exception("No valid Id: \"" + id + "\"");
        }

        public async Task<long> Count() => await liveMatchRepository.Count().ConfigureAwait(false);

        public async Task<ICollection<LiveMatch>> GetByMatchId(string matchId)
        {
            if (Guid.TryParse(matchId, out Guid guid))
            {
                return await liveMatchRepository.GetByMatchId(guid).ConfigureAwait(false);
            }
            throw new Exception("No valid Id: \"" + matchId + "\"");
        }
    }
}
