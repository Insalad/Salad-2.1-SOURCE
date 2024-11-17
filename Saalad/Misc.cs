using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Markup;

namespace Saalad
{
    public partial class Misc : UserControl
    {
        [DllImport("gdi32.dll", SetLastError = true)]
        static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        private void round(Control control, int radius)
        {
            IntPtr ptr = CreateRoundRectRgn(0, 0, control.Width, control.Height, radius, radius);
            control.Region = System.Drawing.Region.FromHrgn(ptr);
        }
        public Misc()
        {
            InitializeComponent();
            round(panel3, 10);
            round(panel4, 10);
            round(panel1, 10);
            round(panel2, 10);
            round(panel6, 10);
            round(panel5, 10);
            round(panel8, 10);
            round(panel7, 10);
            panel3.MouseEnter += new EventHandler(panel3_MouseEnter);
            panel3.MouseLeave += new EventHandler(panel3_MouseLeave);
            panel2.MouseEnter += new EventHandler(panel2_MouseEnter);
            panel2.MouseLeave += new EventHandler(panel2_MouseLeave);
            panel6.MouseEnter += new EventHandler(panel6_MouseEnter);
            panel6.MouseLeave += new EventHandler(panel6_MouseLeave);
            panel8.MouseEnter += new EventHandler(panel8_MouseEnter);
            panel8.MouseLeave += new EventHandler(panel8_MouseLeave);
            panel8.Click += new EventHandler(panel8_Click);
            panel3.Click += new EventHandler(panel3_Click);
            panel2.Click += new EventHandler(panel2_Click);
            panel6.Click += new EventHandler(panel6_Click);
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel3_MouseEnter(object sender, EventArgs e)
        {
            panel3.BackColor = Color.FromArgb(27, 27, 27);
        }

        private void panel3_MouseLeave(object sender, EventArgs e)
        {
            panel3.BackColor = Color.FromArgb(30, 30, 30);
        }

        private void panel2_MouseEnter(object sender, EventArgs e)
        {
            panel2.BackColor = Color.FromArgb(27, 27, 27);
        }

        private void panel2_MouseLeave(object sender, EventArgs e)
        {
            panel2.BackColor = Color.FromArgb(30, 30, 30);
        }

        private void panel6_MouseEnter(object sender, EventArgs e)
        {
            panel6.BackColor = Color.FromArgb(27, 27, 27);
        }

        private void panel6_MouseLeave(object sender, EventArgs e)
        {
            panel6.BackColor = Color.FromArgb(30, 30, 30);
        }

        private void panel3_Click(object sender, EventArgs e)
        {
            try
            {
                string pdir = AppDomain.CurrentDomain.BaseDirectory;
                string wp = Path.Combine(pdir, "workspace");

                if (Directory.Exists(wp))
                {
                    System.Diagnostics.Process.Start("explorer.exe", wp);
                }
                else
                {
                    MessageBox.Show("workspace folder does not exist.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"err occurred: {ex.Message}");
            }
        }


        private void panel6_Click(object sender, EventArgs e)
        {
            ForlornApi.Api.ExecuteScript("loadstring(game:HttpGet('https://raw.githubusercontent.com/unified-naming-convention/NamingStandard/main/UNCCheckEnv.lua'))()");
        }

        private async void panel2_Click(object sender, EventArgs e)
        {
            await Task.Run(() => ForlornApi.Api.ExecuteScript("loadstring(game:HttpGet('https://raw.githubusercontent.com/Insalad/ApiShit/main/Funcs'))()"));
        }

        private void panel8_MouseEnter(object sender, EventArgs e)
        {
            panel8.BackColor = Color.FromArgb(27, 27, 27);
        }

        private void panel8_MouseLeave(object sender, EventArgs e)
        {
            panel8.BackColor = Color.FromArgb(30, 30, 30);
        }

        private void panel8_Click(object sender, EventArgs e)
        {
            ForlornApi.Api.ExecuteScript("game.Players.LocalPlayer.Character.Humanoid.Health = 0");
        }


        private void Misc_Load(object sender, EventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel8_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
