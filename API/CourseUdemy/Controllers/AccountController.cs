using AutoMapper;
using CourseUdemy.DTOs;
using CourseUdemy.Entity;
using CourseUdemy.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace CourseUdemy.Controllers
{
   
    public class AccountController : BaseAPIController
    {
        private readonly UserDbContext _context;
        private readonly UserManager<User> userManager;
        private readonly ITokenServices _tokenService;
        private readonly IMapper mapper;

        public AccountController (UserManager<User> userManager , ITokenServices tokenService,IMapper mapper)       {
            this.userManager = userManager;
            _tokenService = tokenService;
            this.mapper = mapper;
        }


        [HttpPost ("Register")]
        public async Task<ActionResult<UserDTO>> Register ( registerDTO registerDto )
        {
            if ( await UserExists (registerDto.username) ) return BadRequest ("Username is taken");
            var user =mapper.Map<User>(registerDto);
            user.UserName = registerDto.username.ToLower ();
            //using var hmac=new HMACSHA512();
            //user.PasswordHash = hmac.ComputeHash (Encoding.UTF8.GetBytes (registerDto.Password));
            //user.PasswordSalt = hmac.Key;
            var result =await userManager.CreateAsync (user,registerDto.Password);

            if ( !result.Succeeded ) return BadRequest (result.Errors);

            var rolesResult=await userManager.AddToRoleAsync(user,"Member");
            if ( !rolesResult.Succeeded ) BadRequest (rolesResult.Errors);
            return new UserDTO
            {
                UserName = user.UserName,
                Token = await _tokenService.CreateToken (user),
                PhotoUrl=user.Photos.FirstOrDefault(ele=>ele.IsMain)?.Url,
                knownAs= user.KnownAs,
                Gender = user.Gender
                   
            };

        }

        [HttpPost ("Login")]
        public async Task<ActionResult<UserDTO>> Login ( LoginDto loginDto )
        {
            var user= await userManager.Users
        .Include(x=>x.Photos)
        .SingleOrDefaultAsync(x=> x.UserName.ToLower()==loginDto.Username.ToLower());

            if ( user == null ) return Unauthorized ("invalid username");
            var result=await userManager.CheckPasswordAsync (user, loginDto.Password);
            if ( !result ) return Unauthorized ("invalid password");

            //using var hmac=new HMACSHA512(user.PasswordSalt);
            //var ComputedHash=hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            //for ( int i = 0; i < ComputedHash.Length; i++ )
            //{
            //    if ( ComputedHash [ i ] != user.PasswordHash [ i ] ) return Unauthorized ("Invalid password");
            //}
            return new UserDTO
            {
                UserName = user.UserName,
                Token = await _tokenService.CreateToken (user),
                PhotoUrl = user.Photos.FirstOrDefault (x => x.IsMain)?.Url,
                knownAs = user.KnownAs,
                Gender = user.Gender,
                
            }; 

        }

        private async Task<bool> UserExists ( string username )
        {
            return await userManager.Users.AnyAsync (x => x.UserName == username.ToLower ());
        }
    }
}
