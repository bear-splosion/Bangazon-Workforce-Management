using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bangazon_Workforce_Management.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        [Display(Name="First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Department Id")]
        public int DepartmentId { get; set; }

        [Required]
        [Display(Name = "Supervisor?")]
        public bool IsSupervisor { get; set; }

        [Display(Name = "Full Name")]
        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }
    }
}
