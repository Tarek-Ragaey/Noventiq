using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Noventiq.Application.IServices.Interfaces;
using Noventiq.Application.IServices.Models;

namespace Noventiq.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IJwtService _jwtService;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthenticationController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IJwtService jwtService,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _roleManager = roleManager;
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthResponse 
                { 
                    IsSuccess = false, 
                    Message = "Invalid model state",
                    Errors = ModelState.ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.FirstOrDefault()?.ErrorMessage
                    )
                });
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized(new AuthResponse 
                { 
                    IsSuccess = false, 
                    Message = "Invalid credentials" 
                });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (result.Succeeded)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var (token, refreshToken) = await _jwtService.GenerateTokensAsync(user);

                return Ok(new AuthResponse
                {
                    IsSuccess = true,
                    Message = "Login successful",
                    Token = token,
                    RefreshToken = refreshToken,
                    UserId = user.Id,
                    Email = user.Email,
                    Roles = userRoles.ToList()
                });
            }

            if (result.IsLockedOut)
            {
                return BadRequest(new AuthResponse 
                { 
                    IsSuccess = false, 
                    Message = "Account is locked out" 
                });
            }

            return Unauthorized(new AuthResponse 
            { 
                IsSuccess = false, 
                Message = "Invalid credentials" 
            });
        }

        /// <summary>
        /// Refreshes an expired JWT token
        /// </summary>
        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new AuthResponse 
                    { 
                        IsSuccess = false, 
                        Message = "Invalid model state" 
                    });
                }

                var (newToken, newRefreshToken) = await _jwtService.RefreshTokenAsync(
                    model.AccessToken, 
                    model.RefreshToken
                );

                return Ok(new AuthResponse
                {
                    IsSuccess = true,
                    Message = "Token refreshed successfully",
                    Token = newToken,
                    RefreshToken = newRefreshToken
                });
            }
            catch (Exception ex)
            {
                return Unauthorized(new AuthResponse 
                { 
                    IsSuccess = false, 
                    Message = ex.Message 
                });
            }
        }

        /// <summary>
        /// Logs out the current user
        /// </summary>
        [Authorize]
        [HttpPost("logout")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new AuthResponse 
            { 
                IsSuccess = true, 
                Message = "Logged out successfully" 
            });
        }

        /// <summary>
        /// Validates the current token
        /// </summary>
        [Authorize]
        [HttpGet("validate-token")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ValidateToken()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized(new AuthResponse 
                { 
                    IsSuccess = false, 
                    Message = "Invalid token" 
                });
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            return Ok(new AuthResponse
            {
                IsSuccess = true,
                Message = "Token is valid",
                UserId = user.Id,
                Email = user.Email,
                Roles = userRoles.ToList()
            });
        }
    }
}
