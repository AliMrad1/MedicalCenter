using MedicalCenter.Models;
using MedicalCenter.Services;
using MedicalCenter.Services.exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedicalCenter.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VisitingsController : ControllerBase
{

   private VisitingsService _visitingsService;
   private IConfiguration _configuration;

   public VisitingsController(IConfiguration configuration)
   {
      _configuration = configuration;
      _visitingsService = new VisitingsService(_configuration);
   }

   [HttpPost("make")]
   [Authorize("DoctorPolicy")]
   public async Task<IActionResult> Make_Visiting([FromQuery] string phoneNumber,[FromForm] VisitingRequest request)
   {
        var decodedPhoneNumber = Uri.UnescapeDataString(phoneNumber);

        var phoneNumberClaim = User.FindFirst(ClaimTypes.MobilePhone);
        var roleClaim = User.FindFirst(ClaimTypes.Role);

        if (CheckAuthority.isAuthorize(phoneNumberClaim, decodedPhoneNumber, roleClaim, "doctor"))
        {
            try
            {
                await this._visitingsService.Make_Visiting(request);
                return Ok(new VisitingResponse(status: Status.Success.ToString(), message: "Visting done", date_time: DateTime.Now) { });
            }
            catch (ImagesUploadFailedException e)
            {
                return BadRequest(new VisitingResponse(status: Status.Failure.ToString(), message: e.Message, date_time: DateTime.Now) { });
            }
            catch (VisitingMakeFailedException e)
            {
                return BadRequest(new VisitingResponse(status: Status.Failure.ToString(), message: e.Message, date_time: DateTime.Now) { });
            }
        }
        else
        {
            return BadRequest(new VisitingResponse(status: Status.Failure.ToString(), message: "Unauthorized", date_time: DateTime.Now) { });

        }
    }

   [HttpGet("all")]
   [Authorize("CombinedPolicy")]
    public IActionResult AllVisitings()
   {
      try
      {
         List<Visiting> visitings = this._visitingsService.GetVisitings();
         return Ok(visitings);
      }
      catch (VisitingsLoadFailedException e)
      {
         return BadRequest(e.Message);
      }   
   }


    [HttpGet("visiting")]
    [Authorize("CombinedPolicy")]
    public IActionResult Get_Visitings_ByID([FromQuery] int id, [FromQuery] string phoneNumber)
    {

        var decodedPhoneNumber = Uri.UnescapeDataString(phoneNumber);

        var phoneNumberClaim = User.FindFirst(ClaimTypes.MobilePhone);
        var roleClaim = User.FindFirst(ClaimTypes.Role);

        if (CheckAuthority.isAuthorize(phoneNumberClaim, decodedPhoneNumber, roleClaim, "*"))
        {
            try
            {
                Visiting visiting = this._visitingsService.GetVisitingById(id,phoneNumber);
                return Ok(visiting);
            }
            catch (VisitingsLoadFailedException e)
            {
                return BadRequest(e.Message);
            }
        }

        else
        {
            return BadRequest(new VisitingResponse(status: Status.Failure.ToString(), message: "Unauthorized", date_time: DateTime.Now) { });
        }
    }
}