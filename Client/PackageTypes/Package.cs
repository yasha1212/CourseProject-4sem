using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public enum PackageType
    {
        ConnectionRequest,
        ConnectionResponse,
        ImagePackage,
        MouseInfoPackage
    }

    [Serializable]
    public abstract class Package
    {
        public PackageType PackageType { get; private set; }

        public Package(PackageType type)
        {
            PackageType = type;
        }
    }
}
