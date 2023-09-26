using MedicalCenter.exceptions;
using MedicalCenter.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using System.Data;

namespace MedicalCenter.Database
{
    public class DALC_SQL_Appointment
    {
        private IConfiguration _configuration;
        public string _CONN_STR = "";
        public DALC_SQL_Appointment(IConfiguration configuration)
        {
            _configuration = configuration;
            _CONN_STR = this._configuration["_CONN_STR:conn"];
        }

        public void Reserve_Appoitment(AppointmentRequest request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_CONN_STR))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("[dbo].[CHECK_APPOITMENT_DATE_IS_RESERVED]", connection))
                    {
                    
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@AppoitmentDatePARAM", request.AppointmentDate);
                        command.Parameters.AddWithValue("@Patient_Id", request.patient.id);
                        command.Parameters.AddWithValue("@Doctor_Id", request.doctor.id);
                        command.Parameters.AddWithValue("@Reason", request.Reason);

                        // Execute the stored procedure
                        command.ExecuteNonQuery();
                    }

                }

            }
            catch (SqlException e)
            {
                throw new AppointmentReservedFailedException(e.Message);
            }

        }

        public List<DateTime> GetAppointmentDateTimePerDay(DateTime targetDate)
        {
            List<DateTime> datetimeRecords = new List<DateTime>();
            
            using (SqlConnection connection = new SqlConnection(_CONN_STR))
            {
                connection.Open();
                string sql = $"EXECUTE  [dbo].[GET_RESERVED_DATES] '{targetDate}';";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DateTime datetimeValue = reader.GetDateTime(0);
                            datetimeRecords.Add(datetimeValue);
                        }
                    }
                }
            }
            return datetimeRecords;
        }

        //GET_APPOINTMENTS
        public List<Appointment> GET_APPOINTMENTS()
        {
            List<Appointment> appointments = new List<Appointment>();

            using (SqlConnection connection = new SqlConnection(_CONN_STR))
            {
                connection.Open();
                string sql = $"EXECUTE  [dbo].[GET_APPOINTMENTS];";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Appointment p = new()
                            {
                                Id = (long)reader.GetInt64(0),
                                AppointmentDate = reader.GetDateTime(1),
                                patient = new
                                (
                                    id : (int)reader.GetInt64(2),
                                    name : reader.GetString(3),
                                    phonenumber: reader.GetString(4)
                                ),
                                doctor = new
                                (
                                    id: (int)reader.GetInt64(5),
                                    name: reader.GetString(6)
                                ),

                                Reason = reader.GetString(7),
                                isVisited = reader.GetBoolean(8)
                            };

                            appointments.Add(p);
                        }
                    }
                }
            }
            return appointments;

        }

        public List<Appointment> GET_APPOINTMENTS_Patient(string phoneNumber)
        {
            List<Appointment> appointments = new List<Appointment>();

            using (SqlConnection connection = new SqlConnection(_CONN_STR))
            {
                connection.Open();
                string sql = $"EXECUTE  [dbo].[GET_APPOINTMENTS_PATIENT] '{phoneNumber}';";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            
                            Appointment p = new()
                            {
                                Id = (long)reader.GetInt64(0),
                                AppointmentDate = reader.GetDateTime(1),
                                patient = new
                                (
                                    id: (int)reader.GetInt64(2),
                                    name: reader.GetString(3),
                                    phonenumber: reader.GetString(4)
                                ),
                                doctor = new
                                (
                                    id: (int)reader.GetInt64(5),
                                    name: reader.GetString(6)
                                ),
                                Reason = reader.GetString(7),
                                isVisited = reader.GetBoolean(8)
                            };

                            appointments.Add(p);
                        }
                    }
                }
            }
            return appointments;

        }

        public int UpdateAppointment(AppointmentUpdate appointment){
               try
            {
                using (SqlConnection connection = new SqlConnection(_CONN_STR))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("[dbo].[UPDATE_APPOINTMENT]", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Appointment_Id", appointment.Id);
                        command.Parameters.AddWithValue("@Patient_Id", appointment.patient.id);                        
                        command.Parameters.AddWithValue("@AppointmentDateParam", appointment.AppointmentDate);
                        command.Parameters.AddWithValue("@Doctor_Id", appointment.doctor.id);
                        command.Parameters.AddWithValue("@Reason", appointment.Reason);

                        return command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException e)
            {
                throw new AppointmentReservedFailedException(e.Message);
            }
        }

        public int UpdateAppointment_Admin(Appointment appointment)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_CONN_STR))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("[dbo].[UPDATE_APPOINTMENT_Admin]", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Appointment_Id", appointment.Id);
                        command.Parameters.AddWithValue("@Patient_Id", appointment.patient.id);
                        command.Parameters.AddWithValue("@AppointmentDateParam", appointment.AppointmentDate);
                        command.Parameters.AddWithValue("@Reason", appointment.Reason);
                        command.Parameters.AddWithValue("@isVisited", appointment.isVisited);


                        return command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException e)
            {
                throw new AppointmentReservedFailedException(e.Message);
            }
        }


        public int DeleteAppointmentById(int appointmentId){
               try
            {
                using (SqlConnection connection = new SqlConnection(_CONN_STR))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("[dbo].[DeleteAppointmentById]", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@AppointmentId", appointmentId);

                        return command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException e)
            {
                throw new AppointmentDeleteFailedException(e.Message);
            }
        }



        public int CancelAppointment(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_CONN_STR))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("[dbo].[CANCEL_APPOINTMENT]", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Appointment_id", id);

                        return command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException e)
            {
                throw new AppointmentCancelFailedException(e.Message);
            }
        }

        public List<Appointment> GET_APPOINTMENTS_Doctor(string phoneNumber)
        {
            List<Appointment> appointments = new List<Appointment>();

            using (SqlConnection connection = new SqlConnection(_CONN_STR))
            {
                connection.Open();
                string sql = $"EXECUTE  [dbo].[GET_APPOINTMENTS_Doctor] '{phoneNumber}';";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            Appointment p = new()
                            {
                                Id = (long)reader.GetInt64(0),
                                AppointmentDate = reader.GetDateTime(1),
                                patient = new
                                (
                                    id: (int)reader.GetInt64(2),
                                    name: reader.GetString(3),
                                    phonenumber: reader.GetString(4)
                                ),
                                doctor = new
                                (
                                    id: (int)reader.GetInt64(5),
                                    name: reader.GetString(6)
                                ),
                                Reason = reader.GetString(7),
                                isVisited = reader.GetBoolean(8)
                            };

                            appointments.Add(p);
                        }
                    }
                }
            }
            return appointments;
        }

        public Appointment GET_APPOINTMENTS_By_ID(int id)
        {

            Appointment p = null;

            using (SqlConnection connection = new SqlConnection(_CONN_STR))
            {
                connection.Open();
                string sql = $"EXECUTE  [dbo].[GET_APPOINTMENTS_By_ID] '{id}';";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            p = new()
                            {
                                Id = (long)reader.GetInt64(0),
                                AppointmentDate = reader.GetDateTime(1),
                                patient = new
                                (
                                    id: (int)reader.GetInt64(2),
                                    name: reader.GetString(3),
                                    phonenumber: reader.GetString(4)
                                ),
                                doctor = new
                                (
                                    id: (int)reader.GetInt64(5),
                                    name: reader.GetString(6)
                                ),
                                Reason = reader.GetString(7),
                                isVisited = reader.GetBoolean(8)
                            };

                        }
                    }
                }
            }
            return p;
        }

        public DoctorDaysHours GET_Doctor_Days_Hours(int doctor_id)
        {
            DoctorDaysHours? doctorDaysHours = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(_CONN_STR))
                {
                    connection.Open();
                    string sql = $"EXECUTE  [dbo].[GET_DOCTOR_HOURS_DAYS_BY_ID] '{doctor_id}';";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                doctorDaysHours = new(
                                    hours : new()
                                    {
                                        Id = (int) reader.GetInt64(0),
                                        DayOfWeek = reader.GetString(1).Split(',').ToList(), //convert the array to list
                                        time = reader.GetString(2).Split(',').ToList()
                                    }
                                )
                                {};
                            }
                        }
                    }
                }

                return doctorDaysHours;
            }
            catch (SqlException e)
            {
                throw new AppointmentReservedFailedException(e.Message);
            }
          
        }
    }
}
