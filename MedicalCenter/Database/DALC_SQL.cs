using MedicalCenter.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Net;
using System.Numerics;

namespace MedicalCenter.Database
{
    public class DALC_SQL
    {

        public string _CONN_STR = "";
        private IConfiguration _configuration;

        public DALC_SQL(IConfiguration _configuration)
        {
            this._configuration = _configuration;
            _CONN_STR = this._configuration["_CONN_STR:conn"];
        }

        public Doctor GetDoctorByID(int id)
        {
            Doctor doctor = null;
            using (SqlConnection connection = new SqlConnection(_CONN_STR))
            {
                connection.Open();
                string sql = $"EXECUTE  [dbo].[GET_DOCTOR_BY_ID] '{id}';";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            doctor = new()
                            {
                                Id = (int)reader.GetInt64(0),
                                Name = reader.GetString(1),
                                Specialization = reader.GetString(2),
                                Sub_specialty = reader.GetString(3),
                                Address = reader.GetString(4),
                                Hours = new()
                                {
                                    DayOfWeek = reader.GetString(5).Split(',').ToList(), //convert the array to list
                                    time = reader.GetString(6).Split(',').ToList()
                                },
                                Hospital = reader.GetString(7)
                            };
                        }
                    }
                }
            }
            return doctor;
        }

        public PatientLogin GetPatientPhoneNAndPass(string phonenumber)
        {
            PatientLogin p = null;
            using (SqlConnection connection = new SqlConnection(_CONN_STR))
            {
                connection.Open();
                string sql = $"EXECUTE  [dbo].[GET_PATIENT_PHN_PASS] '{phonenumber}';";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            p = new(
                                    phonenumber: reader.GetString(0),
                                    password: reader.GetString(1)
                                );
                        }
                    }
                }
            }
            return p;
        }

        public List<Doctor> get_doctors()
        {
            List<Doctor> doctores = new List<Doctor>();
            using (SqlConnection connection = new SqlConnection(_CONN_STR))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("[dbo].[GET_DOCTORS]", connection))
                {

                    command.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Doctor p = new()
                            {
                                Id = (int)reader.GetInt64(0),
                                Name = reader.GetString(1),
                                Specialization = reader.GetString(2),
                                Sub_specialty = reader.GetString(3),
                                Address = reader.GetString(4),
                               
                                Hours = new Hours()
                                {
                                    Id = (int)reader.GetInt64(5),
                                    DayOfWeek = reader.GetString(7).Split(",").ToList(),
                                    time = reader.GetString(8).Split(",").ToList()
                                },
                                Hospital = reader.GetString(6)
                            };
                            doctores.Add(p);
                        }
                    }
                }
            }
            return doctores;
        }

        public List<Doctor> GetDoctors_BySearch(string search_text)
        {
            List<Doctor> doctores = new List<Doctor>();
            string sql = $"EXEC [dbo].[GET_DOCTORS_By_SEARCH] {search_text};";
            using (SqlConnection connection = new SqlConnection(_CONN_STR))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(sql, connection))
                {

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Doctor p = new()
                            {
                                Id = (int)reader.GetInt64(0),
                                Name = reader.GetString(1),
                                Specialization = reader.GetString(2),
                                Sub_specialty = reader.GetString(3),
                                Address = reader.GetString(4),

                                Hours = new Hours()
                                {
                                    Id = (int)reader.GetInt64(5),
                                    DayOfWeek = reader.GetString(7).Split(",").ToList(),
                                    time = reader.GetString(8).Split(",").ToList()
                                },
                                Hospital = reader.GetString(6)
                            };
                            doctores.Add(p);
                        }
                    }
                }
            }
            return doctores;
        }

        public void AddDoctor(DoctorRequest doctor, string encrypted_password)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_CONN_STR))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("[dbo].[ADD_Doctor]", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters to the command
                        command.Parameters.AddWithValue("@name", doctor.Name);
                        command.Parameters.AddWithValue("@specialization", doctor.Specialization);
                        command.Parameters.AddWithValue("@sub_specialty", doctor.Sub_specialty);
                        command.Parameters.AddWithValue("@address", doctor.Address);
                        command.Parameters.AddWithValue("@dayofweek", string.Join(",", doctor.Hours.DayOfWeek));
                        command.Parameters.AddWithValue("@time", string.Join(",",doctor.Hours.time));
                        command.Parameters.AddWithValue("@Hospital", doctor.Hospital);
                        command.Parameters.AddWithValue("@phonenumber", doctor.PhoneNumber);

                        command.Parameters.AddWithValue("@Password", encrypted_password);

                        // Execute the stored procedure
                        command.ExecuteNonQuery();
                    }

                }

            }
            catch (SqlException e)
            {
                throw new DoctorInsertingFailedException(e.Message);
            }

        }

        public void AddPatient(PatientRequest patient,string encrypted_password)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_CONN_STR))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("[dbo].[AddPatient]", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters to the command
                        command.Parameters.AddWithValue("@name", patient.Name);
                        command.Parameters.AddWithValue("@phonenumber", patient.PhoneNumber);
                        command.Parameters.AddWithValue("@dateofbirth", patient.DateOfBirth);
                        command.Parameters.AddWithValue("@password", encrypted_password);

                        // Execute the stored procedure
                        command.ExecuteNonQuery();
                    }

                }

            }
            catch (SqlException e)
            {
                throw new PatientInsertingFailedException(e.Message);
            }
        }

      

    }
}
