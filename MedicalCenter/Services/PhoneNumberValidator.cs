using System.Text.RegularExpressions;

namespace MedicalCenter.Services
{
    public class PhoneNumberValidator
    {

        public static bool IsPhoneNumberValid(string phoneNumber)
        {
            // Remove any non-numeric characters from the phone number
            string numericPhoneNumber = Regex.Replace(phoneNumber, @"[^0-9+]", "");

            Console.WriteLine(numericPhoneNumber);
            // Validate the length and format of the phone number
            if (numericPhoneNumber.Length >= 8 && numericPhoneNumber.Length <= 15 &&
                numericPhoneNumber.StartsWith("+") || numericPhoneNumber.StartsWith("00"))
            {
                return true;
            }

            return false;
        }
    }
}
