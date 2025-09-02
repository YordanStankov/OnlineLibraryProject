using FInalProject.Application.DTOs.AuthorDTOs;
using FInalProject.Application.Interfaces;
using FInalProject.Application.ViewModels.Author;
using FInalProject.Application.ViewModels.Book;
using FInalProject.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FInalProject.Infrastructure.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public AuthorRepository(ApplicationDbContext context, UserManager<User> userManager)
        {
           _context = context;
           _userManager = userManager;
        }
        public async Task AddPortraitToAuthorAsync(AddAuthorPortraitDTO dto)
        {
            var author = await _context.Authors.FirstOrDefaultAsync(a => a.Id == dto.Id);
            if(author == null)
                throw new Exception("Author not found in AddPortraitToAuthorAsync in AuthorRepository");
            author.Portrait = dto.Portrait;
            _context.Authors.Update(author);
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

        public async Task<AuthorProfileViewModel> RenderAuthorProfileASync(FavouriteAuthorDTO dto)
        {
            var empty = new AuthorProfileViewModel();
            var currUserId = _userManager.GetUserId(dto.UserClaim);
            if (currUserId == null) return empty;

            var author = await GetAuthorWithBooksByIdAsync(dto.Id);

            if (author is null) return empty;

            var fave = author.FavouriteAuthors.Any(fa => fa.AuthorId == dto.Id && fa.UserId == currUserId);

            return new AuthorProfileViewModel
            {
                isAuthorFavourited = fave,
                AuthorId = author.Id,
                AuthorName = author.Name,
                AuthorPortrait = author.Portrait,
                AuthorsBooks = author.Books!.Select(n => new BookListViewModel
                {
                    Id = n.Id,
                    Name = n.Name!,
                    Pages = n.Pages,
                    Category = n.Category,
                    AuthorName = n.Author?.Name ?? "Unknown",
                    CoverImage = n.CoverImage!,
                    Genres = n.BookGenres?
                                .Where(bg => bg.Genre != null)
                                .Select(bg => bg.Genre!.Name!)
                                .ToList() ?? new List<string>()
                }).ToList(),
                FavouritesCount = author.FavouriteAuthors?.Count() ?? 0
            };
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
