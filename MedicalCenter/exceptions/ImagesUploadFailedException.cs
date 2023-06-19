namespace MedicalCenter.Services.exceptions;

public class ImagesUploadFailedException:Exception
{
    public ImagesUploadFailedException(string? message) : base(message)
    {
    }
}