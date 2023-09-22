using System.Security.Claims;

namespace MedicalCenter.Services
{
    public class CheckAuthority
    {

        public CheckAuthority() { }

        public static bool isAuthorize(Claim phoneNumberClaim, string phoneNumber,  Claim roleClaim, string role)
        {
            if (roleClaim.Value != null && roleClaim.Value == role)
            {
                if (phoneNumberClaim.Value != null && phoneNumberClaim.Value == phoneNumber)
                {
                    return true;
                }
                else
                {
                    return false;

                }

            }

            return false;
        }
       
    }
}
