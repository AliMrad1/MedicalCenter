using System.Runtime.Serialization;

namespace MedicalCenter.Services
{
    [Serializable]
    internal class IncorretEmailLoginException : Exception
    {
        public IncorretEmailLoginException()
        {
        }

        public IncorretEmailLoginException(string? message) : base(message)
        {
        }

        public IncorretEmailLoginException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected IncorretEmailLoginException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}