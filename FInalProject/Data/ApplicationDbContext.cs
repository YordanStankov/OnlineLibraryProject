using FInalProject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FInalProject.Data
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
        public DbSet<Favourite > Favorites { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Comment>(c => 
            { 
                c.HasOne(c => c.Book).WithMany(c => c.Comments).OnDelete(DeleteBehavior.Restrict);
            });
          
            modelBuilder.Entity<Book>(c =>
            {
                c.HasOne(c => c.Author).WithMany(c => c.Books).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Favourite>(c => {
                c.HasKey(x => new { x.UserId, x.BookId });

                c.HasOne(u => u.User)
                  .WithMany(u => u.Favourites)
                  .HasForeignKey(u => u.UserId)
                  .OnDelete(DeleteBehavior.Restrict);

                c.HasOne(u => u.Book)
                    .WithMany(fm => fm.Favourites)
                    .HasForeignKey(u => u.BookId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            

        }
    }
}
