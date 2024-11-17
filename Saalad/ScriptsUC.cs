using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Saalad
{
    public partial class ScriptsUC : UserControl
    {
        private bool placeholdercheck = true;

        private Scripting scriptinguc;
        private ToolTip notiftooltip = new ToolTip();
        private string scriptspath;
        private FileSystemWatcher filewatcher;
        public ScriptsUC(Scripting scripting)
        {
            InitializeComponent();
            round(listBox2, 10);
            round(flowLayoutPanel1, 10);
            round(panel7, 10);
            listBox2.DrawMode = DrawMode.OwnerDrawFixed;
            listBox2.DrawItem += listBox2_DrawItem;
            scriptspath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scripts");
            filewatcher = new FileSystemWatcher
            {
                Path = scriptspath,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.CreationTime,
                Filter = "*.*"
            };

            filewatcher.Created += OnScriptsFolderChanged;
            filewatcher.Deleted += OnScriptsFolderChanged;
            filewatcher.Changed += OnScriptsFolderChanged;
            filewatcher.Renamed += OnScriptsFolderChanged;
            filewatcher.EnableRaisingEvents = true;
            scriptinguc = scripting;
            LoadScripts();
        }

        private void ScriptsUC_Load(object sender, EventArgs e)
        {
            LoadScripts();
        }
        private void LoadScripts()
        {
            listBox2.Items.Clear();

            if (Directory.Exists(scriptspath))
            {
                string[] files = Directory.GetFiles(scriptspath);
                foreach (string file in files)
                {
                    listBox2.Items.Add(Path.GetFileName(file));
                }
                UpdatePlaceholder();
            }
            else
            {
                ShowNotification("Scripts folder not found.", 3000);
            }
        }

        private void UpdatePlaceholder()
        {
            if (listBox2.Items.Count == 0)
            {
                if (!listBox2.Items.Contains("No saved scripts."))
                {
                    listBox2.Items.Add("No saved scripts.");
                    listBox2.ForeColor = Color.Gray;
                }
            }
            else
            {
                if (listBox2.Items.Contains("No saved scripts."))
                {
                    listBox2.Items.Remove("No saved scripts.");
                    listBox2.ForeColor = Color.Black; 
                }
            }
        }

        private async void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedItem != null && listBox2.SelectedItem.ToString() != "No saved scripts.")
            {
                string selectedfiletitle = listBox2.SelectedItem.ToString();
                string fpath = Path.Combine(scriptspath, selectedfiletitle);

                if (File.Exists(fpath))
                {
                    string fullscriptc = await Task.Run(() => File.ReadAllText(fpath));
                    await scriptinguc.Seteditortxt(fullscriptc);
                    ShowNotification($"Script set in editor", 3000);
                }
                else
                {
                    ShowNotification("Selected file not found.", 3000);
                }
            }
        }


        private void OnScriptsFolderChanged(object sender, FileSystemEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(LoadScripts));
            }
            else
            {
                LoadScripts();
            }
        }
        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void listBox2_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            string itemtxt = listBox2.Items[e.Index].ToString();
            Color bgcolor;
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                bgcolor = Color.FromArgb(50, 50, 50);
            }
            else
            {
                bgcolor = listBox2.BackColor;
            }
            using (SolidBrush bgbrush = new SolidBrush(bgcolor))
            {
                e.Graphics.FillRectangle(bgbrush, e.Bounds);
            }
            using (SolidBrush txtbrush = new SolidBrush(listBox2.ForeColor))
            {
                e.Graphics.DrawString(itemtxt, e.Font, txtbrush, e.Bounds);
            }
        }


        [DllImport("gdi32.dll", SetLastError = true)]
        static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        private void round(Control control, int radius)
        {
            IntPtr ptr = CreateRoundRectRgn(0, 0, control.Width, control.Height, radius, radius);
            control.Region = System.Drawing.Region.FromHrgn(ptr);
        }

        private Timer notiftimer;
        private Point notifstartloc = new Point(108, 600);
        private Point notifendloc = new Point(108, 259);
        private int animduration = 500;
        private bool animatingcheck = false;
        public void ShowNotification(string message, int duration)
        {
            if (!animatingcheck)
            {
                Label.Text = message;
                panel7.Location = notifstartloc;
                animatingcheck = true;
                AnimatePanel(panel7, notifstartloc, notifendloc, animduration, () =>
                {
                    notiftimer = new Timer();
                    notiftimer.Interval = duration;
                    notiftimer.Tick += (sender, args) => HideNotification();
                    notiftimer.Start();
                });
            }
        }

        private void HideNotification()
        {
            if (animatingcheck)
            {
                notiftimer?.Stop();
                var notifhide = new Point(108, 600);
                AnimatePanel(panel7, notifendloc, notifhide, animduration, () =>
                {
                    animatingcheck = false;
                });
            }
        }

        private void AnimatePanel(Panel panel, Point start, Point end, int duration, Action onComplete = null)
        {
            var timestart = DateTime.Now;
            var timer = new Timer();
            timer.Interval = 1;
            timer.Tick += (s, e) =>
            {
                double progress = (DateTime.Now - timestart).TotalMilliseconds / duration;
                if (progress >= 1)
                {
                    panel.Location = end;
                    timer.Stop();
                    onComplete?.Invoke();
                }
                else
                {
                    int newX = (int)(start.X + (end.X - start.X) * progress);
                    int newY = (int)(start.Y + (end.Y - start.Y) * progress);
                    panel.Location = new Point(newX, newY);
                }
            };
            timer.Start();
        }

        private void bunifuImageButton7_Click(object sender, EventArgs e)
        {

        }
    }
}
