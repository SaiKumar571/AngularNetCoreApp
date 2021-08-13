using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.IServices;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessagesRepository : IMessagesRepository
    {
        private readonly DataContext context;
        private readonly IMapper mapper;

        public MessagesRepository(DataContext context,IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async void AddMessage(Message message)
        {
           await context.Messages.AddAsync(message);
        }

        public void DeleteMessage(Message message)
        {
           context.Messages.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
          return await context.Messages.FindAsync(id);
        }

        public async Task<PagedLists<MessageDTO>> GetMessagesForUser(MessageParams messageParams)
        {
            var query=context.Messages.OrderByDescending(m=>m.MessageSent).AsQueryable();

            query=messageParams.Container switch
            {
                "inbox"=>query.Where(u=>u.Recipient.UserName  ==messageParams.Username && u.RecipientDeleted==false),
                "outbox"=>query.Where(u=>u.Sender.UserName  ==messageParams.Username && u.SenderDeleted==false),
                _=>query.Where(u=>u.Recipient.UserName  ==messageParams.Username && u.DateRead==null && u.RecipientDeleted==false)
            };

            var messages=query.ProjectTo<MessageDTO>(mapper.ConfigurationProvider);

            return await PagedLists<MessageDTO>.CreateAsync(messages,messageParams.PageNumber,messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUsername, string recipientUsername)
        {
            var messages=await context.Messages
            .Include(u=>u.Sender).ThenInclude(p=>p.Photos)
            .Include(u=>u.Recipient).ThenInclude(p=>p.Photos)
            .Where(x=>x.Recipient.UserName==currentUsername && x.RecipientDeleted==false && x.Sender.UserName==recipientUsername  || x.Recipient.UserName==recipientUsername && x.Sender.UserName==currentUsername && x.SenderDeleted==false).OrderBy(x=>x.MessageSent).ToListAsync();

            var unreadMessages= messages.Where(m=>m.DateRead==null && m.RecipientUsername==currentUsername).ToList();

            if(unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead=DateTime.Now;
                }
                await context.SaveChangesAsync();
            }

            return mapper.Map<IEnumerable<MessageDTO>>(messages);

        }

        public async Task<bool> SaveAllAsync()
        {
            return await context.SaveChangesAsync()>0;
        }
    }
}