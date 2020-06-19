using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public class MainFormModel
    {
        private const double WIDTH_SCALE = 1.33043478;
        private const double HEIGHT_SCALE = 1.22878229;
        public readonly Size NORMAL_DISPLAY_SIZE;

        public MainFormModel(Size size)
        {
            NORMAL_DISPLAY_SIZE = size;
        }

        public void SetAppWindow(Form main)
        {
            main.Height = (int)(main.Height * HEIGHT_SCALE);
            main.Width = (int)(main.Width * WIDTH_SCALE);
            ChangeSizes(main);
        }

        private PictureBox GetDisplayControl(Form container)
        {
            PictureBox result = null;

            foreach (Control control in container.Controls)
            {
                if (control.GetType() == typeof(PictureBox))
                {
                    result = control as PictureBox;
                }
            }

            return result;
        }

        public Point GetCoordsOnDisplay(Form main)
        {
            const int Y_DELAY = 84;
            const int X_DELAY = 10;

            var display = GetDisplayControl(main);

            var x = Cursor.Position.X - main.Left - display.Left - X_DELAY;
            var y = Cursor.Position.Y - main.Top - display.Top - Y_DELAY;

            return new Point(x, y);
        }

        public Rectangle GetCorrectSize(Control control)
        {
            var rectangle = new Rectangle();
            rectangle.Width = (int)(control.Width * WIDTH_SCALE);
            rectangle.Height = (int)(control.Height * HEIGHT_SCALE);
            rectangle.Location = new Point((int)(control.Location.X * WIDTH_SCALE), (int)(control.Location.Y * HEIGHT_SCALE));

            return rectangle;
        }

        private void ChangeSizes(Control master)
        {
            foreach (Control child in master.Controls)
            {
                child.Bounds = GetCorrectSize(child);
                if (child.GetType() == typeof(Panel))
                {
                    ChangeSizes(child);
                }
            }
        }
    }
}
