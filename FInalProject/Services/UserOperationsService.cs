using FInalProject.Data;
using FInalProject.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FInalProject.Services
{
    public interface IUserOperationsService
    {

    }
    public class UserOperationsService : IUserOperationsService
    {
        public readonly ILogger<UserOperationsService> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public UserOperationsService(ApplicationDbContext context, UserManager<User> userManager, ILogger<UserOperationsService> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }


    }
}
