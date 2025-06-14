﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Noventiq.Application.IServices.Interfaces;
using Noventiq.Application.IServices.Models;
using Noventiq.Application.IServices.Models.Common;
using Noventiq.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noventiq.Application.Services.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public UserService(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<(IdentityResult Result, UserDto User)> CreateUserAsync(CreateUserModel model)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = new IdentityUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    EmailConfirmed = true // Since this is admin creating the user
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                    return (result, null);

                if (model.Roles?.Any() == true)
                {
                    result = await _userManager.AddToRolesAsync(user, model.Roles);
                    if (!result.Succeeded)
                    {
                        // Rollback if role assignment fails
                        await transaction.RollbackAsync();
                        return (result, null);
                    }
                }

                await transaction.CommitAsync();

                // Return the created user with roles
                var roles = await _userManager.GetRolesAsync(user);
                var userDto = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    Roles = roles.ToList()
                };

                return (IdentityResult.Success, userDto);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return (IdentityResult.Failed(new IdentityError { Description = "Failed to create user" }), null);
            }
        }

        public async Task<(IEnumerable<UserListDto> Users, int TotalCount)> GetAllUsersAsync(PaginationParams paginationParams)
        {
            // Create a query that joins users with their roles
            var query = from user in _userManager.Users
                        join userRole in _context.UserRoles
                            on user.Id equals userRole.UserId into userRoles
                        from ur in userRoles.DefaultIfEmpty()
                        join role in _context.Roles
                            on ur.RoleId equals role.Id into roles
                        group new { user, role = roles.FirstOrDefault() } by user into g
                        select new
                        {
                            User = g.Key,
                            Roles = g.Select(x => x.role.Name)
                                .Where(name => name != null)
                                .ToList()
                        };

            // Get total count
            var totalCount = await _userManager.Users.CountAsync();

            // Get paginated users with their roles
            var usersWithRoles = await query
                .OrderBy(u => u.User.UserName)
                .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize)
                .Select(x => new UserListDto
                {
                    Id = x.User.Id,
                    UserName = x.User.UserName,
                    Email = x.User.Email,
                    Roles = x.Roles
                })
                .ToListAsync();

            return (usersWithRoles, totalCount);
        }

        public async Task<(IdentityUser User, IList<string> Roles)> GetUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return (null, null);

            var roles = await _userManager.GetRolesAsync(user);
            return (user, roles);
        }


        public async Task<IdentityResult> DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });

            return await _userManager.DeleteAsync(user);
        }
    }
}
