using CourseUdemy.Data;
using CourseUdemy.DTOs;
using CourseUdemy.Entity;
using CourseUdemy.Helpers;
using CourseUdemy.Interfaces;
using CourseUdemy.Services;
using CourseUdemy.SignalR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace CourseUdemy.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApllicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentityCore<User> (x =>
            {
                x.Password.RequireNonAlphanumeric = false;
            }).AddRoles<AppRole> ().AddRoleManager<RoleManager<AppRole>> ().AddEntityFrameworkStores<UserDbContext> ();
            services.AddDbContext<UserDbContext>(opt =>
            {
                opt.UseSqlite(configuration.GetConnectionString("DefulatConnections"));
            });

            services.AddCors (opt => opt.AddPolicy ("CorsPolicy", Policy =>
            {
                Policy.AllowAnyHeader ()
                .AllowAnyMethod ().
                AllowCredentials()
                .WithOrigins ("https://localhost:4200", "https://192.168.1.134:4200");
            }));
            services.AddScoped<ITokenServices, TokenService>();
            services.AddScoped<IUser, UserRepository>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));
            services.AddScoped<IPhotoServices,PhotoServices>();
            services.AddScoped<ILikesRepo,LikesRepos>();
            services.AddScoped<LogUserActivity>();
            services.AddScoped<IMessageRepo,MessageRepo>();
            services.AddSignalR ();
            services.AddSingleton <presenceTracer>();
            return services;
        }
    }
}
