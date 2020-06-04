using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public static class ScreenCaptureUtility
    {
        private const int SENDING_DELAY = 10;

        public static Image CaptureDesktop()
        {
            var bounds = Screen.PrimaryScreen.Bounds;
            var bitmap = new Bitmap(bounds.Width, bounds.Height);

            using (var g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
            }

            return bitmap;
        }

        public static int GetDelay(int fps)
        {
            return (int)(1000 / fps - SENDING_DELAY);
        }

        public static (Point, Rectangle) GetRealMouseCoordinates(Point position, Rectangle bounds)
        {
            var size = Screen.PrimaryScreen.Bounds;
            var xScale = (double)(size.Width) / bounds.Width;
            var yScale = (double)(size.Height) / bounds.Height;

            var x = (int)(position.X * xScale);
            var y = (int)(position.Y * yScale);

            return (new Point(x, y), size);
        }
    }
}
