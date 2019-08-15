using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BangazonAPI.Models
{
    public class Computer
    {
        public int Id { get; set; }

        public string Make { get; set; }

        public string Manufacturer { get; set; }

        [Required]
        public DateTime PurchaseDate { get; set; }

        public DateTime? DecomissionDate { get; set; }
    }
}
