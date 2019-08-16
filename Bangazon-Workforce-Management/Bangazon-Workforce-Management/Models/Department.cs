using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BangazonAPI.Models
{
    public class Department
    {
        [Display(Name = "Cohort Id")]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int Budget { get; set; }

    }
}
