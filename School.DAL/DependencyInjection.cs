using Microsoft.Extensions.DependencyInjection;
using School.DAL.Interfaces;
namespace School.DAL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDAL(this IServiceCollection services)
        {
            services.AddScoped<IPersonData, PersonData>();

            services.AddScoped<IStudentData, StudentData>();
            ;
            services.AddScoped<ITeacherData, TeacherData>();

            services.AddScoped<IParentData, ParentData>();

            services.AddScoped<IStudentParentData, StudentParentData>();

            services.AddScoped<ISubjectData, SubjectData>();

            services.AddScoped<IGradeData, GradeData>();

            services.AddScoped<IClassData, ClassData>();

            services.AddScoped<ITeacherSubjectData, TeacherSubjectData>();

            services.AddScoped<IClassSubjectData, ClassSubjectData>();

            services.AddScoped<IUserData, UserData>();

            services.AddScoped<IClassroomData, ClassroomData>();

            services.AddScoped<IScheduleData, ScheduleData>();

            services.AddScoped<IExamData, ExamData>();

            services.AddScoped<IExamTypeData, ExamTypeData>();

            services.AddScoped<IStudentStatusData, StudentStatusData>();

            services.AddScoped<IAttendanceStatusData, AttendanceStatusData>();

            services.AddScoped<IAttendanceData, AttendanceData>();

            services.AddScoped<IStudentGradeData, StudentGradeData>();

            return services;
        }
    }
}