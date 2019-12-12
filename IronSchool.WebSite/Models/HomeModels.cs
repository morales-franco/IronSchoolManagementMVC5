using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IronSchool.WebSite.Models
{
    public class HomeInstructorVM
    {
        public int CantidadCursosDados { get; set; }
        public int CantidadAlumnosEnCursosActivos { get; set; }
        public List<EstudianteTopVM> EstudiantesTop { get; set; }

        public HomeInstructorVM()
        {
            EstudiantesTop = new List<EstudianteTopVM>();
        }
    }

    public class EstudianteTopVM
    {
        public string Estudiante { get; set; }
        public decimal Promedio { get; set; }
    }

    public class HomeAdministratorVM
    {
        public int CantidadProfesores { get; set; }
        public int CantidadCursos { get; set; }
        public int CantidadAlumnos { get; set; }
        public int CantidadAlumnosAprobados { get; set; }
        public int CantidadAlumnosDesaprobados { get; set; }
        public List<EstudianteTopVM> EstudiantesTop { get; set; }
        public HomeAdministratorVM()
        {
            EstudiantesTop = new List<EstudianteTopVM>();
        }
    }
    

}