using App.Core.DTOs.Common;
using App.Core.DTOs.Offer;
using App.Core.Enums;
using App.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OffersController : ControllerBase
    {
        private readonly IOfferService _offerService;

        public OffersController(IOfferService offerService)
        {
            _offerService = offerService;
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponseDto<PaginatedResponseDto<OfferResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] OfferStatus? status = null,
            [FromQuery] string? category = null)
        {
            var result = await _offerService.GetAllAsync(pageNumber, pageSize, status, category);
            return Ok(ApiResponseDto<PaginatedResponseDto<OfferResponseDto>>.Success(result));
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponseDto<OfferResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<OfferResponseDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _offerService.GetByIdAsync(id);
            if (result == null)
                return NotFound(ApiResponseDto<OfferResponseDto>.Failure("Offer not found."));

            return Ok(ApiResponseDto<OfferResponseDto>.Success(result));
        }

        [HttpGet("donor/{donorId:guid}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponseDto<PaginatedResponseDto<OfferResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByDonorId(
            Guid donorId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _offerService.GetByDonorIdAsync(donorId, pageNumber, pageSize);
            return Ok(ApiResponseDto<PaginatedResponseDto<OfferResponseDto>>.Success(result));
        }

        [HttpPost("{donorId:guid}")]
        [Authorize(Roles = "Donor")]
        [ProducesResponseType(typeof(ApiResponseDto<OfferResponseDto>), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(Guid donorId, [FromBody] CreateOfferDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponseDto<OfferResponseDto>.Failure("Validation failed.",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));

            var result = await _offerService.CreateAsync(donorId, dto);
            return CreatedAtAction(nameof(GetById), new { id = result.OfferId },
                ApiResponseDto<OfferResponseDto>.Success(result, "Offer created successfully."));
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Donor")]
        [ProducesResponseType(typeof(ApiResponseDto<OfferResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOfferDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponseDto<OfferResponseDto>.Failure("Validation failed.",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));

            var result = await _offerService.UpdateAsync(id, dto);
            return Ok(ApiResponseDto<OfferResponseDto>.Success(result, "Offer updated successfully."));
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Donor,Admin")]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _offerService.DeleteAsync(id);
            return Ok(ApiResponseDto<object>.Success(null!, "Offer deleted successfully."));
        }
    }
}
