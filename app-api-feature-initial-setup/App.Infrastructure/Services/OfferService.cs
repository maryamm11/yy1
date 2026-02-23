using App.Core.DTOs.Common;
using App.Core.DTOs.Offer;
using App.Core.Entities;
using App.Core.Enums;
using App.Core.Interfaces;

namespace App.Infrastructure.Services
{
    public class OfferService : IOfferService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OfferService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<OfferResponseDto?> GetByIdAsync(Guid offerId)
        {
            var offer = await _unitOfWork.Offers.GetByIdAsync(offerId);
            if (offer == null) return null;

            var donor = await _unitOfWork.DonorOrganizations.GetByIdAsync(offer.DonorOrganizationId);
            return MapToDto(offer, donor?.DonorName ?? "Unknown");
        }

        public async Task<PaginatedResponseDto<OfferResponseDto>> GetAllAsync(
            int pageNumber, int pageSize, OfferStatus? status = null, string? category = null)
        {
            var offers = await _unitOfWork.Offers.GetAllAsync();
            var filtered = offers.AsEnumerable();

            if (status.HasValue)
                filtered = filtered.Where(o => o.Status == status.Value);
            if (!string.IsNullOrEmpty(category))
                filtered = filtered.Where(o => o.Category.Equals(category, StringComparison.OrdinalIgnoreCase));

            var totalCount = filtered.Count();
            var pagedOffers = filtered
                .OrderByDescending(o => o.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var donorIds = pagedOffers.Select(o => o.DonorOrganizationId).Distinct();
            var donors = new Dictionary<Guid, string>();
            foreach (var dId in donorIds)
            {
                var donor = await _unitOfWork.DonorOrganizations.GetByIdAsync(dId);
                if (donor != null)
                    donors[dId] = donor.DonorName;
            }

            var items = pagedOffers.Select(o =>
                MapToDto(o, donors.GetValueOrDefault(o.DonorOrganizationId, "Unknown")));

            return new PaginatedResponseDto<OfferResponseDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<PaginatedResponseDto<OfferResponseDto>> GetByDonorIdAsync(
            Guid donorId, int pageNumber, int pageSize)
        {
            var donor = await _unitOfWork.DonorOrganizations.GetByIdAsync(donorId);
            var donorName = donor?.DonorName ?? "Unknown";

            var offers = await _unitOfWork.Offers.FindAsync(o => o.DonorOrganizationId == donorId);
            var totalCount = offers.Count();

            var pagedOffers = offers
                .OrderByDescending(o => o.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(o => MapToDto(o, donorName));

            return new PaginatedResponseDto<OfferResponseDto>
            {
                Items = pagedOffers,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<OfferResponseDto> CreateAsync(Guid donorId, CreateOfferDto dto)
        {
            var donor = await _unitOfWork.DonorOrganizations.GetByIdAsync(donorId)
                ?? throw new KeyNotFoundException($"Donor with ID {donorId} not found.");

            var offer = new Offer
            {
                OfferId = Guid.NewGuid(),
                DonorOrganizationId = donorId,
                Category = dto.Category,
                ProductName = dto.ProductName,
                Quantity = dto.Quantity,
                ProductImage = dto.ProductImage,
                ExpiryDate = dto.ExpiryDate,
                Status = OfferStatus.Available,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Offers.AddAsync(offer);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(offer, donor.DonorName);
        }

        public async Task<OfferResponseDto> UpdateAsync(Guid offerId, UpdateOfferDto dto)
        {
            var offer = await _unitOfWork.Offers.GetByIdAsync(offerId)
                ?? throw new KeyNotFoundException($"Offer with ID {offerId} not found.");

            offer.Category = dto.Category;
            offer.ProductName = dto.ProductName;
            offer.Quantity = dto.Quantity;
            offer.ProductImage = dto.ProductImage;
            offer.ExpiryDate = dto.ExpiryDate;

            _unitOfWork.Offers.Update(offer);
            await _unitOfWork.SaveChangesAsync();

            var donor = await _unitOfWork.DonorOrganizations.GetByIdAsync(offer.DonorOrganizationId);
            return MapToDto(offer, donor?.DonorName ?? "Unknown");
        }

        public async Task DeleteAsync(Guid offerId)
        {
            var offer = await _unitOfWork.Offers.GetByIdAsync(offerId)
                ?? throw new KeyNotFoundException($"Offer with ID {offerId} not found.");

            _unitOfWork.Offers.Remove(offer);
            await _unitOfWork.SaveChangesAsync();
        }

        private static OfferResponseDto MapToDto(Offer offer, string donorName)
        {
            return new OfferResponseDto
            {
                OfferId = offer.OfferId,
                DonorOrganizationId = offer.DonorOrganizationId,
                DonorName = donorName,
                AdminId = offer.AdminId,
                Category = offer.Category,
                ProductName = offer.ProductName,
                Quantity = offer.Quantity,
                ProductImage = offer.ProductImage,
                ExpiryDate = offer.ExpiryDate,
                Status = offer.Status,
                CreatedAt = offer.CreatedAt,
                UpdatedAt = offer.UpdatedAt
            };
        }
    }
}
