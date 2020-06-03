using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Client.Utilities
{
    public static class CheckUtility
    {
        public static bool IsCorrectPort(string port)
        {
            var regex = new Regex(@"\d{4}");
            return regex.IsMatch(port);
        }

        public static bool IsCorrectAddress(string address)
        {
            var regex = new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}");
            return regex.IsMatch(address);
        }
    }
}
