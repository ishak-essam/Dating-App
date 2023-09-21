using CourseUdemy.DTOs;
using CourseUdemy.Entity;
using CourseUdemy.Extensions;
using CourseUdemy.Helpers;
using CourseUdemy.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CourseUdemy.Controllers
{
    public class LikesController : BaseAPIController
    {
        public IUser UserRepo { get; }
        public ILikesRepo LikesRepo { get; }
        public LikesController(IUser userRepo,ILikesRepo likesRepo)
        {
            UserRepo = userRepo;
            LikesRepo = likesRepo;
        }

        [HttpPost ("{username}")]
        public async Task<ActionResult> AddLike (string Username ) {
            var SourceId =User.GetUserId();
            var LikeUser=  await UserRepo.GetUserByUserNameAsync (Username);
            var SourceUser=await LikesRepo.GetUserWithLikes(SourceId);
            if ( LikeUser == null ) NotFound ();
            if ( SourceUser.UserName == Username ) BadRequest ("You Can't Like UR Self");
            var UserLike=await LikesRepo.GetUserLike(SourceId,LikeUser.Id);
            if ( UserLike != null ) BadRequest ("You already Liked");
            UserLike = new UserLike
            {
                SourceUserId = SourceUser.Id,
                TargetUserId =LikeUser.Id
            };
            SourceUser.LikedUser.Add(UserLike);
            if(await UserRepo.SaveAllAsync()) return Ok ();
            return BadRequest ("Failed to Add Like for user");
        }
        [HttpGet]
        public async Task<ActionResult<PagedList<LikeDTO>>> GetUserLikes ([FromQuery] LikeParams likeParams) 
        {
            likeParams.userId=User.GetUserId();
            var users =await LikesRepo.GetUserLikes (likeParams);
            Response.AddPaginationHeader (new PagintionHelper (users.CurrentPage, users.PageSize, users.TotalCount ,users.TotalPage));
            return Ok (users);
        }
}
}