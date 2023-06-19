namespace MedicalCenter.Services.exceptions;

public class VisitingMakeFailedException : Exception
{
    public VisitingMakeFailedException(string? message) : base(message)
    {
    }
}