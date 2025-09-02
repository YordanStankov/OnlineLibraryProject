using FInalProject.Domain.Models;
using FInalProject.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using FInalProject.Application.ViewModels.Author;
using FInalProject.Application.DTOs.AuthorDTOs;

namespace FInalProject.Infrastructure.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly ApplicationDbContext _context;

        public AuthorRepository(ApplicationDbContext context)
        {
           _context = context;
        }

        public async Task AddPortraitToAuthorAsync(AddAuthorPortraitDTO dto)
        {
            var user = await _context.Authors.FirstOrDefaultAsync(a => a.Id == dto.Id);
            if(user == null)
                throw new Exception("Author not found in AddPortraitToAuthorAsync in AuthorRepository");
            user.Portrait = dto.Portrait;
            _context.Authors.Update(user);
            await _context.SaveChangesAsync();
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

        public Task<AddAuthorPortraitDTO> GetDTOForPortraitAsync(int authorId)
        {
            var dto = _context.Authors
                .Where(a => a.Id == authorId)
                .Select(a => new AddAuthorPortraitDTO(a.Portrait, a.Id))
                .FirstOrDefaultAsync();
            if(dto == null)
                throw new Exception("Author not found in GetDTOForPortraitAsync in AuthorRepository");
            return dto;
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
