using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.IServices
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLike(int sourceUserId,int likedUserId);
        Task<AppUser> GetUserWithLikes(int UserId);
        Task<PagedLists<LikeDTO>> GetUserLikes(LikesParams likesParams);


    }
}