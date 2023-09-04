using MedicalCenter.Database;
using MedicalCenter.Models;
using MedicalCenter.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    }
}
