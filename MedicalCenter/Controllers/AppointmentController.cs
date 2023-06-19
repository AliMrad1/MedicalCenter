using MedicalCenter.Database;
using MedicalCenter.Models;
using MedicalCenter.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;

namespace MedicalCenter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private IConfiguration _configuration;
        private AppointmentService _service;

        public AppointmentController (IConfiguration configuration) 
        {
            _configuration = configuration;
            _service = new AppointmentService (configuration);
        }

        [HttpPost("reserve")]
        [Authorize]
        public async Task<IActionResult> Reserve_Appoitment(AppointmentRequest request)
        {
            try
            {
                await _service.reserveAnAppointment(request);
                return Ok("Appointment has been reserved");
            }
            catch (AppointmentReservedFailedException e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpGet("datesreserved")]
        [Authorize]
        public List<TimeSpan> TimesReservedPerDate([FromQuery(Name = "date")] string dateString)
        {
            DateTime datec = DateTime.ParseExact(dateString, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            return _service.appointmentTimePerDay(datec);   
        }
    }
}
