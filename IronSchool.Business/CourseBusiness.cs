using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronSchool.Business.Base;
using IronSchool.Entities;
using IronSchool.Utils;

namespace IronSchool.Business
{
    public partial class CourseBusiness
    {
        public override Course Read(object id)
        {
            return Get(c => c.CourseId == (long)id, new string[] { "StudentsInCourse.Student", "Instructor.User" });
        }

        public override void Insert(Course entity)
        {
            CheckEntityABM(entity);
            base.Insert(entity);
        }

        private void CheckEntityABM(Course entity)
        {
            //Profesor No este asignado a otro curso en ese Periodo.
            if (Exist(c => c.InstructorId == entity.InstructorId &&
                           c.CourseId != entity.CourseId &&
                           (entity.StartDate >= c.StartDate && entity.StartDate <= c.FinishDate ||
                            entity.FinishDate >= c.StartDate && entity.FinishDate <= c.FinishDate)))
                throw new ValidationException("ValidationInstructorHasOtherCourseInTheRange");


            //No se supere la cantidad de alumnos de la maxima especificada
            if (entity.StudentsInCourse.Count > entity.StudentCountMax)
                throw new ValidationException("ValidationStudentInCourseExceedCapacity");

            //Alumno No forme parte de otro curso en ese periodo.
            if (entity.StudentsInCourse.Any())
            {
                var studentIds = entity.StudentsInCourse.Select(s => s.StudentId).ToList();

                var courseBeginInTheSameRange = GetList(c => c.CourseId != entity.CourseId &&
                                                             (entity.StartDate >= c.StartDate && entity.StartDate <= c.FinishDate ||
                                                              entity.FinishDate >= c.StartDate && entity.FinishDate <= c.FinishDate), new string[] { "StudentsInCourse" });

                var courseWithStudentOverlapping = courseBeginInTheSameRange.SelectMany(s => s.StudentsInCourse);
                var studentsWithCoursesOverlapping = courseWithStudentOverlapping.Where(s => studentIds.Contains(s.StudentId)).Select(s => s.StudentId).Distinct();

                if (studentsWithCoursesOverlapping.Any())
                {
                    var studentBuzz = new StudentBusiness();
                    var studentsNames = string.Join(",", studentBuzz.GetList(s => studentsWithCoursesOverlapping.Contains(s.StudentId)).Select(x => $"{x.LastName} {x.FirstName}").ToArray());

                    throw new ValidationException("ValidationStudentHasCourseOverlapping", studentsNames );

                }

            }


        }

        public List<Course> GetActiveCourses()
        {
            return Repository.GetActiveCourses();
        }
    }
}
