using App.Core.DTOs.Auth;
using App.Core.DTOs.Common;
using App.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponseDto<AuthResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<AuthResponseDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponseDto<AuthResponseDto>.Failure("Validation failed.",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));

            var result = await _authService.RegisterAsync(registerDto);

            if (!result.IsSuccess)
                return BadRequest(ApiResponseDto<AuthResponseDto>.Failure("Registration failed.", result.Errors));

            return Ok(ApiResponseDto<AuthResponseDto>.Success(result, "Registration successful."));
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponseDto<AuthResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<AuthResponseDto>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponseDto<AuthResponseDto>.Failure("Validation failed.",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));

            var result = await _authService.LoginAsync(loginDto);

            if (!result.IsSuccess)
                return Unauthorized(ApiResponseDto<AuthResponseDto>.Failure("Login failed.", result.Errors));

            return Ok(ApiResponseDto<AuthResponseDto>.Success(result, "Login successful."));
        }
    }
}
