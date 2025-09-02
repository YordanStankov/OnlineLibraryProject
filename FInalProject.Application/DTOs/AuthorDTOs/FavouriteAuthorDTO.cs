using System.Security.Claims;

namespace FInalProject.Application.DTOs.AuthorDTOs
{
    public class FavouriteAuthorDTO : AuthorDTOBase
    {
        public FavouriteAuthorDTO(ClaimsPrincipal userClaim, int authorId) : base(authorId)
        {
            UserClaim = userClaim;
            Id = authorId;
        }
        public ClaimsPrincipal UserClaim { get; set; }
    }
}
