using FInalProject;
using FInalProject.Data;
using FInalProject.Models;
namespace FInalProject
{
    public static class SeedDatabase
    {
        public static void Seeding(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            // Seed Authors
            if (!context.Authors.Any())
            {
                context.Authors.AddRange(
                    new Author { Name = "George Orwell" },
                    new Author { Name = "J.K. Rowling" },
                    new Author { Name = "J.R.R. Tolkien" }
                );
                context.SaveChanges(); // Save to generate IDs
            }

            // Seed Genres
            if (!context.Genres.Any())
            {
                context.Genres.AddRange(
                    new Genre { Name = "Dystopian" },
                    new Genre { Name = "Fantasy" },
                    new Genre { Name = "Adventure" },
                    new Genre { Name = "Science Fiction" }
                );
                context.SaveChanges(); // Save to generate IDs
            }

            // Seed Books
            if (!context.Books.Any())
            {
                context.Books.AddRange(
                    new Book
                    {
                        Name = "1984",
                        ReadingTime = 12.5,
                        Pages = 328,
                        AuthorId = context.Authors.First(a => a.Name == "George Orwell").Id,
                        DateTaken = new DateTime(2023, 1, 15),
                        UntillReturn = new DateTimeOffset(new DateTime(2023, 2, 15)),
                        CoverImage = "1984_cover.jpg",
                        Description = "A dystopian novel set in a totalitarian society ruled by Big Brother."
                    },
                    new Book
                    {
                        Name = "Harry Potter and the Sorcerer's Stone",
                        ReadingTime = 8.0,
                        Pages = 309,
                        AuthorId = context.Authors.First(a => a.Name == "J.K. Rowling").Id,
                        DateTaken = new DateTime(2023, 3, 1),
                        UntillReturn = new DateTimeOffset(new DateTime(2023, 4, 1)),
                        CoverImage = "hp_sorcerer_stone_cover.jpg",
                        Description = "The first book in the Harry Potter series introducing the wizarding world."
                    },
                    new Book
                    {
                        Name = "The Hobbit",
                        ReadingTime = 10.0,
                        Pages = 310,
                        AuthorId = context.Authors.First(a => a.Name == "J.R.R. Tolkien").Id,
                        DateTaken = new DateTime(2023, 5, 10),
                        UntillReturn = new DateTimeOffset(new DateTime(2023, 6, 10)),
                        CoverImage = "the_hobbit_cover.jpg",
                        Description = "Bilbo Baggins embarks on an unexpected journey to the Lonely Mountain."
                    }
                );
                context.SaveChanges(); // Save to generate IDs
            }
           
            // Seed BookGenres (Many-to-Many Relationships)
            if (!context.BookGenres.Any())
            {
                context.BookGenres.AddRange(
                    // 1984: Dystopian, Science Fiction
                    new BookGenre
                    {
                        BookId = context.Books.First(b => b.Name == "1984").Id,
                        GenreId = context.Genres.First(g => g.Name == "Dystopian").Id
                    },
                    new BookGenre
                    {
                        BookId = context.Books.First(b => b.Name == "1984").Id,
                        GenreId = context.Genres.First(g => g.Name == "Science Fiction").Id
                    },

                    // Harry Potter: Fantasy, Adventure
                    new BookGenre
                    {
                        BookId = context.Books.First(b => b.Name == "Harry Potter and the Sorcerer's Stone").Id,
                        GenreId = context.Genres.First(g => g.Name == "Fantasy").Id
                    },
                    new BookGenre
                    {
                        BookId = context.Books.First(b => b.Name == "Harry Potter and the Sorcerer's Stone").Id,
                        GenreId = context.Genres.First(g => g.Name == "Adventure").Id
                    },

                    // The Hobbit: Fantasy, Adventure
                    new BookGenre
                    {
                        BookId = context.Books.First(b => b.Name == "The Hobbit").Id,
                        GenreId = context.Genres.First(g => g.Name == "Fantasy").Id
                    },
                    new BookGenre
                    {
                        BookId = context.Books.First(b => b.Name == "The Hobbit").Id,
                        GenreId = context.Genres.First(g => g.Name == "Adventure").Id
                    }
                );
                context.SaveChanges(); // Save the many-to-many relationships
            }
        }
    }
}