using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronSchool.Business.Base;
using IronSchool.Entities;

namespace IronSchool.Business
{
    public partial class StudentsInCourseBusiness
    {
        public override StudentsInCourse Read(object id)
        {
            var courseId = Convert.ToInt64(((string)id).Split('-')[0]);
            var studentId = Convert.ToInt64(((string)id).Split('-')[1]);
            return Get(c => c.CourseId == courseId && c.StudentId == studentId, new string[] { "Student", "Course" });
        }

        public override void Update(StudentsInCourse entity)
        {
            CheckEntityABM(entity);
            base.Update(entity);
        }

        private void CheckEntityABM(StudentsInCourse entity)
        {
            if (entity.ExamQualification.HasValue && entity.ExamQualification.Value <= 0)
                throw new ValidationException("ValidationQualificationMustBeGreaterThanZero");
        }
    }
}
