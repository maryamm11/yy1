using App.Core.DTOs.Common;
using App.Core.DTOs.Donor;
using App.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DonorsController : ControllerBase
    {
        private readonly IDonorService _donorService;

        public DonorsController(IDonorService donorService)
        {
            _donorService = donorService;
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponseDto<PaginatedResponseDto<DonorResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _donorService.GetAllAsync(pageNumber, pageSize);
            return Ok(ApiResponseDto<PaginatedResponseDto<DonorResponseDto>>.Success(result));
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponseDto<DonorResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<DonorResponseDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _donorService.GetByIdAsync(id);
            if (result == null)
                return NotFound(ApiResponseDto<DonorResponseDto>.Failure("Donor not found."));

            return Ok(ApiResponseDto<DonorResponseDto>.Success(result));
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Donor")]
        [ProducesResponseType(typeof(ApiResponseDto<DonorResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDonorDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponseDto<DonorResponseDto>.Failure("Validation failed.",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));

            var result = await _donorService.UpdateAsync(id, dto);
            return Ok(ApiResponseDto<DonorResponseDto>.Success(result, "Donor updated successfully."));
        }
    }
}
