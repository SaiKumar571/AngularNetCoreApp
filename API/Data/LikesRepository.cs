using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.IServices;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext context;

        public LikesRepository(DataContext context)
        {
            this.context = context;
        }

        public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
        {
            return await context.Likes.FindAsync(sourceUserId,likedUserId);
        }

        public async Task<PagedLists<LikeDTO>> GetUserLikes(LikesParams likesParams)
        {
            var users=context.Users.OrderBy(u=>u.UserName).AsQueryable();
            var likes=context.Likes.AsQueryable();

            if(likesParams.Predicate=="liked")
            {
                likes=likes.Where(likes=>likes.SourceUserId==likesParams.UserId);
                users=likes.Select(like=>like.LikedUser);
            }

            if(likesParams.Predicate=="likedBy")
            {
                likes=likes.Where(likes=>likes.LikeUserId==likesParams.UserId);
                users=likes.Select(like=>like.SourceUser);
            }
           var likedUsers= users.Select(user=>new LikeDTO{
                Username=user.UserName,
                KnownAs=user.KnownAs,
                Age=user.DateOfBirth.CalculateAge(),
                PhotoUrl=user.Photos.FirstOrDefault(x=>x.IsMain).URL,
                City=user.City,
                Id=user.Id
            });

            return await PagedLists<LikeDTO>.CreateAsync( likedUsers,likesParams.PageNumber,likesParams.PageSize);
        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await context.Users.Include(x=>x.LikedUsers).FirstOrDefaultAsync(x=>x.Id==userId);
        }
    }
}