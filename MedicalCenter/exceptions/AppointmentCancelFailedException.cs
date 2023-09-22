namespace MedicalCenter.exceptions
{
    public class AppointmentCancelFailedException : Exception
    {
        public AppointmentCancelFailedException()
        {
        }

        public AppointmentCancelFailedException(string? message) : base(message)
        {
        }
    }
}
