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
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoles([FromQuery] PaginationParams paginationParams)
        {
            // Get language from HttpContext.Items (set by middleware)
            paginationParams.LanguageKey = HttpContext.Items["LanguageKey"]?.ToString() ?? "en";

            var (roles, totalCount) = await _roleService.GetAllRolesAsync(paginationParams);

            var paginationHeader = new PaginationHeader(
                paginationParams.PageNumber,
                paginationParams.PageSize,
                totalCount,
                (int)Math.Ceiling(totalCount / (double)paginationParams.PageSize)
            );

            Response.AddPaginationHeader(paginationHeader);

            return Ok(roles);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] RoleModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Message = "Invalid model state" });

            var result = await _roleService.CreateRoleAsync(model);
            if (result.Succeeded)
                return Ok(new { Message = "Role created successfully with translations" });

            return BadRequest(new { Message = string.Join(", ", result.Errors.Select(e => e.Description)) });
        }

        [HttpPut("{currentName}")]
        public async Task<IActionResult> UpdateRole(string currentName, [FromBody] RoleUpdateModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Message = "Invalid model state" });

            var result = await _roleService.UpdateRoleAsync(currentName, model);
            if (result.Succeeded)
                return Ok(new { Message = "Role updated successfully with translations" });

            return BadRequest(new { Message = string.Join(", ", result.Errors.Select(e => e.Description)) });
        }

        [HttpDelete("{name}")]
        public async Task<IActionResult> DeleteRole(string name)
        {
            var result = await _roleService.DeleteRoleAsync(name);
            if (result.Succeeded)
                return Ok(new { Message = "Role deleted successfully" });

            return BadRequest(new { Message = string.Join(", ", result.Errors.Select(e => e.Description)) });
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignRoleToUser([FromBody] UserRoleModel model)
        {
            var result = await _roleService.AssignRoleToUserAsync(model.UserId, model.RoleName);
            if (result.Succeeded)
                return Ok(new { Message = "Role assigned successfully" });

            return BadRequest(new { Message = string.Join(", ", result.Errors.Select(e => e.Description)) });
        }

        [HttpPost("remove")]
        public async Task<IActionResult> RemoveRoleFromUser([FromBody] UserRoleModel model)
        {
            var result = await _roleService.RemoveRoleFromUserAsync(model.UserId, model.RoleName);
            if (result.Succeeded)
                return Ok(new { Message = "Role removed successfully" });

            return BadRequest(new { Message = string.Join(", ", result.Errors.Select(e => e.Description)) });
        }
    }
}
