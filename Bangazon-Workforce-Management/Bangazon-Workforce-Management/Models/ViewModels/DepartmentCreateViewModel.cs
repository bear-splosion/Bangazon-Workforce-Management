using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bangazon_Workforce_Management.Models.ViewModels
{
    public class DepartmentCreateViewModel
{
        public List<SelectListItem> Cohorts { get; set; }
        public Department Department { get; set; }

        private readonly string _connectionString;

        private SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_connectionString);
            }
        }

        //constructor
        public StudentCreateViewModel() { }

        public StudentCreateViewModel(string connectionString)
        {
            _connectionString = connectionString;

            Cohorts = GetAllCohorts()
                .Select(cohort => new SelectListItem
                {
                    Text = cohort.Name,
                    Value = cohort.Id.ToString()
                })
                .ToList();
            Cohorts.Insert(0, new SelectListItem
            {
                Text = "Choose cohort...",
                Value = "0"
            });
        }

        private List<Cohort> GetAllCohorts()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, Name FROM Cohort";
                    SqlDataReader reader = cmd.ExecuteReader();

                    //below, use while for mult cohorts/whatever and if in place of while if you only want one thing
                    List<Cohort> cohorts = new List<Cohort>();
                    while (reader.Read())
                    {
                        cohorts.Add(new Cohort
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                        });
                    }

                    reader.Close();

                    return cohorts;
                }
            }
        }
    }
}
