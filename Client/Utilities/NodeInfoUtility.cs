using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Client.Utilities
{
    public static class NodeInfoUtility
    {
        public static IPAddress GetCurrentIP()
        {
            var addresses = Dns.GetHostAddresses(Dns.GetHostName());
            IPAddress currentAddress = null;

            foreach (var address in addresses)
            {
                if (address.GetAddressBytes().Length == 4)
                {
                    currentAddress = address;
                    break;
                }
            }

            return currentAddress;
        }
    }
}
