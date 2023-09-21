using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CourseUdemy.Entity
{
    public class UserDbContext : IdentityDbContext<User,AppRole,int,
        IdentityUserClaim<int>,
        AppUserRole,
        IdentityUserLogin<int>,
        IdentityRoleClaim<int>,
        IdentityUserToken<int>>
    {
        public UserDbContext ( DbContextOptions options ) : base (options)
        {
        }
        public  DbSet<User> users { get; set; }
        public  DbSet<UserLike> likes { get; set; }
        public  DbSet<Message> Messages { get; set; }
        public  DbSet<Group> Groups { get; set; }
        public  DbSet<Connection> Connections { get; set; }
        protected override void OnModelCreating ( ModelBuilder modelBuilder )
        {
            base.OnModelCreating (modelBuilder);
            modelBuilder.Entity<User>().HasMany(x=>x.UsersRole).WithOne(x=>x.user).HasForeignKey(x=>x.UserId).IsRequired();
            modelBuilder.Entity<AppRole> ().HasMany (x => x.UsersRole).WithOne (x => x.appRole).HasForeignKey (x => x.RoleId).IsRequired ();
            //like
            modelBuilder.Entity<UserLike> ().HasKey (k => new { k.SourceUserId, k.TargetUserId });
            modelBuilder.Entity<UserLike> ().HasOne (k => k.SourceUser).WithMany (
                m => m.LikedUser).HasForeignKey (f => f.SourceUserId).OnDelete (DeleteBehavior.Cascade);
            modelBuilder.Entity<UserLike> ().HasOne (k => k.TargetUser).WithMany (
      m => m.LikedByUser).HasForeignKey (f => f.TargetUserId).OnDelete (DeleteBehavior.Cascade);


            //message
            modelBuilder.Entity<Message> ().HasOne (k => k.Recipient).WithMany (
                    m => m.MessageRecevied).OnDelete (DeleteBehavior.Restrict);
            modelBuilder.Entity<Message> ().HasOne (k => k.Sender).WithMany (
                   m => m.MessageSent).OnDelete (DeleteBehavior.Restrict);
        }

    }
}
