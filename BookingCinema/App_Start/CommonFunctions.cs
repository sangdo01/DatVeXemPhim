using System.Web;
using System.Web.Optimization;

namespace BookingCinema
{
    public  class CommonFunctions
    {
        public static bool IsValidate()
        {
            return true;
        }
    }

    public enum Status { Success = 0, Errors = 1, Warning = 2, Info = 3 }
}
