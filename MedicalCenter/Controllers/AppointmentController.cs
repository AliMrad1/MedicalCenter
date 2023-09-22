using MedicalCenter.Database;
using MedicalCenter.Models;
using MedicalCenter.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using MedicalCenter.exceptions;

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
        [Authorize("PatientPolicy")]
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
        [Authorize("PatientPolicy")]
        public List<TimeSpan> TimesReservedPerDate([FromQuery(Name = "date")] string dateString)
        {
            DateTime datec = DateTime.ParseExact(dateString, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            return _service.appointmentTimePerDay(datec);   
        }

        [HttpPut("update")]
        [Authorize("PatientPolicy")]
        public IActionResult UpdateAppointment([FromBody] AppointmentUpdate appointment){
            try
            {

                 this._service.UpdateAppointment(appointment);
                 return Ok(new AppointmentResponse(
                    status: Status.Success.ToString(),
                    message:"Appointment Update Successfully",
                    date_time:DateTime.Now
                ));
            }
            catch (AppointmentReservedFailedException e)
            {
                return BadRequest(new AppointmentResponse(
                    status: Status.Failure.ToString(),
                    message:e.Message,
                    date_time:DateTime.Now
                ));
            }
           
        }

        [HttpGet("patient/all")]
        [Authorize("PatientPolicy")]
        public IActionResult Get_Appoitments_Patient([FromQuery] string phoneNumber)
        {
            var decodedPhoneNumber = Uri.UnescapeDataString(phoneNumber);

            var phoneNumberClaim = User.FindFirst(ClaimTypes.MobilePhone);
            var roleClaim = User.FindFirst(ClaimTypes.Role);

            if (CheckAuthority.isAuthorize(phoneNumberClaim, decodedPhoneNumber, roleClaim, "patient"))
            {
                List<Appointment> appointments_Patient = _service.GET_APPOINTMENTS_Patient(decodedPhoneNumber);
                return Ok(appointments_Patient);
            }
            else
            {
                return BadRequest(new DoctorResponse(status: Status.Failure.ToString(),
                        message: "Unauthorized Error!!",
                       date_time: DateTime.Now)
                { }
                );
            }
        }

        [HttpGet("doctor/all")]
        [Authorize("DoctorPolicy")]
        public IActionResult Get_Appoitments_Doctor([FromQuery] string phoneNumber)
        {
            var decodedPhoneNumber = Uri.UnescapeDataString(phoneNumber);

            var phoneNumberClaim = User.FindFirst(ClaimTypes.MobilePhone);
            var roleClaim = User.FindFirst(ClaimTypes.Role);

            if (CheckAuthority.isAuthorize(phoneNumberClaim, decodedPhoneNumber, roleClaim, "doctor"))
            {
                List<Appointment> appointments_Patient = _service.GET_APPOINTMENTS_Doctor(decodedPhoneNumber);
                return Ok(appointments_Patient);
            }
            else
            {
                return BadRequest(new DoctorResponse(status: Status.Failure.ToString(),
                        message: "Unauthorized Error!!",
                       date_time: DateTime.Now)
                { }
                );
            }
        }

        [HttpDelete("cancel")]
        [Authorize("PatientPolicy")]
        public IActionResult Cancel_Appointment([FromQuery] int id,[FromQuery] string phoneNumber)
        {
            var decodedPhoneNumber = Uri.UnescapeDataString(phoneNumber);

            var phoneNumberClaim = User.FindFirst(ClaimTypes.MobilePhone);
            var roleClaim = User.FindFirst(ClaimTypes.Role);

            if (CheckAuthority.isAuthorize(phoneNumberClaim, decodedPhoneNumber, roleClaim, "patient"))
            {
                try
                {
                    _service.CancelAppointment(id);
                    return Ok(new AppointmentResponse(
                            status: Status.Success.ToString(),
                            message: "Appoitment Canceled Successfully",
                            date_time: DateTime.Now)
                    { }
                    );
                }
                catch(AppointmentCancelFailedException e)
                {
                    return BadRequest(new DoctorResponse(status: Status.Failure.ToString(),
                                            message: e.Message,
                                           date_time: DateTime.Now)
                    { }
                    );
                }
               
            }
            else
            {
                return BadRequest(new DoctorResponse(status: Status.Failure.ToString(),
                        message: "Unauthorized Error!!",
                       date_time: DateTime.Now)
                { }
                );
            }
        }
    }
}
