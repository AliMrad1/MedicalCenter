using MedicalCenter.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using MedicalCenter.Database;
using Microsoft.IdentityModel.Tokens;
using MedicalCenter.Services;
using MedicalCenter.exceptions;
using MedicalCenter.Models;


namespace MedicalCenter.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private static List<string> selectedItems = new List<string>();
        private static string _Specialication = "";
        private static List<string> _times = new List<string>();
        private IConfiguration _configuration;
        private static List<Doctor> doctors = new List<Doctor>();
        private CenterService centerService;
        private AppointmentService appointmentService;
        private VisitingsService _visitingsService;

        public DALC_SQL dALC_SQL;

        public HomeController(IConfiguration _configuration, ILogger<HomeController> logger)
        {
            this._configuration = _configuration;
            dALC_SQL = new DALC_SQL(_configuration);
            _logger = logger;
            centerService = new CenterService(_configuration);
            appointmentService = new AppointmentService(_configuration);
            _visitingsService = new VisitingsService(_configuration);
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }


        [HttpGet]
        public IActionResult AddDoctors()
        {

            DoctorRequest doctor = new DoctorRequest();

            return View(doctor);
        }


        [HttpPost]
        public IActionResult AddDoctors(DoctorRequest doctor)
        {
            doctor.Specialization = _Specialication;
            // save in database;
            Console.WriteLine(" hghghgjhhg");

            AddDoctorResponse res = new AddDoctorResponse();
            doctor.Hours.DayOfWeek.AddRange(selectedItems);
            doctor.Hours.time.AddRange(_times);
            string msg;

            Console.WriteLine(doctor.Hours.time);   
            try
            {
                this.centerService.AddDoctor(doctor);

                res.response = "Doctor Add Successfully!!";

            }
            catch (DoctorInsertingFailedException e)
            {
                Console.WriteLine(e.Message);
                res.response = string.Concat("Something wrong , please try again!!", e.Message);

            }

            Console.WriteLine(res.response);

            selectedItems.Clear();

            return View("AddDoctorResult",res);

        }

        [HttpPost]
        public IActionResult SelectedItem([FromBody] List<string> checkedItems)
        {
            selectedItems.Clear();
            // Process the checkedItems list
            selectedItems.AddRange(checkedItems);
            // Return JSON response
            return Json(new { success = true, message = "Data processed successfully" });
        }


        //catch time 
        [HttpPost]
        public IActionResult Time([FromBody] List<string> times)
        {
            _times.Clear();
            _times.AddRange(times);
            return Json(new { success = true, message = "Data processed successfully" });
        }

        [HttpPost]
        public IActionResult Specialization([FromBody] string Specialization)
        {
            _Specialication = Specialization;
            return Json(new { success = true, message = "Data processed successfully" });
        }

        public IActionResult Appointments()
        {

            List<Appointment> appointments = appointmentService.GET_APPOINTMENTS();

            // Pass the list of appointments to the view
            return View(appointments);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult AddDoctorResult()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AllDoctors()
        {
            if(doctors.Count == 0)
            {
                List<Doctor> _doctors = dALC_SQL.get_doctors();
                return View(_doctors);
            }
            else
            {
                return View(doctors);
            }
           
        }

        // in two cases , when search_text is empty(get ALl data) 
        [HttpPost]
        public IActionResult SearchDoctor([FromBody] string searchInput)
        {
            doctors.Clear();
            if (searchInput.IsNullOrEmpty())
            {
                searchInput = "";
            }
            List<Doctor> doc = dALC_SQL.GetDoctors_BySearch(searchInput);
           doctors.AddRange(doc);
            return Json(new { success = true, message = "Data processed successfully", doctors = doc });
        }

        public IActionResult Visitings()
        {

            List<Visiting> visitings = this._visitingsService.GetVisitings();//later on will be get data from database
            return View(visitings);
        }

        [HttpGet]
        public IActionResult VisitingDetails(int id)
        {
            try
            {
                Visiting visiting = this._visitingsService.GetVisitingById(id);
                return View(visiting);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }


        [HttpPost]
        public IActionResult GetDoctorById(Doctor doctor){
            try
            {
                centerService.UpdateDoctor(doctor);
                return Ok("Updated Successfully");
            }
            catch (Exception e)
            {
                return Json(new { success = true, message = "Data Not Found", doctors = doctor });
                throw;
            }
        }


        public IActionResult Edit(int appointmentId)
        {
            var appointment = appointmentService.GetAppointmentById(appointmentId);
            return View("Edit", appointment);
        }

        [HttpPost]
        public IActionResult EditAppointment(Appointment appointment)
        {
            try
            {
              
                appointmentService.UpdateAppointment_Admin(appointment);                
                return RedirectToAction("Appointments"); 
            }
            catch (AppointmentReservedFailedException)
            {
                
                return View("Error");
            }
           
        }


        public IActionResult Delete(int appointmentId)
        {
            try
            {
                appointmentService.DeleteAppointmentById(appointmentId);
                return View("DeleteConfirmation");
            }
            catch (AppointmentDeleteFailedException)
            {
                
                return View("Error");
            }
        }

    }
}