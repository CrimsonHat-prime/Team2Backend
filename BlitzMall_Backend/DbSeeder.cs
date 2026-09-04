using BlitzMall_Backend.Data;
using BlitzMall_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace BlitzMall_Backend
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(
            AppDbContext db,
            IConfiguration configuration)
        {
            var roles = new[]
            {
                new { Name = "Buyer", Description = "Buyer" },
                new { Name = "Seller", Description = "Seller" },
                new { Name = "Admin", Description = "Administrator" }
            };

            foreach (var roleData in roles)
            {
                if (!await db.Roles.AnyAsync(r => r.Name == roleData.Name))
                {
                    db.Roles.Add(new Role
                    {
                        Name = roleData.Name,
                        Description = roleData.Description
                    });
                }
            }

            await db.SaveChangesAsync();

            var adminRole = await db.Roles
                .FirstAsync(r => r.Name == "Admin");

            var adminEmail = configuration["Admin:Email"];
            var adminPassword = configuration["Admin:Password"];

            if (string.IsNullOrWhiteSpace(adminEmail) ||
                string.IsNullOrWhiteSpace(adminPassword))
            {
                return;
            }

            var adminExists = await db.Users
                .AnyAsync(u => u.Email == adminEmail);

            if (!adminExists)
            {
                var admin = new User
                {
                    Name = "Admin",
                    Email = adminEmail,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
                    Status = "Active",
                    RoleId = adminRole.Id,
                    CreatedAt = DateTime.UtcNow
                };

                db.Users.Add(admin);
                await db.SaveChangesAsync();
            }
        }
    }
}