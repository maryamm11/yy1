using App.Core.DTOs.Common;
using App.Core.DTOs.Donor;
using App.Core.Interfaces;

namespace App.Infrastructure.Services
{
    public class DonorService : IDonorService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DonorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DonorResponseDto?> GetByIdAsync(Guid donorId)
        {
            var donor = await _unitOfWork.DonorOrganizations.GetByIdAsync(donorId);
            if (donor == null) return null;

            return MapToDto(donor);
        }

        public async Task<PaginatedResponseDto<DonorResponseDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            var donors = await _unitOfWork.DonorOrganizations.GetAllAsync();
            var totalCount = await _unitOfWork.DonorOrganizations.CountAsync();

            var pagedDonors = donors
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(MapToDto);

            return new PaginatedResponseDto<DonorResponseDto>
            {
                Items = pagedDonors,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<DonorResponseDto?> GetByUserIdAsync(Guid userId)
        {
            var donor = await _unitOfWork.DonorOrganizations.FirstOrDefaultAsync(d => d.UserId == userId);
            if (donor == null) return null;

            return MapToDto(donor);
        }

        public async Task<DonorResponseDto> UpdateAsync(Guid donorId, UpdateDonorDto dto)
        {
            var donor = await _unitOfWork.DonorOrganizations.GetByIdAsync(donorId)
                ?? throw new KeyNotFoundException($"Donor with ID {donorId} not found.");

            donor.DonorName = dto.DonorName;
            donor.DonorOrganizationImage = dto.DonorOrganizationImage ?? donor.DonorOrganizationImage;

            _unitOfWork.DonorOrganizations.Update(donor);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(donor);
        }

        private static DonorResponseDto MapToDto(App.Core.Entities.DonorOrganization donor)
        {
            return new DonorResponseDto
            {
                DonorId = donor.DonorId,
                DonorName = donor.DonorName,
                DonorOrganizationImage = donor.DonorOrganizationImage,
                IsVerified = donor.IsVerified,
                IsActive = donor.IsActive,
                CreatedAt = donor.CreatedAt,
                UpdatedAt = donor.UpdatedAt,
                UserId = donor.UserId
            };
        }
    }
}
