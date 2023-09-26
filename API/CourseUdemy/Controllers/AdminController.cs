using AutoMapper;
using AutoMapper.QueryableExtensions;
using CourseUdemy.Data;
using CourseUdemy.DTOs;
using CourseUdemy.Entity;
using CourseUdemy.Excetions;
using CourseUdemy.Extensions;
using CourseUdemy.Interfaces;
using CourseUdemy.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CourseUdemy.Controllers
{
    public class AdminController : BaseAPIController
    {
        private readonly IMapper _mapper;
        private readonly IPhotoServices _photoServices;
        private readonly IUnitOfWork _unitOfWork;

        public UserManager<User> _userManager { get; }
        public AdminController (IMapper mapper , UserManager<User> userManager, IPhotoServices photoServices, IUnitOfWork
unitOfWork )
        {
            _mapper = mapper;
            _userManager = userManager;
            _photoServices = photoServices;
            _unitOfWork = unitOfWork;
        }
        [Authorize (policy: "RequiredAdminRole")]
        [HttpGet ("users-with-roles")]
        public async Task<ActionResult> GetUserWithRoles ( )
        {
            var users= await _userManager.Users.OrderBy(x=>x.UserName).Select(x=>new
            {
                x.Id,
                Username= x.UserName,
                Role=x.UsersRole.Select(u=>u.appRole.Name).ToList()
            }).ToListAsync();
            return Ok (users);
        }
        [Authorize (policy: "RequiredAdminRole")]
        [HttpPost ("edit-roles/{username}")]
        public async Task<ActionResult> editRoles ( string username, [FromQuery] string roles )
        {
            if ( string.IsNullOrEmpty (roles) ) return BadRequest ("U must select at least  1 role ");
            var selectedRoles=roles.Split(",").ToArray();
            var user =await _userManager.FindByNameAsync(username);
            if ( user == null ) NotFound ();
            var userRoles=await _userManager.GetRolesAsync(user);
            var result=await _userManager.AddToRolesAsync(user,selectedRoles.Except(userRoles));
            if ( !result.Succeeded ) return BadRequest ("failed to add role");
            result = await _userManager.RemoveFromRolesAsync (user, userRoles.Except (selectedRoles));
            if ( !result.Succeeded ) return BadRequest ("failed to remove role");
            return Ok (await _userManager.GetRolesAsync (user));
        }
        [Authorize (policy: "ModeratePhotoRole")]
        [HttpGet ("photos-to-moderate")]
        public async Task<ActionResult> GetPhotosForModerators ( )
        {
            var photos = await
                _unitOfWork.PhotoRepository.GetUnapprovedPhotos();
            return Ok (photos);
        }


       
        
        
        [Authorize (Policy = "ModeratePhotoRole")]
        [HttpPost ("reject-photo/{photoId}")]
        public async Task<ActionResult> RejectPhoto ( int photoId )
        {
            var photo = await 
                _unitOfWork.PhotoRepository.GetPhotoById(photoId);
            if ( photo.PublicId != null )
            {
                var result = await _photoServices.DeletePhotoAsync(photo.PublicId);
                if ( result.Result == "ok" )
                {
                    _unitOfWork.PhotoRepository.RemovePhoto (photo);
                }
            }
            else
            {
                _unitOfWork.PhotoRepository.RemovePhoto (photo);
            }
            await _unitOfWork.Compelete ();
            return Ok ();
        }


        [HttpPost ("add-photo")]
        public async Task<ActionResult<PhotoDTO>> AddPhoto ( IFormFile file )
        {
            var user = await
_unitOfWork.user.GetUserByUserNameAsync(User.GetUsername());
            var result = await _photoServices.AddPhotoAsync(file);
            if ( result.Error != null ) return BadRequest (result.Error.Message);
            var photo = new photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };
            user.Photos.Add (photo);
            if ( await _unitOfWork.Compelete () )
            {
                return CreatedAtRoute ("GetUser", new
                {
                    username =
               user.UserName
                }, _mapper.Map<PhotoDTO> (photo));
            }
            return BadRequest ("Problem addding photo");
        }

        [Authorize (Policy = "ModeratePhotoRole")]
        [HttpPost ("approve-photo/{photoId}")]
        public async Task<ActionResult> ApprovePhoto ( int photoId )
        {
            var photo = await
_unitOfWork.PhotoRepository.GetPhotoById(photoId);
            if ( photo == null ) return NotFound ("Could not find photo");
            photo.IsApproved = true;
            var user = await
_unitOfWork.user.GetUserByPhotoId(photoId);
            if ( !user.Photos.Any (x => x.IsMain) ) photo.IsMain = true;
            await _unitOfWork.Compelete ();
            return Ok ();
        }
    }
}
