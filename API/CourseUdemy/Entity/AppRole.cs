using Microsoft.AspNetCore.Identity;

namespace CourseUdemy.Entity
{
    public class AppRole:IdentityRole<int>
    {
        public ICollection<AppUserRole> UsersRole { get; set; }
    }
}
