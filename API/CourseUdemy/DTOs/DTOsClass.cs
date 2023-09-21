using System.ComponentModel.DataAnnotations;

namespace CourseUdemy.DTOs
{
    public class DTOsClass
    {
        [Required]
        public string username { get; set; }
        [Required]
        [StringLength(8,MinimumLength =4)]
        public string Password { get; set; }
    }
}
