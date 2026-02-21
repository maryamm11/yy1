using App.Core.DTOs.Common;
using App.Core.DTOs.Need;
using App.Core.Entities;
using App.Core.Enums;
using App.Core.Interfaces;

namespace App.Infrastructure.Services
{
    public class NeedService : INeedService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NeedService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<NeedResponseDto?> GetByIdAsync(Guid needId)
        {
            var need = await _unitOfWork.CharityNeeds.GetByIdAsync(needId);
            if (need == null) return null;

            var charity = await _unitOfWork.Charities.GetByIdAsync(need.CharityId);
            return MapToDto(need, charity?.CharityName ?? "Unknown");
        }

        public async Task<PaginatedResponseDto<NeedResponseDto>> GetAllAsync(
            int pageNumber, int pageSize, NeedStatus? status = null, string? category = null)
        {
            var needs = await _unitOfWork.CharityNeeds.GetAllAsync();
            var filtered = needs.AsEnumerable();

            if (status.HasValue)
                filtered = filtered.Where(n => n.Status == status.Value);
            if (!string.IsNullOrEmpty(category))
                filtered = filtered.Where(n => n.Category.Equals(category, StringComparison.OrdinalIgnoreCase));

            var totalCount = filtered.Count();
            var pagedNeeds = filtered
                .OrderByDescending(n => n.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var charityIds = pagedNeeds.Select(n => n.CharityId).Distinct();
            var charities = new Dictionary<Guid, string>();
            foreach (var cId in charityIds)
            {
                var charity = await _unitOfWork.Charities.GetByIdAsync(cId);
                if (charity != null)
                    charities[cId] = charity.CharityName;
            }

            var items = pagedNeeds.Select(n =>
                MapToDto(n, charities.GetValueOrDefault(n.CharityId, "Unknown")));

            return new PaginatedResponseDto<NeedResponseDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<PaginatedResponseDto<NeedResponseDto>> GetByCharityIdAsync(
            Guid charityId, int pageNumber, int pageSize)
        {
            var charity = await _unitOfWork.Charities.GetByIdAsync(charityId);
            var charityName = charity?.CharityName ?? "Unknown";

            var needs = await _unitOfWork.CharityNeeds.FindAsync(n => n.CharityId == charityId);
            var totalCount = needs.Count();

            var pagedNeeds = needs
                .OrderByDescending(n => n.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(n => MapToDto(n, charityName));

            return new PaginatedResponseDto<NeedResponseDto>
            {
                Items = pagedNeeds,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<NeedResponseDto> CreateAsync(Guid charityId, CreateNeedDto dto)
        {
            var charity = await _unitOfWork.Charities.GetByIdAsync(charityId)
                ?? throw new KeyNotFoundException($"Charity with ID {charityId} not found.");

            var need = new CharityNeed
            {
                CharityNeedId = Guid.NewGuid(),
                CharityId = charityId,
                Category = dto.Category,
                ProductName = dto.ProductName,
                Quantity = dto.Quantity,
                Priority = dto.Priority,
                Status = NeedStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.CharityNeeds.AddAsync(need);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(need, charity.CharityName);
        }

        public async Task<NeedResponseDto> UpdateAsync(Guid needId, UpdateNeedDto dto)
        {
            var need = await _unitOfWork.CharityNeeds.GetByIdAsync(needId)
                ?? throw new KeyNotFoundException($"Need with ID {needId} not found.");

            need.Category = dto.Category;
            need.ProductName = dto.ProductName;
            need.Quantity = dto.Quantity;
            need.Priority = dto.Priority;

            _unitOfWork.CharityNeeds.Update(need);
            await _unitOfWork.SaveChangesAsync();

            var charity = await _unitOfWork.Charities.GetByIdAsync(need.CharityId);
            return MapToDto(need, charity?.CharityName ?? "Unknown");
        }

        public async Task DeleteAsync(Guid needId)
        {
            var need = await _unitOfWork.CharityNeeds.GetByIdAsync(needId)
                ?? throw new KeyNotFoundException($"Need with ID {needId} not found.");

            _unitOfWork.CharityNeeds.Remove(need);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<NeedResponseDto> UpdateStatusAsync(Guid needId, NeedStatus status, Guid? adminId = null)
        {
            var need = await _unitOfWork.CharityNeeds.GetByIdAsync(needId)
                ?? throw new KeyNotFoundException($"Need with ID {needId} not found.");

            need.Status = status;
            if (adminId.HasValue)
                need.AdminId = adminId;

            _unitOfWork.CharityNeeds.Update(need);
            await _unitOfWork.SaveChangesAsync();

            var charity = await _unitOfWork.Charities.GetByIdAsync(need.CharityId);
            return MapToDto(need, charity?.CharityName ?? "Unknown");
        }

        private static NeedResponseDto MapToDto(CharityNeed need, string charityName)
        {
            return new NeedResponseDto
            {
                CharityNeedId = need.CharityNeedId,
                CharityId = need.CharityId,
                CharityName = charityName,
                AdminId = need.AdminId,
                Category = need.Category,
                ProductName = need.ProductName,
                Quantity = need.Quantity,
                Priority = need.Priority,
                Status = need.Status,
                CreatedAt = need.CreatedAt,
                UpdatedAt = need.UpdatedAt
            };
        }
    }
}
