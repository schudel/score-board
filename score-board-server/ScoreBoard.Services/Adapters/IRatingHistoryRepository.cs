using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScoreBoard.Domain.Models;

namespace ScoreBoard.Services.Adapters
{
    public interface IRatingHistoryRepository : IBaseRepository
    {
        Task<RatingHistory> GetById(Guid id, bool slim = false);
        Task<ICollection<RatingHistory>> GetAll(bool slim = false);
        Task Add(RatingHistory ratingHistory);
        Task Update(RatingHistory ratingHistory);
        Task Remove(RatingHistory ratingHistory);
    }
}
