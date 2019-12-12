using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using IronSchool.Entities;

namespace IronSchool.Repositories
{
    public partial class CourseRepository
    {
        public override void Update(Course entity)
        {
            using (DBEntities connection = new DBEntities())
            {
                var entityDb = connection.Course.Include("StudentsInCourse")
                                         .FirstOrDefault(c => c.CourseId == entity.CourseId);

                UpdateChildren(entity, connection, entityDb);

                connection.Entry(entityDb).CurrentValues.SetValues(entity);
                connection.SaveChanges();
            }
        }

        private void UpdateChildren(Course entity, DBEntities connection, Course dbEnt)
        {
            ICollection<StudentsInCourse> newCollection = entity.StudentsInCourse;
            ICollection<StudentsInCourse> dbCollection = dbEnt.StudentsInCourse;

            List<string> newIDs = newCollection.Select(c => $"{c.CourseId}-{c.StudentId}").ToList();
            List<string> oldIDs = dbCollection.Select(c => $"{c.CourseId}-{c.StudentId}").ToList();

            foreach (var id in oldIDs)
            {
                //Remove student deleted
                if (!newIDs.Contains(id))
                {
                    StudentsInCourse oldEntity = null;
                    foreach (var e in dbCollection)
                    {
                        if ($"{e.CourseId}-{e.StudentId}" == id)
                        {
                            oldEntity = e;
                            break;
                        }
                    }
                    dbCollection.Remove(oldEntity);
                }
            }

            foreach (var id in newIDs)
            {
                //Add student added
                if (!oldIDs.Contains(id))
                {
                    StudentsInCourse newEntity = newCollection.FirstOrDefault(c =>c.CourseId.ToString() + "-" + c.StudentId.ToString() == id);
                    newEntity.Course = null;
                    if (newEntity != null)
                        dbCollection.Add(newEntity);
                }
            }
        }


        public List<Course> GetActiveCourses()
        {
            var now = DateTime.Now.Date;
            var activeCourses = GetList(c => now >= DbFunctions.TruncateTime(c.StartDate) && DbFunctions.TruncateTime(c.FinishDate) >= now, new string[] { "StudentsInCourse.Student", "Instructor" }).ToList();
            return activeCourses;
        }

    }
}
