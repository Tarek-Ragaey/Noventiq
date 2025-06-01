using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Noventiq.Application.IServices.Interfaces;
using System.Threading.Tasks;
using System.Security.Claims;
using Noventiq.Application.IServices.Models;

namespace Noventiq.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
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

            var response = await _authenticationService.LoginAsync(model);
            if (!response.IsSuccess)
            {
                return response.Message == "Account is locked out" 
                    ? BadRequest(response) 
                    : Unauthorized(response);
            }

            return Ok(response);
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
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthResponse
                {
                    IsSuccess = false,
                    Message = "Invalid model state"
                });
            }

            var response = await _authenticationService.RefreshTokenAsync(model);
            if (!response.IsSuccess)
            {
                return Unauthorized(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Logs out the current user
        /// </summary>
        [Authorize]
        [HttpPost("logout")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout()
        {
            var response = await _authenticationService.LogoutAsync();
            return Ok(response);
        }
    }
}
