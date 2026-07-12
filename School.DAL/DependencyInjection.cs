using Microsoft.Extensions.DependencyInjection;

namespace School.DAL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDAL(this IServiceCollection services, string connectionString)
        {
            services.AddScoped<PersonData>(_ => new PersonData(connectionString));

            services.AddScoped<StudentData>(_ => new StudentData(connectionString))
                ;
            services.AddScoped<TeacherData>(_ => new TeacherData(connectionString));

            services.AddScoped<ParentData>(_ => new ParentData(connectionString));

            services.AddScoped<StudentParentData>(_ => new StudentParentData(connectionString));

            return services;
        }
    }
}