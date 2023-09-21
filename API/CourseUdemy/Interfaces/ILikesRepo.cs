using CourseUdemy.DTOs;
using CourseUdemy.Entity;
using CourseUdemy.Helpers;

namespace CourseUdemy.Interfaces
{
    public interface ILikesRepo
    {
        Task<UserLike> GetUserLike ( int SourceUserId, int TargetUserId );
        Task<User> GetUserWithLikes ( int userId );
        Task<PagedList<LikeDTO>> GetUserLikes ( LikeParams likeParams );
    }
}
