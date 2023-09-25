namespace MedicalCenter.Models;

public class Visiting
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public PatientTemp Patient { get; set; }
    public DoctorTemp Doctor { get; set; }
    public string[]? FileUrls { get; set; } 
    public string? Description { get; set; } 

}

public class VisitingRequest
{
    public int Patient_ID { get; set; }
    public int Doctor_ID { get; set; }
    public string Description { get; set; }
    public List<IFormFile> Photos { get; set; }
    public int Appointment_Id { get; set; }
    
}



public record VisitingResponse(string status, string message, DateTime date_time) { }