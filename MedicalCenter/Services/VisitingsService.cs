using MedicalCenter.Database;
using MedicalCenter.Models;
using MedicalCenter.Services.exceptions;

namespace MedicalCenter.Services;

public class VisitingsService
{

    private readonly FirebaseService _Fservice;
    private IConfiguration _configuration;
    private DALC_SQL_Visitings _sqlVisitings;
    
    public VisitingsService(IConfiguration configuration)
    {
        _configuration = configuration;
        _Fservice = new FirebaseService(_configuration);
        _sqlVisitings = new DALC_SQL_Visitings(_configuration);
    }

    public async Task Make_Visiting(VisitingRequest visitingRequest)
    {
        try
        {
            List<string> urls = await this._Fservice.UploadPhotos(visitingRequest.Photos);
            this._sqlVisitings .Make_Visitings(visitingRequest, urls);
        }
        catch (ImagesUploadFailedException e) 
        {
            throw new ImagesUploadFailedException(e.Message);
        }
        catch (VisitingMakeFailedException e) when(e.Message.Contains("CHECK constraint \"ck_date_visiApp\""))
        {
            throw new VisitingMakeFailedException("The Visiting Date has not coming yet!");
        }
       
    }

    public List<Visiting> GetVisitings()
    {
        
        try
        {
            List<Visiting> visitings = this._sqlVisitings.GetVisitings();
            return visitings;
        }
        catch (VisitingsLoadFailedException e)
        {
            throw new VisitingsLoadFailedException(e.Message);
        }
    }

    public Visiting GetVisitingById(int id)
    {
        try
        {
            Visiting visiting = this._sqlVisitings.GetVisitingsBYID(id);
            return visiting;
        }
        catch (VisitingsLoadFailedException e)
        {
            throw new VisitingsLoadFailedException(e.Message);
        }
    }
}