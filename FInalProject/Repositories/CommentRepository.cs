using FInalProject.Data;
using FInalProject.Data.Models;
using FInalProject.Application.Interfaces;

namespace FInalProject.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDbContext _context;
        public CommentRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public void AddComment(Comment newComment)
        {
            _context.Comments.Add(newComment);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
