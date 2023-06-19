using System.Runtime.Serialization;

namespace MedicalCenter.Services
{
    [Serializable]
    internal class PhoneNumberValidationException : Exception
    {
        public PhoneNumberValidationException()
        {
        }

        public PhoneNumberValidationException(string? message) : base(message)
        {
        }

        public PhoneNumberValidationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected PhoneNumberValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}