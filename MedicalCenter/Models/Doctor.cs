using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Models
{
    public class Doctor
    {

        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Specialization { get; set; }
        [Required]
        public string Sub_specialty { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public Hours Hours { get; set; } = new Hours();
        [Required]
        public string Hospital { get; set; }
        [Required]
        public string PhoneNumber { get; set; }

    }

    public class DoctorRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Specialization { get; set; }
        [Required]
        public string Sub_specialty { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public Hours Hours { get; set; } = new Hours();
        [Required]
        public string Hospital { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public record DoctorDaysHours(Hours hours){}
    public record DoctorAppointment(int id, string name) { }
    public record DoctorTemp(int id, string name) { }
    public record DoctorLogin(string phoneNumber, string password) { }
    public record DoctorLoginResponse(string token, DateTime expiredAt) { }

    public record DoctorResponse(string status, string message, DateTime date_time)
    {
       
    }


}
