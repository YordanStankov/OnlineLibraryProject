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

            string LibrarianEmail = "Librarian2108@gmail.com";
            string LibrarianPassword = "Librarian2107$";

            string[] roles = { "Librarian", "User" }; 
            foreach (var role in roles)
            {
                if(!await roleManager.RoleExistsAsync(role)) 
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
            var adminUser = await userManager.FindByEmailAsync(LibrarianEmail);
            if (adminUser == null)
            {
                adminUser = new User { UserName = LibrarianEmail, Email = LibrarianEmail};
                await userManager.CreateAsync(adminUser, LibrarianPassword);
                await userManager.AddToRoleAsync(adminUser, "Librarian"); 
                throw new Exception("Admin not present in db");
            }
            else
            {
                await userManager.AddToRoleAsync(adminUser, "Librarian"); 
            }
        }
    }
}
