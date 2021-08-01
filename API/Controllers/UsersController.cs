using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.IServices;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class UsersController : BaseController
    {
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;
        private readonly IPhotoService photoService;

        public UsersController(IUserRepository userRepository, IMapper mapper,IPhotoService photoService)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
            this.photoService = photoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers()
        {
            var users = await userRepository.GetMembersAsync();
            return Ok(users);
        }

        [HttpGet("{username}",Name ="GetUser")]
        public async Task<ActionResult<MemberDTO>> GetUser(string username)
        {
            return await userRepository.GetMemberAsync(username.ToLower());
        }

        [HttpPut]
        public async Task<ActionResult> updateUser(MemberUpdateDTO updateDTO)
        {
            var username = User.GetUsername();
            var user = await userRepository.GetUserByUsernameAsync(username);
            mapper.Map(updateDTO, user);

            userRepository.Update(user);

            if (await userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update the user");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDTO>> AddPhoto(IFormFile file)
        {
            var user=await userRepository.GetUserByUsernameAsync(User.GetUsername());

            var result=await photoService.AddPhotoAsync(file);

            if(result.Error!=null){
                return BadRequest(result.Error.Message);
            }

            var photo=new Photo{
                URL=result.SecureUrl.AbsoluteUri,
                PublicId=result.PublicId
            };

            if(user.Photos.Count==0)
            {
                photo.IsMain=true;
            }

            user.Photos.Add(photo);

            if(await userRepository.SaveAllAsync()){
                
                return CreatedAtRoute("GetUser",new {username=user.UserName},mapper.Map<PhotoDTO>(photo));
            }

            return BadRequest("Problem adding photo!");    
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user=await userRepository.GetUserByUsernameAsync(User.GetUsername());  

            var photo=user.Photos.FirstOrDefault(x=>x.Id==photoId);

            if (photo.IsMain) 
            return BadRequest("This is already your main photo");

            var currentMain=user.Photos.FirstOrDefault(x=>x.IsMain);

            if(currentMain!=null)
            currentMain.IsMain=false;

            photo.IsMain=true;

           if( await userRepository.SaveAllAsync()) return NoContent();

           return BadRequest("Failed to set main photo");

        }
        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user=await userRepository.GetUserByUsernameAsync(User.GetUsername()); 

            var photo=user.Photos.FirstOrDefault(x=>x.Id==photoId);

            if (photo.IsMain) 
            return BadRequest("You cannot delete your main photo");

            if (photo==null) 
            return NotFound();

            if(photo.PublicId!=null){
            
            var result=await photoService.DeletePhotoAsync(photo.PublicId);

            if(result==null) return BadRequest(result.Error.Message);
                        
            }  

            user.Photos.Remove(photo);

            if(await userRepository.SaveAllAsync()) return Ok();

            return BadRequest("Failed to delete the photo"); 

        }
    }
}