using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    [Serializable]
    public class ConnectionResponsePackage : Package
    {
        public bool IsAllowed { get; private set; }
        public IPAddress SenderIP { get; private set; }
        public int SenderPort { get; private set; }

        public ConnectionResponsePackage(bool isAllowed, IPAddress ip, int port, PackageType type) : base(type)
        {
            IsAllowed = isAllowed;
            SenderIP = ip;
            SenderPort = port;
        }
    }
}
