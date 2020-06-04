using Client.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class MainForm : Form
    {
        private const double WIDTH_SCALE = 1.33043478;
        private const double HEIGHT_SCALE = 1.22878229;
        private readonly Size NORMAL_DISPLAY_SIZE;

        private ClientService client;
        private bool isFormLoading = true;

        public MainForm()
        {
            InitializeComponent();
            InitializeForm();

            client = ClientService.GetInstance();
            client.SetUpdateHandler(UpdateRemoteDisplay);
            client.SetErrorHandler(DisplayErrorMessage);
            client.SetRequestHandler(DisplayRequestBox);
            SetStandartValues();

            DPIUtility.SetDpiAwareness();
            NORMAL_DISPLAY_SIZE = pbScreen.Size;
            SetAppWindow();
        }

        private void InitializeForm()
        {
            cbFPS.SelectedItem = cbFPS.Items[0];
        }

        private void SetStandartValues()
        {
            var bounds = new Rectangle();
            bounds.Width = 817;
            bounds.Height = 458;

            client.FPS = 60;
            client.MouseParameters.AreaBounds = bounds;
        }

        private void SetAppWindow()
        {
            this.Height = (int)(this.Height * HEIGHT_SCALE);
            this.Width = (int)(this.Width * WIDTH_SCALE);
            ChangeSizes(this);
        }

        private Rectangle GetCorrectSize(Control control)
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

        private void UpdateRemoteDisplay(Image last)
        {
            pbScreen.Image = last;
        }

        private void DisplayErrorMessage(string message)
        {
            MessageBox.Show(message, "Соединение", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private bool DisplayRequestBox()
        {
            var result = MessageBox.Show("Разрешить пользователю подключение?", "Подключение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (result == DialogResult.Yes)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            client.Close();
        }

        private void bConnect_Click(object sender, EventArgs e)
        {
            try
            {
                client.SetRemoteParams(tbRemoteIP.Text, int.Parse(tbRemotePort.Text));
                client.FPS = int.Parse(cbFPS.SelectedItem.ToString());
                client.SendConnectionRequest(); 

                bConnect.Enabled = false;
                tbRemotePort.Enabled = false;
                tbRemoteIP.Enabled = false;
                this.MaximizeBox = true;
                bDisconnect.Enabled = true;
                bFullScreen.Enabled = true;
            }
            catch
            {
                client.Disconnect();
                MessageBox.Show("Данные адрес и порт не могут быть использованы для получения изображения.");
            }
        }

        private void bStart_Click(object sender, EventArgs e)
        {
            isFormLoading = false;

            if (CheckUtility.IsCorrectPort(tbPort.Text))
            {
                client.Port = int.Parse(tbPort.Text);

                try
                {
                    client.Start();

                    tbIP.Text = client.IP.ToString();
                    tbPort.Text = client.Port.ToString();

                    tbRemoteIP.Enabled = true;
                    tbRemotePort.Enabled = true;
                    bStart.Enabled = false;
                }
                catch
                {
                    client.Close();
                    MessageBox.Show("Невозможно использовать данный порт.");
                }
            }
            else
            {
                MessageBox.Show("Неправильное значение порта.");
            }
        }

        private void tbRemotePort_TextChanged(object sender, EventArgs e)
        {
            if (CheckUtility.IsCorrectAddress(tbRemoteIP.Text) && CheckUtility.IsCorrectPort(tbRemotePort.Text))
            {
                bConnect.Enabled = true;
            }
        }

        private void bDisconnect_Click(object sender, EventArgs e)
        {
            client.Disconnect();

            bDisconnect.Enabled = false;
            bFullScreen.Enabled = false;
            bConnect.Enabled = false;
            this.MaximizeBox = false;
            tbRemoteIP.Enabled = true;
            tbRemotePort.Enabled = true;
        }

        private void tbPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                bStart_Click(sender, e);
            }
        }

        private void tbRemotePort_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter) && bConnect.Enabled)
            {
                bConnect_Click(sender, e);
            }
        }

        private void bFullScreen_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Maximized;
            MainForm_SizeChanged(sender, e);
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            if (!isFormLoading)
            {
                if (WindowState == FormWindowState.Maximized)
                {
                    foreach (Control control in this.Controls)
                    {
                        if (control != pbScreen)
                        {
                            control.Visible = false;
                        }
                    }

                    pbScreen.Size = this.Size;
                }
                else if (WindowState == FormWindowState.Normal)
                {
                    foreach (Control control in this.Controls)
                    {
                        control.Visible = true;
                    }

                    pbScreen.Size = NORMAL_DISPLAY_SIZE;
                    pbScreen.Bounds = GetCorrectSize(pbScreen);
                }
            }
        }

        private Point GetCoordsOnDisplay()
        {
            const int Y_DELAY = 84;
            const int X_DELAY = 10;

            var x = Cursor.Position.X - this.Left - pbScreen.Left - X_DELAY;
            var y = Cursor.Position.Y - this.Top - pbScreen.Top - Y_DELAY;

            return new Point(x, y);
        }

        private void pbScreen_MouseClick(object sender, MouseEventArgs e)
        {
            var position = GetCoordsOnDisplay();
            var bounds = client.MouseParameters.AreaBounds;

            client.MouseParameters = new MouseOperationArgs(position, e.Button, bounds);
            client.SendMouseCoordinates();
        }
    }
}
