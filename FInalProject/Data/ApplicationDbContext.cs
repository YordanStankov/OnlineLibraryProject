using FInalProject.Models.Normal;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FInalProject.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        { 
        }
        public override DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Download> Downloads{ get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Comment>(c => 
            { 
                c.HasOne(c => c.Book).WithMany(c => c.Comments).OnDelete(DeleteBehavior.Restrict);

            });
            modelBuilder.Entity<Download>(c => 
            {
                c.HasKey(x => new {x.UserId, x.BookId });

                c.HasOne(u => u.User)
                .WithMany(d => d.Downloads)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

                c.HasOne(b => b.Book)
                .WithMany(d => d.Downloads)
                .HasForeignKey(b => b.BookId)
                .OnDelete(DeleteBehavior.Cascade);
            });

        }
    }
}
