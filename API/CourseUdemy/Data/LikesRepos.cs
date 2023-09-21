using CourseUdemy.DTOs;
using CourseUdemy.Entity;
using CourseUdemy.Excetions;
using CourseUdemy.Helpers;
using CourseUdemy.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CourseUdemy.Data
{
    public class LikesRepos : ILikesRepo
    {
        public UserDbContext UserDbContext { get; }
        public LikesRepos(UserDbContext userDbContext)
        {
            UserDbContext = userDbContext;
        }


        public async Task<UserLike> GetUserLike ( int SourceUserId, int TargetUserId )
        {
            return await UserDbContext.likes.FindAsync (SourceUserId, TargetUserId);

        }

        public async Task<PagedList<LikeDTO>> GetUserLikes ( LikeParams likeParams )
        {
            var user=UserDbContext.users.OrderBy (i=>i.UserName ).AsQueryable();
            var likes=UserDbContext.likes.AsQueryable();
            if ( likeParams.predicate == "liked" ) {
                likes = likes.Where (x => x.SourceUserId == likeParams.userId);
                user = likes.Select (x => x.TargetUser);
            }
            if ( likeParams.predicate == "likedBy" )
            {
                likes = likes.Where (x => x.TargetUserId == likeParams.userId);
                user = likes.Select (x => x.SourceUser);
            }
            var likeusers=  user.Select (x => new LikeDTO
            {
                UserName= x.UserName,
                KnowAs=x.KnownAs,
                Age=x.DateofBirth.CalcAgetion(),
                PhotoUrl=x.Photos.FirstOrDefault(x=>x.IsMain).Url,
                City=x.City,
                Id=x.Id

            });
            return await PagedList<LikeDTO>.CreatAsync (likeusers, likeParams.PageNumber,likeParams.PageSize);
        }

        public async Task<User> GetUserWithLikes ( int userId )
        {
            return await UserDbContext.users.Include (x => x.LikedUser).FirstOrDefaultAsync (x=>x.Id==userId);
        }
    }
}
