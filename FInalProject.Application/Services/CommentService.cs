using FInalProject.Domain.Models;
using FInalProject.Application.Interfaces;
using FInalProject.Application.ViewModels.Comment.CommentOperations;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace FInalProject.Application.Services
{
    public interface ICommentService
    {
        Task<int> CreateCommentAsync(CreateCommentViewModel model, ClaimsPrincipal user);
    }
    public class CommentService : ICommentService
    {
        private readonly UserManager<User> _userManager;
        private readonly ICommentRepository _commentRepository;

        public CommentService(UserManager<User> userManager, ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
            _userManager = userManager;
        }

        public async Task<int> CreateCommentAsync(CreateCommentViewModel model, ClaimsPrincipal user)
        {
            
            var userId = _userManager.GetUserId(user);
            if (userId == null)
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
    }
}
