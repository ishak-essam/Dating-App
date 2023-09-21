using CourseUdemy.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseUdemy.Excetions
{
    [Table ("Photos")]
    public class photo
        {
            public int Id { get; set; }
            public string? Url { get; set; }
            public bool IsMain { get; set; }
            public string? PublicId { get; set; }
        public int AppUserId { get; set; }
        public User User { get; set; }
    }
}