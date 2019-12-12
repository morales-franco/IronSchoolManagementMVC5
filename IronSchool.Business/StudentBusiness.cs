using IronSchool.Entities;
using IronSchool.Entities.Dto;
using IronSchool.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronSchool.Business
{
    public partial class StudentBusiness
    {
        public List<TopStudentDto> GetTopTenStudent()
        {
            List<TopStudentDto> top = new List<TopStudentDto>();
            var activeCourses = new CourseBusiness().GetActiveCourses();

            var students =  activeCourses.SelectMany(c => c.StudentsInCourse).Where(c => c.ExamQualification.HasValue);

            if (!students.Any())
                return top;
            
            foreach (var studentGroup in students.GroupBy(s => s.StudentId ))
            {
                TopStudentDto data = new TopStudentDto();
                data.Name = $"{ studentGroup.First().Student.LastName } { studentGroup.First().Student.FirstName }";

                int total = 0;

                foreach (var student in studentGroup)
                {
                    total += student.ExamQualification.Value;
                }

                data.Average = total / studentGroup.Count();

                top.Add(data);
            }

            top = top.OrderByDescending(t => t.Average).Take(10).ToList();
            return top;
        }

        public List<Student> GetStudentApproved()
        {
            var activeCourses = new CourseBusiness().GetActiveCourses();

            var students = activeCourses.SelectMany(c => c.StudentsInCourse).Where(c => c.ExamQualification.HasValue);

            if (!students.Any())
                return new List<Student>();

            return students.Where(x => x.ExamQualification.Value >= Constants.QualificationApproved)
                           .Select(s => s.Student).ToList();
        }

        public List<Student> GetActiveStudents()
        {
            var activeCourses = new CourseBusiness().GetActiveCourses();

            if (!activeCourses.Any())
                return new List<Student>();

            var studentsInCourse = activeCourses.SelectMany(c => c.StudentsInCourse);

            if (!studentsInCourse.Any())
                return new List<Student>();

            return studentsInCourse.Select(s => s.Student).ToList();
        }

        public List<Student> GetStudentDispproved()
        {
            var activeCourses = new CourseBusiness().GetActiveCourses();

            var students = activeCourses.SelectMany(c => c.StudentsInCourse).Where(c => c.ExamQualification.HasValue);

            if (!students.Any())
                return new List<Student>();

            return students.Where(x => x.ExamQualification.Value < Constants.QualificationApproved)
                           .Select(s => s.Student).ToList();
        }

    }
}
