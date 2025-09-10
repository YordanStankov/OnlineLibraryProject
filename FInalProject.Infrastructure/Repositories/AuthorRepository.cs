using FInalProject.Domain.Models;
using FInalProject.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FInalProject.Infrastructure.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly ApplicationDbContext _context;

        public AuthorRepository(ApplicationDbContext context)
        {
           _context = context;
        }

        public async Task AddAuthorAsync(Author author)
        {
            await _context.Authors.AddAsync(author);
        }

        public  void AddToAuthorBookList(Author author, Book book)
        {
            if(author != null && book != null) 
             author.Books.Add(book);
        }

        public async Task<Author> GetAuthorByIdAsync(int authorId)
        {
            return await _context.Authors.FirstOrDefaultAsync(a => a.Id == authorId);
        }

        public async Task<Author> GetAuthorByNameAsync(string name)
        {
            return await _context.Authors.FirstOrDefaultAsync(a => a.Name == name);
        }

        public async Task<Author> GetAuthorWithBooksByIdAsync(int authorId)
        {
            Author author = new Author();
            author = await _context.Authors
                .AsNoTracking()
                .Include(a => a.Books)
                    .ThenInclude(b => b.BookGenres)
                        .ThenInclude(bg => bg.Genre)
                .Include(a => a.FavouriteAuthors)
                .FirstOrDefaultAsync(a => a.Id == authorId);
            return author;
        }

        public async Task<List<Author>> ReturnAuthorListAsync()
        {
            return await _context.Authors.AsNoTracking().ToListAsync();
            
        }

        public async Task<List<Author>> ReturnSearchedAuthorListAsync(string searchQuery)
        {
            string loweredQuery = searchQuery.ToLower(); 
            return await _context.Authors
                .Where(a => a.Name.ToLower().Contains(loweredQuery))
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public  void UpdateAuthor(Author author)
        {
            _context.Authors.Update(author);
        }
    }
}
