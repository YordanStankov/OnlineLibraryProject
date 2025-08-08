using FInalProject.Domain.Models;
using FInalProject.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using FInalProject.Application.ViewModels.Book;
using FInalProject.Application.ViewModels.Admin.Book;
using FInalProject.Application.ViewModels.Comment;

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

        public async Task<BookFocusViewModel> GetSingleBookForFocusAsync(int bookId)
        {
           
            return await _context.Books
               .AsNoTracking()
              .Include(b => b.Favourites)
              .Include(b => b.Author)
              .Include(b => b.Comments)
              .ThenInclude(c => c.User)
              .Include(b => b.BookGenres)
              .ThenInclude(b => b.Genre)
              .Where(b => b.Id == bookId)
              .Select(b => new BookFocusViewModel
              {
                  BookCover = b.CoverImage,
                  BookId = b.Id,
                  BookName = b.Name,
                  BookPages = b.Pages,
                  Category = b.Category,
                  DateWritten = b.DateWritten,
                  BookAuthorName = b.Author.Name,
                  AmountInStock = b.AmountInStock,
                  BookReadingTime = b.ReadingTime,
                  Description = b.Description,
                  genres = b.BookGenres.Select(bg => bg.Genre).ToList(),
                  comments = b.Comments.Select(c => new CommentViewModel
                  {
                      UserName = c.User.UserName ?? "Unknown User",
                      Description = c.CommentContent ?? string.Empty
                  }).ToList(),
                  Favourites = b.Favourites,
                  Borrowed = false
              }).FirstOrDefaultAsync();
              
              

        }

        public async Task<BookCreationViewModel> GetSingleBookForEditAsync(int editId)
        {
            List<Genre> genreOpts = await _context.Genres.AsNoTracking().ToListAsync(); 
            return await _context.Books
                .AsNoTracking()
                .Include(b => b.Author)
                .Include(b => b.BookGenres)
                .ThenInclude(b => b.Genre)
                .Where(b => b.Id == editId)
                .Select(book => new BookCreationViewModel
                {
                    Id = book.Id,
                    Name = book.Name,
                    Category = book.Category,
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

        public async Task<List<AdminBookListViewModel>> RenderAdminBookListAsync()
        {
            return await _context.Books.AsNoTracking().Select(b => new AdminBookListViewModel
            {
                BookId = b.Id,
                BookName = b.Name,
                BooksBorrowed = b.BorrowedBooks.Count(),
                BookStock = b.AmountInStock,
                Category = b.CategoryString,
                genres = b.BookGenres.Select(bg => bg.Genre.Name).ToList() ?? new List<string>()
            }).ToListAsync();
        }

        public async Task<List<BookListViewModel>> RenderBookListAsync()
        {
            return await _context.Books
               .AsNoTracking()
               .Include(a => a.Author)
               .Include(bg => bg.BookGenres)
               .ThenInclude(g => g.Genre)
               .Where(b => b.AmountInStock > 0)
               .Select(n => new BookListViewModel()
               {
                   Id = n.Id,
                   Name = n.Name,
                   Pages = n.Pages,
                   Category = n.Category,
                   AuthorName = n.Author.Name,
                   DateWritten = n.DateWritten,
                   CoverImage = n.CoverImage,
                   Genres = n.BookGenres.Select(bg => bg.Genre.Name).ToList(),
               }).ToListAsync();
        }

        public async Task<List<BookListViewModel>> RenderBooksByCategoryAsync(int modifier)
        {
            return await _context.Books
                .AsNoTracking()
                .Where(b => (int)b.Category == modifier && b.AmountInStock > 0)
                .Take(20)
                .Select(n => new BookListViewModel()
                {
                    Id = n.Id,
                    Name = n.Name,
                    Pages = n.Pages,
                    AuthorName = n.Author.Name,
                    Category = n.Category,
                    DateWritten = n.DateWritten,
                    CoverImage = n.CoverImage,
                    Genres = n.BookGenres.Select(bg => bg.Genre.Name).ToList(),
                }).ToListAsync();
        }

        public async Task<List<BooksLeaderboardViewModel>> RenderBooksLeaderboardAsync()
        {
            return await _context.Books
                .OrderByDescending(b => b.Favourites.Sum(f => f.Amount))
                .Select(b => new BooksLeaderboardViewModel
                {
                    BookId = b.Id,
                    AuthorName = b.Author.Name,
                    BookName = b.Name,
                    CategoryString = b.CategoryString,
                    PositiveReviews = b.Favourites.Sum(f => f.Amount)
                })
                .ToListAsync();
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

        public async Task<List<BookListViewModel>> RenderSearchedBookListAsync(string searchQuery)
        {
            string loweredQuery = searchQuery.ToLower();
            List<BookListViewModel> queriedBooks = new List<BookListViewModel>();

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
            queriedBooks = books.Select(b => new BookListViewModel
            {
                Id = b.Id,
                Name = b.Name,
                Pages = b.Pages,
                AuthorName = b.Author.Name,
                Category = b.Category,
                CoverImage = b.CoverImage,
                DateWritten = b.DateWritten,
                Genres = b.BookGenres?.Select(bg => bg.Genre.Name).ToList()
            }).ToList();

                return queriedBooks;
        

        }
    }
}
