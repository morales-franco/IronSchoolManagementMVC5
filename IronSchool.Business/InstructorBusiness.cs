using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronSchool.Business.Base;
using IronSchool.Entities;
using IronSchool.Entities.Dto;

namespace IronSchool.Business
{
    public partial class InstructorBusiness
    {
        public override Instructor Read(object id)
        {
            return Get(i => i.InstructorId == (long)id, new string[] { "User" });
        }

        public override void Insert(Instructor entity)
        {
            CheckEntityABM(entity);
            base.Insert(entity);
        }

        public override void Update(Instructor entity)
        {
            CheckEntityABM(entity);
            base.Update(entity);
        }

        public List<Course> GetHistoricoDeCursosImpartidos()
        {
            long currentUserId = UserBusiness.Current.UserId;
            var instructor = Get(i => i.UserId == currentUserId, new string[] { "Course" });
            return instructor.Course.ToList();
        }

        public List<Student> GetStudentsInActiveCourse()
        {
            List<Student> students = new List<Student>();
            long currentUserId = UserBusiness.Current.UserId;
            var course = GetActiveCourseByUserId(currentUserId);

            if (course == null)
                return students;

            return course.StudentsInCourse.Select(s => s.Student).ToList();

        }

        public Course GetActiveCourseByUserId(long userId)
        {
            var instructor = Get(i => i.UserId == userId, new string[] { "Course.StudentsInCourse.Student" });
            var activeCourse = instructor.Course.Where(c => DateTime.Now.Date >= c.StartDate.Date &&
                                                            c.FinishDate.Date >= DateTime.Now.Date)
                                                          .FirstOrDefault();
            return activeCourse;
        }

        public List<TopStudentDto> GetTopTenBestStudentsInActiveCourses()
        {
            List<TopStudentDto> top = new List<TopStudentDto>();
            long currentUserId = UserBusiness.Current.UserId;

            var activeCourse = GetActiveCourseByUserId(currentUserId);

            if (activeCourse == null)
                return top;

            var students = activeCourse.StudentsInCourse.Where(s => s.ExamQualification.HasValue);

            if (!students.Any())
                return top;

            top.AddRange(students.Take(10).Select(s => new TopStudentDto() { Name = $"{ s.Student.LastName } { s.Student.FirstName}", Average = s.ExamQualification.Value }).ToList());
            return top;
        }

        public List<Instructor> GetActiveInstructors()
        {
            var activeCourses = new CourseBusiness().GetActiveCourses();

            if (!activeCourses.Any())
                return new List<Instructor>();

            return activeCourses.Select(c => c.Instructor).ToList();
        }

        private void CheckEntityABM(Instructor entity)
        {

            var isRepeteadUser = this.Exist(i => i.UserId == entity.UserId &&
                                                 i.InstructorId != entity.InstructorId);

            if (isRepeteadUser)
                throw new ValidationException("ValidationUserSelectIsUsed");

            if (entity.Salary <= 0)
                throw new ValidationException("ValidationInstructorSalaryMustBeUpperZero");

        }
    }
}
