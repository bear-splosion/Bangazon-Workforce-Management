using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bangazon_Workforce_Management.Models.ViewModels
{
    public class TrainingProgramEditViewModel
    {
        public TrainingProgram trainingProgram { get; set; }

        public TrainingProgramEditViewModel() { }

        public TrainingProgramEditViewModel((TrainingProgram trainingProgram)
        {
            TrainingProgram = trainingProgram;
        }

        public List<SelectListItem> TrainingPrograms { get; set; }
        public TrainingProgramEditViewModel((TrainingProgram trainingProgram, List<TrainingProgram> trainingProgramList)
        {
            //TrainingProgram = trainingProgram;
            TrainingPrograms = trainingProgramList
                .Select(trainingProgram => new SelectListItem
                {
                    Text = trainingProgram.Name,
                    Value = trainingProgram.Id.ToString()
                })
                .ToList();

            TrainingPrograms.Insert(0, new SelectListItem
            {
                Text = "Choose training program...",
                Value = "0"
            });
        }
    }
}