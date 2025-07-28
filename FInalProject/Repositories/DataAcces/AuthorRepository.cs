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

        public  void AddToAuhtorBookList(Author author, Book book)
        {
            if(author != null && book != null) 
             author.Books.Add(book);
        }

        public async Task<Author> GetAuthorByIdAsync(int authorId)
        {
            return await _context.Authors.AsNoTracking().FirstOrDefaultAsync(a => a.Id == authorId);
        }

        public async Task<Author> GetAuthorByNameAsync(string name)
        {
            return await _context.Authors.AsNoTracking().FirstOrDefaultAsync(a => a.Name == name);
        }

        public Task SaveChangesAsync()
        {
            throw new NotImplementedException();
        }

        public  void UpdateAuthor(Author author)
        {
            _context.Authors.Update(author);
        }
    }
}
