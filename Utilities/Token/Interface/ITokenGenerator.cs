using gateway.api.Models.Token;
using gateway.api.Persistence.Entities;
using gateway.api.Shared;

namespace gateway.api.Utilities.Token.Interface
{
    public interface ITokenGenerator
    {
        Task<string> GenerateTokenAsync(AppUser appUser, string tenantId, string staffId);
        Task<GenericResponse<RefreshTokenToReturnDto>> GenerateRefreshTokenAsync(RefreshTokenRequestDto token, string tenantId, string staffId);
    }
}