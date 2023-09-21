using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CourseUdemy.Entity;
using CourseUdemy.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace CourseUdemy.Services
{
    public class TokenService : ITokenServices
    {
        private readonly IConfiguration _config;
        private readonly UserManager<User> userManager;
        private readonly SymmetricSecurityKey _key;

        public TokenService ( IConfiguration config,UserManager<User> userManager )
        {
            _config = config;
            this.userManager = userManager;
            _key = new SymmetricSecurityKey (Encoding.UTF8.GetBytes (_config [ "TokenKey" ]));
        }

        public async Task<string> CreateToken ( User user )
        {
            var claims=new List<Claim>
           {
                 new Claim(JwtRegisteredClaimNames.NameId,user.Id.ToString()),
                 new Claim(JwtRegisteredClaimNames.UniqueName,user.UserName)
           };
            var Roles=await userManager.GetRolesAsync(user);
            claims.AddRange (Roles.Select (role => new Claim (ClaimTypes.Role, role)));
            var creds=new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken (token);
        }
    }
}
