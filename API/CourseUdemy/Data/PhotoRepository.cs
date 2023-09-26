using CourseUdemy.DTOs;
using CourseUdemy.Entity;
using CourseUdemy.Excetions;
using CourseUdemy.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CourseUdemy.Data
{
    public class PhotoRepository: IPhotoRepository
    {
        private readonly UserDbContext _context;
        public PhotoRepository ( UserDbContext context )
        {
            _context = context;
        }
        public async Task<IEnumerable<PhotoForApprovalDto>>
       GetUnapprovedPhotos ( )
        {
            return await _context.Photos
            .IgnoreQueryFilters ()
            .Where (p => p.IsApproved == false)
            .Select (u => new PhotoForApprovalDto
            {
                Id = u.Id,
                Username = u.User.UserName,
                Url = u.Url,
                IsApproved = u.IsApproved
            }).ToListAsync ();
        }
        public async Task<photo> GetPhotoById ( int id )
        {
            return await _context.Photos
            .IgnoreQueryFilters ()
            .SingleOrDefaultAsync (x => x.Id == id);
        }
        public void RemovePhoto ( photo photo )
        {
            _context.Photos.Remove (photo);
        }
    }
}
