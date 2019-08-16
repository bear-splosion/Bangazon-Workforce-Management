using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Bangazon_Workforce_Management.Models.ViewModels
{
    public class DepartmentCreateViewModel
    {
        public Department Department { get; set; }
    }
}