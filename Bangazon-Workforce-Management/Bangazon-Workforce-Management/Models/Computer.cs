using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bangazon_Workforce_Management.Models
{
    public class Computer
    {
        public int Id { get; set; }

        public string Make { get; set; }

        public string Manufacturer { get; set; }

        [Required]
        [Display(Name = "Purchased")]
        public DateTime PurchaseDate { get; set; }

        [Display(Name = "Decommissioned")]
        public DateTime? DecomissionDate { get; set; }
    }
}
