using System.Security.Claims;
namespace CourseUdemy.Extensions
{
    public static class ClaimsExtansions
    {
        public static string GetUsername ( this ClaimsPrincipal claimsPrincipal ) {
             return claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value;
        }
        public static int GetUserId ( this ClaimsPrincipal claimsPrincipal )
        {
            return int.Parse(claimsPrincipal.FindFirst (ClaimTypes.NameIdentifier)?.Value);
        }
    }
}
