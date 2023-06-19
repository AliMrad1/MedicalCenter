using System.Text.RegularExpressions;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.TwiML.Messaging;

namespace MedicalCenter.Services
{
    public class SendSMS
    {

        IConfiguration configuration;

        public SendSMS(IConfiguration configuration)
        {
            this.configuration = configuration;
        }


        public Task Send(string phoneNumber, string msg)
        {

            return Task.Run(async () =>
            {
                // Your Twilio account SID and auth token
                string accountSid = this.configuration["twilio:ACCOUNT_SID"];
                string authToken = this.configuration["twilio:AUTH_TOKEN"];
                // Initialize the Twilio client
                TwilioClient.Init(accountSid, authToken);


                // Create and send an SMS message
                var message = MessageResource.Create(
                    body: msg,
                    from: new Twilio.Types.PhoneNumber(configuration["twilio:phone_number"]),
                    to: new Twilio.Types.PhoneNumber(phoneNumber)
                );

                Console.WriteLine("SMS sent successfully. SID: " + message.Sid);

                if (message.ErrorCode.HasValue)
                {
                    throw new PhoneNumberValidationException($"Failed to send SMS. Error code: {message.ErrorCode}");
                }

            });

        }
    }
}
