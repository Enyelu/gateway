using gateway.api.Models.Login;
using gateway.api.Persistence.Database;
using gateway.api.Persistence.Entities;
using gateway.api.Shared;
using gateway.api.Utilities.Token.Interface;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace gateway.api.Commands
{
    public class HandleLogin
    {
        public class Command : IRequest<GenericResponse<LoginResponseDto>>
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class Handler : IRequestHandler<Command, GenericResponse<LoginResponseDto>>
        {
            private readonly ApplicationContext _context;
            private readonly UserManager<AppUser> _userManager;
            private readonly ITokenGenerator _tokenGenerator;

            public Handler(UserManager<AppUser> userManager, ITokenGenerator tokenGenerator, ApplicationContext context)
            {
                _context = context;
                _userManager = userManager;
                _tokenGenerator = tokenGenerator;
            }

            public async Task<GenericResponse<LoginResponseDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByEmailAsync(request.Email);

                if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
                    return GenericResponse<LoginResponseDto>.Fail($"Username or password is invalid", 401);

                if (!await _userManager.IsEmailConfirmedAsync(user) && user.IsActive)
                    return GenericResponse<LoginResponseDto>.Fail($"Account not activated", 403);

                var tenantId = string.Empty;
                var staffId = string.Empty;
                if (user.IsTenantStaff)
                {
                    var tenant = await _context.StaffMembers.Where(x => x.AppUserId.ToString() == user.Id).FirstOrDefaultAsync(cancellationToken);
                    tenantId = tenant?.TenantId;
                    staffId = tenant?.Id;
                }

                var result = new LoginResponseDto()
                {
                    Id = user.Id,
                    Token = await _tokenGenerator.GenerateTokenAsync(user, tenantId, staffId),
                    RefreshToken = Guid.NewGuid()
                };

                user.RefereshTokenExpiry = DateTime.UtcNow.AddDays(1);
                user.RefreshToken = result.RefreshToken.ToString();
                await _userManager.UpdateAsync(user);
                return GenericResponse<LoginResponseDto>.Success("Login was successful!", result);

            }
        }
    }
}