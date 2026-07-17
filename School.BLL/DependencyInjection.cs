using Microsoft.Extensions.DependencyInjection;
using School.BLL.Interfaces;
using School.DAL;
namespace School.BLL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBLL(this IServiceCollection services)
        {
            services.AddDAL();

            services.AddScoped<IPersonService, PersonService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<ITeacherService, TeacherService>();
            services.AddScoped<IParentService, ParentService>();
            services.AddScoped<IStudentParentService, StudentParentService>();
            services.AddScoped<ISubjectService, SubjectService>();
            services.AddScoped<IGradeService, GradeService>();
            services.AddScoped<IClassService, ClassService>();
            services.AddScoped<ITeacherSubjectService, TeacherSubjectService>();
            services.AddScoped<IClassSubjectService, ClassSubjectService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IClassroomService, ClassroomService>();
            services.AddScoped<IScheduleService, ScheduleService>();
            services.AddScoped<IExamService, ExamService>();
            services.AddScoped<IExamTypeService, ExamTypeService>();
            services.AddScoped<IStudentStatusService, StudentStatusService>();
            services.AddScoped<IAttendanceStatusService, AttendanceStatusService>();
            services.AddScoped<IAttendanceService, AttendanceService>();
            services.AddScoped<IStudentGradeService, StudentGradeService>();

            return services;
        }
    }
}