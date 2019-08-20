using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bangazon_Workforce_Management.Models
{
    public class TrainingProgram
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }
        [Required]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }
        [Required]
        [Display(Name = "Max Attendees")]
        public int MaxAttendees { get; set; }

        public List<Employee> AttendingEmployees { get; set; } = new List<Employee>();
    }
}
