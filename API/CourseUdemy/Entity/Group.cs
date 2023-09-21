using System.ComponentModel.DataAnnotations;

namespace CourseUdemy.Entity
{
    public class Group
    {
        public Group ( )
        {
        }
        
        public Group ( string name)
        {
            Name = name;
            this.connections = connections;
        }

        [Key]
        public string Name { get; set; }
        public ICollection<Connection> connections { get; set; }=new List<Connection>();
    }
}
