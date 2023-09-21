using CourseUdemy.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace CourseUdemy.Controllers
{
    [ApiController]
    [ServiceFilter(typeof(LogUserActivity))]
    [Route ("api/[controller]")]
    public class BaseAPIController : ControllerBase
    {
       
    }
}
