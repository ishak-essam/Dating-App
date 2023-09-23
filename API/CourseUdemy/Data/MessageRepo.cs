using AutoMapper;
using AutoMapper.QueryableExtensions;
using CourseUdemy.DTOs;
using CourseUdemy.Entity;
using CourseUdemy.Helpers;
using CourseUdemy.Interfaces;
using Microsoft.EntityFrameworkCore;

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

        public async Task<Group> GetGroupForConnection ( string ConnectionId )
        {
            return await _context.Groups.Include (x => x.connections).Where (y => y.connections.Any (c=>c.ConnectionId== ConnectionId)).FirstOrDefaultAsync();
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
            var query= _context.Messages.Include (u=>u.Sender).ThenInclude(p=>p.Photos)
                .Include (u=>u.Recipient).ThenInclude(p=>p.Photos)
                .Where(m=>m.RecipientUsername==currentUsername   &&
                m.RecipientDeleteed==false &&
                m.SenderUsername==recipientUsername  ||
               m.RecipientUsername== recipientUsername &&
                 m.SenderDeleteed==false &&
                m.SenderUsername==currentUsername
                ).OrderByDescending(m=>m.MessageSent).AsQueryable();
            var unreadMessage=query.Where(m=>m.DateRead==null&& m.RecipientUsername==currentUsername ).ToList();
            if ( unreadMessage.Any () ) {
                foreach (var item in unreadMessage )
                {
                    item.DateRead = DateTime.UtcNow;
                }
            }
            return await query.ProjectTo<MessageDTO> (mapper.ConfigurationProvider).ToListAsync();
        }

        public void removeConnections ( Connection connection )
        {
            _context.Connections.Remove (connection);
        }

     
    }
}
