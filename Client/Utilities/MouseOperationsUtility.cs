using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client.Utilities
{
    [Flags]
    public enum MouseEventFlags
    {
        LeftDown = 0x00000002,
        LeftUp = 0x00000004,
        MiddleDown = 0x00000020,
        MiddleUp = 0x00000040,
        Move = 0x00000001,
        Absolute = 0x00008000,
        RightDown = 0x00000008,
        RightUp = 0x00000010
    }

    [Serializable]
    public class MouseOperationArgs
    {
        public Point Position { get; set; }
        public MouseButtons Button { get; set; }
        public Rectangle AreaBounds { get; set; }

        public MouseOperationArgs(Point position, MouseButtons button, Rectangle bounds)
        {
            Position = position;
            Button = button;
            AreaBounds = bounds;
        }

        public MouseOperationArgs()
        {
            Position = new Point();
            Button = new MouseButtons();
            AreaBounds = new Rectangle();
        }
    }

    public static class MouseOperationsUtility
    {
        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwlags, int dx, int dy, int dwData, int dwExtraInfo);


        public static MouseOperationArgs AdaptParameters(MouseOperationArgs args)
        {
            var size = Screen.PrimaryScreen.Bounds;
            var xScale = (double)(size.Width) / args.AreaBounds.Width;
            var yScale = (double)(size.Height) / args.AreaBounds.Height;

            var x = (int)(args.Position.X * xScale);
            var y = (int)(args.Position.Y * yScale);

            return new MouseOperationArgs(new Point(x, y), args.Button, size);
        }

        public static void DoMouseEvent(MouseOperationArgs args)
        {
            args = AdaptParameters(args);

            Cursor.Position = args.Position;

            var value = GetFlags(args.Button);

            mouse_event((int)value, args.Position.X, args.Position.Y, 0, 0);
        }

        private static MouseEventFlags GetFlags(MouseButtons button)
        {
            var flags = MouseEventFlags.Move;

            switch (button)
            {
                case MouseButtons.Left:
                    flags = MouseEventFlags.LeftDown | MouseEventFlags.LeftUp;
                    break;
                case MouseButtons.Right:
                    flags = MouseEventFlags.RightDown | MouseEventFlags.RightUp;
                    break;
                case MouseButtons.Middle:
                    flags = MouseEventFlags.MiddleDown | MouseEventFlags.MiddleUp;
                    break;
            }

            return flags;
        }
    }
}
