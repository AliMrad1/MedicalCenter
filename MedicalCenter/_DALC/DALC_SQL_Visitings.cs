using System.Data;
using MedicalCenter.Models;
using MedicalCenter.Services.exceptions;
using Microsoft.Data.SqlClient;

namespace MedicalCenter.Database;

public class DALC_SQL_Visitings
{
    private readonly IConfiguration _configuration;
    private readonly string _connStr = "";
    DateTime currentDateTime = DateTime.Now;
    public DALC_SQL_Visitings(IConfiguration configuration)
    {
        _configuration = configuration;
        _connStr = this._configuration["_CONN_STR:conn"];
    }

    public void Make_Visitings(VisitingRequest request, List<string> imgs_url)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(_connStr))
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        SqlCommand command;

                        int visitingId;
                        command = new SqlCommand("SELECT NEXT VALUE FOR visiting_id_sequence;", connection, transaction);
                        visitingId = (int)command.ExecuteScalar();

                        command = new SqlCommand("[dbo].[ADD_VISITING]", connection, transaction);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Id", visitingId); // get value from sequence
                        command.Parameters.AddWithValue("@patient_Id", request.Patient_ID);
                        command.Parameters.AddWithValue("@doctor_Id", request.Doctor_ID);
                        command.Parameters.AddWithValue("@description", request.Description);
                        command.Parameters.AddWithValue("@date", currentDateTime);
                        command.Parameters.AddWithValue("@appointment_id", request.Appointment_Id);

                        command.ExecuteNonQuery();
                        
                        foreach (var img in imgs_url)
                        {
                            command = new SqlCommand("[dbo].[ADD_PHOTOS]", connection, transaction);
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@img_url", img);
                            command.Parameters.AddWithValue("@visiting_id", visitingId); // get value from sequence
                            command.ExecuteNonQuery();
                        }

                       

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new VisitingMakeFailedException("Transaction rolled back: " + ex.Message);
                    }
                }
            }
        }
        catch (SqlException e)
        {
            throw new VisitingMakeFailedException(e.Message);
        }
    }


    public List<Visiting> GetVisitings()
    {
        List<Visiting> visitings = new List<Visiting>();

        try
        {
            using (SqlConnection connection = new SqlConnection(_connStr))
            {
                connection.Open();
                string sql = $"EXECUTE  [dbo].[Get_VISITINGS];";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Visiting v = new()
                            {
                                Id = (int) reader.GetInt64(0),
                                Date = reader.GetDateTime(1),
                                Patient = new 
                                (
                                    id:0, // default one
                                    name : reader.GetString(2)
                                ),
                                Doctor = new 
                                (
                                    id:0,// default one
                                    name : reader.GetString(3)
                                ),
                                Description = null,
                                FileUrls = Array.Empty<string>()
                            };
                        
                            visitings.Add(v);
                        }
                    }
                }
            }
            return visitings;
        }
        catch (VisitingsLoadFailedException e)
        {
            throw new VisitingsLoadFailedException(e.Message);
        }
       

    }
    
    
    public Visiting GetVisitingsBYID(int id)
    {
        Visiting v = null;
        List<string> urls = new List<string>();

        try
        {
            using (SqlConnection connection = new SqlConnection(_connStr))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("[dbo].[Get_VisitingsByID]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ID", id); // get value from sequence
                    
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        
                        while (reader.Read())
                        {   
                            v = new()
                            {
                                Id = (int) reader.GetInt64(0),
                                Date = reader.GetDateTime(1),
                                Patient = new 
                                (
                                    id: (int) reader.GetInt64(2),
                                    name : reader.GetString(3)
                                ),
                                Doctor = new 
                                (
                                    id: (int) reader.GetInt64(4),
                                    name : reader.GetString(5)
                                ),
                                Description = reader.GetString(6),
                                FileUrls = Array.Empty<string>()
                            };
                        
                        }
                    }
                }

                if(v != null)
                {
                    using (SqlCommand command = new SqlCommand("[dbo].[GetAttachmentVisitingByID]", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ID", id); // get value from sequence

                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                urls.Add(reader.GetString(0));
                            }
                        }
                    }
                }
                
            }

            v.FileUrls = urls.ToArray();
            return v;
        }
        catch (VisitingsLoadFailedException e)
        {
            throw new VisitingsLoadFailedException(e.Message);
        }
        

    }


    public Visiting Get_PatientOrDoctor_VisitingsBYID(int id,string phoneNumber)
    {
        Visiting v = null;
        List<string> urls = new List<string>();

        try
        {
            using (SqlConnection connection = new SqlConnection(_connStr))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("[dbo].[Get_Patient_OR_Doctor_VisitingsByID]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ID", id); // get value from sequence
                    command.Parameters.AddWithValue("@PhoneNumber", phoneNumber); // get value from sequence

                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            v = new()
                            {
                                Id = (int)reader.GetInt64(0),
                                Date = reader.GetDateTime(1),
                                Patient = new
                                (
                                    id: (int)reader.GetInt64(2),
                                    name: reader.GetString(3)
                                ),
                                Doctor = new
                                (
                                    id: (int)reader.GetInt64(4),
                                    name: reader.GetString(5)
                                ),
                                Description = reader.GetString(6),
                                FileUrls = Array.Empty<string>()
                            };

                        }
                    }
                }
                if( v != null)
                {
                    using (SqlCommand command = new SqlCommand("[dbo].[GetAttachmentVisitingByID]", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ID", id); // get value from sequence

                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                urls.Add(reader.GetString(0));
                            }
                        }
                    }

                    v.FileUrls = urls.ToArray();

                }

            }

            return v;
        }
        catch (VisitingsLoadFailedException e)
        {
            throw new VisitingsLoadFailedException(e.Message);
        }


    }
}