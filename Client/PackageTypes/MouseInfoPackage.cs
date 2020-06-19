using Client.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    [Serializable]
    public class MouseInfoPackage : Package
    {
        public MouseOperationArgs MouseParameters { get; private set; }

        public MouseInfoPackage(MouseOperationArgs args, PackageType type) : base(type)
        {
            MouseParameters = args;
        }
    }
}
