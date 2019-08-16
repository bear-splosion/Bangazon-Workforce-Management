using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bangazon_Workforce_Management.Models.ViewModels
{
    public class EmployeeDisplayViewModel
    {
        public Employee Employee { get; set; }
        public Department Department { get; set; }
        public Computer Computer { get; set; }
        public TrainingProgram TrainingProgram { get; set; }
        public List<TrainingProgram> TrainingPrograms { get; set; }
    }
}