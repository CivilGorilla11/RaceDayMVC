using RaceDay.Models;
using RaceDay.Models.Results;


namespace RaceDay.Services.Interfaces
{
    public interface IResultApiService
    {
        Task<ApiResult<List<ResultViewModel>>> GetForEventAsync(int eventId);        // GET /api/events/{id}/results
        Task<ApiResult<ResultViewModel>> CreateAsync(ResultFormViewModel model);     // POST /api/results
        Task<ApiResult<ResultViewModel>> UpdateAsync(int resultId, ResultFormViewModel model); // PUT /api/results/{id}
        Task<ApiResult<bool>> DeleteAsync(int resultId);                             // DELETE /api/results/{id}

        Task<ApiResult<List<ResultViewModel>>> GetMineAsync();

    }
}
