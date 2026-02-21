using App.Core.DTOs.Common;
using App.Core.DTOs.Donor;

namespace App.Core.Interfaces
{
    public interface IDonorService
    {
        Task<DonorResponseDto?> GetByIdAsync(Guid donorId);
        Task<PaginatedResponseDto<DonorResponseDto>> GetAllAsync(int pageNumber, int pageSize);
        Task<DonorResponseDto?> GetByUserIdAsync(Guid userId);
        Task<DonorResponseDto> UpdateAsync(Guid donorId, UpdateDonorDto dto);
    }
}
