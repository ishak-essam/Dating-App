using AutoMapper.QueryableExtensions;
using CourseUdemy.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CourseUdemy.Controllers
{
    public class AdminController : BaseAPIController
    {
        public UserManager<User> _userManager { get; }

        public AdminController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        [Authorize(policy:"RequiredAdminRole")]
        [HttpGet("users-with-roles")]
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
        public ActionResult GetPhotosForModerators ( ) {
            return Ok ("Moderators Or admin can see");
        }

    }
}
