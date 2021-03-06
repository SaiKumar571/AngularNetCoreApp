using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.IServices;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class LikesController : BaseController
    {
        private readonly IUserRepository userRepository;
        private readonly ILikesRepository likesRepository;

        public LikesController(IUserRepository userRepository,ILikesRepository likesRepository)
        {
            this.userRepository = userRepository;
            this.likesRepository = likesRepository;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId=User.GetUserId();

            var likedUser=await userRepository.GetUserByUsernameAsync(username);

            var sourceUser=await likesRepository.GetUserWithLikes(sourceUserId);

            if(likedUser==null)
            return NotFound();

            if(sourceUser.UserName==username)
            return BadRequest("You cannot like yourself");

            var userLike=await likesRepository.GetUserLike(sourceUserId,likedUser.Id);

            if(userLike!=null) return BadRequest("You already liked this user");

            userLike=new UserLike{
                SourceUserId=sourceUserId,
                LikeUserId=likedUser.Id
            };

            sourceUser.LikedUsers.Add(userLike);

            if(await userRepository.SaveAllAsync()) return Ok();

            return BadRequest("Failed to like the user");
        }

        [HttpGet()]
        public async Task<ActionResult<IEnumerable<LikeDTO>>> GetUserLikes([FromQuery]LikesParams likesParams)
        {
            likesParams.UserId=User.GetUserId();
            var users= await likesRepository.GetUserLikes(likesParams);

            Response.AddPaginationHeader(users.CurrentPage,users.PageSize,users.TotalCount,users.TotalPages);

            return Ok(users);
        }


    }
}