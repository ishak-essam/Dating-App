using AutoMapper;
using AutoMapper.QueryableExtensions;
using CourseUdemy.DTOs;
using CourseUdemy.Entity;
using CourseUdemy.Helpers;
using CourseUdemy.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CourseUdemy.Data
{
    public class UserRepository : IUser
    {
        private readonly IMapper _mapper;

        private readonly  UserDbContext _context;
        public UserRepository( UserDbContext user ,IMapper mapper)
        {
            this._context = user;
            this._mapper = mapper;
        }

        public async Task<MemberDTO> GetMemberAsync ( string UserName )
        {
            return await _context.users.Where (x => x.UserName == UserName).ProjectTo<MemberDTO> (_mapper.ConfigurationProvider).SingleOrDefaultAsync();
        }

        public async Task<PagedList<MemberDTO>> GetMembersAsync ( UserParams userParams )
        {
            var query=_context.users.AsQueryable();
            query = query.Where (x => x.UserName != userParams.currentUserName);
            query = query.Where (x => x.Gender == userParams.Gender);
            var  minBob=DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge-1));
            var   maxBob=DateOnly.FromDateTime(DateTime.Today.AddYears(userParams.MinAge));
            //var query= _context.users.ProjectTo<MemberDTO> (_mapper.ConfigurationProvider).AsNoTracking();
            //return await _context.users.ProjectTo<MemberDTO> (_mapper.ConfigurationProvider).ToListAsync ()
            query = query.Where (x => x.DateofBirth >minBob&&x.DateofBirth<maxBob);
            query = userParams.OrderBy switch
            {
                "creadted"=>query.OrderByDescending(x=>x.Created),
               _ =>query.OrderByDescending(x=>x.LastActive)
            };
            //return await PagedList<MemberDTO>.CreatAsync (query, userParams.PageNumber, userParams.PageSize);
            return await PagedList<MemberDTO>.CreatAsync (query.AsNoTracking().ProjectTo<MemberDTO>(_mapper.ConfigurationProvider), userParams.PageNumber, userParams.PageSize);
        }

    public async Task<User> GetUserByIDAsync ( int id )
        {
            return await _context.users.FindAsync(id);
        }

        public async Task<User> GetUserByUserNameAsync ( string UserName )
        {
           var user = _context.users.Include(p=>p.Photos).SingleOrDefaultAsync (ele => ele.UserName == UserName);
            return await user;
        }

        public async  Task<IEnumerable<User>> GetUsersAsync ( )
        {
            return await _context.users.Include(p=>p.Photos).ToListAsync();
        }

        //public async Task<bool> SaveAllAsync ( )
        //{
        //    return await _context.SaveChangesAsync()>0;

        //}

      
        //public async Task<MemberDTO> GetMemberAysc ( string UseNam ) {
        //    var userone=_context.users.Where(ele=>ele.UserName==UseNam).ProjectTo<MemberDTO>(_mapper.ConfigurationProvider).SingleOrDefaultAsync();
        //    return await _context.users.Where(ele=>ele.UserName==UseNam).ProjectTo<MemberDTO>(_mapper.ConfigurationProvider).SingleOrDefaultAsync();
        //}
        public async Task<List<MemberDTO>> GetAllMembersAsysc ( )
        {
            var us=_context.users.ProjectTo<MemberDTO> (_mapper.ConfigurationProvider).ToListAsync ();
            return await _context.users.ProjectTo<MemberDTO> (_mapper.ConfigurationProvider).ToListAsync ();
        }

        public void Update ( User user )
        {
           _context.Entry(user).State= EntityState.Modified;
        }

        public async Task<string> GetUserGender ( string UserName )
        {
            return await _context.users.Where (x => x.UserName == UserName).Select(x=>x.Gender).FirstOrDefaultAsync();

        }
    }
}
