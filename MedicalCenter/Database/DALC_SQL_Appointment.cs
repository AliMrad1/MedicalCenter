using MedicalCenter.Models;
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
                        /*
                         * 
                         * 	@AppoitmentDatePARAM DateTime,
	                        @Patient_Id bigint,
	                        @Doctor_Id bigint,
	                        @Reason varchar(150)
                         */
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
    }
}
