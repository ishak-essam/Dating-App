namespace CourseUdemy.Interfaces
{
    public interface IUnitOfWork
    {
        IUser user { get; }
      IMessageRepo messageRepo { get; }
        ILikesRepo likesRepo { get; }
        Task<bool> Compelete ();
        bool HasChanges();
    }
}
