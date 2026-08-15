using School.API.Extensions;
using School.API.Middlewares;
using School.BLL;
using School.BLL.Logging;
namespace SchoolManagement
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add rate limiting services to the container using the extension method
            builder.Services.AddApiRateLimiting();

            // Add services to the container.
            builder.Services.AddBLL();
            builder.Services.AddDatabaseLogging();
            builder.Logging.AddDatabaseLogger(builder.Configuration);
            //Extension  method to add JWT authentication
            builder.Services.AddJwtAuthentication(builder.Configuration);
            builder.Services.AddPermissionAuthorization();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            //this extension method is used to add swagger documentation with JWT authentication support
            builder.Services.AddSwaggerDocumentation();
            var app = builder.Build();
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseRateLimiter();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}