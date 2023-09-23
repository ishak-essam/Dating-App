using AutoMapper;
using CourseUdemy.Entity;
using CourseUdemy.Interfaces;

namespace CourseUdemy.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IMapper _mapper;
        private readonly UserDbContext _userDbContext;

        public UnitOfWork(IMapper mapper,UserDbContext userDbContext)
        {
            this._mapper = mapper;
            this._userDbContext = userDbContext;
        }
        public IUser user => new UserRepository(_userDbContext, _mapper) ;

        public IMessageRepo messageRepo => new MessageRepo(_userDbContext,_mapper);

        public ILikesRepo likesRepo =>  new LikesRepos (_userDbContext);

        public async Task<bool> Compelete ( )
        {
            return await _userDbContext.SaveChangesAsync ()>0;
        }

        public bool HasChanges ( )
        {
            return _userDbContext.ChangeTracker.HasChanges ();
        }
    }
}
