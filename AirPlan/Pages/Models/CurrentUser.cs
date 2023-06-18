using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirPlan.Pages.Models
{
    public static class CurrentUser
    {
        public static int ID { get; set; }
        public static int RoleID { get; set; }
        public static string Email { get; set; }
        public static string Password { get; set; }
        public static string FirstName { get; set; }
        public static string LastName { get; set; }
    }
}
