using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bangazon_Workforce_Management.Models
{
    public class Department
    {
        [Display(Name = "Department Id")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Department")]
        public string Name { get; set; }

        [Required]
        public int Budget { get; set; }

        public List<Employee> Employees { get; set; } = new List<Employee>();

    }
}
