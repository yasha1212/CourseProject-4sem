using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    [Serializable]
    public class DestinationPackage : Package
    {
        public Point MouseCoordinates { get; private set; }
        public Rectangle Bounds { get; private set; }

        public DestinationPackage(Point mouseCoords, Rectangle bounds, PackageType type) : base(type)
        {
            MouseCoordinates = mouseCoords;
            Bounds = bounds;
        }
    }
}
