using System;
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
    public class EmployeesController : Controller
    {
        private readonly IConfiguration _config;

        public EmployeesController(IConfiguration config)
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
        // GET: Employees
        public ActionResult Index()
        {
            var employees = new List<Employee>();

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {

                    cmd.CommandText = @"
                        SELECT e.Id, e.FirstName, e.LastName, e.DepartmentId, e.IsSupervisor, d.Name AS DepartmentName, d.Id As DeptId, d.Budget
                        FROM Employee e
                        LEFT JOIN Department d ON d.Id = e.DepartmentId
                        ";

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        employees.Add(new Employee()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                            IsSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSupervisor")),
                            Department = new Department()
                            {
                                Name = reader.GetString(reader.GetOrdinal("DepartmentName")),
                                Id = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                Budget = reader.GetInt32(reader.GetOrdinal("Budget"))
                            }
                        });
                    }
                    reader.Close();
                }
            }
            return View(employees);
        }


        // GET: Employees/Details/5
        public ActionResult Details(int id)
        {
            Employee employee = null;
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    SELECT e.Id, e.FirstName, e.LastName, e.DepartmentId, e.IsSupervisor, ce.ComputerId, c.Make, c.Manufacturer, c.Id, d.Name AS DepartmentName, et.TrainingProgramId, et.EmployeeId, t.Name
                    FROM Employee e
                    LEFT JOIN ComputerEmployee ce ON ce.EmployeeId = e.Id
                    LEFT JOIN Computer c ON c.Id = ce.ComputerId
                    LEFT JOIN Department d ON d.Id = e.DepartmentId
                    LEFT JOIN EmployeeTraining et ON et.EmployeeId = e.Id
                    LEFT JOIN TrainingProgram t ON t.Id = et.TrainingProgramId
                    WHERE e.Id = @id;
                    ";

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<TrainingProgram> programs = new List<TrainingProgram>();

                    while (reader.Read())
                    {

                        employee = new Employee()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                            IsSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSupervisor")),
                            Department = new Department()
                            {
                                Name = reader.GetString(reader.GetOrdinal("DepartmentName")),
                                Id = reader.GetInt32(reader.GetOrdinal("DepartmentId"))
                            }
                        };
                        if (!reader.IsDBNull(reader.GetOrdinal("ComputerId")))
                        {
                            employee.Computer = new Computer()
                            {
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                                Id = reader.GetInt32(reader.GetOrdinal("ComputerId"))
                            };
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("TrainingProgramId")))
                        {
                            TrainingProgram program = new TrainingProgram()
                            {
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Id = reader.GetInt32(reader.GetOrdinal("TrainingProgramId"))
                            };
                            programs.Add(program);
                        }
                    }
                    employee.TrainingPrograms = programs;

                    reader.Close();
                }
            }
            return View(employee);
        }

        // GET: Employees/Create
        [HttpGet]
        public ActionResult Create()
        {
            var viewModel = new EmployeeCreateViewModel(_config.GetConnectionString("DefaultConnection"));
            return View(viewModel);
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Employee employee)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            INSERT INTO Employee (
                                FirstName, 
                                LastName, 
                                DepartmentId,
                                IsSupervisor
                            ) VALUES (
                                @firstName,
                                @lastName,
                                @departmentId,
                                @isSupervisor
                            )
                        ";

                        cmd.Parameters.AddWithValue("@firstName", employee.FirstName);
                        cmd.Parameters.AddWithValue("@lastName", employee.LastName);
                        cmd.Parameters.AddWithValue("@departmentId", employee.DepartmentId);
                        cmd.Parameters.AddWithValue("@isSupervisor", employee.IsSupervisor);

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

        // GET: Employees/Edit/5
        public ActionResult Edit(int id)
        {
            Employee employee = GetSingleEmployee(id);
            List<Department> departments = GetAllDepartments();
            var viewModel = new EmployeeEditViewModel(employee, departments);
            return View(viewModel);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, EmployeeEditViewModel model)
        {
            try
            {
                // TODO: Add update logic here
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Employee
                                            SET
                                                FirstName = @firstName,
                                                LastName = @lastName,
                                                DepartmentId = @departmentId,
                                                IsSupervisor = @isSupervisor
                                            WHERE Id = @id";
                        cmd.Parameters.AddWithValue("@firstName", model.Employee.FirstName);
                        cmd.Parameters.AddWithValue("@lastName", model.Employee.LastName);
                        cmd.Parameters.AddWithValue("@departmentId", model.Employee.DepartmentId);
                        cmd.Parameters.AddWithValue("@isSupervisor", model.Employee.IsSupervisor);
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

        // GET: Employees/Delete/5
        public ActionResult Delete(int id)
        {
            //use GetSingleEmployee to get the Employee you want to delete
            Employee employee = GetSingleEmployee(id);
            //pass that employee into View()
            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteEmployee(int id)
        {
            try
            {
                // TODO: Add delete logic here
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE FROM Employee
                                                WHERE Id = @id";
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

        // GET: Employees/Assign/5

        public ActionResult Assign(int id)
        {
            Employee employee = GetSingleEmployee(id);
            List<TrainingProgram> trainingPrograms = GetAllTrainingPrograms();
            var viewModel = new EmployeeTrainingProgramViewModel(employee, trainingPrograms);
            return View(viewModel);
        }

        // POST: Employees/Assign/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Assign(int id, EmployeeTrainingProgramViewModel model)
        {
            try
            {
                // TODO: Add update logic here
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO EmployeeTraining 
                        (EmployeeId, TrainingProgramId)
                        VALUES (@employeeId, @trainingProgramId)";
                        cmd.Parameters.AddWithValue("@employeeId", id);
                        cmd.Parameters.AddWithValue("@trainingProgramId", id);

                    }
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        private Employee GetSingleEmployee(int id)
        {
            using (SqlConnection conn = Connection)
            {
                Employee employee = null;
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, FirstName, LastName, DepartmentId, IsSupervisor
                        FROM Employee
                        WHERE Id = @id
                    ";

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        employee = new Employee()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                            IsSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSupervisor"))
                        };
                    }
                }
                return employee;
            }
        }

        private List<Department> GetAllDepartments()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, Name FROM Department";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Department> departments = new List<Department>();
                    while (reader.Read())
                    {
                        departments.Add(new Department
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                        });
                    }

                    reader.Close();

                    return departments;
                }
            }
        }

        private List<TrainingProgram> GetAllTrainingPrograms()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, Name FROM TrainingProgram
WHERE StartDate > GetDate()";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<TrainingProgram> trainingPrograms = new List<TrainingProgram>();
                    while (reader.Read())
                    {
                        trainingPrograms.Add(new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                        });
                    }

                    reader.Close();

                    return trainingPrograms;
                }
            }
        }
    }
}
