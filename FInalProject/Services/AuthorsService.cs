
namespace FInalProject.Services
{
    public interface IAuthorsService
    {
        Task<bool> LikeAuthorAsync();
    }
    public class AuthorsService : IAuthorsService
    {
        public Task<bool> LikeAuthorAsync()
        {
            throw new NotImplementedException();
        }
    }
}
