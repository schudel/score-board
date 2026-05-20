using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScoreBoard.Domain.Models;
using ScoreBoard.Services.Adapters;

namespace ScoreBoard.Services.UseCases
{
    public class RatingHistoryService : IRatingHistoryService
    {
        private readonly IRatingHistoryRepository ratingHistoryRepository;

        public RatingHistoryService(IRatingHistoryRepository ratingHistoryRepository)
        {
            this.ratingHistoryRepository = ratingHistoryRepository;
        }

        public void Initialize(string dbConnectionString)
        {
            ratingHistoryRepository.Initialize(dbConnectionString);
        }

        public async Task<RatingHistory> GetById(string id, bool slim = false)
        {
            if (Guid.TryParse(id, out Guid guid))
            {
                return await ratingHistoryRepository.GetById(guid, slim).ConfigureAwait(false);
            }
            throw new Exception("No valid Id: \"" + id + "\"");
        }

        public async Task<ICollection<RatingHistory>> GetAll(bool slim = false) => await ratingHistoryRepository.GetAll(slim).ConfigureAwait(false);

        public async Task Add(RatingHistory ratingHistory)
        {
            if (ratingHistory == null)
            {
                throw new Exception("Rating History is required.");
            }
            await ratingHistoryRepository.Add(ratingHistory).ConfigureAwait(false);
        }

        public async Task Update(string id, RatingHistory ratingHistory)
        {
            if (!Guid.TryParse(id, out Guid guid))
            {
                throw new Exception("No valid Id: \"" + id + "\"");
            }
            RatingHistory rh = await ratingHistoryRepository.GetById(guid).ConfigureAwait(false);
            if (rh == null)
            {
                throw new Exception("Rating History not found");
            }
            await ratingHistoryRepository.Update(ratingHistory).ConfigureAwait(false);
        }

        public async Task Update(RatingHistory ratingHistory)
        {
            await ratingHistoryRepository.Update(ratingHistory).ConfigureAwait(false);
        }

        public async Task Remove(RatingHistory ratingHistory)
        {
            if (ratingHistory == null)
            {
                throw new Exception("Rating History is required.");
            }
            await ratingHistoryRepository.Remove(ratingHistory).ConfigureAwait(false);
        }

        public async Task Remove(string id)
        {
            if (!Guid.TryParse(id, out Guid guid))
            {
                throw new Exception("No valid Id: \"" + id + "\"");
            }
            RatingHistory ratingHistory = await ratingHistoryRepository.GetById(guid).ConfigureAwait(false);
            await ratingHistoryRepository.Remove(ratingHistory).ConfigureAwait(false);
        }
    }
}
