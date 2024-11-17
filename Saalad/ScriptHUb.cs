using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Saalad
{
    public partial class ScriptHub : UserControl
    {
        private Scripting scriptinguc;
        private bool placeholdercheck = true;
        private Dictionary<string, string> scriptcodez = new Dictionary<string, string>();

        public ScriptHub(Scripting scripting)
        {
            InitializeComponent();
            round(richTextBox1, 10);
            round(panel1, 10);
            round(flowLayoutPanel1, 10);
            round(listBox1, 10);
            scriptinguc = scripting;
            listBox1.DrawMode = DrawMode.OwnerDrawFixed;
            listBox1.DrawItem += listBox1_DrawItem;

            richTextBox1.Text = "Enter the name of the script you want to use then press enter to search";
            richTextBox1.ForeColor = Color.Gray;
            richTextBox1.Enter += RemovePlaceholder;
            richTextBox1.Leave += SetPlaceholder;
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.SelectionLength = 0;
            round(panel7, 10);
        }

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            string itemtext = listBox1.Items[e.Index].ToString();
            Color bgcolor;
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                bgcolor = Color.FromArgb(50, 50, 50);
            }
            else
            {
                bgcolor = listBox1.BackColor;
            }
            using (SolidBrush bgbrush = new SolidBrush(bgcolor))
            {
                e.Graphics.FillRectangle(bgbrush, e.Bounds);
            }
            using (SolidBrush txtbrush = new SolidBrush(listBox1.ForeColor))
            {
                e.Graphics.DrawString(itemtext, e.Font, txtbrush, e.Bounds);
            }
        }

        private async Task FetchScripts(int page)
        {
            string searchedtxt = richTextBox1.Text;
            string modeselection = "Free";
            string url = $"https://scriptblox.com/api/script/search?q={searchedtxt}&script%20name=5&mode={modeselection}&page={page}";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    string rb = await response.Content.ReadAsStringAsync();
                    JObject json = JObject.Parse(rb);
                    JArray scripts = (JArray)json["result"]["scripts"];

                    if (page == 1)
                    {
                        listBox1.Items.Clear();
                        scriptcodez.Clear();
                    }

                    foreach (var script in scripts)
                    {
                        string scripttitle = script["title"].ToString();
                        string scriptcode = script["script"].ToString();

                        if (!scriptcodez.ContainsKey(scripttitle))
                        {
                            scriptcodez.Add(scripttitle, scriptcode);
                        }

                        listBox1.Items.Add(scripttitle);
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"rq error: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"error occurred: {ex.Message}");
            }
        }

        private void RemovePlaceholder(object sender, EventArgs e)
        {
            if (placeholdercheck)
            {
                richTextBox1.Text = "";
                richTextBox1.ForeColor = Color.White;
                placeholdercheck = false;
            }
        }

        private void SetPlaceholder(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(richTextBox1.Text))
            {
                richTextBox1.Text = "Enter the name of the script you want to use then press enter to search";
                richTextBox1.ForeColor = Color.Gray;
                placeholdercheck = true;
            }
        }

        [DllImport("gdi32.dll", SetLastError = true)]
        static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        private void round(Control control, int radius)
        {
            IntPtr ptr = CreateRoundRectRgn(0, 0, control.Width, control.Height, radius, radius);
            control.Region = System.Drawing.Region.FromHrgn(ptr);
        }

        private void ScriptHub_Load(object sender, EventArgs e)
        {
            richTextBox1.KeyDown += new KeyEventHandler(richTextBox1_KeyDown);
        }

        private async void richTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                await FetchScripts(1);
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private async void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                string selectedscripttitle = listBox1.SelectedItem.ToString();
                if (scriptcodez.TryGetValue(selectedscripttitle, out string scriptcode))
                {
                    string warningmsg = "--[[\n Warning | Some scripts might be malicious, and salad has some vulnerabilities, use this script at your own risk\n]]\n\n";
                    string fullscriptcode = warningmsg + scriptcode;
                    if (scriptinguc != null)
                    {
                        await scriptinguc.Seteditortxt(fullscriptcode);
                        ShowNotification($"Script set in editor", 3000);
                    }
                }
            }
        }


        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel7_Paint(object sender, PaintEventArgs e)
        {

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
            HideNotification();
        }
    }
}
