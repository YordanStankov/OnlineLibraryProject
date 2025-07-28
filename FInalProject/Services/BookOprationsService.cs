using FInalProject.Data.Models;
using FInalProject.EmailTemplates;
using FInalProject.Repositories.DataAcces;
using FInalProject.Repositories.Interfaces;
using FInalProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FInalProject.Services
{
    public interface IBookOprationsService
    {
        Task<bool> BorrowBookAsync(int borrowId, ClaimsPrincipal Use);
        Task<bool> EditBookAsync(BookCreationViewModel model);
        Task<SearchResultsViewModel> ReturnSearchResultsAync(string searchedString);
        Task<bool> DeleteBookAsync(int doomedId);
        Task<int> CreateCommentAsync(CreateCommentViewModel model, ClaimsPrincipal user);
        Task UpdateFavouritesAsync(int amount, int bookId, ClaimsPrincipal user);
        Task<bool> ReturnBookAsync(ReturnBookViewModel model, ClaimsPrincipal User);
        Task<List<BookListViewModel>> ApplyFiltering (IEnumerable<BookListViewModel> books, FilteringViewModel filtering);
       
    }
    public class BookOprationsService : IBookOprationsService
    {
        private readonly ILogger<BookOprationsService> _logger;
       
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;
        private readonly IBookRepository _bookRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly IBookGenreRepository _bookGenreRepository;
        private readonly IBorrowedBookRepository _borrowedBookRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IFavouriteRepository _favouriteRepository;
        public BookOprationsService( 
            UserManager<User> userManager, 
            ILogger<BookOprationsService> logger, 
            IEmailService emailService, 
            IBookRepository bookRepository, 
            IAuthorRepository authorRepository, 
            IBookGenreRepository bookGenreRepository,
            IBorrowedBookRepository borrowedBookRepository,
            ICommentRepository commentRepository,
            IFavouriteRepository favouriteRepository)
        {
           
            _userManager = userManager;
            _logger = logger;
            _emailService = emailService;
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _bookGenreRepository = bookGenreRepository;
            _borrowedBookRepository = borrowedBookRepository;
            _commentRepository = commentRepository;
            _favouriteRepository = favouriteRepository;
        }

        public async Task<bool> BorrowBookAsync(int borrowedId, ClaimsPrincipal user)
        {
            var borrowingUser = await _userManager.GetUserAsync(user);
            if (borrowingUser == null) return false;

            _logger.LogInformation("BORROWING BOOK");
            var book = await _bookRepository.ReturnBookEntityToBorrowAsync(borrowedId);

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
                    _borrowedBookRepository.AddBorrowedBook(borrowedBook);
               await _borrowedBookRepository.SaveChangesAsync();

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
             _commentRepository.AddComment(comment);
            await _commentRepository.SaveChangesAsync();
            return comment.BookId;
        }

        public async Task<bool> DeleteBookAsync(int doomedId)
        {
            _logger.LogInformation("DELETING BOOK");
            var doomedBook = await _bookRepository.ReturnBookEntityToDeleteAsync(doomedId);

            if(doomedBook != null)
            {
                 _bookRepository.RemoveBook(doomedBook);
                await _bookRepository.SaveChangesAsync();
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

                _authorRepository.UpdateAuthor(bookToEdit.Author);
                 _bookRepository.UpdateBook(bookToEdit);
                _logger.LogInformation("EDITED BOOK BY ADMIN");
                 await _bookRepository.SaveChangesAsync();
                return true;
            }
           else if(model.editor == 1)
            {
                _logger.LogInformation("EDITED BOOK BY LIBRARIAN");
                bookToEdit.Name = model.Name;
                 _bookRepository.UpdateBook(bookToEdit);
                await _bookRepository.SaveChangesAsync();
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

            var bookToBeReturned = await _borrowedBookRepository.ReturnBorrowedBookToReturnAsync(model.BookId, model.UserId);

            if (bookToBeReturned == null)
            {
                return false;
            }

            if (bookToBeReturned.StrikeGiven)
            {
                returningUser.Strikes -= 1;
            }
            
            _borrowedBookRepository.RemoveBorrowedBook(bookToBeReturned);
            await _borrowedBookRepository.SaveChangesAsync();
            
            bool stillOverdue = await _borrowedBookRepository.UserHasOverdueBooksAsync(returningUser.Id);

            if(!stillOverdue && returningUser.Strikes == 0)
            {
                returningUser.CantBorrow = false;
                await _borrowedBookRepository.SaveChangesAsync();
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

            var searchedBooks = await _bookRepository.RenderSearchedBookListAsync(results.SearchQuery);

            if(searchedBooks.Any())
            {
                results.Message = $"Search results for: {results.SearchQuery}";
                results.BooksMatchingQuery = searchedBooks;
                return results;
            }
            else
            {
                results.Message = $"No books found matching this search: {results.SearchQuery}";
            }
            return results;
        }

        public async Task UpdateFavouritesAsync(int amount, int bookId, ClaimsPrincipal user)
        {
            if(user != null)
            {
                string userId = _userManager.GetUserId(user);

                var Rating = await _favouriteRepository.ReturnFavouriteEntityToUpdateAsync(bookId, userId);
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
                    _favouriteRepository.UpdateFavourite(Rating);
                }
                else
                {
                    Rating = new Favourite
                    {
                        UserId = userId,
                        Amount = amount,
                        BookId = bookId
                    };
                    await _favouriteRepository.AddFavouriteAsync(Rating);
                }
                await _favouriteRepository.SaveChangesAsync();
            }
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
            var searchedAuthor = await _authorRepository.GetAuthorByIdAsync(bookToEdit.AuthorId);
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


