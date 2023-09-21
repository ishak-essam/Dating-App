using AutoMapper;
using AutoMapper.QueryableExtensions;
using CourseUdemy.DTOs;
using CourseUdemy.Entity;
using CourseUdemy.Helpers;
using CourseUdemy.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CourseUdemy.Data
{
    public class MessageRepo : IMessageRepo
    {
        private readonly UserDbContext _context;
        private readonly IMapper mapper;

        public MessageRepo ( UserDbContext context, IMapper mapper )
        {
            _context = context;
            this.mapper = mapper;
        }

        public void addGroup ( Group group )
        {
            _context.Groups.Add (group);
        }

        public void AddMessage ( Message message )
        {
            _context.Messages.Add (message);
        }

        public void DeleteMessage ( Message message )
        {
            _context.Messages.Remove (message);
        }

        public async Task<Connection> GetConnection ( string connectionId )
        {
            return await _context.Connections.FindAsync (connectionId);
        }

        public async Task<Message> GetMessageAsync ( int id )
        {
            return await _context.Messages.FindAsync (id);
        }
        public async Task<PagedList<MessageDTO>> GetMessageForUserAsync ( MessageParams messageParams )
        {
            var query =_context.Messages.OrderByDescending(x=>x.MessageSent).AsQueryable();
            query = messageParams.Container switch
            {
                "Inbox" => query.Where (u => u.RecipientUsername == messageParams.Username && u.RecipientDeleteed==false),
                "Outbox" => query.Where (x => x.SenderUsername == messageParams.Username&&x.SenderDeleteed == false),
                _ => query.Where (x => x.RecipientUsername == messageParams.Username &&x.RecipientDeleteed==false && x.DateRead == null)
            };
            var messges=query.ProjectTo<MessageDTO>(mapper.ConfigurationProvider);
            return await PagedList<MessageDTO>.CreatAsync (messges, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<Group> GetMessageGroup ( string GroupName )
        {
            return await _context.Groups.Include (x =>x.connections).FirstOrDefaultAsync(x=>x.Name==GroupName);
        }

        public async Task<IEnumerable<MessageDTO>> GetMessageThread ( string currentUsername, string recipientUsername )
        {
            var messages=await _context.Messages.Include (u=>u.Sender).ThenInclude(p=>p.Photos)
                .Include (u=>u.Recipient).ThenInclude(p=>p.Photos)
                .Where(m=>m.RecipientUsername==currentUsername   &&
                m.RecipientDeleteed==false &&
                m.SenderUsername==recipientUsername  ||
               m.RecipientUsername== recipientUsername &&
                 m.SenderDeleteed==false &&
                m.SenderUsername==currentUsername
                ).OrderByDescending(m=>m.MessageSent).ToListAsync();
            var unreadMessage=messages.Where(m=>m.DateRead==null&& m.RecipientUsername==currentUsername ).ToList();
            if ( unreadMessage.Any () ) {
                foreach (var item in unreadMessage )
                {
                    item.DateRead = DateTime.UtcNow;
                }
                await _context.SaveChangesAsync ();
            }
            return mapper.Map<IEnumerable<MessageDTO>>(messages);
        }

        public void removeConnections ( Connection connection )
        {
            _context.Connections.Remove (connection);
        }

        public async Task<bool> SaveChangeAsync ( )
        {
            return await _context.SaveChangesAsync () > 0;
        }
    }
}
