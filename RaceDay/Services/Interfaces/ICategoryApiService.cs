using RaceDay.Models;
using RaceDay.Models.Categories;

namespace RaceDay.Services.Interfaces
{
    public interface ICategoryApiService
    {
        Task<ApiResult<List<CategoryViewModel>>> GetForEventAsync(int eventId);                    // GET /api/events/{id}/categories
        Task<ApiResult<CategoryViewModel>> CreateAsync(int eventId, CategoryFormViewModel model);  // POST /api/events/{id}/categories
        Task<ApiResult<CategoryViewModel>> UpdateAsync(int categoryId, CategoryFormViewModel model); // PUT /api/categories/{id}
        Task<ApiResult<bool>> DeleteAsync(int categoryId);                                         // DELETE /api/categories/{id}

    }
}
