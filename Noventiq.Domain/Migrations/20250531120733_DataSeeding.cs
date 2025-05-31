using Microsoft.EntityFrameworkCore.Migrations;
using System.Data;
using System.Security.Principal;

#nullable disable

namespace Noventiq.Migrations
{
    /// <inheritdoc />
    public partial class DataSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                                    -- 1.Insert Roles
                    DECLARE @SuperAdminRoleId UNIQUEIDENTIFIER = NEWID()
                    DECLARE @AdminRoleId UNIQUEIDENTIFIER = NEWID()
                    DECLARE @UserRoleId UNIQUEIDENTIFIER = NEWID()

                    INSERT INTO AspNetRoles(Id, Name, NormalizedName, ConcurrencyStamp)
                    VALUES
                    (@SuperAdminRoleId, 'Super Admin', 'SUPER ADMIN', NEWID()),
                    (@AdminRoleId, 'Admin', 'ADMIN', NEWID()),
                    (@UserRoleId, 'User', 'USER', NEWID());

                                --2.Insert Role Translations
                    INSERT INTO RoleTranslations(RoleId, LanguageKey, TranslatedName)
                    VALUES
                    -- Super Admin translations
                    (@SuperAdminRoleId, 'en', 'Super Admin'),
                    (@SuperAdminRoleId, 'hi', N'सुपर एडमिन'),

                    --Admin translations
                    (@AdminRoleId, 'en', 'Admin'),
                    (@AdminRoleId, 'hi', N'व्यवस्थापक'),

                    --User translations
                    (@UserRoleId, 'en', 'User'),
                    (@UserRoleId, 'hi', N'उपयोगकर्ता');

                                --3.Insert Admin User
                    DECLARE @AdminUserId UNIQUEIDENTIFIER = NEWID()
                    DECLARE @PasswordHash NVARCHAR(MAX) = N'AQAAAAEAACcQAAAAENnz5dlZo4Eue96u5/GCa2qakq7x9UzTzHRPj+GCVjlNFvDBBBwc2uWLYUN6/G8i8Q=='
                    DECLARE @SecurityStamp UNIQUEIDENTIFIER = NEWID()
                    DECLARE @ConcurrencyStamp UNIQUEIDENTIFIER = NEWID()

                    INSERT INTO AspNetUsers(
                        Id,
                        UserName,
                        NormalizedUserName,
                        Email,
                        NormalizedEmail,
                        EmailConfirmed,
                        PasswordHash,
                        SecurityStamp,
                        ConcurrencyStamp,
                        PhoneNumberConfirmed,
                        TwoFactorEnabled,
                        LockoutEnabled,
                        AccessFailedCount
                    )
                    VALUES(
                        @AdminUserId,
                        'admin@admin.com',
                        'ADMIN@ADMIN.COM',
                        'admin@admin.com',
                        'ADMIN@ADMIN.COM',
                        1, --EmailConfirmed
                        @PasswordHash,
                        @SecurityStamp,
                        @ConcurrencyStamp,
                        0, --PhoneNumberConfirmed
                        0, --TwoFactorEnabled
                        1, --LockoutEnabled
                        0-- AccessFailedCount
                    );

                                --4.Assign Super Admin role to the admin user
                                INSERT INTO AspNetUserRoles(UserId, RoleId)
                    VALUES(@AdminUserId, @SuperAdminRoleId);
            ");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
