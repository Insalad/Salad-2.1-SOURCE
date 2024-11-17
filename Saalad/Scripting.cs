using System;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Windows.Forms;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using Microsoft.Web.WebView2.WinForms;
using System.Drawing;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Core;
using System.Collections.Generic;

namespace Saalad
{
    public partial class Scripting : UserControl
    {
        private ToolTip notiftooltip = new ToolTip();
        public IEnumerable<WebView2> GetAllWebViews()
        {
            foreach (TabPage tp in visualStudioTabControl1.TabPages)
            {
                WebView2 vw = tp.Controls.OfType<WebView2>().FirstOrDefault();
                if (vw != null)
                {
                    yield return vw;
                }
            }
        }

        private List<WebView2> vws;
        private Form1 main;
        public List<WebView2> GetAllStoredWebViews()
        {
            return vws ?? new List<WebView2>();
        }

        private bool tabsloaded = false;
        private Timer autosavetimer;
        private WebView2 currenteditor;
        public async Task Seteditortxt(string text)
        {
            if (currenteditor == null)
            {
                currenteditor = webView21;
            }

            string escapedtxt = text.Replace("\\", "\\\\").Replace("'", "\\'").Replace("\n", "\\n").Replace("\r", "\\r");
            string script = $"editor.setValue('{escapedtxt}');";
            await currenteditor.ExecuteScriptAsync(script);
        }


        public WebView2 WebView21Control => webView21;

        [DllImport("gdi32.dll", SetLastError = true)]
        static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        private void round(Control control, int radius)
        {
            IntPtr ptr = CreateRoundRectRgn(0, 0, control.Width, control.Height, radius, radius);
            control.Region = System.Drawing.Region.FromHrgn(ptr);
        }

        Timer time = new Timer();
        private static Timer checkfiles;

        public Scripting(Form1 mainform)
        {
            InitializeComponent();
            webView21.Dock = DockStyle.Fill;
            main = mainform;
            webView21.Margin = new Padding(0);
            foreach (TabPage tabPage in visualStudioTabControl1.TabPages)
            {
                tabPage.Padding = new Padding(0);
            }

            autosavetimer = new Timer
            {
                Interval = 1000
            };
            autosavetimer.Tick += autosavetimer_Tick;
            autosavetimer.Start();

            string appdir = AppDomain.CurrentDomain.BaseDirectory;
            string editorfolder = Path.Combine(appdir, "Editor");
            string monacodir = Path.Combine(editorfolder, "Monaco.html");
            string fileuri = new Uri(monacodir).AbsoluteUri;
            webView21.Source = new Uri(fileuri);
            webView21.NavigationCompleted += WebView21_NavigationCompleted;
            visualStudioTabControl1.SelectedIndexChanged += VisualStudioTabControl1_SelectedIndexChanged;
            LoadTabsFromFiles();
            visualStudioTabControl1.MouseDoubleClick += VisualStudioTabControl1_MouseDoubleClick;
            AttachTooltipToButton(bunifuImageButton1, "Execute");
            AttachTooltipToButton(bunifuImageButton2, "Clear");
            AttachTooltipToButton(bunifuImageButton3, "Save script");
            AttachTooltipToButton(bunifuImageButton4, "Load a script to editor");
            AttachTooltipToButton(bunifuImageButton5, "Execute from file");
            AttachTooltipToButton(bunifuImageButton6, "Inject");
            round(panel7, 10);
            time.Tick += timertick;
            time.Start();
            ForlornApi.Api.InitializeForlorn();
            string scriptsf = Path.Combine(appdir, "Scripts");
            if (!Directory.Exists(scriptsf))
            {
                Directory.CreateDirectory(scriptsf);
            }
            string autoexecf = Path.Combine(appdir, "autoexec");
            string fpath = Path.Combine(autoexecf, "SaladFunctions.txt");
            string content = "-- don't delete this file if you want salad custom functions.\nloadstring(game:HttpGet(\"https://raw.githubusercontent.com/Insalad/ApiShit/refs/heads/main/Funcs\"))()";

            File.WriteAllText(fpath, content);

            checkfiles = new Timer();
            checkfiles.Interval = 1000;
            checkfiles.Start();
        }

        public bool injected = false;
        private void timertick(object sender, EventArgs e)
        {
            if (ForlornApi.Api.IsInjected())
            {
                injected = true;
            }
            else
            {
                injected = false;
            }
        }

        private ToolTip _notiftooltip;
        private Bunifu.Framework.UI.BunifuImageButton _currentbutton;

        private void AttachTooltipToButton(Bunifu.Framework.UI.BunifuImageButton button, string commentText)
        {
            if (_notiftooltip == null)
            {
                _notiftooltip = new ToolTip();
            }

            if (_currentbutton != null && _currentbutton != button)
            {
                _notiftooltip.Hide(_currentbutton);
            }

            _notiftooltip.SetToolTip(button, commentText);
            _currentbutton = button;
        }


        private void VisualStudioTabControl1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (visualStudioTabControl1.SelectedTab == null || visualStudioTabControl1.SelectedTab.Text == "Main")
            {
                return;
            }

            RenameTab(visualStudioTabControl1.SelectedTab);
        }

        private void RenameTab(TabPage tabPage)
        {
            string oldtabname = tabPage.Text;

            using (Form renamewindow = new Form())
            {
                renamewindow.Text = "Rename Tab";
                renamewindow.BackColor = Color.FromArgb(15, 15, 15);
                renamewindow.FormBorderStyle = FormBorderStyle.FixedDialog;
                renamewindow.StartPosition = FormStartPosition.CenterParent;
                renamewindow.Size = new Size(300, 150);
                renamewindow.MaximizeBox = false;
                renamewindow.MinimizeBox = false;
                renamewindow.TopMost = true;

                Label label = new Label()
                {
                    Text = "Enter new tab name:",
                    ForeColor = Color.White,
                    AutoSize = true,
                    Location = new Point(10, 10),
                    Font = new Font("Microsoft Sans Serif", 12, FontStyle.Regular) 
                };
                renamewindow.Controls.Add(label);
                TextBox txtbox = new TextBox()
                {
                    Text = oldtabname,
                    ForeColor = Color.White, 
                    BackColor = Color.FromArgb(30, 30, 30),  
                    Location = new Point(10, 40),
                    Width = 260,
                    Font = new Font("Microsoft Sans Serif", 10, FontStyle.Regular), 
                    BorderStyle = BorderStyle.None, 
                };
                renamewindow.Controls.Add(txtbox);

                Button okbutton = new Button()
                {
                    Text = "OK",
                    DialogResult = DialogResult.OK,
                    Location = new Point(200, 80),
                    BackColor = Color.FromArgb(30, 30, 30),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat, 
                    Font = new Font("Microsoft Sans Serif", 10, FontStyle.Regular),
                    FlatAppearance =
                    {
                       BorderSize = 0, 
                       MouseOverBackColor = Color.FromArgb(35, 35, 35), 
                       MouseDownBackColor = Color.FromArgb(25, 25, 25) 
                    }
                };
                renamewindow.Controls.Add(okbutton);
                renamewindow.AcceptButton = okbutton;

                if (renamewindow.ShowDialog() == DialogResult.OK)
                {
                    string newtabname = txtbox.Text;

                    if (string.IsNullOrWhiteSpace(newtabname) || newtabname == oldtabname || newtabname == "Main")
                    {
                        return;
                    }

                    tabPage.Text = newtabname;

                    string appdir = AppDomain.CurrentDomain.BaseDirectory;
                    string tabsdir = Path.Combine(appdir, "Tabs");

                    string oldfname = Path.GetInvalidFileNameChars().Aggregate(oldtabname, (current, c) => current.Replace(c.ToString(), "")) + ".txt";
                    string oldfpath = Path.Combine(tabsdir, oldfname);

                    string newfname = Path.GetInvalidFileNameChars().Aggregate(newtabname, (current, c) => current.Replace(c.ToString(), "")) + ".txt";
                    string newfpath = Path.Combine(tabsdir, newfname);

                    if (File.Exists(oldfpath))
                    {
                        try
                        {
                            File.Move(oldfpath, newfpath);
                            Console.WriteLine($"Renamed file from {oldfpath} to {newfpath}");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"err renaming file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }


        private async void autosavetimer_Tick(object sender, EventArgs e)
        {
            if (tabsloaded)
            {
                await SaveTabsToFiles();
            }
        }


        private async Task SaveTabsToFiles()
        {
            string appdir = AppDomain.CurrentDomain.BaseDirectory;
            string tabsdir = Path.Combine(appdir, "Tabs");

            if (!Directory.Exists(tabsdir))
            {
                Directory.CreateDirectory(tabsdir);
            }

            foreach (TabPage tp in visualStudioTabControl1.TabPages)
            {
                WebView2 vw = tp.Controls.OfType<WebView2>().FirstOrDefault();

                if (vw != null)
                {
                    string script = "GetText();";
                    string editortxt = await vw.ExecuteScriptAsync(script);
                    editortxt = System.Text.Json.JsonSerializer.Deserialize<string>(editortxt);

                    string fname = Path.GetInvalidFileNameChars()
                        .Aggregate(tp.Text, (current, c) => current.Replace(c.ToString(), ""))
                        + ".txt";
                    string fpath = Path.Combine(tabsdir, fname);

                    File.WriteAllText(fpath, editortxt);
                }
            }
        }

        private void WebView21_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
            }
        }

        private bool setthing = false;

        private async void VisualStudioTabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (visualStudioTabControl1.SelectedTab != null)
            {
                Control firstcontr = visualStudioTabControl1.SelectedTab.Controls.OfType<WebView2>().FirstOrDefault();

                if (firstcontr is WebView2 vw)
                {
                    if (vw.CoreWebView2 == null)
                    {
                        await vw.EnsureCoreWebView2Async(null);
                    }
                    currenteditor = vw;
                    if (visualStudioTabControl1.SelectedTab.Text == "Main")
                    {
                        if (!setthing)
                        {
                            string script = "GetText();";
                            string editortxt = await vw.ExecuteScriptAsync(script);
                            editortxt = System.Text.Json.JsonSerializer.Deserialize<string>(editortxt);

                            if (string.IsNullOrWhiteSpace(editortxt))
                            {
                                await InitializeAndSetEditorContent(vw, "--[[\n    Paste scripts here and press the execute button to run them\n    NOTE: SOME SCRIPTS MIGHT NOT WORK\n]]");
                            }
                            setthing = true;
                        }
                    }
                }
                else
                {
                    WebView2 neweditor = new WebView2
                    {
                        Dock = DockStyle.Fill,
                        Margin = new Padding(0),
                        Source = webView21.Source
                    };
                    neweditor.NavigationCompleted += WebView21_NavigationCompleted;
                    visualStudioTabControl1.SelectedTab.Controls.Add(neweditor);
                    await neweditor.EnsureCoreWebView2Async(null);
                    currenteditor = neweditor;
                }
            }
            else
            {
                currenteditor = webView21;
            }
        }


        private async void LoadTabsFromFiles()
        {
            string appdir = AppDomain.CurrentDomain.BaseDirectory;
            string tabsdir = Path.Combine(appdir, "Tabs");

            if (Directory.Exists(tabsdir))
            {
                var txtfiles = Directory.GetFiles(tabsdir, "*.txt");
                var tabnametoeditor = new Dictionary<string, WebView2>();
                foreach (TabPage tp in visualStudioTabControl1.TabPages)
                {
                    WebView2 vw = tp.Controls.OfType<WebView2>().FirstOrDefault();
                    if (vw != null)
                    {
                        tabnametoeditor[tp.Text] = vw;
                    }
                }

                foreach (var fpath in txtfiles)
                {
                    string tabname = Path.GetFileNameWithoutExtension(fpath);
                    string content = File.ReadAllText(fpath);
                    Console.WriteLine($"Loaded content for tab '{tabname}': {content}");
                    if (tabnametoeditor.TryGetValue(tabname, out WebView2 exwv))
                    {
                        await InitializeAndSetEditorContent(exwv, content);
                    }
                    else
                    {
                        TabPage newpage = new TabPage(tabname);
                        WebView2 neweditor = new WebView2
                        {
                            Dock = DockStyle.Fill,
                            Source = webView21.Source
                        };

                        newpage.Controls.Add(neweditor);
                        visualStudioTabControl1.TabPages.Add(newpage);
                        visualStudioTabControl1.SelectedTab = newpage;
                        await InitializeAndSetEditorContent(neweditor, content);
                        tabnametoeditor[tabname] = neweditor;
                    }
                }
                tabsloaded = true;
            }
            else
            {
                Console.WriteLine("Tabs directory not found");
            }
        }

        private async Task InitializeAndSetEditorContent(WebView2 vw, string content)
        {
            if (vw.CoreWebView2 == null)
            {
                try
                {
                    await vw.EnsureCoreWebView2Async(null);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"err initializing WebView2: {ex.Message}");
                    return;
                }
            }

            bool readycheck = false;
            int attempts = 0;
            const int maxattempts = 10;
            const int delay = 500;

            while (!readycheck && attempts < maxattempts)
            {
                readycheck = await vw.CoreWebView2.ExecuteScriptAsync("typeof editor !== 'undefined'") == "true";
                if (!readycheck)
                {
                    await Task.Delay(delay);
                    attempts++;
                }
            }

            if (!readycheck)
            {
                Console.WriteLine("Editor is not ready after initialization what the sigma");
                return;
            }

            try
            {
                string escapedtxt = content.Replace("\\", "\\\\").Replace("'", "\\'").Replace("\n", "\\n").Replace("\r", "\\r");
                string script = $"console.log('Setting content'); if (typeof editor !== 'undefined') {{ editor.setValue('{escapedtxt}'); }} else {{ console.error('Editor not defined'); }}";
                Console.WriteLine($"Executing script: {script}");
                string result = await vw.CoreWebView2.ExecuteScriptAsync(script);
                Console.WriteLine($"Script execution result: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"err executing script in WebView2: {ex.Message}");
            }
        }




        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
        }

        private void webBrowser1_DocumentCompleted_1(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
        }

        private async void bunifuImageButton1_Click(object sender, EventArgs e) // execute
        {
            if (currenteditor == null)
            {
                currenteditor = webView21;
            }

            string script = "GetText();";
            string editortxt = await currenteditor.ExecuteScriptAsync(script);
            editortxt = System.Text.Json.JsonSerializer.Deserialize<string>(editortxt);
            ForlornApi.Api.ExecuteScript(editortxt);
        }


        private async void bunifuImageButton2_Click(object sender, EventArgs e) // clear
        {
            if (currenteditor == null)
            {
                currenteditor = webView21;
            }
            string script = "editor.setValue('');";
            await currenteditor.ExecuteScriptAsync(script);
        }

        private async void bunifuImageButton3_Click(object sender, EventArgs e) // save script
        {
            if (currenteditor == null)
            {
                currenteditor = webView21;
            }
            string getshit = "GetText();";
            string editortxt = await currenteditor.ExecuteScriptAsync(getshit);
            editortxt = System.Text.Json.JsonSerializer.Deserialize<string>(editortxt);
            string script = editortxt.ToString();

            using (System.Windows.Forms.SaveFileDialog savefiuledg = new System.Windows.Forms.SaveFileDialog())
            {
                savefiuledg.Filter = "Lua files (*.lua)|*.lua|Text files (*.txt)|*.txt|All files (*.*)|*.*";
                savefiuledg.DefaultExt = "lua";
                savefiuledg.AddExtension = true;

                if (savefiuledg.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(savefiuledg.FileName, script);
                }
            }
        }


        internal class Functions
        {
            public static System.Windows.Forms.OpenFileDialog openfiledialog = new System.Windows.Forms.OpenFileDialog
            {
                Filter = "Lua files (*.lua)|*.lua|Text files (*.txt)|*.txt|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true,
                Title = "Lua Script"
            };
        }

        private async void bunifuImageButton4_Click(object sender, EventArgs e) // load script
        {
            if (currenteditor == null)
            {
                currenteditor = webView21;
            }

            if (Functions.openfiledialog.ShowDialog() == DialogResult.OK)
            {
                string maintxt = File.ReadAllText(Functions.openfiledialog.FileName);

                string escapedtxt = maintxt.Replace("\\", "\\\\")
                                             .Replace("'", "\\'")
                                             .Replace("\n", "\\n")
                                             .Replace("\r", "\\r");

                string script = $"editor.setValue('{escapedtxt}');";
                await currenteditor.ExecuteScriptAsync(script);
            }
        }

        private async void bunifuImageButton6_Click(object sender, EventArgs e) // inject
        {
            await Task.Run(() => ForlornApi.Api.Inject());
        }


        private void webView21_Click(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            TabPage newpage = new TabPage($"Tab {visualStudioTabControl1.TabCount + 1}");
            WebView2 neweditor = new WebView2
            {
                Dock = DockStyle.Fill,
                Source = webView21.Source
            };

            newpage.Controls.Add(neweditor);
            visualStudioTabControl1.TabPages.Add(newpage);
            visualStudioTabControl1.SelectedTab = newpage;
            currenteditor = neweditor;

            await InitializeAndSetEditorContent(neweditor, "--[[\n    Paste scripts here and press the execute button to run them\n    NOTE: SOME SCRIPTS MIGHT NOT WORK\n]]");
        }


        private void button4_Click(object sender, EventArgs e)
        {
            if (visualStudioTabControl1.SelectedTab != null)
            {
                if (visualStudioTabControl1.SelectedTab.Text == "Main")
                {
                    MessageBox.Show("The 'Main' tab cannot be deleted.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string tabname = visualStudioTabControl1.SelectedTab.Text;
                string appdir = AppDomain.CurrentDomain.BaseDirectory;
                string tabsdir = Path.Combine(appdir, "Tabs");
                string fn = Path.GetInvalidFileNameChars()
                    .Aggregate(tabname, (current, c) => current.Replace(c.ToString(), ""))
                    + ".txt";
                string fpath = Path.Combine(tabsdir, fn);
                if (File.Exists(fpath))
                {
                    try
                    {
                        File.Delete(fpath);
                        Console.WriteLine($"Deleted file: {fpath}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"err deleting file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                int selectedix = visualStudioTabControl1.SelectedIndex;
                visualStudioTabControl1.TabPages.RemoveAt(selectedix);

                if (visualStudioTabControl1.TabPages.Count > 0)
                {
                    int newindex = (selectedix > 0) ? selectedix - 1 : 0;
                    visualStudioTabControl1.SelectedIndex = newindex;
                    Control vwc = visualStudioTabControl1.SelectedTab.Controls.OfType<WebView2>().FirstOrDefault();
                    if (vwc != null && vwc is WebView2 vw)
                    {
                        currenteditor = vw;
                    }
                    else
                    {
                        currenteditor = webView21;
                    }
                }
                else
                {
                    currenteditor = webView21;
                }
            }
        }



        private void webView21_Click_1(object sender, EventArgs e)
        {

        }
        private void Scripting_Load(object sender, EventArgs e)
        {

        }

        private void bunifuImageButton5_Click(object sender, EventArgs e) // execute from file
        {

            if (Functions.openfiledialog.ShowDialog() == DialogResult.OK)
            {
                string maintxt = File.ReadAllText(Functions.openfiledialog.FileName);

                string escapedtxt = maintxt.Replace("\\", "\\\\")
                                             .Replace("'", "\\'")
                                             .Replace("\n", "\\n")
                                             .Replace("\r", "\\r");
                ForlornApi.Api.ExecuteScript(escapedtxt);
            }
        }

        private void panel7_Paint(object sender, PaintEventArgs e)
        {

        }

        private void bunifuImageButton7_Click(object sender, EventArgs e)
        {
            HideNotification();
        }

        private void Label_Click(object sender, EventArgs e)
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

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }
    }
}