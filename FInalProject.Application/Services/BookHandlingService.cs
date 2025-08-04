using FInalProject.Data.Models;
using FInalProject.Application.Interfaces;
using FInalProject.Application.ViewModels.Book.BookOperations;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using FInalProject.Application.EmailTemplates;

namespace FInalProject.Application.Services
{
    public interface IBookHandlingService
    {
        Task<bool> BorrowBookAsync(int borrowId, ClaimsPrincipal Use);

        Task<bool> ReturnBookAsync(ReturnBookViewModel model, ClaimsPrincipal User);
    }
    public class BookHandlingService : IBookHandlingService
    {
        private readonly IEmailService _emailService;
        private readonly IBookRepository _bookRepository;
        private readonly IBorrowedBookRepository _borrowedBookRepository;
        private readonly UserManager<User> _userManager;

        public BookHandlingService(IEmailService emailService,IBookRepository bookRepository,IBorrowedBookRepository borrowedBookRepository, UserManager<User> userManager)
        {
            _bookRepository = bookRepository;
            _emailService = emailService;   
            _borrowedBookRepository = borrowedBookRepository;
            _userManager = userManager;
        }
        public async Task<bool> BorrowBookAsync(int borrowedId, ClaimsPrincipal user)
        {
            var borrowingUser = await _userManager.GetUserAsync(user);
            if (borrowingUser == null) return false;

            var book = await _bookRepository.ReturnBookEntityToBorrowAsync(borrowedId);

            if (book != null && !book.BorrowedBooks.Any(bb => bb.UserId == borrowingUser.Id && bb.BookId == book.Id))
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

            if (!stillOverdue && returningUser.Strikes == 0)
            {
                returningUser.CantBorrow = false;
                await _borrowedBookRepository.SaveChangesAsync();
            }
            await SendEmailForReturnAsync(returningUser, bookToBeReturned.Book.Name);
            return true;
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
    }
}
