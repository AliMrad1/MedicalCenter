namespace MedicalCenter.exceptions
{
    public class AppointmentDeleteFailedException : Exception
    {
        public AppointmentDeleteFailedException()
        {
        }

        public AppointmentDeleteFailedException(string? message) : base(message)
        {
        }
    }
}
