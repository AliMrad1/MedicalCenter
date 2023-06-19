using System.Runtime.Serialization;

namespace MedicalCenter.Database
{
    [Serializable]
    internal class AppointmentReservedFailedException : Exception
    {
        public AppointmentReservedFailedException()
        {
        }

        public AppointmentReservedFailedException(string? message) : base(message)
        {
        }

        public AppointmentReservedFailedException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected AppointmentReservedFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}