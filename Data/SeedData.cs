using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace mentor.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        // Tạo vai trò Administrator nếu chưa tồn tại
        if (!await roleManager.RoleExistsAsync("Administrator"))
        {
            await roleManager.CreateAsync(new IdentityRole("Administrator"));
        }

        // Kiểm tra xem tài khoản admin đã tồn tại chưa
        var adminUser = await userManager.FindByNameAsync("admin");
        if (adminUser == null)
        {
            // Tạo tài khoản admin mới
            adminUser = new IdentityUser
            {
                UserName = "admin",
                Email = "admin@example.com",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(adminUser, "Admin@123");

            // Gán vai trò Administrator cho tài khoản admin
            await userManager.AddToRoleAsync(adminUser, "Administrator");
        }
    }
}