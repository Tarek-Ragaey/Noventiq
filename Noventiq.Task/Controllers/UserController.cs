using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Noventiq.Application.IServices.Extensions;
using Noventiq.Application.IServices.Interfaces;
using Noventiq.Application.IServices.Models;
using Noventiq.Application.IServices.Models.Common;

namespace Noventiq.API.Controllers
{
    [Authorize(Roles = "Admin,Super Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Message = "Invalid model state" });

            var (result, user) = await _userService.CreateUserAsync(model);
            if (result.Succeeded)
                return Ok(new { Message = "User created successfully", User = user });

            return BadRequest(new { Message = string.Join(", ", result.Errors.Select(e => e.Description)) });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] PaginationParams paginationParams)
        {
            // Get language from HttpContext.Items (set by middleware)
            paginationParams.LanguageKey = HttpContext.Items["LanguageKey"]?.ToString() ?? "en";

            var (users, totalCount) = await _userService.GetAllUsersAsync(paginationParams);

            var paginationHeader = new PaginationHeader(
                paginationParams.PageNumber,
                paginationParams.PageSize,
                totalCount,
                (int)Math.Ceiling(totalCount / (double)paginationParams.PageSize)
            );

            Response.AddPaginationHeader(paginationHeader);

            return Ok(users.Select(u => new { u.Id, u.UserName, u.Email }));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var (user, roles) = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(new { Message = "User not found" });

            return Ok(new { user.Id, user.UserName, user.Email, Roles = roles });
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (result.Succeeded)
                return Ok(new { Message = "User deleted successfully" });

            return BadRequest(new { Message = string.Join(", ", result.Errors.Select(e => e.Description)) });
        }
    }
}
