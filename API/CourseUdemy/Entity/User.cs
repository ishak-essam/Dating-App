using System.ComponentModel.DataAnnotations;
using CourseUdemy.Excetions;
using Microsoft.AspNetCore.Identity;

namespace CourseUdemy.Entity
{
    public class User :IdentityUser<int>
    {
        [Key]
        //public int Id { get; set; } 
        //public string UserName { get; set; }
        //public byte[] PasswordHash { get; set; }

        //public byte[] PasswordSalt { get; set; }
        public DateOnly DateofBirth { get; set; }
        public string? KnownAs { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime LastActive { get; set; } = DateTime.UtcNow;
        public string? Gender { get; set; }
        public string? Introduction { get; set; }
        public string? LookingFor { get; set; }
        public string? Interests { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        //public List<photo> Photos { get; set; } = new List<photo>();
        public List<photo>? Photos { get; set; } =new ();
        public List<UserLike> LikedByUser { get; set; }
        public List<UserLike> LikedUser { get; set; }
        public List<Message> MessageSent { get; set; }
        public List<Message> MessageRecevied { get; set; }
        public ICollection<AppUserRole> UsersRole { get; set; }

    }
}
