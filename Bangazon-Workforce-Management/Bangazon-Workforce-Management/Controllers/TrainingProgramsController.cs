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
    public class TrainingProgramsController : Controller
    {
        private readonly IConfiguration _config;

        public TrainingProgramsController(IConfiguration config)
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
        // GET: TRAINING PROGRAMS
        public ActionResult Index()
        {
            var trainingPrograms = new List<TrainingProgram>();
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, Name, StartDate, EndDate, MaxAttendees
                        FROM TrainingProgram
                        WHERE StartDate > GetDate();

                    ";

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        trainingPrograms.Add(new TrainingProgram()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
                        });
                    }
                    reader.Close();
                }
            }

            return View(trainingPrograms);
        }


        // GET: TrainingPrograms/Details/5
        public ActionResult Details(int id)
        {
            TrainingProgram trainingProgram = null;
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, Name, StartDate, EndDate, MaxAttendees
                        FROM TrainingProgram
                        WHERE Id = @id
                    ";

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        trainingProgram = new TrainingProgram()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
                        };
                    }
                }
            }

            return View(trainingProgram);
        }

        // GET: TrainingPrograms/Create
        [HttpGet]
        public ActionResult Create()
        {
            var viewModel = new TrainingProgramCreateViewModel(_config.GetConnectionString("DefaultConnection"));
            return View(viewModel);
        }

        // POST: TrainingPrograms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TrainingProgram trainingProgram)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            INSERT INTO TrainingProgram (
                                Name, 
                                StartDate, 
                                EndDate,
                                MaxAttendees
                            ) VALUES (
                                @Name,
                                @StartDate,
                                @EndDate,
                                @MaxAttendees
                            )
                        ";

                        cmd.Parameters.AddWithValue("@Name", trainingProgram.Name);
                        cmd.Parameters.AddWithValue("@StartDate", trainingProgram.StartDate);
                        cmd.Parameters.AddWithValue("@EndDate", trainingProgram.EndDate);
                        cmd.Parameters.AddWithValue("@MaxAttendees", trainingProgram.MaxAttendees);

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
        // GET: TrainingPrograms/Edit/5
        public ActionResult Edit(int id)
        {
            var trainingProgram = GetSingleTrainingProgram(id);
            DateTime currentDate = DateTime.Now;

            if (trainingProgram.StartDate > currentDate)
            {
                return View(trainingProgram);
            }
            else
            {
                throw new Exception("An Error Occurred");
            }
        }


        // POST: TrainingPrograms/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
         public ActionResult Edit(int id, TrainingProgram model)
        {
            try
            {
                // TODO: Add update logic here
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE TrainingProgram
                                            SET
                                                Name = @name,
                                                StartDate = @startDate,
                                                EndDate = @endDate,
                                                MaxAttendees = @maxAttendees
                                            WHERE Id = @id";
                        cmd.Parameters.AddWithValue("@name", model.Name);
                        cmd.Parameters.AddWithValue("@startDate", model.StartDate);
                        cmd.Parameters.AddWithValue("@endDate", model.EndDate);
                        cmd.Parameters.AddWithValue("@maxAttendees", model.MaxAttendees);
                        cmd.Parameters.AddWithValue("@id", id);

                        cmd.ExecuteNonQuery();

                        return RedirectToAction(nameof(Index));

                    }
                }
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Delete(int id)
        {
            //use GetSingleInstructor to get the Instructor you want to delete
            TrainingProgram program = GetSingleTrainingProgram(id);
            //pass that instructor into View()
            return View(program);
        }

        // POST: Instructors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteTrainingProgram(int id)
        {
            try
            {
                // TODO: Add delete logic here
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                                            DELETE FROM EmployeeTraining
                                            WHERE TrainingProgramId = @id;
                                            DELETE FROM TrainingProgram
                                            WHERE Id = @id;
                                            ";
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

        private TrainingProgram GetSingleTrainingProgram(int id)
        {
            using (SqlConnection conn = Connection)
            {
                TrainingProgram program = null;
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, [Name], StartDate, EndDate, MaxAttendees
                        FROM TrainingProgram
                        WHERE Id = @id
                    ";

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        program = new TrainingProgram()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
                        };
                    }
                }
                return program;
            }
        }
        private List<Employee> GetAllProgramEmployees()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, FirstName, LastName FROM Employee";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Employee> employees = new List<Employee>();
                    while (reader.Read())
                    {
                        employees.Add(new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName"))
                        });
                    }

                    reader.Close();

                    return employees;
                }
            }
        }

        // GET: PAST TRAINING PROGRAMS
        public ActionResult GetPastDates()
        {
            var trainingPrograms = new List<TrainingProgram>();
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT tp.Id, tp.Name, tp.StartDate, tp.EndDate, tp.MaxAttendees,e.Id AS EmployeeId, e.FirstName, e.LastName
                        FROM TrainingProgram tp
                        LEFT JOIN Employee e
                        ON tp.Id = e.Id
                        WHERE StartDate < GetDate();

                    ";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Employee> employees = new List<Employee>();

                    while (reader.Read())
                    {
                        TrainingProgram trainingProgram = new TrainingProgram()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
                        };

                        if(!reader.IsDBNull(reader.GetOrdinal("EmployeeId")))
                        {
                            Employee employee = new Employee()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName"))
                            };
                            employees.Add(employee);
                        }
                        trainingProgram.AttendingEmployees = employees;
                        trainingPrograms.Add(trainingProgram);
                    }

                    reader.Close();
                }
            }

            return View(trainingPrograms);
        }
    }
}