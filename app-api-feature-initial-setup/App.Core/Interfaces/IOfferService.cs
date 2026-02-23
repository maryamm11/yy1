using App.Core.DTOs.Common;
using App.Core.DTOs.Offer;
using App.Core.Enums;

namespace App.Core.Interfaces
{
    public interface IOfferService
    {
        Task<OfferResponseDto?> GetByIdAsync(Guid offerId);
        Task<PaginatedResponseDto<OfferResponseDto>> GetAllAsync(int pageNumber, int pageSize, OfferStatus? status = null, string? category = null);
        Task<PaginatedResponseDto<OfferResponseDto>> GetByDonorIdAsync(Guid donorId, int pageNumber, int pageSize);
        Task<OfferResponseDto> CreateAsync(Guid donorId, CreateOfferDto dto);
        Task<OfferResponseDto> UpdateAsync(Guid offerId, UpdateOfferDto dto);
        Task DeleteAsync(Guid offerId);
    }
}
