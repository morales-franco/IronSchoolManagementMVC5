using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronSchool.Entities.Base
{
    public interface IAuditableEntity
    {
        DateTime AuditDate { get; set; }/*
        string AudTerminal { get; set; }
        string AudUser { get; set; }*/
    }
}
