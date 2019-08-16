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
                    cmd.CommandText = @"SELECT d.Id, d.[Name], d.Budget, COUNT(e.Id) AS Employees 
                                            FROM Department d JOIN Employee e ON d.Id = e.DepartmentId 
                                                GROUP BY d.Id, d.[Name], d.Budget, e.DepartmentId";

                    SqlDataReader reader = cmd.ExecuteReader();

                    while(reader.Read())
                    {
                        departments.Add(new Department()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Budget = reader.GetInt32(reader.GetOrdinal("Budget")),
                            Employees = reader.GetInt32(reader.GetOrdinal("Employees"))
                        });

                        
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
                        SELECT d.Id, d.[Name], d.Budget, COUNT(e.Id) AS Employees 
                            FROM Department d JOIN Employee e ON d.Id = e.DepartmentId 
                            WHERE d.Id = @id
                            GROUP BY d.Id, d.[Name], d.Budget, e.DepartmentId
                        ";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        department = new Department()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Budget = reader.GetInt32(reader.GetOrdinal("Budget")),
                            Employees = reader.GetInt32(reader.GetOrdinal("Employees"))
                        };
                    }
                }
            }
            return View(department);
        }
        // GET: Department/Create
        public ActionResult Create()
        {
            var ViewModel = new DepartmentCreateViewModel
                (_config.GetConnectionString("DefaultConnection"));
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
                          Budget,
                      ) VALUES (
                           @name,
                           @budget
                       )
                      ";
                        cmd.Parameters.AddWithValue("@Name", department.Name);
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

        // GET: Department/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Department/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Department/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Department/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}