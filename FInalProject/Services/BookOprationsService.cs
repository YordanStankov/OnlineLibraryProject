using FInalProject.Data.Models;
using FInalProject.ViewModels;
using FInalProject.Data;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using FInalProject.EmailTemplates;
using FInalProject.Repositories.Interfaces;


namespace FInalProject.Services
{
    public interface IBookOprationsService
    {
        Task<bool> BorrowBookAsync(int borrowId, ClaimsPrincipal Use);
        Task<bool> EditBookAsync(BookCreationViewModel model);
        Task<SearchResultsViewModel> ReturnSearchResultsAync(string searchedString);
        Task<bool> DeleteBookAsync(int doomedId);
        Task<int> CreateCommentAsync(CreateCommentViewModel model, ClaimsPrincipal user);
        Task<bool> UpdateFavouritesAsync(int amount, int bookId, ClaimsPrincipal user);
        Task<bool> ReturnBookAsync(ReturnBookViewModel model, ClaimsPrincipal User);
        Task<List<BookListViewModel>> ApplyFiltering (IEnumerable<BookListViewModel> books, FilteringViewModel filtering);
       
    }
    public class BookOprationsService : IBookOprationsService
    {
        private readonly ILogger<BookOprationsService> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;
        private readonly IBookRepository _bookRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly IBookGenreRepository _bookGenreRepository;

        public BookOprationsService(ApplicationDbContext context, 
            UserManager<User> userManager, 
            ILogger<BookOprationsService> logger, 
            IEmailService emailService, 
            IBookRepository bookRepository, 
            IAuthorRepository authorRepository, 
            IBookGenreRepository bookGenreRepository)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _emailService = emailService;
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _bookGenreRepository = bookGenreRepository;
        }

        public async Task<bool> BorrowBookAsync(int borrowedId, ClaimsPrincipal user)
        {
            var borrowingUser = await _userManager.GetUserAsync(user);
            if (borrowingUser == null) return false;

            _logger.LogInformation("BORROWING BOOK");
            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.BorrowedBooks)
                .FirstOrDefaultAsync(b => b.Id == borrowedId);


            if(book != null && !book.BorrowedBooks.Any(bb => bb.UserId == borrowingUser.Id && bb.BookId == book.Id))
            {
                    book.AmountInStock -= 1;

                    var borrowedBook = new BorrowedBook
                    {
                        DateTaken = DateTime.Now,
                        UntillReturn = DateTime.Now.AddDays(14),
                        UserId = borrowingUser.Id,
                        Book = book,
                        BookId = book.Id,
                        User = borrowingUser
                    };
                    await _context.BorrowedBooks.AddAsync(borrowedBook);
                    await _context.SaveChangesAsync();

                var email = borrowingUser.Email ?? string.Empty;
                    Dictionary<string, string> placeees = new Dictionary<string, string>
                    {
                        {"UserName", email },
                        {"BookTitle", book.Name },
                        {"ReturnDate", string.Format("{0:dd MMM yyyy}", borrowedBook.UntillReturn) }
                    };
                    var emailBody = await _emailService.LoadEmailTemplateAsync(TemplateNames.BorrowingConfirmationEmail, placeees);
                    await _emailService.SendEmailFromServiceAsync(email, "Book return", emailBody);
                    return true;
                }
                return false;
        }

        public async Task<int> CreateCommentAsync(CreateCommentViewModel model, ClaimsPrincipal user)
        {
            _logger.LogInformation("CREATING COMMENT");
            var userId = _userManager.GetUserId(user);
            if(userId == null)
            {
                return -1; 
            }
            var comment = new Comment
            {
                UserId = userId, 
                BookId = model.BookId,
                CommentContent = model.Description
            };
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return comment.BookId;
        }

        public async Task<bool> DeleteBookAsync(int doomedId)
        {
            _logger.LogInformation("DELETING BOOK");
            var doomedBook = await _context.Books.
                Include(b => b.Comments)
                .Include(b => b.BookGenres)
                .Include(b => b.Favourites)
                .FirstOrDefaultAsync(b => b.Id == doomedId); 

            if(doomedBook != null)
            {
                _context.Remove(doomedBook);
                await _context.SaveChangesAsync();
                _logger.LogInformation("BLITZED THE BOOK");
                return true;
            }
            return false;
        }

        public async Task<bool> EditBookAsync(BookCreationViewModel model)
        {
            _logger.LogInformation("EDITING BOOK");
            var bookToEdit = await _bookRepository.ReturnBookEntityToEditAsync(model.Id);
            
           if(model.editor == 0)
            {
                MapAdminFields(bookToEdit, model);
                
                await MapAuthor(bookToEdit, model);

                await MapBookGenres(bookToEdit, model);

                await _authorRepository.UpdateAuthorAsync(bookToEdit.Author);
                await _bookRepository.UpdateBookAsync(bookToEdit);
                _logger.LogInformation("EDITED BOOK BY ADMIN");
                return true;
            }
           else if(model.editor == 1)
            {
                _logger.LogInformation("EDITED BOOK BY LIBRARIAN");
                bookToEdit.Name = model.Name;
                await _bookRepository.UpdateBookAsync(bookToEdit);
                return true;
            }
            _logger.LogError("Book editing failed");
            return false; 
        }

        public async Task<List<BookListViewModel>> ApplyFiltering(IEnumerable<BookListViewModel> books, FilteringViewModel filtering)
        {
            return await Task.FromResult(filtering.SortBy switch
            {
                "Name" => filtering.SortDirection == "desc"
                    ? books.OrderByDescending(b => b.Name).ToList()
                    : books.OrderBy(b => b.Name).ToList(),

                "Date" => filtering.SortDirection == "desc"
                    ? books.OrderByDescending(b => b.DateWritten).ToList()
                    : books.OrderBy(b => b.DateWritten).ToList(),

                _ => books.ToList()
            });
        }
        public async Task<bool> ReturnBookAsync(ReturnBookViewModel model, ClaimsPrincipal User)
        {
            var returningUser = await _userManager.GetUserAsync(User);
            if (returningUser == null)
            {
                return false;
            }
            

            var bookToBeReturned = await _context.BorrowedBooks
                .Include(bb => bb.Book)
                .FirstOrDefaultAsync(bb => bb.BookId == model.BookId && bb.UserId == model.UserId);

            if (bookToBeReturned == null)
            {
                return false;
            }

            if (bookToBeReturned.StrikeGiven)
            {
                returningUser.Strikes -= 1;
            }
            
            _context.Remove(bookToBeReturned);
            await _context.SaveChangesAsync();
            var stillOverdue = await _context.BorrowedBooks
                  .AnyAsync(bb => bb.UserId == returningUser.Id && bb.StrikeGiven);

            if(!stillOverdue && returningUser.Strikes == 0)
            {
                returningUser.CantBorrow = false;
                await _context.SaveChangesAsync();
            }
            await SendEmailForReturnAsync(returningUser, bookToBeReturned.Book.Name);
            return true;
        }

        public async Task<SearchResultsViewModel> ReturnSearchResultsAync(string searchedString)
        {
            SearchResultsViewModel results = new SearchResultsViewModel
            {
                SearchQuery = searchedString
            };

            if (string.IsNullOrWhiteSpace(results.SearchQuery))
            {
                results.Message = "Nothing was typed into the search bar";
                return results;
            }

            string loweredSearch = results.SearchQuery.ToLower();

            var books1 = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.BookGenres)
                .ThenInclude(bg => bg.Genre)
                .Where(b => b.AmountInStock > 0 && (b.Name.ToLower().Contains(loweredSearch) ||
                    (b.Author != null && b.Author.Name.ToLower().Contains(loweredSearch)) || 
                    b.BookGenres.Any(bg => bg.Genre.Name.ToLower().Contains(loweredSearch)) ||
                    b.CategoryString.ToLower().Contains(loweredSearch)))
                .ToListAsync();

            if(books1.Any())
            {
                results.Message = $"Search results for: {results.SearchQuery}";
                results.BooksMatchingQuery = books1.Select(b => new BookListViewModel
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
                return results;
            }
            else
            {
                results.Message = $"No books found matching this search: {results.SearchQuery}";
            }
            return results;
        }

        public async Task<bool> UpdateFavouritesAsync(int amount, int bookId, ClaimsPrincipal user)
        {
            var Rating = await _context.Favourites
                .FirstOrDefaultAsync(r => r.BookId == bookId && r.UserId == _userManager.GetUserId(user));
            if (Rating is not null)
            {
                if (Rating.Amount == amount)
                {
                    Rating.Amount -= amount;
                }
                else
                {
                    Rating.Amount = amount;
                }
                _context.Update(Rating);
                await _context.SaveChangesAsync();
                return true;
            }

            Rating = new Favourite
            {
                UserId = _userManager.GetUserId(user),
                Amount = amount,
                BookId = bookId
            };
            await _context.AddAsync(Rating);
            await _context.SaveChangesAsync();
            return false;
        }
        private async Task SendEmailForReturnAsync(User User, string BookTitle)
        {
            var email = User.Email ?? string.Empty;
            Dictionary<string, string> placeholders = new Dictionary<string, string>()
            {
                {"UserName", User.UserName },
                {"BookTitle", BookTitle }
            };
            var emailBody = await _emailService.LoadEmailTemplateAsync(TemplateNames.ReturnConfirmationEmail, placeholders);
            await _emailService.SendEmailFromServiceAsync(email, "Book Return", emailBody);
        }

        //SEPERATE HELPER METHODS THAT DONT EXPORT ANYTHING TO CONTROLLERS
        private void MapAdminFields(Book bookToEdit, BookCreationViewModel model)
        {
            bookToEdit.Name = model.Name;
            bookToEdit.DateWritten = model.DateWritten;
            bookToEdit.AmountInStock = model.AmountInStock;
            bookToEdit.Pages = model.Pages;
            bookToEdit.Category = model.Category;
            bookToEdit.CategoryString = model.Category.ToString();
            bookToEdit.Description = model.Description;
            bookToEdit.CoverImage = model.CoverImage;
            bookToEdit.ReadingTime = model.ReadingTime;
        }
        private async Task MapAuthor(Book bookToEdit, BookCreationViewModel model)
        {
            var searchedAuthor = await _context.Authors.FirstOrDefaultAsync(a => a.Id == bookToEdit.Author.Id);
            searchedAuthor.Name = model.AuthorName;
        }
        private async Task MapBookGenres(Book bookToEdit, BookCreationViewModel model)
        {
            var existingGenreIds = bookToEdit.BookGenres.Select(bg => bg.GenreId).ToList();
            if (existingGenreIds != null && model.SelectedGenreIds != null)
            {
                var newGenres = model.SelectedGenreIds.Except(existingGenreIds);
                var removedGenres = existingGenreIds.Except(model.SelectedGenreIds);
                bookToEdit.BookGenres = bookToEdit.BookGenres.Where(bg => !removedGenres.Contains(bg.GenreId)).ToList();

                List<BookGenre> bookGenresToAdd = new List<BookGenre>();
                foreach (var genreId in newGenres)
                {
                    bookGenresToAdd.Add(new BookGenre { BookId = bookToEdit.Id, GenreId = genreId });
                }
                if (bookGenresToAdd.Count > 1)
                    await _bookGenreRepository.AddListOfNewBookGenresAsync(bookGenresToAdd);
                else if (bookGenresToAdd.Count == 1)
                    await _bookGenreRepository.AddNewBookGenreAsync(bookGenresToAdd[0]);
            }

        }
    }
}


