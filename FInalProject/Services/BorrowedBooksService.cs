using FInalProject.Data;
using FInalProject.Data.Models;
using FInalProject.EmailTemplates;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FInalProject.Services
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
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<BorrowedBooksService>>();

                    var now = DateTimeOffset.UtcNow;
                    var overdue = await dbContext.BorrowedBooks
                        .Include(bb => bb.Book)
                        .Include(bb => bb.User)
                        .Where(bb => bb.UntillReturn < now && !bb.StrikeGiven)
                        .ToListAsync(stoppingToken);

                    foreach (var bb in overdue)
                    {
                        var user = bb.User;
                        if (user == null) continue;

                        if (user.Strikes >= 3)
                        {
                            await SendRevokedEmailAsync(emailService, user, bb.Book.Name, stoppingToken);
                            user.CanBorrow = false;
                        }

                        if (!bb.StrikeGiven)
                        {
                            await SendStrikeEmailAsync(emailService, user, bb.Book.Name, stoppingToken);
                        }
                        bb.StrikeGiven = true;
                        user.Strikes++;
                    }

                    await dbContext.SaveChangesAsync(stoppingToken);
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
                TemplateNames.RevokedBorrowingEmail,
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
