using FInalProject.Domain.Models;

namespace FInalProject.Application.Interfaces
{
    public interface ICommentRepository
    {
        void AddComment(Comment newComment);
        Task SaveChangesAsync();
    }
}
