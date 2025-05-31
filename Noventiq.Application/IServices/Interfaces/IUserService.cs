using Microsoft.AspNetCore.Identity;
using Noventiq.Application.IServices.Models;
using Noventiq.Application.IServices.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noventiq.Application.IServices.Interfaces
{
    public interface IUserService
    {
        Task<(IEnumerable<IdentityUser> Users, int TotalCount)> GetAllUsersAsync(PaginationParams paginationParams);
        Task<(IdentityUser User, IList<string> Roles)> GetUserByIdAsync(string id);
        Task<IdentityResult> DeleteUserAsync(string id);
        Task<(IdentityResult Result, UserDto User)> CreateUserAsync(CreateUserModel model);
    }
}
