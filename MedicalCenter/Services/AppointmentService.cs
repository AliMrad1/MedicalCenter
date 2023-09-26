using MedicalCenter.Database;
using MedicalCenter.exceptions;
using MedicalCenter.Models;

namespace MedicalCenter.Services
{
    public class AppointmentService
    {
        private DALC_SQL_Appointment sQL_Appointment;
        private IConfiguration _configuration;
        private SendSMS sendSMS;

        public AppointmentService(IConfiguration configuration)
        {
            _configuration = configuration;
            this.sQL_Appointment = new DALC_SQL_Appointment(_configuration);
            this.sendSMS = new SendSMS(_configuration);
        }

        public async Task reserveAnAppointment(AppointmentRequest appointment)
        {
            checkIfAppDateMatchDoctorDate(appointment);
          
            // store appoitment
            try
            {
                this.sQL_Appointment.Reserve_Appoitment(appointment);
                // alert the patient that the appoitment has been reserved for him via sms 
                string msg_text = $"the appoitment has been reserved for you in" +
                    $" {appointment.AppointmentDate.ToString()} ";
                await this.sendSMS.Send(appointment.patient.phonenumber, msg_text);


                TimeSpan timeDifference = appointment.AppointmentDate - DateTime.Now;
                Console.WriteLine(timeDifference);
                TimeSpan delay = TimeSpan.FromDays(timeDifference.Days - 1);

                await Task.Run(async () =>
                {
                    await Task.Delay(delay);
                    // send sms after few days
                    string msg_text = $"Remember You have Appointment Tomorrow " +
                    $" {appointment.AppointmentDate.ToString()} with Doctor ${appointment.doctor.name}";
                    await this.sendSMS.Send(appointment.patient.phonenumber, msg_text);
                });


            }
            catch (AppointmentReservedFailedException e) when(e.Message.Contains("KEY constraint \'aD_appointment_uq_ck\'"))
            {

                //trow exception
                throw new AppointmentReservedFailedException("The Appointment Date Already reserved!");
            }
            catch (AppointmentReservedFailedException e) when(e.Message.Contains(" "))
            {

                //trow exception
                throw new AppointmentReservedFailedException(e.Message);
            }
        }

        private void checkIfAppDateMatchDoctorDate(AppointmentRequest appointment)
        {
            var dayOfWeek = appointment.AppointmentDate.DayOfWeek.ToString();
            var hour_app = appointment.AppointmentDate.Hour.ToString();

            DoctorDaysHours doctor = sQL_Appointment.GET_Doctor_Days_Hours(appointment.doctor.id);

            bool foundDay = false;
            bool foundHour = false;

            foreach (var day in doctor.hours.DayOfWeek)
            {
                if (day == dayOfWeek)
                {
                    foundDay = true; 

                    foreach (var hour in doctor.hours.time)
                    {
                        var hours_spliting = hour.Split("-");
                        int hour_app_int = Convert.ToInt32(hour_app);
                        int hours_spliting_lower = Convert.ToInt32(hours_spliting[0]);
                        int hours_spliting_bigger = Convert.ToInt32(hours_spliting[1]);

                        if (hour_app_int >= hours_spliting_lower && hour_app_int <= hours_spliting_bigger)
                        {
                            foundHour = true;
                            break; 
                        }
                    }

                    if (foundHour)
                    {
                        break; 
                    }
                }
            }

            if (!foundDay)
            {
                throw new AppointmentReservedFailedException($"The desired day of the week ({dayOfWeek}) was not found in the doctor's schedule.");
            }

            if (!foundHour)
            {
                throw new AppointmentReservedFailedException($"The desired hour ({hour_app}) was not found in the doctor's schedule.");
            }
        }

        public List<TimeSpan> appointmentTimePerDay(DateTime appoit_date)
        {
            List<TimeSpan> timeList = new List<TimeSpan>();

            List<DateTime> datetimeList = this.sQL_Appointment.GetAppointmentDateTimePerDay(appoit_date);

            foreach (DateTime datetimeValue in datetimeList)
            {
                TimeSpan time = datetimeValue.TimeOfDay;
                timeList.Add(time);
            }

            timeList.Sort();

            return timeList;
        }

        public List<Appointment> GET_APPOINTMENTS()
        {

            List<Appointment> appointments = this.sQL_Appointment.GET_APPOINTMENTS();
            appointments.Sort((a, b) => b.AppointmentDate.CompareTo(a.AppointmentDate));
            return appointments;
        }

        public void UpdateAppointment(AppointmentUpdate appointment){
            try
            {
                this.sQL_Appointment.UpdateAppointment(appointment);
            }
            catch (AppointmentReservedFailedException e)
            {
                throw new AppointmentReservedFailedException(e.Message);
            }
        }

        public void UpdateAppointment_Admin(Appointment appointment)
        {
            try
            {
                this.sQL_Appointment.UpdateAppointment_Admin(appointment);
            }
            catch (AppointmentReservedFailedException e)
            {
                throw new AppointmentReservedFailedException(e.Message);
            }
        }

        public void CancelAppointment(int id)
        {
            try
            {
                this.sQL_Appointment.CancelAppointment(id);
            }
            catch (AppointmentCancelFailedException e)
            {
                throw new AppointmentCancelFailedException(e.Message);
            }
        }

        public List<Appointment> GET_APPOINTMENTS_Patient(string phoneNumber)
        {

            List<Appointment> appointments = this.sQL_Appointment.GET_APPOINTMENTS_Patient(phoneNumber);
            appointments.Sort((a, b) => b.AppointmentDate.CompareTo(a.AppointmentDate));
            return appointments;
        }

        public List<Appointment> GET_APPOINTMENTS_Doctor(string phoneNumber)
        {
            List<Appointment> appointments = this.sQL_Appointment.GET_APPOINTMENTS_Doctor(phoneNumber);
            appointments.Sort((a, b) => b.AppointmentDate.CompareTo(a.AppointmentDate));
            return appointments;
        }

        public Appointment GetAppointmentById(int appointmentId)
        {
            return sQL_Appointment.GET_APPOINTMENTS_By_ID(appointmentId);
        }

        public void DeleteAppointmentById(int appointmentId)
        {
            try
            {
                sQL_Appointment.DeleteAppointmentById(appointmentId);
            }
            catch (AppointmentDeleteFailedException e)
            {
                
                throw new AppointmentDeleteFailedException(e.Message);
            }
        }
    }
}
