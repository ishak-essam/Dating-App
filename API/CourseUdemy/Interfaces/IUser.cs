using CourseUdemy.DTOs;
using CourseUdemy.Entity;
using CourseUdemy.Helpers;

namespace CourseUdemy.Interfaces
{
    public interface IUser
    {
        //Task<bool> SaveAllAsync ( );

        Task<IEnumerable<User>> GetUsersAsync ( );
        void Update(User user );
        Task<User> GetUserByIDAsync ( int id );
        Task<User> GetUserByUserNameAsync ( string UserName );
        Task<PagedList<MemberDTO>> GetMembersAsync ( UserParams userParams );
        Task<MemberDTO> GetMemberAsync ( string UserName );
        Task<List<MemberDTO>> GetAllMembersAsysc (  );
        Task<string>GetUserGender ( string UserName );
        
    }
}
