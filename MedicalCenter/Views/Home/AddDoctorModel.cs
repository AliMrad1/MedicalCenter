using MedicalCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MedicalCenter.Views.Home
{
    public class AddDoctorModel 
    {
        [BindProperty]
        public Doctor Doctor { get; set; }

    }
}
