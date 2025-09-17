using FInalProject.Application.DTOs.Admin;
using FInalProject.Application.DTOs.Book;
using FInalProject.Application.DTOs.Comment;
using FInalProject.Application.DTOs.Genre;
using FInalProject.Application.Interfaces;
using FInalProject.Application.Services;
using FInalProject.Application.ViewModels.Comment;
using FInalProject.Application.ViewModels.Genre;
using FInalProject.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FInalProject.Infrastructure.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext _context;


        public BookRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void AddBook(Book book)
        {
            _context.Books.Add(book);
        }

        public async Task<List<Book>> GetAllBooksAsync()
        {
            return await _context.Books.AsNoTracking().ToListAsync();
        }

        public async Task<BookFocusDTO> GetSingleBookDTOForFocusAsync(int bookId)
        {

            return await _context.Books
               .AsNoTracking()
              .Where(b => b.Id == bookId)
              .Select(b => new BookFocusDTO
              {
                  BookCover = b.CoverImage,
                  BookId = b.Id,
                  BookName = b.Name,
                  BookPages = b.Pages,
                  Category = b.CategoryString,
                  DateWritten = b.DateWritten,
                  BookAuthorName = b.Author.Name,
                  AmountInStock = b.AmountInStock,
                  BookReadingTime = b.ReadingTime,
                  Description = b.Description,
                  genres = b.BookGenres.Select(bg => new GenreListDTO
                  {
                      Id = bg.Genre.Id,
                      Name = bg.Genre.Name
                  }).ToList(),
                  comments = b.Comments.Select(c => new CommentDTO
                  {
                      UserName = c.User.UserName ?? "Unknown User",
                      Description = c.CommentContent ?? string.Empty
                  }).ToList(),
                  Rating = b.Favourites.Sum(f => f.Amount),
                  Borrowed = false
              }).FirstOrDefaultAsync();
        }

        public async Task<BookCreationDTO> GetSingleBookDTOForEditAsync(int editId)
        {
            List<GenreListDTO> genreOpts = await _context.Genres.Select(g => new GenreListDTO
            {
                Id = g.Id,
                Name = g.Name
            }).ToListAsync();

            return await _context.Books
                .AsNoTracking()
                .Include(b => b.Author)
                .Include(b => b.BookGenres)
                .ThenInclude(b => b.Genre)
                .Where(b => b.Id == editId)
                .Select(book => new BookCreationDTO
                {
                    Id = book.Id,
                    Name = book.Name,
                    Category = book.Category.ToString(),
                    AuthorName = book.Author.Name,
                    DateWritten = book.DateWritten,
                    AmountInStock = book.AmountInStock,
                    CoverImage = book.CoverImage,
                    Description = book.Description,
                    Pages = book.Pages,
                    ReadingTime = book.ReadingTime,
                    GenreOptions = genreOpts
                }).FirstOrDefaultAsync();
        }

        public async Task<List<BookListDTO>> ReturnBooksByCategoryDTOAsync(int modifier)
        {
            return await _context.Books
                .AsNoTracking()
                .Where(b => (int)b.Category == modifier && b.AmountInStock > 0)
                .Take(20)
                .Select(n => new BookListDTO()
                {
                    Id = n.Id,
                    Name = n.Name,
                    Pages = n.Pages,
                    AuthorName = n.Author.Name,
                    Category = n.Category.ToString(),
                    DateWritten = n.DateWritten,
                    CoverImage = n.CoverImage,
                    Genres = n.BookGenres.Select(bg => bg.Genre.Name).ToList(),
                }).ToListAsync();
        }

        public async Task<List<BooksLeaderboardDTO>> ReturnBooksLeaderboardDTOAsync()
        {
            return await _context.Books
                .OrderByDescending(b => b.Favourites.Sum(f => f.Amount))
                .Select(b => new BooksLeaderboardDTO
                {
                    BookId = b.Id,
                    AuthorName = b.Author.Name,
                    BookName = b.Name,
                    CategoryString = b.CategoryString,
                    CommunityRating = b.Favourites.Sum(f => f.Amount)
                }).ToListAsync();
        }

        public void UpdateBook(Book book)
        {
            _context.Books.Update(book);
        }
        public void RemoveBook(Book book)
        {
            _context.Books.Remove(book);

        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }



        public async Task<Book> ReturnBookEntityToEditAsync(int bookId)
        {
            return await _context.Books
                .Include(bte => bte.Author)
                .Include(bte => bte.BookGenres)
                .ThenInclude(bte => bte.Genre)
                .FirstOrDefaultAsync(bte => bte.Id == bookId);
        }

        public async Task<Book> ReturnBookEntityToBorrowAsync(int bookId)
        {
            Book book = new Book();
            return book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.BorrowedBooks)
                .FirstOrDefaultAsync(b => b.Id == bookId);
        }
        public async Task<Book> ReturnBookEntityToDeleteAsync(int bookId)
        {
            Book book = new Book();
            return book = await _context.Books
                .Include(b => b.Comments)
                .Include(b => b.BookGenres)
                .Include(b => b.Favourites)
                .FirstOrDefaultAsync(b => b.Id == bookId);
        }

        public async Task<List<BookListDTO>> ReturnSearchedBooksDTOAsync(string searchQuery)
        {
            string loweredQuery = searchQuery.ToLower();
            List<BookListDTO> queriedBooks = new List<BookListDTO>();

            var books = await _context.Books
                .AsNoTracking()
                .Include(b => b.Author)
                .Include(b => b.BookGenres)
                .ThenInclude(bg => bg.Genre)
                .Where(b => b.AmountInStock > 0 && (b.Name.ToLower().Contains(loweredQuery) ||
                    (b.Author != null && b.Author.Name.ToLower().Contains(loweredQuery)) ||
                    b.BookGenres.Any(bg => EF.Functions.Like(bg.Genre.Name, $"%{loweredQuery}%")) ||
                    b.CategoryString.ToLower().Contains(loweredQuery)))
                .ToListAsync();

            if (books.Any())
                queriedBooks = books.Select(b => new BookListDTO
                {
                    Id = b.Id,
                    Name = b.Name,
                    Pages = b.Pages,
                    AuthorName = b.Author.Name,
                    Category = b.Category.ToString(),
                    CoverImage = b.CoverImage,
                    DateWritten = b.DateWritten,
                    Genres = b.BookGenres?.Select(bg => bg.Genre.Name).ToList()
                }).ToList();

            return queriedBooks;
        }

        public Task<List<BookListDTO>> GetAllBooksDTOAsync()
        {
            var booksDTO = _context.Books
                .AsNoTracking()
                .Select(b => new BookListDTO
                {
                    Id = b.Id,
                    Name = b.Name,
                    AuthorName = b.Author.Name,
                    Pages = b.Pages,
                    CoverImage = b.CoverImage,
                    DateWritten = b.DateWritten,
                    Category = b.CategoryString,
                    Genres = b.BookGenres.Select(bg => bg.Genre.Name).ToList()
                }).ToListAsync();

            return booksDTO;
        }

        public async Task<List<AdminBookListDTO>> GetAllAdminBooksDTOAsync()
        {
            var books = await _context.Books
                .Select(b => new AdminBookListDTO
                {
                    BookId = b.Id,
                    BookName = b.Name,
                    BookStock = b.AmountInStock,
                    genres = b.BookGenres.Select(bg => bg.Genre.Name).ToList(),
                    Category = b.CategoryString,
                    BooksBorrowed = b.BorrowedBooks.Count
                })
                .ToListAsync();
            return books;
        }
    }
}
