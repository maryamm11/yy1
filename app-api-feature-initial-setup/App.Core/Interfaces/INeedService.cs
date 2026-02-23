using App.Core.DTOs.Common;
using App.Core.DTOs.Need;
using App.Core.Enums;

namespace App.Core.Interfaces
{
    public interface INeedService
    {
        Task<NeedResponseDto?> GetByIdAsync(Guid needId);
        Task<PaginatedResponseDto<NeedResponseDto>> GetAllAsync(int pageNumber, int pageSize, NeedStatus? status = null, string? category = null);
        Task<PaginatedResponseDto<NeedResponseDto>> GetByCharityIdAsync(Guid charityId, int pageNumber, int pageSize);
        Task<NeedResponseDto> CreateAsync(Guid charityId, CreateNeedDto dto);
        Task<NeedResponseDto> UpdateAsync(Guid needId, UpdateNeedDto dto);
        Task DeleteAsync(Guid needId);
        Task<NeedResponseDto> UpdateStatusAsync(Guid needId, NeedStatus status, Guid? adminId = null);
    }
}
