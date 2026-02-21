using App.Core.DTOs.Common;
using App.Core.DTOs.Need;
using App.Core.Enums;
using App.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NeedsController : ControllerBase
    {
        private readonly INeedService _needService;

        public NeedsController(INeedService needService)
        {
            _needService = needService;
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponseDto<PaginatedResponseDto<NeedResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] NeedStatus? status = null,
            [FromQuery] string? category = null)
        {
            var result = await _needService.GetAllAsync(pageNumber, pageSize, status, category);
            return Ok(ApiResponseDto<PaginatedResponseDto<NeedResponseDto>>.Success(result));
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponseDto<NeedResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<NeedResponseDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _needService.GetByIdAsync(id);
            if (result == null)
                return NotFound(ApiResponseDto<NeedResponseDto>.Failure("Need not found."));

            return Ok(ApiResponseDto<NeedResponseDto>.Success(result));
        }

        [HttpGet("charity/{charityId:guid}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponseDto<PaginatedResponseDto<NeedResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByCharityId(
            Guid charityId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _needService.GetByCharityIdAsync(charityId, pageNumber, pageSize);
            return Ok(ApiResponseDto<PaginatedResponseDto<NeedResponseDto>>.Success(result));
        }

        [HttpPost("{charityId:guid}")]
        [Authorize(Roles = "Charity")]
        [ProducesResponseType(typeof(ApiResponseDto<NeedResponseDto>), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(Guid charityId, [FromBody] CreateNeedDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponseDto<NeedResponseDto>.Failure("Validation failed.",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));

            var result = await _needService.CreateAsync(charityId, dto);
            return CreatedAtAction(nameof(GetById), new { id = result.CharityNeedId },
                ApiResponseDto<NeedResponseDto>.Success(result, "Need created successfully."));
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Charity")]
        [ProducesResponseType(typeof(ApiResponseDto<NeedResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateNeedDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponseDto<NeedResponseDto>.Failure("Validation failed.",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));

            var result = await _needService.UpdateAsync(id, dto);
            return Ok(ApiResponseDto<NeedResponseDto>.Success(result, "Need updated successfully."));
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Charity,Admin")]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _needService.DeleteAsync(id);
            return Ok(ApiResponseDto<object>.Success(null!, "Need deleted successfully."));
        }

        [HttpPatch("{id:guid}/status")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponseDto<NeedResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromQuery] NeedStatus status)
        {
            var result = await _needService.UpdateStatusAsync(id, status);
            return Ok(ApiResponseDto<NeedResponseDto>.Success(result, "Need status updated successfully."));
        }
    }
}
