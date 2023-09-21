using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;

namespace CourseUdemy.Extensions
{
    public static class IdentitiyServices
    {
        public static IServiceCollection AddIdetityServices ( this IServiceCollection services,
            IConfiguration config )
        {
            services.AddAuthentication (JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer (options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey (Encoding.UTF8.GetBytes (config [ "TokenKey" ])),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accesstoken=context.Request.Query["Access_Token"];
                        var path =context.HttpContext.Request.Path;
                        if ( !string.IsNullOrEmpty (accesstoken) && path.StartsWithSegments ("/hubs") )
                        {
                            context.Token = accesstoken;

                        };
                        return Task.CompletedTask;
                    }
                };
            });
            services.AddAuthorization (options =>
            {
                options.AddPolicy ("RequiredAdminRole", policy => policy.RequireRole ("Admin"));
                options.AddPolicy ("ModeratePhotoRole", policy => policy.RequireRole ("Admin", "Moderator"));
            });

            return services;
        }
    }
}
