using IronSchool.Business;
using IronSchool.Entities;
using IronSchool.Entities.Dto;
using IronSchool.WebSite.Controllers.Base;
using IronSchool.WebSite.Filters;
using IronSchool.WebSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IronSchool.WebSite.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            var userBuzz = new UserBusiness();

            if (userBuzz.IsAdministrator())
                return RedirectToAction("IndexAdmin");

            var instructorBuzz = new InstructorBusiness();
            List<TopStudentDto> topTenStudents = instructorBuzz.GetTopTenBestStudentsInActiveCourses();

            HomeInstructorVM model = new HomeInstructorVM()
            {
                CantidadAlumnosEnCursosActivos = instructorBuzz.GetStudentsInActiveCourse().Count,
                CantidadCursosDados = instructorBuzz.GetHistoricoDeCursosImpartidos().Count
            };

            topTenStudents.ForEach(s => model.EstudiantesTop.Add(new EstudianteTopVM() { Estudiante = s.Name, Promedio = s.Average  }));
            model.EstudiantesTop = model.EstudiantesTop.OrderByDescending(e => e.Promedio).ToList();

            return View(model);
        }
        
        [AuthorizeRule("Index", true)]
        public  ActionResult IndexAdmin()
        {
            //Check for manual url access
            var userBuzz = new UserBusiness();
            if (!userBuzz.IsAdministrator())
                return RedirectToAction("Login", "Account");

            StudentBusiness studentBuzz = new StudentBusiness();
            CourseBusiness courseBuzz = new CourseBusiness();
            InstructorBusiness instructorBuzz = new InstructorBusiness();

            HomeAdministratorVM adminVM = new HomeAdministratorVM()
            {
                CantidadAlumnos = studentBuzz.GetActiveStudents().Count,
                CantidadCursos = courseBuzz.GetActiveCourses().Count,
                CantidadProfesores = instructorBuzz.GetActiveInstructors().Count,
                EstudiantesTop = studentBuzz.GetTopTenStudent().Select(s => new EstudianteTopVM() { Estudiante = s.Name, Promedio = s.Average } ).OrderByDescending(e => e.Promedio).ToList(),
                CantidadAlumnosAprobados = studentBuzz.GetStudentApproved().Count,
                CantidadAlumnosDesaprobados = studentBuzz.GetStudentDispproved().Count
            };

            return View(adminVM);
        }

    }
}