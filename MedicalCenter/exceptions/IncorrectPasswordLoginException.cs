using System.Runtime.Serialization;

namespace MedicalCenter.Services
{
    [Serializable]
    internal class IncorrectPasswordLoginException : Exception
    {
        public IncorrectPasswordLoginException()
        {
        }

        public IncorrectPasswordLoginException(string? message) : base(message)
        {
        }

        public IncorrectPasswordLoginException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected IncorrectPasswordLoginException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}