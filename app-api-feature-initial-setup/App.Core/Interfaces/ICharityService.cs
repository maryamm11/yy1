using App.Core.DTOs.Charity;
using App.Core.DTOs.Common;

namespace App.Core.Interfaces
{
    public interface ICharityService
    {
        Task<CharityResponseDto?> GetByIdAsync(Guid charityId);
        Task<PaginatedResponseDto<CharityResponseDto>> GetAllAsync(int pageNumber, int pageSize);
        Task<CharityResponseDto?> GetByUserIdAsync(Guid userId);
        Task<CharityResponseDto> UpdateAsync(Guid charityId, UpdateCharityDto dto);
    }
}
