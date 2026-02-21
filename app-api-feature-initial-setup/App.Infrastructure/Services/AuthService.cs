using App.Core.DTOs.Auth;
using App.Core.Entities;
using App.Core.IdentityEntities;
using App.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace App.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager,
            IUnitOfWork unitOfWork,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Errors = new[] { "A user with this email already exists." }
                };
            }

            var user = new ApplicationUser
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                Whatsapp = registerDto.Whatsapp,
                City = registerDto.City,
                Governorate = registerDto.Governorate,
                PostalCode = registerDto.PostalCode,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Errors = result.Errors.Select(e => e.Description)
                };
            }

            // Ensure role exists and assign
            var roleName = registerDto.UserType == "Charity" ? "Charity" : "Donor";
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new ApplicationRole { Name = roleName });
            }
            await _userManager.AddToRoleAsync(user, roleName);

            // Create the associated organization entity
            if (registerDto.UserType == "Charity")
            {
                var charity = new Charity
                {
                    CharityId = Guid.NewGuid(),
                    CharityName = registerDto.OrganizationName ?? registerDto.UserName,
                    CharityDescription = registerDto.OrganizationDescription ?? string.Empty,
                    IsVerified = false,
                    IsActive = true,
                    UserId = user.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _unitOfWork.Charities.AddAsync(charity);
            }
            else
            {
                var donor = new DonorOrganization
                {
                    DonorId = Guid.NewGuid(),
                    DonorName = registerDto.OrganizationName ?? registerDto.UserName,
                    IsVerified = false,
                    IsActive = true,
                    UserId = user.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _unitOfWork.DonorOrganizations.AddAsync(donor);
            }

            await _unitOfWork.SaveChangesAsync();

            var token = await GenerateJwtToken(user);

            return new AuthResponseDto
            {
                IsSuccess = true,
                Token = token,
                Email = user.Email,
                UserName = user.UserName,
                UserType = roleName,
                UserId = user.Id
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Errors = new[] { "Invalid email or password." }
                };
            }

            if (!user.IsActive)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Errors = new[] { "Your account has been deactivated." }
                };
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Errors = new[] { "Invalid email or password." }
                };
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = await GenerateJwtToken(user);

            return new AuthResponseDto
            {
                IsSuccess = true,
                Token = token,
                Email = user.Email,
                UserName = user.UserName,
                UserType = roles.FirstOrDefault(),
                UserId = user.Id
            };
        }

        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured.")));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(
                    double.Parse(_configuration["Jwt:ExpirationInHours"] ?? "24")),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
