using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Noventiq.Application.IServices.Interfaces;
using Noventiq.Application.IServices.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Noventiq.Application.Services.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IJwtService _jwtService;

        public AuthenticationService(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IJwtService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
        }

        public async Task<AuthResponse> LoginAsync(LoginModel model)
        {
            if (model == null)
            {
                return new AuthResponse
                {
                    IsSuccess = false,
                    Message = "Invalid model"
                };
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new AuthResponse
                {
                    IsSuccess = false,
                    Message = "Invalid credentials"
                };
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (result.Succeeded)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var (token, refreshToken) = await _jwtService.GenerateTokensAsync(user);

                return new AuthResponse
                {
                    IsSuccess = true,
                    Message = "Login successful",
                    Token = token,
                    RefreshToken = refreshToken,
                    UserId = user.Id,
                    Email = user.Email,
                    Roles = userRoles.ToList()
                };
            }

            if (result.IsLockedOut)
            {
                return new AuthResponse
                {
                    IsSuccess = false,
                    Message = "Account is locked out"
                };
            }

            return new AuthResponse
            {
                IsSuccess = false,
                Message = "Invalid credentials"
            };
        }

        public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenModel model)
        {
            try
            {
                if (model == null)
                {
                    return new AuthResponse
                    {
                        IsSuccess = false,
                        Message = "Invalid model"
                    };
                }

                var (newToken, newRefreshToken) = await _jwtService.RefreshTokenAsync(
                    model.AccessToken,
                    model.RefreshToken
                );

                return new AuthResponse
                {
                    IsSuccess = true,
                    Message = "Token refreshed successfully",
                    Token = newToken,
                    RefreshToken = newRefreshToken
                };
            }
            catch (Exception ex)
            {
                return new AuthResponse
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<AuthResponse> LogoutAsync()
        {
            await _signInManager.SignOutAsync();
            return new AuthResponse
            {
                IsSuccess = true,
                Message = "Logged out successfully"
            };
        }

        public async Task<AuthResponse> ValidateTokenAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new AuthResponse
                {
                    IsSuccess = false,
                    Message = "Invalid token"
                };
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            return new AuthResponse
            {
                IsSuccess = true,
                Message = "Token is valid",
                UserId = user.Id,
                Email = user.Email,
                Roles = userRoles.ToList()
            };
        }
    }
} 