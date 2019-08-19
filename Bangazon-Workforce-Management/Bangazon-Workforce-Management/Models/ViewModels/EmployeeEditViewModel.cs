using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bangazon_Workforce_Management.Models.ViewModels
{
    public class EmployeeEditViewModel
    {
        public Employee Employee { get; set; }

        public EmployeeEditViewModel() { }

        public EmployeeEditViewModel(Employee employee)
        {
            Employee = employee;
        }

        public List<SelectListItem> Departments { get; set; }
        public EmployeeEditViewModel(Employee employee, List<Department> departmentList)
        {
            Employee = employee;
            Departments = departmentList
                .Select(department => new SelectListItem
                {
                    Text = department.Name,
                    Value = department.Id.ToString()
                })
                .ToList();

            Departments.Insert(0, new SelectListItem
            {
                Text = "Choose department...",
                Value = "0"
            });
        }
    }
}