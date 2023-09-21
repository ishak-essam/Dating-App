using CourseUdemy.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace CourseUdemy.Controllers
{
    public class BuggyController : BaseAPIController
    {
        private readonly UserDbContext _context;

        public BuggyController ( UserDbContext context )
        {
            _context = context;

        }


        [Authorize]
        [HttpGet ("auth")]
        public ActionResult<string> GetSecret ( )
        {
            return "secret text";
        }



        [HttpGet ("not-found")]
        public ActionResult<User> GetNotFound ( )
        {
            var thing = _context.users.Find(-1);
            if ( thing == null )
            {
                return NotFound ();
            }
            return thing;

        }


        [HttpGet ("server-error")]
        public ActionResult<string> GetServerError ( )
        {
            var thing = _context.users.Find(-1);

            var thingToReturn = thing.ToString();

            return thingToReturn;
        }


        [HttpGet ("bad-request")]
        public ActionResult<string> GetBadRequest ( )
        {
            return BadRequest ("This was not a good request");
        }


    }
}
