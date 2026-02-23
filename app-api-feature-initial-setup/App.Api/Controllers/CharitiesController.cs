using App.Core.DTOs.Charity;
using App.Core.DTOs.Common;
using App.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CharitiesController : ControllerBase
    {
        private readonly ICharityService _charityService;

        public CharitiesController(ICharityService charityService)
        {
            _charityService = charityService;
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponseDto<PaginatedResponseDto<CharityResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _charityService.GetAllAsync(pageNumber, pageSize);
            return Ok(ApiResponseDto<PaginatedResponseDto<CharityResponseDto>>.Success(result));
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponseDto<CharityResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<CharityResponseDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _charityService.GetByIdAsync(id);
            if (result == null)
                return NotFound(ApiResponseDto<CharityResponseDto>.Failure("Charity not found."));

            return Ok(ApiResponseDto<CharityResponseDto>.Success(result));
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Charity")]
        [ProducesResponseType(typeof(ApiResponseDto<CharityResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCharityDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponseDto<CharityResponseDto>.Failure("Validation failed.",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));

            var result = await _charityService.UpdateAsync(id, dto);
            return Ok(ApiResponseDto<CharityResponseDto>.Success(result, "Charity updated successfully."));
        }
    }
}
