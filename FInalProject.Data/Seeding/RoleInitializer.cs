using Microsoft.AspNetCore.Identity;
using FInalProject.Data.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace FInalProject.Data.Seeding
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

            string LibrarianEmail = "NewLibrarian@gmail.com";
            string LibrarianPassword = "New2107$";

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

            var LibrarianUser = await userManager.FindByEmailAsync(LibrarianEmail);
            if (LibrarianUser == null)
            {
                LibrarianUser = new User { UserName = LibrarianEmail, Email = LibrarianEmail };
                await userManager.CreateAsync(LibrarianUser, LibrarianPassword);
                await userManager.AddToRoleAsync(LibrarianUser, "Librarian");
                throw new Exception("Librarian not present in db");
            }
        }
    }
}
