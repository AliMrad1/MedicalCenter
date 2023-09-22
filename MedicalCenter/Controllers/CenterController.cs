using MedicalCenter.Database;
using MedicalCenter.Models;
using MedicalCenter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;

namespace MedicalCenter.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CenterController : ControllerBase
    {

        public DALC_SQL dALC_SQL;
        public IConfiguration _configuration;
        private CenterService centerService;

        public CenterController(IConfiguration _configuration){
            this._configuration = _configuration;
            this.dALC_SQL = new DALC_SQL(_configuration);
            this.centerService = new CenterService(_configuration);
        }

        [HttpPost("AddPatient")]
        public async Task<IActionResult> register([FromBody] PatientRequest request)
        {
            try{
                await this.centerService.ADD_Patient(request);
                return Ok("Add Successfully");

            }
            catch(PatientInsertingFailedException e){
                return BadRequest(e.Message);
            }
            catch (PhoneNumberValidationException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("loginPatient")]
        public IActionResult loginPatient([FromBody] PatientLogin request)
        {
            try
            {
                PatientLoginResponse p =  this.centerService.LOGIN_PATIENT(request);
                return Ok(p);

            }
            catch (IncorrectPasswordLoginException e)
            {
                return BadRequest(e.Message);
            }
            catch (PhoneNumberValidationException e)
            {
                return BadRequest(e.Message);
            }
            catch (IncorretEmailLoginException e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpPost("loginDoctor")]
        public IActionResult loginDoctor([FromBody] DoctorLogin request)
        {
            try
            {
                DoctorLoginResponse p = this.centerService.LOGIN_DOCTOR(request);
                return Ok(p);

            }
            catch (IncorrectPasswordLoginException e)
            {
                return BadRequest(e.Message);
            }
            catch (PhoneNumberValidationException e)
            {
                return BadRequest(e.Message);
            }
            catch (IncorretEmailLoginException e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpGet("doctors")]
        public IActionResult Get_Doctors()
        {
            List<Doctor> doctors = centerService.GetDoctors();
            return Ok(doctors);
        }

        [HttpGet("doctors/doctor")]
        [Authorize("DoctorPolicy")]
        public IActionResult Get_Doctor_By_PhoneNumber([FromQuery] string phoneNumber)
        {
            var decodedPhoneNumber = Uri.UnescapeDataString(phoneNumber);

            var phoneNumberClaim = User.FindFirst(ClaimTypes.MobilePhone);
            var roleClaim = User.FindFirst(ClaimTypes.Role);

            if (CheckAuthority.isAuthorize(phoneNumberClaim, decodedPhoneNumber, roleClaim, "doctor"))
            {
                Doctor d = centerService.Get_Doctor_By_PhoneNumber(decodedPhoneNumber);
                return Ok(d);
            }
            else
            {
                return BadRequest(new DoctorResponse(status: "failure",
                        message: "Unauthorized Error!!",
                       date_time: DateTime.Now)
                    {}
                );
            }
            
           
        }

        [HttpGet("patients/patient")]
        [Authorize("PatientPolicy")]
        public IActionResult Get_Patient_By_PhoneNumber([FromQuery] string phoneNumber)
        {
            var decodedPhoneNumber = Uri.UnescapeDataString(phoneNumber);

            var phoneNumberClaim = User.FindFirst(ClaimTypes.MobilePhone);
            var roleClaim = User.FindFirst(ClaimTypes.Role);

            if (CheckAuthority.isAuthorize(phoneNumberClaim, decodedPhoneNumber, roleClaim, "patient"))
            {
                Patient p = centerService.Get_Patient_By_PhoneNumber(decodedPhoneNumber);
                return Ok(p);
            }
            else
            {
                return BadRequest(new DoctorResponse(status: "failure",
                        message: "Unauthorized Error!!",
                       date_time: DateTime.Now)
                { }
                );
            }
        }

        [HttpGet("doctors/specialities")]
        public IActionResult GetDoctors_Spcialities()
        {
            List<String> specialities = centerService.GetDoctors_Spcialities();
            return Ok(specialities);
        }
    }
}
