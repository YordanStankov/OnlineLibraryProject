using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FInalProject.Application.Services;
using FInalProject.Infrastructure.Seeding;
using FInalProject.Domain.Models;
using Microsoft.AspNetCore.Authentication.Google;
using FInalProject.Application.Interfaces;
using FInalProject.Infrastructure.Repositories;
using FInalProject.Infrastructure;


namespace FInalProject.Web
{
    public class Program
    {

        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddAntiforgery(options =>
            {
                options.HeaderName = "RequestVerificationToken";
            });
            //registers the roles
            builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedEmail = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddControllersWithViews();

            //Registering the services
            builder.Services.AddScoped<IBooksService, BooksService>();
            builder.Services.AddScoped<IHomeService, HomeService>();
            builder.Services.AddScoped<IGenreService, GenreService>();
            builder.Services.AddScoped<IAuthorsService, AuthorsService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IAdminService, AdminService>();
            builder.Services.AddScoped<IBookCRUDService, BookCRUDService>();
            builder.Services.AddScoped<IBookFilteringService, BookFilteringService>();
            builder.Services.AddScoped<IBookHandlingService, BookHandlingService>();
            builder.Services.AddScoped<ICommentService, CommentService>();
            builder.Services.AddScoped<IFavouriteService, FavouriteService>();


            //Repository pattern implementation
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IBookRepository, BookRepository>();
            builder.Services.AddScoped<IGenreRepository, GenreRepository>();
            builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
            builder.Services.AddScoped<IBookGenreRepository, BookGenreRepository>();
            builder.Services.AddScoped<IBorrowedBookRepository, BorrowedBookRepository>();
            builder.Services.AddScoped<ICommentRepository, CommentRepository>();
            builder.Services.AddScoped<IFavouriteRepository, FavouriteRepository>();
            builder.Services.AddScoped<IFavouriteAuthorRepository, FavouriteAuthorRepository>();


            builder.Services.AddHostedService<BorrowedBooksService>();

            builder.Services.AddAuthentication(opts =>
            {
                opts.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
                opts.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                opts.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })

        .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
        {
         options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
         options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
         options.CallbackPath = "/signin-google";
            // tell Google to use Identity’s External cookie
         options.SignInScheme = IdentityConstants.ExternalScheme;
         options.Scope.Add("email");
         options.SaveTokens = true;
        });

            //the app gets built
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            await app.SeedRolesAndAdminAsync();
            await app.SeedBooksAuthorsAndRest();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStatusCodePagesWithReExecute("/UserErrors/NotFound404");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();
            using (var scope = app.Services.CreateScope())
            {

            }

            app.Run();
        }
    }
}