using FInalProject.Data.Models;
using FInalProject.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using FInalProject.Application.EmailTemplates;


namespace FInalProject.Application.Services
{
    public class BorrowedBooksService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _checkTime = TimeSpan.FromHours(24);

        public BorrowedBooksService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var borrowedBookRepository = scope.ServiceProvider.GetRequiredService<IBorrowedBookRepository>();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<BorrowedBooksService>>();

                    var now = DateTimeOffset.UtcNow;
                    var overdue = await borrowedBookRepository.GetOverdueBooksListAsync(stoppingToken ,now);

                    foreach (var bb in overdue)
                    {
                        var user = bb.User;
                        if (user == null) continue;

                        if (user.Strikes >= 3)
                        {
                            await SendRevokedEmailAsync(emailService, user, bb.Book.Name);
                            user.CantBorrow = true;
                        }

                        if (!bb.StrikeGiven)
                        {
                            await SendStrikeEmailAsync(emailService, user, bb.Book.Name);
                        }
                        bb.StrikeGiven = true;
                        user.Strikes++;
                    }

                    await borrowedBookRepository.TSaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"[{DateTimeOffset.UtcNow}] Service error: {ex}");
                }

                var jitter = TimeSpan.FromMinutes(new Random().Next(0, 5));
                await Task.Delay(_checkTime + jitter, stoppingToken);
            }
        }

        private static async Task SendRevokedEmailAsync(
            IEmailService emailService,
            User user,
            string bookName)
        {
            var placeholders = new Dictionary<string, string>
            {
                ["UserName"] = user.UserName
            };
            var body = await emailService.LoadEmailTemplateAsync(
                TemplateNames/**/.RevokedBorrowingEmail,
                placeholders);
            await emailService.SendEmailFromServiceAsync(
                user.Email,
                "Revoked borrowing",
                body);
        }

        private static async Task SendStrikeEmailAsync(
            IEmailService emailService,
            User user,
            string bookName)
        {
            var placeholders = new Dictionary<string, string>
            {
                ["UserName"] = user.UserName,
                ["Strikes"] = user.Strikes.ToString(),
                ["BookName"] = bookName
            };
            var body = await emailService.LoadEmailTemplateAsync(
                TemplateNames.HoardingWarningEmail,
                placeholders);
            await emailService.SendEmailFromServiceAsync(
                user.Email,
                "Strike for hoarding",
                body);
        }
    }
}
