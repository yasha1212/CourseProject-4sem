using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    [Serializable]
    public class ConnectionRequestPackage : Package
    {
        public IPAddress SenderIP { get; private set; }
        public int SenderPort { get; private set; }

        public ConnectionRequestPackage(IPAddress ip, int port, PackageType type) : base(type)
        {
            SenderIP = ip;
            SenderPort = port;
        }
    }
}
