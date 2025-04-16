using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using System;
using System.Linq;
using System.Threading.Tasks;
using FInalProject.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace FInalProject.Data.Seeding
{
    public static class SeedDb
    {
        public static async Task SeedBooksAuthorsAndRest(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var serviceProvider = scope.ServiceProvider;
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            // Ensure the database is created
            await context.Database.EnsureCreatedAsync();

            // Seed Authors
            if (!context.Authors.Any())
            {
                context.Authors.AddRange(
                    new Author { Name = "George Orwell" },
                    new Author { Name = "Harper Lee" },
                    new Author { Name = "J.D. Salinger" },
                    new Author { Name = "F. Scott Fitzgerald" },
                    new Author { Name = "J.R.R. Tolkien" }
                );
                await context.SaveChangesAsync();
            }

            // Seed Genres
            if (!context.Genres.Any())
            {
                context.Genres.AddRange(
                    new Genre { Name = "Dystopian" },
                    new Genre { Name = "Classic" },
                    new Genre { Name = "Fiction" },
                    new Genre { Name = "Fantasy" },
                    new Genre { Name = "Adventure" }
                );
                await context.SaveChangesAsync();
            }

            // Seed Books
            if (!context.Books.Any())
            {
                var georgeOrwell = context.Authors.First(a => a.Name == "George Orwell");
                var harperLee = context.Authors.First(a => a.Name == "Harper Lee");
                var jdsalinger = context.Authors.First(a => a.Name == "J.D. Salinger");
                var fScottFitzgerald = context.Authors.First(a => a.Name == "F. Scott Fitzgerald");
                var jrrTolkien = context.Authors.First(a => a.Name == "J.R.R. Tolkien");

                var dystopian = context.Genres.First(g => g.Name == "Dystopian");
                var classic = context.Genres.First(g => g.Name == "Classic");
                var fiction = context.Genres.First(g => g.Name == "Fiction");
                var fantasy = context.Genres.First(g => g.Name == "Fantasy");
                var adventure = context.Genres.First(g => g.Name == "Adventure");

                context.Books.AddRange(
                    new Book
                    {
                        Name = "1984",
                        ReadingTime = 10,
                        Pages = 328,
                        Author = georgeOrwell,
                        Category = Category.Books,
                        CategoryString = "Books",
                        CoverImage = "https://covers.openlibrary.org/b/id/7222246-L.jpg",
                        Description = "A dystopian novel set in a totalitarian society under constant surveillance.",
                        AmountInStock = 5,
                        BookGenres = new List<BookGenre>
                        {
                            new BookGenre { Genre = dystopian },
                            new BookGenre { Genre = classic }
                        }
                    },
                    new Book
                    {
                        Name = "To Kill a Mockingbird",
                        ReadingTime = 8,
                        Pages = 281,
                        Author = harperLee,
                        Category = Category.Books,
                        CategoryString = "Books",
                        CoverImage = "https://covers.openlibrary.org/b/id/8225261-L.jpg",
                        Description = "A novel about racial injustice in the Deep South, seen through the eyes of a young girl.",
                        AmountInStock = 5,
                        BookGenres = new List<BookGenre>
                        {
                            new BookGenre { Genre = classic },
                            new BookGenre { Genre = fiction }
                        }
                    },
                    new Book
                    {
                        Name = "The Catcher in the Rye",
                        ReadingTime = 7,
                        Pages = 277,
                        Author = jdsalinger,
                        Category = Category.Books,
                        CategoryString = "Books",
                        CoverImage = "https://covers.openlibrary.org/b/id/7222246-L.jpg",
                        Description = "A story about a disillusioned teenager's journey through New York City.",
                        AmountInStock = 5,
                        BookGenres = new List<BookGenre>
                        {
                            new BookGenre { Genre = classic },
                            new BookGenre { Genre = fiction }
                        }
                    },
                    new Book
                    {
                        Name = "The Great Gatsby",
                        ReadingTime = 6,
                        Pages = 180,
                        Author = fScottFitzgerald,
                        Category = Category.Books,
                        CategoryString = "Books",
                        CoverImage = "https://covers.openlibrary.org/b/id/7222246-L.jpg",
                        Description = "A novel set in the Jazz Age, exploring themes of decadence and excess.",
                        AmountInStock = 5,
                        BookGenres = new List<BookGenre>
                        {
                            new BookGenre { Genre = classic },
                            new BookGenre { Genre = fiction }
                        }
                    },
                    new Book
                    {
                        Name = "The Hobbit",
                        ReadingTime = 15,
                        Pages = 310,
                        Author = jrrTolkien,
                        Category = Category.Books,
                        CategoryString = "Books",
                        CoverImage = "https://covers.openlibrary.org/b/id/7222246-L.jpg",
                        Description = "A fantasy novel about the adventures of a hobbit named Bilbo Baggins.",
                        AmountInStock = 5,
                        BookGenres = new List<BookGenre>
                        {
                            new BookGenre { Genre = fantasy },
                            new BookGenre { Genre = adventure }
                        }
                    }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}
