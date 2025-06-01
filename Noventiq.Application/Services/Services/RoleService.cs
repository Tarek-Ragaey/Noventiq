using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Noventiq.Application.IServices.Interfaces;
using Noventiq.Application.IServices.Models;
using Noventiq.Application.IServices.Models.Common;
using Noventiq.Domain;
using Noventiq.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noventiq.Application.Services.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;
        public RoleService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }
        public async Task<IdentityResult> CreateRoleAsync(RoleModel model)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (await _roleManager.RoleExistsAsync(model.Name))
                    return IdentityResult.Failed(new IdentityError { Description = "Role already exists" });

                // Create the role
                var role = new IdentityRole(model.Name);
                var result = await _roleManager.CreateAsync(role);
                if (!result.Succeeded)
                    return result;

                // Add translations
                foreach (var translation in model.Translations)
                {
                    var roleTranslation = new RoleTranslation
                    {
                        RoleId = role.Id,
                        LanguageKey = translation.Key,
                        TranslatedName = translation.Value
                    };
                    _context.RoleTranslations.Add(roleTranslation);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return IdentityResult.Success;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return IdentityResult.Failed(new IdentityError { Description = "Failed to create role with translations" });
            }
        }

        public async Task<IdentityResult> UpdateRoleAsync(string currentName, RoleUpdateModel model)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var role = await _roleManager.FindByNameAsync(currentName);
                if (role == null)
                    return IdentityResult.Failed(new IdentityError { Description = "Role not found" });

                // Update role name if provided and different
                if (!string.IsNullOrEmpty(model.NewName) && model.NewName != currentName)
                {
                    role.Name = model.NewName;
                    var updateResult = await _roleManager.UpdateAsync(role);
                    if (!updateResult.Succeeded)
                        return updateResult;
                }

                // Update translations
                var existingTranslations = await _context.RoleTranslations
                    .Where(rt => rt.RoleId == role.Id)
                    .ToListAsync();

                // Remove translations that are not in the update model
                foreach (var existing in existingTranslations)
                {
                    if (!model.Translations.ContainsKey(existing.LanguageKey))
                    {
                        _context.RoleTranslations.Remove(existing);
                    }
                }

                // Update or add new translations
                foreach (var translation in model.Translations)
                {
                    var existingTranslation = existingTranslations
                        .FirstOrDefault(t => t.LanguageKey == translation.Key);

                    if (existingTranslation != null)
                    {
                        existingTranslation.TranslatedName = translation.Value;
                    }
                    else
                    {
                        _context.RoleTranslations.Add(new RoleTranslation
                        {
                            RoleId = role.Id,
                            LanguageKey = translation.Key,
                            TranslatedName = translation.Value
                        });
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return IdentityResult.Success;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return IdentityResult.Failed(new IdentityError { Description = "Failed to update role with translations" });
            }
        }
        public async Task<IdentityResult> DeleteRoleAsync(string roleName)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role == null)
                    return IdentityResult.Failed(new IdentityError { Description = "Role not found" });

                // First delete the translations
                var translations = await _context.RoleTranslations
                    .Where(rt => rt.RoleId == role.Id)
                    .ToListAsync();

                if (translations.Any())
                {
                    _context.RoleTranslations.RemoveRange(translations);
                    await _context.SaveChangesAsync();
                }

                // Then delete the role
                var result = await _roleManager.DeleteAsync(role);
                if (!result.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return result;
                }

                await transaction.CommitAsync();
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return IdentityResult.Failed(new IdentityError 
                { 
                    Description = $"Failed to delete role: {ex.Message}" 
                });
            }
        }

        public async Task<IdentityResult> AssignRoleToUserAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });

            if (!await _roleManager.RoleExistsAsync(roleName))
                return IdentityResult.Failed(new IdentityError { Description = "Role not found" });

            if (await _userManager.IsInRoleAsync(user, roleName))
                return IdentityResult.Failed(new IdentityError { Description = "User already has this role" });

            return await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<IdentityResult> RemoveRoleFromUserAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });

            if (!await _roleManager.RoleExistsAsync(roleName))
                return IdentityResult.Failed(new IdentityError { Description = "Role not found" });

            if (!await _userManager.IsInRoleAsync(user, roleName))
                return IdentityResult.Failed(new IdentityError { Description = "User doesn't have this role" });

            return await _userManager.RemoveFromRoleAsync(user, roleName);
        }

        public async Task<bool> RoleExistsAsync(string roleName)
        {
            return await _roleManager.RoleExistsAsync(roleName);
        }

        public async Task<(IEnumerable<RoleWithTranslationDto> Roles, int TotalCount)> GetAllRolesAsync(PaginationParams paginationParams)
        {
            var languageKey = string.IsNullOrEmpty(paginationParams.LanguageKey) ? "en" : paginationParams.LanguageKey.ToLower();

            // Get roles with their translations
            var query = from role in _roleManager.Roles
                        join rt in _context.RoleTranslations
                        on role.Id equals rt.RoleId into translations
                        from translation in translations.DefaultIfEmpty()
                        where translation == null || translation.LanguageKey == languageKey
                        select new RoleWithTranslationDto
                        {
                            Id = role.Id,
                            Name = role.Name,
                            TranslatedName = translation != null ? translation.TranslatedName : role.Name
                        };

            // Get total count
            var totalCount = await query.CountAsync();

            // Apply pagination
            var roles = await query
                .OrderBy(r => r.Name)
                .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize)
                .ToListAsync();

            return (roles, totalCount);
        }
    }
}



