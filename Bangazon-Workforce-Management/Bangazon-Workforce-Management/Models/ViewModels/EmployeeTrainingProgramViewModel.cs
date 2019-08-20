using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bangazon_Workforce_Management.Models.ViewModels
{
    public class EmployeeTrainingProgramViewModel
    {
        public EmployeeTrainingProgramViewModel() { }

        public int TrainingProgramId { get; set; }

        public List<SelectListItem> TrainingPrograms { get; set; }

        public TrainingProgram TrainingProgram { get; set; }

        public Employee Employee { get; set; }

        public Department Department { get; set; }

        public EmployeeTrainingProgramViewModel(Employee employee, List<TrainingProgram> trainingProgramList)
        {
            Employee = employee;
            TrainingPrograms = trainingProgramList
                .Select(trainingProgram => new SelectListItem
                {
                    Text = trainingProgram.Name,
                    Value = trainingProgram.Id.ToString()
                })
                .ToList();

            TrainingPrograms.Insert(0, new SelectListItem
            {
                Text = "Choose a training program...",
                Value = "0"
            });
        }
    }
}



