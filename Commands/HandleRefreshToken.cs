﻿using gateway.api.Models.Token;
using gateway.api.Persistence.Database;
using gateway.api.Persistence.Entities;
using gateway.api.Shared;
using gateway.api.Utilities.Token.Interface;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace gateway.api.Commands
{
    public class HandleRefreshToken
    {
        public class Command : IRequest<GenericResponse<RefreshTokenToReturnDto>>
        {
            public string UserId { get; set; }
            public Guid RefreshToken { get; set; }
        }

        public class Handler : IRequestHandler<Command, GenericResponse<RefreshTokenToReturnDto>>
        {
            private readonly ApplicationContext _context;
            private readonly UserManager<AppUser> _userManager;
            private readonly ITokenGenerator _tokenGenerator;
            private readonly ILogger<Handler> _logger;

            public Handler(UserManager<AppUser> userManager, ITokenGenerator tokenGenerator, ApplicationContext context, ILogger<Handler> logger)
            {
                _context = context;
                _userManager = userManager;
                _tokenGenerator = tokenGenerator;
                _logger = logger;
            }

            public async Task<GenericResponse<RefreshTokenToReturnDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                _logger.LogInformation($"Refresh token Attempt for {request.UserId} at {DateTime.Now} ===> Validating user credentials");
                var user = await _userManager.FindByIdAsync(request.UserId);

                if (user == null)
                    return GenericResponse<RefreshTokenToReturnDto>.Fail($"User unathurized", 401);
 
                var tenantId = string.Empty;
                var staffId = string.Empty;
                if (user.IsTenantStaff)
                {
                    var tenant = await _context.StaffMembers.Where(x => x.AppUserId.ToString() == user.Id).FirstOrDefaultAsync(cancellationToken);
                    tenantId = tenant?.TenantId;
                    staffId = tenant?.Id;
                }

                var result = await _tokenGenerator.GenerateRefreshTokenAsync(new RefreshTokenRequestDto
                {
                    UserId = request.UserId,
                    RefreshToken = request.RefreshToken,
                }, tenantId, staffId);

                _logger.LogInformation($"Refresh token retrieval for {request.UserId} was successful....");
                return result;
            }
        }
    }
}
