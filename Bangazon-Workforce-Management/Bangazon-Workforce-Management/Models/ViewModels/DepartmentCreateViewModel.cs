using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Bangazon_Workforce_Management.Models.ViewModels
{
    public class DepartmentCreateViewModel
{
        public List<SelectListItem> Cohorts { get; set; }
        public Department Department { get; set; }

        private readonly string _connectionString;

        public List<SelectListItem> Departments { get; }

        private SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_connectionString);
            }
        }

        //constructor
        public DepartmentCreateViewModel() { }

        public DepartmentCreateViewModel(string connectionString)
        {
            _connectionString = connectionString;

            Departments = GetAllDepartments()
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

        private List<Department> GetAllDepartments()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, Name, Budget FROM Department";
                    SqlDataReader reader = cmd.ExecuteReader();

                    //below, use while for mult depts/whatever and if in place of while if you only want one thing
                    List<Department> departments = new List<Department>();
                    while (reader.Read())
                    {
                        departments.Add(new Department
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Budget = reader.GetInt32(reader.GetOrdinal("Budget")),
                        });
                    }

                    reader.Close();

                    return departments;
                }
            }
        }
    }
}
