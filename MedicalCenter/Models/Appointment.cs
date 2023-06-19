namespace MedicalCenter.Models
{
    public class Appointment
    {
        public long Id { get; set; }
        public DateTime AppointmentDate { get; set; }
        public PatientAppointment patient { get; set; }
        public DoctorAppointment doctor { get; set; }
        public string Reason { get; set; }
        public string createdAT { get; set; }
        public bool isVisited { get; set; }
    }

    public record AppointmentRequest(DateTime AppointmentDate , 
        PatientAppointment patient,
        DoctorAppointment doctor,
        string Reason)
    {

    }

}
