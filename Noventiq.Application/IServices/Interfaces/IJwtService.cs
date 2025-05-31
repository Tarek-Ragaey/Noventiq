using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noventiq.Application.IServices.Interfaces
{
    public interface IJwtService
    {
        Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(IdentityUser user);
        Task<(string AccessToken, string RefreshToken)> RefreshTokenAsync(string accessToken, string refreshToken);
        Task RevokeRefreshTokenAsync(string refreshToken);
        bool ValidateAccessToken(string accessToken);
        Task<bool> ValidateRefreshTokenAsync(string refreshToken);
    }
}
