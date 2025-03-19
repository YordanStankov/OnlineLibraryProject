using Microsoft.AspNetCore.Identity;
using FInalProject.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FInalProject
{
    public static class RoleInitializer
    {
        public static async Task SeedRolesAndAdminAsync(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var serviceProvider = scope.ServiceProvider.GetRequiredService<IServiceProvider>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            string AdminEmail = "Admin2108@gmail.com";
            string AdminPassword = "Admin2107$";

            string[] roles = { "Librarian", "User", "Admin"}; 
            foreach (var role in roles)
            {
                if(!await roleManager.RoleExistsAsync(role)) 
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
            var adminUser = await userManager.FindByEmailAsync(AdminEmail);
            if (adminUser == null)
            {
                adminUser = new User { UserName = AdminEmail, Email = AdminEmail};
                await userManager.CreateAsync(adminUser, AdminPassword);
                await userManager.AddToRoleAsync(adminUser, "Admin"); 
                throw new Exception("Admin not present in db");
            }
            else
            {
                await userManager.AddToRoleAsync(adminUser, "Admin"); 
            }
        }
    }
}
