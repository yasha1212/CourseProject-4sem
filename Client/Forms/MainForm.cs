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

        private ClientService client;
        private int fps;

        public MainForm()
        {
            InitializeComponent();
            InitializeForm();

            client = ClientService.GetInstance();
            client.FPS = fps;
            client.SetUpdateHandler(UpdateRemoteDisplay);
            client.SetErrorHandler(DisplayErrorMessage);

            DPIUtility.SetDpiAwareness();
            SetAppWindow();
        }

        private void InitializeForm()
        {
            cbFPS.SelectedItem = cbFPS.Items[0];
            fps = int.Parse(cbFPS.SelectedItem.ToString());
        }

        private void SetAppWindow()
        {
            this.Height = (int)(this.Height * HEIGHT_SCALE);
            this.Width = (int)(this.Width * WIDTH_SCALE);
            ChangeSizes(this);
        }

        private void ChangeSizes(Control master)
        {
            foreach (Control child in master.Controls)
            {
                child.Width = (int)(child.Width * WIDTH_SCALE);
                child.Height = (int)(child.Height * HEIGHT_SCALE);
                child.Location = new Point((int)(child.Location.X * WIDTH_SCALE), (int)(child.Location.Y * HEIGHT_SCALE));
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
                client.Connect();

                bConnect.Enabled = false;
                tbRemotePort.Enabled = false;
                tbRemoteIP.Enabled = false;
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
            tbRemoteIP.Enabled = true;
            tbRemotePort.Enabled = true;
            pbScreen.Image.Dispose();
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
    }
}
