using CourseUdemy.Entity;

namespace CourseUdemy.Interfaces
{
    public interface ITokenServices
    {
        Task<string> CreateToken ( User user);
    }
}
