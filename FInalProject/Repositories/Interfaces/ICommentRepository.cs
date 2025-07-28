using FInalProject.Data.Models;

namespace FInalProject.Repositories.Interfaces
{
    public interface ICommentRepository
    {
        void AddComment(Comment newComment);
        Task SaveChangesAsync();
    }
}
