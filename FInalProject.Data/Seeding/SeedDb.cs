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
            var serviceProvider = scope.ServiceProvider.GetRequiredService<IServiceProvider>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            // Ensure the database is created
            await context.Database.EnsureCreatedAsync();

            // Seed Authors
            if (!context.Authors.Any())
            {
                context.Authors.AddRange(
                    new Author { Name = "George Orwell", Portrait = "https://m.media-amazon.com/images/M/MV5BYWUxN2Q1OTItOTYxMS00ZThmLWI4Y2MtZWQ3MDI3NDlmMTYzXkEyXkFqcGc@._V1_.jpg" },
                    new Author { Name = "Harper Lee", Portrait = "https://hips.hearstapps.com/hmg-prod/images/harper-lee-wikimedia-commons.jpg" },
                    new Author { Name = "J.D. Salinger", Portrait = "https://upload.wikimedia.org/wikipedia/commons/9/93/J._D._Salinger_%28Catcher_in_the_Rye_portrait%29.jpg" },
                    new Author { Name = "F. Scott Fitzgerald", Portrait = "https://cdn.britannica.com/47/24647-050-E6E25F22/F-Scott-Fitzgerald.jpg" },
                    new Author { Name = "J.R.R. Tolkien", Portrait = "https://hips.hearstapps.com/hmg-prod/images/jrr-tolkien-9508428-1-402.jpg" },
                    new Author { Name = "C.S. Lewis", Portrait = "https://upload.wikimedia.org/wikipedia/en/thumb/1/1e/C.s.lewis3.JPG/250px-C.s.lewis3.JPG" },
                    new Author { Name = "Cormac McCarthy", Portrait = "https://upload.wikimedia.org/wikipedia/commons/7/7f/Cormac_McCarthy_%28Child_of_God_author_portrait_-_high-res%29.jpg" },
                    new Author { Name = "H.P. Lovecraft", Portrait = "https://cdn.britannica.com/50/186850-050-B7B4AE24/HP-Lovecraft.jpg" },
                    new Author
                    {
                        Name = "Jeffrey Kluger",
                        Portrait = "https://m.media-amazon.com/images/M/MV5BMTYyNmJjY2YtNWYxZi00MjEwLTk1MDMtZjE3N2YwNDM3Zjg3XkEyXkFqcGc@._V1_.jpg"
                    },
    new Author
    {
        Name = "The New York Times Editorial Team",
        Portrait = "https://upload.wikimedia.org/wikipedia/commons/4/40/New_York_Times_logo_variation.jpg"
    },
    new Author
    {
        Name = "Stan Lee",
        Portrait = "https://m.media-amazon.com/images/M/MV5BMTk3NDE3Njc5M15BMl5BanBnXkFtZTYwOTY5Nzc1._V1_.jpg"
    },
    new Author
    {
        Name = "Bob Kane",
        Portrait = "https://m.media-amazon.com/images/M/MV5BMTM2MDQ2MDAyOV5BMl5BanBnXkFtZTcwMDUyMjcxOA@@._V1_.jpg"
    },
    new Author
    {
        Name = "Stephen Hawking",
        Portrait = "https://www.usatoday.com/gcdn/-mm-/0db2ccf25305cbb6908a2661859c91e39d7cad25/c=693-0-3911-3218/local/-/media/2018/03/14/USATODAY/USATODAY/636565852197338918-AFP-AFP-12H63P.jpg"
    },
    new Author
    {
        Name = "Richard Feynman",
        Portrait = "https://upload.wikimedia.org/wikipedia/en/4/42/Richard_Feynman_Nobel.jpg"
    }


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
                    new Genre { Name = "Adventure" },
                    new Genre { Name = "Thriller"},
                    new Genre { Name = "Cosmic Horror"},
                    new Genre { Name = "Horror" },
                    new Genre { Name = "Science" },
                    new Genre { Name = "Journalism" },
                    new Genre { Name = "Superhero" },
                    new Genre { Name = "Education" }

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
                var csLewis = context.Authors.First(a => a.Name == "C.S. Lewis");
                var cormacMcCarthy = context.Authors.First(a => a.Name == "Cormac McCarthy");
                var hpLovecraft = context.Authors.First(a => a.Name == "H.P. Lovecraft");
                var jeffreyKluger = context.Authors.First(a => a.Name == "Jeffrey Kluger");
                var nytEditorial = context.Authors.First(a => a.Name == "The New York Times Editorial Team");
                var stanLee = context.Authors.First(a => a.Name == "Stan Lee");
                var bobKane = context.Authors.First(a => a.Name == "Bob Kane");
                var stephenHawking = context.Authors.First(a => a.Name == "Stephen Hawking");
                var richardFeynman = context.Authors.First(a => a.Name == "Richard Feynman");


                var dystopian = context.Genres.First(g => g.Name == "Dystopian");
                var classic = context.Genres.First(g => g.Name == "Classic");
                var fiction = context.Genres.First(g => g.Name == "Fiction");
                var fantasy = context.Genres.First(g => g.Name == "Fantasy");
                var adventure = context.Genres.First(g => g.Name == "Fantasy");
                var thriller = context.Genres.First(g => g.Name == "Thriller");
                var cosmicHorror = context.Genres.First(g => g.Name == "Cosmic Horror");
                var horror = context.Genres.First(g => g.Name == "Horror");
                var science = context.Genres.First(g => g.Name == "Science");
                var journalism = context.Genres.First(g => g.Name == "Journalism");
                var superhero = context.Genres.First(g => g.Name == "Superhero");
                var education = context.Genres.First(g => g.Name == "Education");


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
                        DateWritten = new DateTime(1949, 06, 8),

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
                        CoverImage = "https://upload.wikimedia.org/wikipedia/commons/thumb/4/4f/To_Kill_a_Mockingbird_%28first_edition_cover%29.jpg/1200px-To_Kill_a_Mockingbird_%28first_edition_cover%29.jpg",
                        Description = "A novel about racial injustice in the Deep South, seen through the eyes of a young girl.",
                        AmountInStock = 5,
                        DateWritten = new DateTime(1960, 07, 11),
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
                        CoverImage = "https://m.media-amazon.com/images/I/8125BDk3l9L.jpg",
                        Description = "A story about a disillusioned teenager's journey through New York City.",
                        AmountInStock = 5,
                        DateWritten = new DateTime(1951, 07, 16),
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
                        CoverImage = "https://upload.wikimedia.org/wikipedia/commons/thumb/7/7a/The_Great_Gatsby_Cover_1925_Retouched.jpg/960px-The_Great_Gatsby_Cover_1925_Retouched.jpg",
                        Description = "A novel set in the Jazz Age, exploring themes of decadence and excess.",
                        AmountInStock = 5,
                        DateWritten = new DateTime(1925, 04, 10),
                        BookGenres = new List<BookGenre>
                        {
                            new BookGenre { Genre = classic },
                            new BookGenre { Genre = fiction }
                        }
                    },
                     new Book
                     {
                         Name = "Shadow over Innsmouth",
                         ReadingTime = 4,
                         Pages = 120,
                         Author = hpLovecraft,
                         Category = Category.Books,
                         CategoryString = "Books",
                         CoverImage = "https://upload.wikimedia.org/wikipedia/en/e/e0/Shadows_over_innsmouth.jpg",
                         Description = "A novel set in a catastrophic setting of an accursed village.",
                         AmountInStock = 5,
                         DateWritten = new DateTime(1936, 04, 01),
                         BookGenres = new List<BookGenre>
                        {
                            new BookGenre { Genre = horror },
                            new BookGenre { Genre = cosmicHorror }
                        }
                     },
                      new Book
                      {
                          Name = "Cormac McCarthy",
                          ReadingTime = 6,
                          Pages = 180,
                          Author = cormacMcCarthy,
                          Category = Category.Books,
                          CategoryString = "Books",
                          CoverImage = "https://pagechewing.com/wp-content/uploads/2024/05/Blood-Meridian-768x1229-2387184016.jpeg",
                          Description = "Book exploring the cruelty happening at the amercan-mexican border in the 18 hundreds.",
                          AmountInStock = 5,
                          DateWritten = new DateTime(1985, 04, 01),
                          BookGenres = new List<BookGenre>
                        {
                            new BookGenre { Genre = horror },
                            new BookGenre { Genre = thriller }
                        }
                      },
                       new Book
                       {
                           Name = "Chronicles of Narnina: The Magician's Nephew",
                           ReadingTime = 6,
                           Pages = 167,
                           Author = csLewis,
                           Category = Category.Books,
                           CategoryString = "Books",
                           CoverImage = "https://upload.wikimedia.org/wikipedia/en/d/df/TheMagiciansNephew%281stEd%29.jpg",
                           Description = "The story of 2 children discovering new worlds and witnessing their creation as well as destruction.",
                           AmountInStock = 5,
                           DateWritten = new DateTime(1955, 05, 02),
                           BookGenres = new List<BookGenre>
                        {
                            new BookGenre { Genre = adventure },
                            new BookGenre { Genre = fiction }
                        }
                       },
                        new Book
                        {
                            Name = "The Hobbit",
                            ReadingTime = 6,
                            Pages = 180,
                            Author = jrrTolkien,
                            Category = Category.Books,
                            CategoryString = "Books",
                            CoverImage = "https://upload.wikimedia.org/wikipedia/en/4/4a/TheHobbit_FirstEdition.jpg",
                            Description = "A novel describing the adventures of Bilbo Baggins through a magical and dangerous world",
                            AmountInStock = 5,
                            DateWritten = new DateTime(1937, 08, 21),
                            BookGenres = new List<BookGenre>
                        {
                            new BookGenre { Genre = classic },
                            new BookGenre { Genre = fiction },
                            new BookGenre {Genre = adventure}
                        }
                        },
                         new Book
                         {
                             Name = "Time Magazine: The Science Issue",
                             ReadingTime = 2,
                             Pages = 50,
                             Author = jeffreyKluger,
                             Category = Category.Magazine,
                             CategoryString = "Magazine",
                             CoverImage = "https://content.time.com/time/magazine/archive/covers/1991/1101910826_400.jpg",
                             Description = "A special edition focusing on recent scientific breakthroughs.",
                             AmountInStock = 10,
                             DateWritten = new DateTime(2025, 5, 1),
                             BookGenres = new List<BookGenre>
        {
            new BookGenre { Genre = science }
        }
                         },
    new Book
    {
        Name = "Time Magazine: Climate Change",
        ReadingTime = 2,
        Pages = 48,
        Author = jeffreyKluger,
        Category = Category.Magazine,
        CategoryString = "Magazine",
        CoverImage = "https://content.time.com/time/magazine/archive/covers/2006/1101060403_400.jpg",
        Description = "An in-depth look at the impacts of climate change globally.",
        AmountInStock = 10,
        DateWritten = new DateTime(2025, 4, 15),
        BookGenres = new List<BookGenre>
        {
            new BookGenre { Genre = science }
        }
    },

    // Newspapers
    new Book
    {
        Name = "The New York Times: May 20, 2025 Edition",
        ReadingTime = 1,
        Pages = 40,
        Author = nytEditorial,
        Category = Category.Newspaper,
        CategoryString = "Newspaper",
        CoverImage = "https://static01.nyt.com/images/2025/05/20/nytfrontpage/scan.jpg",
        Description = "Daily news covering global events and local stories.",
        AmountInStock = 15,
        DateWritten = new DateTime(2025, 5, 20),
        BookGenres = new List<BookGenre>
        {
            new BookGenre { Genre = journalism }
        }
    },
    new Book
    {
        Name = "The New York Times: May 21, 2025 Edition",
        ReadingTime = 1,
        Pages = 42,
        Author = nytEditorial,
        Category = Category.Newspaper,
        CategoryString = "Newspaper",
        CoverImage = "https://static01.nyt.com/images/2025/05/21/nytfrontpage/scan.jpg",
        Description = "Latest updates on politics, economy, and culture.",
        AmountInStock = 15,
        DateWritten = new DateTime(2025, 5, 21),
        BookGenres = new List<BookGenre>
        {
            new BookGenre { Genre = journalism }
        }
    },

    // Comics
    new Book
    {
        Name = "The Amazing Spider-Man #1",
        ReadingTime = 1,
        Pages = 32,
        Author = stanLee,
        Category = Category.Comics,
        CategoryString = "Comics",
        CoverImage = "https://www.coverbrowser.com/image/amazing-spider-man/1-1.jpg",
        Description = "The debut issue introducing Spider-Man to the world.",
        AmountInStock = 20,
        DateWritten = new DateTime(1963, 3, 1),
        BookGenres = new List<BookGenre>
        {
            new BookGenre { Genre = superhero }
        }
    },
    new Book
    {
        Name = "Batman #1",
        ReadingTime = 1,
        Pages = 30,
        Author = bobKane,
        Category = Category.Comics,
        CategoryString = "Comics",
        CoverImage = "https://www.coverbrowser.com/image/batman/1-1.jpg",
        Description = "The first standalone Batman comic book.",
        AmountInStock = 20,
        DateWritten = new DateTime(1940, 4, 25),
        BookGenres = new List<BookGenre>
        {
            new BookGenre { Genre = superhero }
        }
    },

    // Education
    new Book
    {
        Name = "A Brief History of Time",
        ReadingTime = 6,
        Pages = 256,
        Author = stephenHawking,
        Category = Category.Education,
        CategoryString = "Education",
        CoverImage = "https://images.penguinrandomhouse.com/cover/9780553380163",
        Description = "An overview of cosmology from the Big Bang to black holes.",
        AmountInStock = 10,
        DateWritten = new DateTime(1988, 4, 1),
        BookGenres = new List<BookGenre>
        {
            new BookGenre { Genre = science },
            new BookGenre { Genre = education }
        }
    },
    new Book
    {
        Name = "The Feynman Lectures on Physics",
        ReadingTime = 10,
        Pages = 560,
        Author = richardFeynman,
        Category = Category.Education,
        CategoryString = "Education",
        CoverImage = "https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1433168047i/5546.jpg",
        Description = "Comprehensive lectures covering fundamental physics concepts.",
        AmountInStock = 10,
        DateWritten = new DateTime(1964, 1, 1),
        BookGenres = new List<BookGenre>
        {
            new BookGenre { Genre = science },
            new BookGenre { Genre = education }
        }
    }

                );
                await context.SaveChangesAsync();
            }
        }
    }
}
