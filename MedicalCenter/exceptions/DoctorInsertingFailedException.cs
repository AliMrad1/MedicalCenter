using System.Runtime.Serialization;

namespace MedicalCenter.Database
{
    [Serializable]
    internal class DoctorInsertingFailedException : Exception
    {
        public DoctorInsertingFailedException()
        {
        }

        public DoctorInsertingFailedException(string? message) : base(message)
        {
        }

        public DoctorInsertingFailedException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected DoctorInsertingFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}