using MedicalCenter.Models;
using MedicalCenter.Services;
using MedicalCenter.Services.exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
   [Authorize]
   public async Task<IActionResult> Make_Visiting([FromForm] VisitingRequest request)
   {
      try
      {
         await this._visitingsService.Make_Visiting(request);
         return Ok("Visiting done");
      }
      catch (ImagesUploadFailedException e)
      {
         return BadRequest($"error: {e.Message}");
      }
      catch (VisitingMakeFailedException e)
      {
         return BadRequest($"error: {e.Message}");
      }
   }

   [HttpGet("all")]
   [Authorize]
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
}