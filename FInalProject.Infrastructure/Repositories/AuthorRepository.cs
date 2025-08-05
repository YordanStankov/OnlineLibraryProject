using FInalProject.Data.Models;
using FInalProject.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using FInalProject.Application.ViewModels.Author;

namespace FInalProject.Infrastructure.Repositories
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

        public async Task<List<AuthorListViewModel>> RenderAuthorListAsync()
        {
            List<AuthorListViewModel> listOfAuthors = new List<AuthorListViewModel>();  

            listOfAuthors = await _context.Authors.Select(a => new AuthorListViewModel
            {
                AuthorId = a.Id,
                AuthorPortrait = a.Portrait,
                AuthorName = a.Name,
                Favourites = a.FavouriteAuthors.Count()
            }).ToListAsync();
            return listOfAuthors;
        }

        public async Task<List<AuthorListViewModel>> RenderAuthorSearchResutlsAsync(string searchQuery)
        {
            string loweredQuery = searchQuery.ToLower(); 
            List<AuthorListViewModel> authorList = new List<AuthorListViewModel>();
            authorList = await _context.Authors
                    .Where(a => a.Name.ToLower().Contains(loweredQuery))
                    .Select(a => new AuthorListViewModel
                    {
                        AuthorId = a.Id,
                        AuthorPortrait = a.Portrait,
                        AuthorName = a.Name,
                        Favourites = a.FavouriteAuthors.Count()
                    }).ToListAsync();
            return authorList;
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
