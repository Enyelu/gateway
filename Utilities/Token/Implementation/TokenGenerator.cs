﻿using gateway.api.Models.Token;
using gateway.api.Persistence.Entities;
using gateway.api.Shared;
using gateway.api.Utilities.Token.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace gateway.api.Utilities.Token.Implementation
{
    public class TokenGenerator : ITokenGenerator
    {
        private readonly JWTSettings _jWTSettings;
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public TokenGenerator(RoleManager<ApplicationRole> roleManager, IOptions<JWTSettings> options, UserManager<AppUser> userManager, IConfiguration configuration, IWebHostEnvironment env)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _configuration = configuration;
            _jWTSettings = options.Value;
        }

        public async Task<string> GenerateTokenAsync(AppUser appUser, string tenantId, string staffId)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, appUser.Email),
                new Claim(ClaimTypes.NameIdentifier,appUser.Id),
                new Claim(ClaimTypes.MobilePhone, appUser.PhoneNumber),
                new Claim(ClaimTypes.Name, $"{appUser.FirstName} {appUser.LastName}"),
                new Claim(ClaimTypes.GivenName, appUser.UserName),
                new Claim("TenantId", tenantId ?? ""),
                new Claim("StaffId", staffId ?? ""),
            };

            var roles = await _userManager.GetRolesAsync(appUser);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            foreach (var roleName in roles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    claims.Add(new Claim("RoleDefinitions", $"{role.Name}:{role.Id}"));
                }
            }
            
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jWTSettings.SecretKey));

            var token = new JwtSecurityToken
                (
                    audience: _jWTSettings.Audience,
                    issuer: _jWTSettings.Issuer,
                    signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256),
                    expires: DateTime.Now.AddHours(1),
                    claims: claims
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<GenericResponse<RefreshTokenToReturnDto>> GenerateRefreshTokenAsync(RefreshTokenRequestDto token, string tenantId, string staffId)
        {
            var refreshToken = token.RefreshToken;
            var userId = token.UserId;

            var user = await _userManager.FindByIdAsync(token.UserId);

            if (user.RefreshToken != refreshToken.ToString() || user.RefereshTokenExpiry <= DateTime.Now)
            {
                return GenericResponse<RefreshTokenToReturnDto>.Fail("Invalid request", (int)HttpStatusCode.BadRequest);
            }

            var result = new RefreshTokenToReturnDto
            {
                NewJwtAccessToken = await GenerateTokenAsync(user, tenantId, staffId),
                NewRefreshToken = Guid.NewGuid(),
            };
            user.RefreshToken = result.NewRefreshToken.ToString();
            user.RefereshTokenExpiry = DateTime.Now.AddDays(3);
            await _userManager.UpdateAsync(user);

            return GenericResponse<RefreshTokenToReturnDto>.Success("Token Successfully refreshed", result);
        }
    }
}