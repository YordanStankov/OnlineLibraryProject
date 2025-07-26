using FInalProject.Data;
using FInalProject.Data.Models;
using FInalProject.Repositories.Interfaces;

namespace FInalProject.Repositories.DataAcces
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDbContext _context;
        public CommentRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAndSaveCommentAsync(Comment newComment)
        {
            _context.Comments.Add(newComment);
            await _context.SaveChangesAsync();
        }
    }
}
