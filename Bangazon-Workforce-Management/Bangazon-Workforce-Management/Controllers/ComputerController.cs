using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
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
                    cmd.CommandText = @" SELECT Id, PurchaseDate,DecomissionDate
                    ,Make
                    ,Manufacturer 
                        FROM Computer";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while(reader.Read())
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
        public ActionResult Create()
        {

            return View();

        }
        // POST: Computer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Computer computer)
        {
            try
            {
                using(SqlConnection conn = Connection)
                {
                    conn.Open();
                    using(SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                        INSERT INTO Computer(PurchaseDate
                     ,DecomissionDate
                        ,Make
                        ,Manufacturer) VALUES (@purchaseDate, @decomissionDate, @make, @manufacturer')
                            ";
                        cmd.Parameters.AddWithValue("purchaseDate", computer.PurchaseDate);
                        cmd.Parameters.AddWithValue("decomissionDate", computer.DecomissionDate);
                        cmd.Parameters.AddWithValue("make", computer.PurchaseDate);
                        cmd.Parameters.AddWithValue("make", computer.Make);
                        cmd.Parameters.AddWithValue("manufacturer", computer.Manufacturer);
                        cmd.ExecuteNonQuery();
                    }
                }
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
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
        {
            return View();
        }

        // POST: Computer/Delete/5
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
        // GET: Computers By Search
        public ActionResult Search(IFormCollection input)
        {
            if (input == null)
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
                                        WHERE Id LIKE '%' + @input + '%'
                                        OR Make LIKE '%' + @input + '%'
                                        OR Manufacturer LIKE '%' + @input + '%'";

                    cmd.Parameters.AddWithValue("@input", input);

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
        public Computer GetSearchResults(int id)
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
            return computer;
        }
    }
}