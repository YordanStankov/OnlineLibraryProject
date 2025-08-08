using FInalProject.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FInalProject.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        { 
        }
        public override DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Favourite > Favourites { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<BookGenre> BookGenres { get; set; }
        public DbSet<BorrowedBook> BorrowedBooks { get; set; }
        public DbSet<FavouriteAuthor> FavouriteAuthors { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //normal tables
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Comment>(c => 
            { 
                c.HasOne(c => c.Book).WithMany(c => c.Comments).OnDelete(DeleteBehavior.Cascade);
            });
          
            modelBuilder.Entity<Book>(c =>
            {
                c.HasOne(c => c.Author).WithMany(c => c.Books).OnDelete(DeleteBehavior.Cascade);
            });

            //mapping tables
            modelBuilder.Entity<Favourite>(c => {
                c.HasKey(x => new { x.UserId, x.BookId });

                c.HasOne(u => u.User)
                  .WithMany(u => u.Favourites)
                  .HasForeignKey(u => u.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

                c.HasOne(u => u.Book)
                    .WithMany(fm => fm.Favourites)
                    .HasForeignKey(u => u.BookId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<BookGenre>(c => {
                c.HasKey(x => new { x.BookId, x.GenreId });

                c.HasOne(u => u.Book)
                  .WithMany(u => u.BookGenres)
                  .HasForeignKey(u => u.BookId)
                  .OnDelete(DeleteBehavior.Cascade);

                c.HasOne(u => u.Genre)
                    .WithMany(fm => fm.BookGenres)
                    .HasForeignKey(u => u.GenreId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
                
            modelBuilder.Entity<BorrowedBook>(c => {
                c.HasKey(x => new { x.BookId, x.UserId });

                c.HasOne(u => u.Book)
                  .WithMany(u => u.BorrowedBooks)
                  .HasForeignKey(u => u.BookId)
                  .OnDelete(DeleteBehavior.Cascade);

                c.HasOne(u => u.User)
                    .WithMany(fm => fm.BorrowedBooks)
                    .HasForeignKey(u => u.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<FavouriteAuthor>(c => {
                c.HasKey(x => new { x.AuthorId, x.UserId });

                c.HasOne(a => a.Author)
                  .WithMany(fa => fa.FavouriteAuthors)
                  .HasForeignKey(a => a.AuthorId)
                  .OnDelete(DeleteBehavior.Cascade);

                c.HasOne(u => u.User)
                    .WithMany(fa => fa.FavouriteAuthors)
                    .HasForeignKey(u => u.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
