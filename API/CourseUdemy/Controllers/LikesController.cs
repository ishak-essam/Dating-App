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
        private readonly IUnitOfWork _unitOfWork;

        public LikesController(IUnitOfWork unitOfWork)
        {
          _unitOfWork = unitOfWork;
        }

        [HttpPost ("{username}")]
        public async Task<ActionResult> AddLike (string Username ) {
            var SourceId =User.GetUserId();
            var LikeUser=  await _unitOfWork.user.GetUserByUserNameAsync (Username);
            var SourceUser=await _unitOfWork.likesRepo.GetUserWithLikes(SourceId);
            if ( LikeUser == null ) NotFound ();
            if ( SourceUser.UserName == Username ) BadRequest ("You Can't Like UR Self");
            var UserLike=await _unitOfWork.likesRepo.GetUserLike(SourceId,LikeUser.Id);
            if ( UserLike != null ) BadRequest ("You already Liked");
            UserLike = new UserLike
            {
                SourceUserId = SourceUser.Id,
                TargetUserId =LikeUser.Id
            };
            SourceUser.LikedUser.Add(UserLike);
            if(await _unitOfWork.Compelete ()) return Ok ();
            return BadRequest ("Failed to Add Like for user");
        }
        [HttpGet]
        public async Task<ActionResult<PagedList<LikeDTO>>> GetUserLikes ([FromQuery] LikeParams likeParams) 
        {
            likeParams.userId=User.GetUserId();
            var users =await _unitOfWork.likesRepo.GetUserLikes (likeParams);
            Response.AddPaginationHeader (new PagintionHelper (users.CurrentPage, users.PageSize, users.TotalCount ,users.TotalPage));
            return Ok (users);
        }
}
}