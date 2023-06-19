namespace MedicalCenter.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string DateOfBirth { get; set; }

        public Patient(int id, string name, string phoneNumber, string dateofbirth)
        {
            Id = id;
            Name = name;
            PhoneNumber = phoneNumber;
            DateOfBirth = dateofbirth;
        }
    }

    public record PatientRequest(string Name, string PhoneNumber, string DateOfBirth,string Password) { }

    public record PatientResponse(string msg) { }

    public record PatientLogin(string phonenumber, string password) { }
    public record PatientLoginResponse(string token) { }

    public record PatientAppointment(int id, string name, string phonenumber){}

    public record PatientTemp(int id, string name){}
}
