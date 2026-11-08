using RaceDay.Models;
using RaceDay.Models.Enrolments;

namespace RaceDay.Services.Interfaces
{
    public interface IEnrolmentApiService
    {
        Task<ApiResult<List<EnrolmentViewModel>>> GetForEventAsync(int eventId); // GET /api/events/{id}/enrolments

        Task<ApiResult<EnrolmentViewModel>> EnrolAsync(int categoryId);
        Task<ApiResult<List<EnrolmentViewModel>>> GetMineAsync();
        Task<ApiResult<bool>> WithdrawAsync(int enrolmentId);
    }
}
