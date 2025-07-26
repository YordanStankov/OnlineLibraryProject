using FInalProject.Data.Models;

namespace FInalProject.Repositories.Interfaces
{
    public interface ICommentRepository
    {
        Task AddAndSaveCommentAsync(Comment newComment);
    }
}
