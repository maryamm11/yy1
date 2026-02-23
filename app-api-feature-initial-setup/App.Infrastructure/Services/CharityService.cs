using App.Core.DTOs.Charity;
using App.Core.DTOs.Common;
using App.Core.Interfaces;

namespace App.Infrastructure.Services
{
    public class CharityService : ICharityService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CharityService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CharityResponseDto?> GetByIdAsync(Guid charityId)
        {
            var charity = await _unitOfWork.Charities.GetByIdAsync(charityId);
            if (charity == null) return null;

            return MapToDto(charity);
        }

        public async Task<PaginatedResponseDto<CharityResponseDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            var charities = await _unitOfWork.Charities.GetAllAsync();
            var totalCount = await _unitOfWork.Charities.CountAsync();

            var pagedCharities = charities
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(MapToDto);

            return new PaginatedResponseDto<CharityResponseDto>
            {
                Items = pagedCharities,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<CharityResponseDto?> GetByUserIdAsync(Guid userId)
        {
            var charity = await _unitOfWork.Charities.FirstOrDefaultAsync(c => c.UserId == userId);
            if (charity == null) return null;

            return MapToDto(charity);
        }

        public async Task<CharityResponseDto> UpdateAsync(Guid charityId, UpdateCharityDto dto)
        {
            var charity = await _unitOfWork.Charities.GetByIdAsync(charityId)
                ?? throw new KeyNotFoundException($"Charity with ID {charityId} not found.");

            charity.CharityName = dto.CharityName;
            charity.CharityDescription = dto.CharityDescription ?? charity.CharityDescription;

            _unitOfWork.Charities.Update(charity);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(charity);
        }

        private static CharityResponseDto MapToDto(App.Core.Entities.Charity charity)
        {
            return new CharityResponseDto
            {
                CharityId = charity.CharityId,
                CharityName = charity.CharityName,
                CharityDescription = charity.CharityDescription,
                IsVerified = charity.IsVerified,
                IsActive = charity.IsActive,
                CreatedAt = charity.CreatedAt,
                UpdatedAt = charity.UpdatedAt,
                UserId = charity.UserId
            };
        }
    }
}
