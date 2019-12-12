using AutoMapper;
using IronSchool.Entities;
using IronSchool.WebSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IronSchool.WebSite.App_Start
{
    public class AutoMapperConfig
    {
        public static void Initialize()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMissingTypeMaps = false;

                cfg.CreateMap<User, UserVM>();
                cfg.CreateMap<UserVM, User>();

                cfg.CreateMap<User, RoleUserVM>();
                cfg.CreateMap<RoleUserVM, User>();

                cfg.CreateMap<User, MyProfileVM>();
                cfg.CreateMap<MyProfileVM, User>();

                cfg.CreateMap<Role, RoleVM>();
                cfg.CreateMap<RoleVM, Role>();

                cfg.CreateMap<Rule, RoleRuleVM>();
                cfg.CreateMap<RoleRuleVM, Rule>();

                cfg.CreateMap<Student, StudentVM>();
                cfg.CreateMap<StudentVM, Student>();

                cfg.CreateMap<Instructor, InstructorVM>()
                    .ForMember(d => d.UserName, o => o.MapFrom(s => s.User.UserName ))
                    .ForMember(d => d.ContactMail, o => o.MapFrom(s => s.User.Email))
                    .ForMember(d => d.UserCompletedName, o => o.MapFrom(s => $"{ s.User.LastName } { s.User.FirstName }"));

                cfg.CreateMap<InstructorVM, Instructor>();

                cfg.CreateMap<Course, CourseVM>()
                    .ForMember(d => d.InstructorName, o => o.MapFrom(s => $"{ s.Instructor.User.LastName } { s.Instructor.User.FirstName }"))
                    .ForMember(d => d.Students, o => o.Ignore())
                    .AfterMap((s, d) => {
                        foreach (var studentInCourseEntity in s.StudentsInCourse)
                        {
                            d.Students.Add(new StudentBasicIndexVM()
                            {
                                FirstName = studentInCourseEntity.Student.FirstName,
                                LastName = studentInCourseEntity.Student.LastName,
                                StudentId = studentInCourseEntity.StudentId
                            });
                        }
                    });

                cfg.CreateMap<CourseVM, Course>()
                    .ForMember(d => d.StudentsInCourse, o => o.Ignore())
                    .AfterMap((s, d) => {
                        foreach (var studentVM in s.Students)
                        {
                            d.StudentsInCourse.Add(new StudentsInCourse()
                            {
                                StudentId = studentVM.StudentId,
                                CourseId = s.CourseId
                            });
                        }
                    });


                cfg.CreateMap<StudentsInCourse, QualificationVM>()
                    .ForMember(d => d.CourseDescription, o => o.MapFrom(s => s.Course.Description))
                    .ForMember(d => d.StudentName, o => o.MapFrom(s => $"{s.Student.LastName} {s.Student.FirstName}"));

                cfg.CreateMap<QualificationVM, StudentsInCourse>();

            });
        }
    }
}