using CourseUdemy.DTOs;
using CourseUdemy.Entity;
using CourseUdemy.Helpers;

namespace CourseUdemy.Interfaces
{
    public interface IMessageRepo
    {
        void AddMessage (Message message );
        void DeleteMessage ( Message message );
        Task<Message> GetMessageAsync ( int id );
        Task<PagedList<MessageDTO>> GetMessageForUserAsync ( MessageParams messageParams );
        Task<IEnumerable<MessageDTO>> GetMessageThread ( string currentUsername, string recipientUsername );
        void addGroup( Group group );
        void removeConnections ( Connection connection );
        Task<Connection> GetConnection ( string ConnectionId );
        Task<Group> GetMessageGroup ( string GroupName );
        Task<Group> GetGroupForConnection(string ConnectionId );
    }
}
