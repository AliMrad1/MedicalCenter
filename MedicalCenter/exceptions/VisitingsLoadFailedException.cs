namespace MedicalCenter.Services.exceptions;

public class VisitingsLoadFailedException : Exception
{
    public VisitingsLoadFailedException(string? message) : base(message)
    {
    }
}