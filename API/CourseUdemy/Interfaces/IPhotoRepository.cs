using CourseUdemy.DTOs;
using CourseUdemy.Excetions;

namespace CourseUdemy.Interfaces
{
    public  interface IPhotoRepository
    {
        Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos ( );
        Task<photo> GetPhotoById ( int id );
        void RemovePhoto ( photo photo );
    }
}
