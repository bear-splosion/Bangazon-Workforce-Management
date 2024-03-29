﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Bangazon_Workforce_Management.Models.ViewModels;
using Bangazon_Workforce_Management.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;


namespace Bangazon_Workforce_Management.Controllers
{
    public class ComputerController : Controller
    {

        private readonly IConfiguration _config;

        public ComputerController(IConfiguration config)
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
        // GET: Computer
        public ActionResult Index()
        {
            var computers = new List<Computer>();
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using(SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @" SELECT c.Id, c.Make, c.Manufacturer, c.PurchaseDate, c.DecomissionDate, e.Id AS EmployeeId
                FROM Computer c 
                LEFT JOIN ComputerEmployee ce ON ce.ComputerId = c.Id
                LEFT JOIN Employee e ON ce.EmployeeId = e.Id ";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Computer computer = new Computer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                           

                        };


                        if (!reader.IsDBNull(reader.GetOrdinal("DecomissionDate")))
                        {
                            computer.DecomissionDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate"));
                        } else if (!reader.IsDBNull(reader.GetOrdinal("EmployeeId")))
                        {
                            computer.EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId"));
                        };

                    
                        computers.Add(computer);
                    }
                    reader.Close();
                }
            }
                return View(computers);
        }

        // GET: Computer/Details/5
        public ActionResult Details(int id)
        {
            Computer computer = null;
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                        SELECT Id, PurchaseDate, DecomissionDate, Make, Manufacturer
                        FROM Computer
                        WHERE Id = @id
                    ";
                        cmd.Parameters.AddWithValue("@id", id);

                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            computer = new Computer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                               

                            };

                            if (!reader.IsDBNull(reader.GetOrdinal("DecomissionDate")))
                            {
                                computer.DecomissionDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate"));
                            }
                        }
                        reader.Close();
                    }
                }
            }
            return View(computer);
        }

        // GET: Computer/Create
        [HttpGet]
        public ActionResult Create()
        {
           
            var viewModel = new ComputerCreateViewModel(_config.GetConnectionString("DefaultConnection"));
            return View(viewModel);

        }
        // POST: Computer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Computer computer)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                        INSERT INTO Computer (
                            Make,
                            Manufacturer,
                             PurchaseDate)
                            OUTPUT INSERTED.Id
                            
                        VALUES (
                             @make, 
                             @manufacturer,
                             @purchaseDate 
                            )
                         ";
                        
                        cmd.Parameters.AddWithValue("@make", computer.Make);
                        cmd.Parameters.AddWithValue("@manufacturer", computer.Manufacturer);
                        cmd.Parameters.AddWithValue("@purchaseDate", computer.PurchaseDate);
                      
                        int newId = (int)cmd.ExecuteScalar();
                        computer.Id = newId;
                       
                        cmd.CommandText = @" INSERT INTO ComputerEmployee
                                            ( ComputerId, EmployeeId, AssignDate )
                                                VALUES( @newId, @employeeId, CURRENT_TIMESTAMP)";
                        cmd.Parameters.AddWithValue("@newId", computer.Id);
                        cmd.Parameters.AddWithValue("@employeeId", computer.EmployeeId);
                       
                        cmd.ExecuteNonQuery();

                    }
                }
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                return View();
            }
        }

        // GET: Computer/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Computer/Edit/5
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

        // GET: Computer/Delete/5
        public ActionResult Delete(int id)
        {  //use GetSingleComputer to get the computer you want to delete
            Computer computer = GetSingleComputer(id);
            //pass that computerinto View()
            return View(computer);
        }

        // POST: Computer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteComputer(int id)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE FROM ComputerEmployee WHERE ComputerId = @id
                                                 DELETE FROM Computer WHERE Id = @id";
                        cmd.Parameters.AddWithValue("@id", id);
                     

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

        private Computer GetSingleComputer(int id)
        {
            using (SqlConnection conn = Connection)
            {
              Computer computer = null;
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, Make, Manufacturer, PurchaseDate
                        FROM Computer
                        WHERE Id = @id
                    ";

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        computer = new Computer()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate"))

                        };
                    }
                }
                return computer;
            }
        }
        private List<Employee> GetAllEmployees()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, FirstName FROM Employee";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Employee> employees = new List<Employee>();
                    while (reader.Read())
                    {
                        employees.Add(new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                        });
                    }

                    reader.Close();

                    return employees;
                }
            }
        }
        public ActionResult SearchComputers(IFormCollection search)
        {
            if (string.IsNullOrEmpty(search["SearchString"][0]))
            {
                return RedirectToAction(nameof(Index));
            }

            List<Computer> computers = new List<Computer>();
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, PurchaseDate, DecomissionDate, Make, Manufacturer 
                                        FROM Computer
                                        WHERE Make LIKE '%' + @search + '%'
                                        OR Manufacturer LIKE '%' + @search + '%'";

                    cmd.Parameters.AddWithValue("@search", search["SearchString"][0]);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Computer computer = new Computer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                        };

                        if (!reader.IsDBNull(reader.GetOrdinal("DecomissionDate")))
                        {
                            computer.DecomissionDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate"));
                        }

                        computers.Add(computer);
                    }
                    reader.Close();
                }
            }
            return View(computers);
        }

        public Computer GetComputerById(int id)
        {
            Computer computer = null;
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, PurchaseDate, DecomissionDate, Make, Manufacturer 
                        FROM Computer
                        WHERE Id = @id
                    ";
                    cmd.Parameters.AddWithValue("@id", id);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        computer = new Computer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                        };

                        if (!reader.IsDBNull(reader.GetOrdinal("DecomissionDate")))
                        {
                            computer.DecomissionDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate"));
                        }
                    }
                    reader.Close();
                }
            }
            return computer;
        }

    }
}