using Microsoft.AspNetCore.Identity;

namespace CourseUdemy.Entity
{
    public class AppUserRole:IdentityUserRole<int>
    {
        public User user { get; set; }
        public AppRole appRole { get; set; }
    }
}
