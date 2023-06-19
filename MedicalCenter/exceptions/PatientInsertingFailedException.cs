using System.Runtime.Serialization;

namespace MedicalCenter.Database
{
    [Serializable]
    internal class PatientInsertingFailedException : Exception
    {
        public PatientInsertingFailedException()
        {
        }

        public PatientInsertingFailedException(string? message) : base(message)
        {
        }

        public PatientInsertingFailedException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected PatientInsertingFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}