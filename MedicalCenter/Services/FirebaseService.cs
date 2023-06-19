using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using MedicalCenter.Services.exceptions;

namespace MedicalCenter.Services;

public class FirebaseService
{
    private readonly StorageClient _storageClient;
    private readonly string _bucketName;
    private IConfiguration _configuration;
    public FirebaseService(IConfiguration configuration)
    {
        _configuration = configuration;

        var credentials = GoogleCredential.FromFile(_configuration["firebase:jsonfilepath"]);
        _bucketName = _configuration["firebase:bucket_name"];


        _storageClient = StorageClient.Create(credentials);
    
    }
    
    public async Task<List<string>> UploadPhotos(List<IFormFile> photos)
    {
        List<string> downloadUrls = new List<string>();

        try
        {
            foreach (var photo in photos)
            {
                string objectName = "photos/" + Path.GetFileName(photo.FileName);

                using (var memoryStream = new MemoryStream())
                {
                    await photo.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    _storageClient.UploadObject(_bucketName, objectName, "image/jpeg", memoryStream);
                    
                    var downloadUrl = $"https://firebasestorage.googleapis.com/v0/b/{_bucketName}/o/photos%2F{photo.FileName}?alt=media&token=e69bfd41-3a81-44cf-a243-0a5be44699e6&_gl=1*1bznofo*_ga*MTk3NjUyNzkzNS4xNjg2MzkyNzI0*_ga_CW55HF8NVT*MTY4NjQwNzkwOC4zLjEuMTY4NjQxMTY0Ni4wLjAuMA..";

                    downloadUrls.Add(downloadUrl);
                }
            }

            return downloadUrls;
        }
        catch (FirebaseException e)
        {
            throw new ImagesUploadFailedException(e.Message);
        }
       
    }
}