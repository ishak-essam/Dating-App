using AutoMapper;
using CourseUdemy.DTOs;
using CourseUdemy.Interfaces;
using Microsoft.AspNetCore.Mvc;
using CourseUdemy.Extensions;
using CourseUdemy.Excetions;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using CourseUdemy.Helpers;

namespace CourseUdemy.Controllers
{
    [Authorize]
    public class UsersController : BaseAPIController
    {
      
        private readonly IUser User_Repo;
        private readonly IMapper mapper;
        private readonly IPhotoServices photoServices;
        public UsersController ( IUser user ,IMapper mapper, IPhotoServices photoServices)
        {
            this.User_Repo = user;
            this.mapper = mapper;
            this.photoServices = photoServices;
        }
        [HttpGet("AllMember")]
        public async Task<ActionResult<List<MemberDTO>>> GetAllMember (  )
        {
            return Ok (await User_Repo.GetAllMembersAsysc());

        }
        [HttpGet]
        public async Task<ActionResult<PagedList<MemberDTO>>> GetAll ( [FromQuery] UserParams userParams )
        {
            var usernaem=User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var current=await User_Repo.GetUserByUserNameAsync(User.GetUsername());
            //var current=await User_Repo.GetUserByUserNameAsync("lisa");
            userParams.currentUserName = current.UserName;
            if ( string.IsNullOrEmpty (userParams.Gender) ) {
                userParams.Gender = current.Gender == "male" ? "female" : "male";
            }
            var users=await User_Repo.GetMembersAsync(userParams);
            Response.AddPaginationHeader (new PagintionHelper (users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPage));

            return Ok (users);
        }
        [HttpGet("filter")]

        public async Task<ActionResult<PagedList<MemberDTO>>> Filter ( [FromQuery] UserParams userParams )
        {
            var current=await User_Repo.GetUserByUserNameAsync(User.GetUsername());
            //var current=await User_Repo.GetUserByUserNameAsync("lisa");
            userParams.currentUserName = current.UserName;
            if ( string.IsNullOrEmpty (userParams.Gender) )
            {
                userParams.Gender = current.Gender == "male" ? "female" : "male";
            }
            var users=await User_Repo.GetMembersAsync (userParams);
            Response.AddPaginationHeader (new PagintionHelper (users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPage));

            return Ok (users);
        }

        [HttpGet ("{username}")]
        public async Task<ActionResult<MemberDTO>> GetOne ( string UserName )
        {
            return await User_Repo.GetMemberAsync (UserName);
        }
        [HttpPut]
        public async Task<ActionResult> UpdateCh ( UpdateMemberDTO updateMemberDTO ) 
        {
            var user= await User_Repo.GetUserByUserNameAsync (User.GetUsername());
            if ( user == null ) return NotFound ();
            mapper.Map (updateMemberDTO, user);
            if (await  User_Repo.SaveAllAsync () )  return  Ok ("Save Changes");
            return  BadRequest ("Failed to Update Date");
        }
        [HttpPost ("add-photo")]
        public async Task<ActionResult<PhotoDTO>> PostPhoto (IFormFile file ) 
        {
            var user =await User_Repo.GetUserByUserNameAsync(User.GetUsername());
            if ( user == null ) return NotFound ();
            var resulat= await photoServices.AddPhotoAsync(file);
            if(resulat.Error !=null) return BadRequest ("Failed to Update Date");
            var photo=new photo
            {
                Url=resulat.SecureUrl.AbsoluteUri,
                PublicId=resulat.PublicId
            };
            if( user.Photos.Count > 0 ) photo.IsMain=false;
            user.Photos.Add (photo);
            if ( await User_Repo.SaveAllAsync () ) return mapper.Map<PhotoDTO> (photo);
            return BadRequest ("Failed to Update Photo");
        }

        [HttpPut ("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetPhoto ( int photoId )
        {
            var user =await User_Repo.GetUserByUserNameAsync(User.GetUsername());
            if ( user == null ) return NotFound ();
            var photo =user.Photos.FirstOrDefault(x=>x.Id==photoId);
            if ( photo  == null ) return NotFound ();
            if ( photo.IsMain == true ) return BadRequest ("Photo alreay is Main  Photo");
            var currentMain=user.Photos.FirstOrDefault (x=>x.IsMain);
            if ( currentMain != null ) currentMain.IsMain = false;
            photo.IsMain = true;
            if ( await User_Repo.SaveAllAsync () ) return Ok ();
            return BadRequest ("Problem Setting the main Photo");
        }
        [HttpDelete ("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto ( int photoId ) {
            var user =await User_Repo.GetUserByUserNameAsync(User.GetUsername());
            if ( user == null ) return NotFound ();
            var photo =user.Photos.FirstOrDefault(x=>x.Id==photoId);
            if ( photo == null ) return NotFound ();

            if ( photo.IsMain ) return NotFound ("Can't delete beaucase that is the main photo");
            if ( photo.PublicId !=null ) { 
                var resulat= await photoServices.DeletePhotoAsync(photo.PublicId);
                if(resulat.Error != null )  return BadRequest (resulat.Error.Message);
            }
            user.Photos.Remove(photo);
            if ( await User_Repo.SaveAllAsync () ) return Ok ();
            return BadRequest ("Error When Remove Photo");
        }
    }
}
