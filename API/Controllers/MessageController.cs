using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.IServices;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class MessageController : BaseController
    {
        private readonly IUserRepository userRepository;
        private readonly IMessagesRepository messagesRepository;
        private readonly IMapper mapper;

        public MessageController(IUserRepository userRepository,IMessagesRepository messagesRepository,IMapper mapper)
        {
            this.userRepository = userRepository;
            this.messagesRepository = messagesRepository;
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDTO>> CreateMessage(CreateMessageDTO messageDTO)
        {
            var username=User.GetUsername();

            if(username==messageDTO.RecipientUsername.ToLower())
            {
                return BadRequest("You cannot send messages to yourself");
            }

            var sender=await userRepository.GetUserByUsernameAsync(username);

            var recipient=await userRepository.GetUserByUsernameAsync(messageDTO.RecipientUsername);

            if(recipient==null)
            return NotFound();

            var message=new Message{
                Sender=sender,
                Recipient=recipient,
                SenderUsername=sender.UserName,
                RecipientUsername=recipient.UserName,
                Content=messageDTO.Content
            };

            messagesRepository.AddMessage(message);

            if(await messagesRepository.SaveAllAsync())
            return Ok(mapper.Map<MessageDTO>(message));

            return BadRequest("Failed to send message");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessagesForUser([FromQuery]MessageParams messageParams)
        {
            messageParams.Username=User.GetUsername();

            var messages=await messagesRepository.GetMessagesForUser(messageParams);

            Response.AddPaginationHeader(messages.CurrentPage,messages.PageSize,messages.TotalCount,messages.TotalPages);

            return messages;
        }

         [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessageThread(string username)
        {
            var currentUsername=User.GetUsername();

            return Ok(await messagesRepository.GetMessageThread(currentUsername,username));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username=User.GetUsername();

            var message=await messagesRepository.GetMessage(id);

            if(message.SenderUsername!=username && message.RecipientUsername!=username){
                return Unauthorized();
            }

            if(message.SenderUsername==username) message.SenderDeleted=true;
            
            if(message.RecipientUsername==username) message.RecipientDeleted=true;

            if(message.SenderDeleted && message.RecipientDeleted)
            messagesRepository.DeleteMessage(message);

            if(await messagesRepository.SaveAllAsync())
            return Ok();

            return BadRequest("Problem deleting the message!");

        }



    }
}