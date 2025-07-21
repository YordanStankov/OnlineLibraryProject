using FInalProject.Data;
using FInalProject.Data.Models;
using FInalProject.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FInalProject.Repositories.DataAcces
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly ApplicationDbContext _context;

        public AuthorRepository(ApplicationDbContext context)
        {
           _context = context;
        }

        public async Task AddToAuhtorBookListAsync(Author author, Book book)
        {
            author.Books.Add(book);
            await _context.SaveChangesAsync();
        }

        public async Task<Author> GetAuthorByIdAsync(int authorId)
        {
            return await _context.Authors.AsNoTracking().FirstOrDefaultAsync(a => a.Id == authorId);
        }

        public async Task<Author> GetAuthorByNameAsync(string name)
        {
            return await _context.Authors.AsNoTracking().FirstOrDefaultAsync(a => a.Name == name);
        }
    }
}
