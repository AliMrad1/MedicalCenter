using MedicalCenter.Database;
using MedicalCenter.Models;
using Twilio.Types;

namespace MedicalCenter.Services
{
    public class CenterService
    {
        private IConfiguration configuration;
        private DALC_SQL _SQL;
        private SendSMS sms;
        private GenerateToken generateToken;
        public CenterService(IConfiguration configuration) 
        {
            this.configuration = configuration;
            _SQL = new DALC_SQL(configuration);
            this.sms = new SendSMS(configuration);
            this.generateToken = new GenerateToken(configuration);
        }


        public async Task ADD_Patient(PatientRequest request)
        {
            try
            {

                if (PhoneNumberValidator.IsPhoneNumberValid(request.PhoneNumber))
                {
                    string encrypted_pass = EncryptPassword.HashPassword(request.Password);
                    this._SQL.AddPatient(request, encrypted_pass);
                }
                else
                {
                    throw new PhoneNumberValidationException("The PHone Number is Not Valid");
                }
            }
            catch (PatientInsertingFailedException e)
            {
                throw new PatientInsertingFailedException(e.Message);
            }
            //pass token with url to verify the phone number
            await this.sendSms(request.PhoneNumber, "Your account created successfuly , " +
                "Please try to verify your Phone Number!");

        }

        public void AddDoctor(DoctorRequest doctor)
        {
            try
            {
                string encrypted_pass = EncryptPassword.HashPassword(doctor.Password);
                this._SQL.AddDoctor(doctor, encrypted_pass);
            }
            catch (DoctorInsertingFailedException e)
            {
                throw new DoctorInsertingFailedException(e.Message);
            }
        }

        private async Task sendSms(string phoneNumber, string message)
        {
            try
            {
                await Task.Run(async () =>
                {
                    await this.sms.Send(phoneNumber, message);
                });
            }
            catch (PhoneNumberValidationException e)
            {
                throw new PhoneNumberValidationException(e.Message);
            }

        }

        public PatientLoginResponse LOGIN_PATIENT(PatientLogin request)
        {
            if (PhoneNumberValidator.IsPhoneNumberValid(request.phonenumber))
            {
                PatientLogin p = this._SQL.GetPatientPhoneNAndPass(request.phonenumber);
                if(p == null)
                {
                    throw new IncorretEmailLoginException("Incorrect Phone Number !!");
                }
                else
                {
                    if (EncryptPassword.VerifyPassword(request.password, p.password))
                    {
                        //generate a token and redirect Back to Client
                        string tokenValue = generateToken.GenerateJwtToken(request.phonenumber);
                        return new PatientLoginResponse(token: tokenValue);
                    }

                    throw new IncorrectPasswordLoginException("Incorrect Password!!");
                }
                
            }
            else
            {
                throw new PhoneNumberValidationException("The PHone Number is Not Valid");
            }

        }

    }
}
