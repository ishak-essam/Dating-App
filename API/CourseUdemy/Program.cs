using CourseUdemy.Data;
using CourseUdemy.Entity;
using CourseUdemy.Excetions;
using CourseUdemy.Extensions;
using CourseUdemy.Middleware;
using CourseUdemy.SignalR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class Program
{
    private static async Task Main ( string [ ] args )
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddSwaggerGen ();
        //add services
        builder.Services.AddControllers ();
        builder.Services.AddApllicationServices (builder.Configuration);
        builder.Services.AddIdetityServices (builder.Configuration);
        var connString="";
        if ( builder.Environment.IsDevelopment () )
            connString = builder.Configuration.GetConnectionString ("DefaultConnection");
        else
        {
            // Use connection string provided at runtime by Heroku.
            var connUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

            // Parse connection URL to connection string for Npgsql
            connUrl = connUrl.Replace ("postgres://", string.Empty);
            var pgUserPass = connUrl.Split("@")[0];
            var pgHostPortDb = connUrl.Split("@")[1];
            var pgHostPort = pgHostPortDb.Split("/")[0];
            var pgDb = pgHostPortDb.Split("/")[1];
            var pgUser = pgUserPass.Split(":")[0];
            var pgPass = pgUserPass.Split(":")[1];
            var pgHost = pgHostPort.Split(":")[0];
            var pgPort = pgHostPort.Split(":")[1];
            var updatedHost = pgHost.Replace("flycast", "internal");

            connString = $"Server={updatedHost};Port={pgPort};User Id={pgUser};Password={pgPass};Database={pgDb};";
        }
        builder.Services.AddDbContext<UserDbContext> (opt =>
        {
            opt.UseNpgsql (connString);
        });

        var app = builder.Build();
        app.UseMiddleware<ExceptionMiddleware> ();
        app.UseCors ("CorsPolicy");
        //builder.Services.AddCors (x=>xAllowAnyHeader ()
        //        .AllowAnyMethod ()
        //        .AllowCredentials ()
        //        .AllowAnyOrigin ());
        //app.UseCors (x=>x.AllowAnyHeader ()
        //        .AllowAnyMethod ()
        //        .AllowCredentials ()
        //        .WithOrigins ("https://localhost:4200/" ));
        app.UseAuthentication ();
        app.UseAuthorization ();
        app.UseDefaultFiles ();
        app.UseStaticFiles ();
        app.MapControllers ();
        app.MapFallbackToController ("Index", "Fallback");
        app.MapHub <PresenceHub>("hubs/presence");
        app.MapHub <MessageHub>("hubs/message");
        if ( app.Environment.IsDevelopment () )
        {
            app.UseSwagger ();
            app.UseSwaggerUI ();
        }
        using var scope =app.Services.CreateScope ();
        var services=scope.ServiceProvider;
        var context= services.GetRequiredService<UserDbContext>();
        var logger=services.GetRequiredService<ILogger<Program>>();
        try {
            var usermanger=services.GetRequiredService<UserManager<User>>();
            var roleManger=services.GetRequiredService<RoleManager<AppRole>>();
            await context.Database.MigrateAsync ();
            //await context.Database.ExecuteSqlRawAsync (" Delete from \"Connections\"");
            await Seed.ClearConnection (context);
            await Seed.SeedUser (usermanger,roleManger);
        }
        catch ( Exception ex )
        {

            logger.LogError (ex, "Error Occured while Migrating Process");
        }
        app.Run ();
    }
}
/*
builder.Services.AddControllers ();
builder.Services.AddCors (opt =>
{
    opt.AddPolicy ("CorsPolicy", Policy =>
    {
        Policy.AllowAnyHeader ()
                .AllowAnyMethod ()
                .AllowAnyOrigin ();
    });
});
builder.Services.AddAuthentication (JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer (options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey (Encoding.UTF8.GetBytes (builder.Configuration [ "TokenKey" ])),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });
builder.Services.AddScoped<ITokenServices, TokenServices> ();
builder.Services.AddScoped<IUser, UserRepository> ();
builder.Services.AddAutoMapper (AppDomain.CurrentDomain.GetAssemblies ());
//builder.Services.AddEndpointsApiExplorer ();
//builder.Services.AddSwaggerGen ();
builder.Services.AddDbContext<UserDbContext> (opt =>
{
    opt.UseSqlite (builder.Configuration.GetConnectionString ("DefulatConnections"));
});
//app.UseAuthorization ();
app.UseCors ("CorsPolicy");
app.UseMiddleware<ExceptionModule> ();
//app.UseCors (cors => cors.AllowAnyHeader ().AllowAnyMethod ().AllowAnyOrigin ());

// Configure the HTTP request pipeline.
if ( app.Environment.IsDevelopment () )
{
    app.UseSwagger ();
    app.UseSwaggerUI ();
}
app.UseHttpsRedirection ();
app.UseAuthorization ();
app.MapControllers ();
using var scope = app.Services.CreateScope();
var Services = scope.ServiceProvider;
try
{
    var context= Services.GetRequiredService<UserDbContext>();
    await context.Database.MigrateAsync ();
    await Seed.SeedUser (context);
}
catch ( Exception ex )
{
    var logger=Services.GetRequiredService<ILogger<Program>>();

    logger.LogError (ex, "Error Occured while Migrating Process" + ex.Message);
}
 */