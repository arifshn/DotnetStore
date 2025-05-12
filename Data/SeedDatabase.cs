using Microsoft.AspNetCore.Identity;
using dotnet_store.Models;

namespace dotnet_store.Data
{
    public static class SeedDatabase
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();


            string[] roleNames = { "Admin", "User" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var roleResult = await roleManager.CreateAsync(new AppRole { Name = roleName });
                    if (!roleResult.Succeeded)
                    {
                        Console.WriteLine($"Rol oluşturma hatası ({roleName}): {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                    }
                    else
                    {
                        Console.WriteLine($"Rol oluşturuldu: {roleName}");
                    }
                }
            }


            var admin = new AppUser
            {
                UserName = "arifsahin",
                Email = "habibarifsahin@gmail.com",
                AdSoyad = "Arif Sahin"
            };
            string adminPassword = "123456";

            if (await userManager.FindByEmailAsync(admin.Email) == null)
            {
                var adminResult = await userManager.CreateAsync(admin, adminPassword);
                if (adminResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                    Console.WriteLine("Admin kullanıcı oluşturuldu: arifsahin");
                }
                else
                {
                    Console.WriteLine($"Admin kullanıcı oluşturma hatası: {string.Join(", ", adminResult.Errors.Select(e => e.Description))}");
                }
            }

            var user = new AppUser
            {
                UserName = "kullanici@example.com",
                Email = "kullanici@example.com",
                AdSoyad = "Test Kullanıcı"
            };
            string userPassword = "Test123!";

            if (await userManager.FindByEmailAsync(user.Email) == null)
            {
                var userResult = await userManager.CreateAsync(user, userPassword);
                if (userResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "User");
                    Console.WriteLine("Normal kullanıcı oluşturuldu: kullanici@example.com");
                }
                else
                {
                    Console.WriteLine($"Normal kullanıcı oluşturma hatası: {string.Join(", ", userResult.Errors.Select(e => e.Description))}");
                }
            }
        }
    }
}