using MedicalCenter.Database;
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
            // check if appointment date is available
            // store appoitment
            try
            {
                this.sQL_Appointment.Reserve_Appoitment(appointment);
                // alert the patient that the appoitment has been reserved for him via sms 
                string msg_text = $"the appoitment has been reserved for you in" +
                    $" {appointment.AppointmentDate.ToString()} ";
                await this.sendSMS.Send(appointment.patient.phonenumber, msg_text);

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
    }
}
