using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bangazon_Workforce_Management.Models
{
    public class Department
    {
        [Display(Name = "Department Id")]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int Budget { get; set; }

        public int Employees { get; set; }

    }
}
