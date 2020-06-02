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

        public MainForm()
        {
            InitializeComponent();

            client = ClientService.GetInstance();
            client.SetUpdateHandler(UpdateRemoteDisplay);

            DPIUtility.SetDpiAwareness();
            SetAppWindow();
        }

        private void SetAppWindow()
        {
            this.Height = (int)(this.Height * HEIGHT_SCALE);
            this.Width = (int)(this.Width * WIDTH_SCALE);

            foreach(Control elemUI in this.Controls)
            {
                elemUI.Width = (int)(elemUI.Width * WIDTH_SCALE);
                elemUI.Height = (int)(elemUI.Height * HEIGHT_SCALE);
                elemUI.Location = new Point((int)(elemUI.Location.X * WIDTH_SCALE), (int)(elemUI.Location.Y * HEIGHT_SCALE));
            }
        }

        private void UpdateRemoteDisplay(Image last)
        {
            pbScreen.Image = last;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void bConnect_Click(object sender, EventArgs e)
        {
            client.SetRemoteParams(tbRemoteIP.Text, int.Parse(tbRemotePort.Text));
            client.Connect();
        }

        private void bStart_Click(object sender, EventArgs e)
        {
            client.Port = int.Parse(tbPort.Text);

            client.Start();

            tbIP.Text = client.IP.ToString();
            tbPort.Text = client.Port.ToString();
        }
    }
}
