using System.Runtime.Serialization;

namespace MedicalCenter.exceptions
{
    [Serializable]
    public class DoctorUpdatingFailedException:Exception
    {
        public DoctorUpdatingFailedException(string? message) : base(message)
        {
        }

    }

}
