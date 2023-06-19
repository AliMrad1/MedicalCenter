using MedicalCenter.Models;
using System;

namespace MedicalCenter.Controllers
{
    public class Class
    {

        public static List<Appointment> GetAppointments()
        {
            // Retrieve appointments from a data source and return the list
            // For demonstration purposes, let's create some sample appointments
            return new List<Appointment>
                {
                    new Appointment { Id=0,AppointmentDate = DateTime.Now.AddDays(7),
                        patient = new PatientAppointment(id : 1,name: "John Doe",phonenumber:"81674951"),
                        doctor = new DoctorAppointment(id: 1,name:"Dr. Smith"),
                        Reason = "Follow-up",
                        createdAT = DateTime.Now.ToString()
                    },
                    new Appointment { Id=0,AppointmentDate = DateTime.Now.AddDays(7),
                                            patient = new PatientAppointment(id : 1,name: "John Doe",phonenumber:"81674951"),
                                            doctor = new DoctorAppointment(id: 1,name:"Dr. Smith"),
                                            Reason = "Follow-up",
                                            createdAT = DateTime.Now.ToString()
                    },
                    new Appointment { Id=0,AppointmentDate = DateTime.Now.AddDays(7),
                                            patient = new PatientAppointment(id : 1,name: "John Doe",phonenumber:"81674951"),
                                            doctor = new DoctorAppointment(id: 1,name:"Dr. Smith"),
                                            Reason = "Follow-up",
                                            createdAT = DateTime.Now.ToString()
                    }
                };
        }
    }
}
