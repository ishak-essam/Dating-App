using CourseUdemy.Extensions;
using CourseUdemy.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CourseUdemy.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync ( ActionExecutingContext context, ActionExecutionDelegate next )
        {
            var resultContext=await next();
            if ( !resultContext.HttpContext.User.Identity.IsAuthenticated  ) return;
            var userId=resultContext.HttpContext.User.GetUserId();
            var repo =resultContext.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
            var user  =await repo.user.GetUserByIDAsync( userId);   
            user.LastActive = DateTime.UtcNow;
            await repo.Compelete ();
        }
    }
}
