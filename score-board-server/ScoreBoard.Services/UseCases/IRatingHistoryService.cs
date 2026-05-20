using System.Collections.Generic;
using System.Threading.Tasks;
using ScoreBoard.Domain.Models;

namespace ScoreBoard.Services.UseCases
{
    public interface IRatingHistoryService
    {
        void Initialize(string dbConnectionString);
        Task<RatingHistory> GetById(string id, bool slim = false);
        Task<ICollection<RatingHistory>> GetAll(bool slim = false);
        Task Add(RatingHistory ratingHistory);
        Task Update(string id, RatingHistory ratingHistory);
        Task Update(RatingHistory ratingHistory);
        Task Remove(RatingHistory ratingHistory);
        Task Remove(string id);
    }
}
