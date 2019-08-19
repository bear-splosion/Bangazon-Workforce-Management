using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Bangazon_Workforce_Management.Models;
using Bangazon_Workforce_Management.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Bangazon_Workforce_Management.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly IConfiguration _config;

        public DepartmentController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        // GET: Department
        public ActionResult Index()
        {
            var departments = new List<Department>();
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT d.Id, d.[Name], d.Budget, e.Id, e.FirstName, e.LastName, e.DepartmentId, e.IsSupervisor FROM Department d LEFT JOIN Employee e ON d.Id = e.DepartmentId";

                    SqlDataReader reader = cmd.ExecuteReader();

                    while(reader.Read())
                    {
                        Department department = new Department
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Budget = reader.GetInt32(reader.GetOrdinal("Budget"))
                        };

                        Employee employee = new Employee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                IsSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSupervisor"))
                            };

                        if (departments.Any(d => d.Id == department.Id))
                        {
                            Department ExistingDepo = departments.Find(d => d.Id == department.Id);
                            ExistingDepo.Employees.Add(employee);
                        }
                        else
                        {
                            department.Employees.Add(employee);
                            departments.Add(department);
                        }
                    }
                    reader.Close();
                }
            }
            return View(departments);
        }

        // GET: Department/Details/5
        
        public ActionResult Details(int id)
        {
            Department department = null;
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT d.Id, d.[Name], d.Budget, e.Id, e.FirstName, e.LastName, e.DepartmentId, e.IsSupervisor FROM Department d LEFT JOIN Employee e ON d.Id = e.DepartmentId WHERE d.Id = @id
                        ";

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        if (department == null)
                        {
                            department = new Department()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Budget = reader.GetInt32(reader.GetOrdinal("Budget"))
                            };
                        }
                        department.Employees.Add(
                            new Employee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                IsSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSupervisor"))
                            }
                        );
                    }
                }
            }
            return View(department);
        }
        // GET: Department/Create
        [HttpGet]
        public ActionResult Create()
        {
            var ViewModel = new DepartmentCreateViewModel(_config.GetConnectionString("DefaultConnection"));
            return View(ViewModel);
        }

        // POST: Department/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Department department)
        {
            try
            {

                //now, write it to the DB
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                       INSERT INTO Department (
                          Name,
                          Budget
                      ) VALUES (
                           @name,
                           @budget
                       )
                      ";
                        cmd.Parameters.AddWithValue("@name", department.Name);
                        cmd.Parameters.AddWithValue("@budget", department.Budget);

                        //now, Execute command
                        cmd.ExecuteNonQuery();
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}