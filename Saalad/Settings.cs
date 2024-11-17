using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Saalad
{
    public partial class Settings : UserControl
    {
        private Form1 main;
        private Scripting scriptinguc;

        [DllImport("gdi32.dll", SetLastError = true)]
        static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        private void round(Control control, int radius)
        {
            IntPtr ptr = CreateRoundRectRgn(0, 0, control.Width, control.Height, radius, radius);
            control.Region = System.Drawing.Region.FromHrgn(ptr);
        }

        private Timer webviewchecktimer;

        public Settings(Form1 mainform, Scripting scripting)
        {
            InitializeComponent();
            main = mainform;
            scriptinguc = scripting;
            round(panel2, 10);
            round(panel1, 10);
            round(panel3, 10);
            round(panel4, 10);
            round(panel7, 10);
            round(panel8, 10);
            round(panel6, 10);
            round(panel5, 10);
            siticoneToggleSwitch1.CheckedChanged += new EventHandler(siticoneToggleSwitch1_CheckedChanged);
            siticoneToggleSwitch2.CheckedChanged += new EventHandler(siticoneToggleSwitch2_CheckedChanged);
            siticoneToggleSwitch3.CheckedChanged += new EventHandler(siticoneToggleSwitch3_CheckedChanged);
            siticoneToggleSwitch4.CheckedChanged += new EventHandler(siticoneToggleSwitch4_CheckedChanged);
            Setupwebviewchecktimer();
        }

        private void Setupwebviewchecktimer()
        {
            webviewchecktimer = new Timer();
            webviewchecktimer.Interval = 2000;
            webviewchecktimer.Tick += async (s, e) => await ApplySettingsToAllWebViews();
            webviewchecktimer.Start();
        }

        private async Task ApplySettingsToAllWebViews()
        {
            try
            {
                if (scriptinguc?.GetAllWebViews() != null)
                {
                    foreach (var webView in scriptinguc.GetAllWebViews())
                    {
                        if (webView?.CoreWebView2 != null)
                        {
                            if (siticoneToggleSwitch3.Checked)
                            {
                                await webView.CoreWebView2.ExecuteScriptAsync("ShowMinimap()");
                            }
                            else
                            {
                                await webView.CoreWebView2.ExecuteScriptAsync("HideMinimap()");
                            }

                            if (siticoneToggleSwitch2.Checked)
                            {
                                await webView.CoreWebView2.ExecuteScriptAsync("enableAntiSkid()");
                            }
                            else
                            {
                                await webView.CoreWebView2.ExecuteScriptAsync("disableAntiSkid()");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"err applying settings to all WebViews: {ex.Message}");
            }
        }



        private void panel2_Paint(object sender, PaintEventArgs e)
        {
        }

        private void siticoneToggleSwitch1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                main.TopMost = siticoneToggleSwitch1.Checked;
                UpdateSettingFile("TopMost.txt", siticoneToggleSwitch1.Checked);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"err updating TopMost setting: {ex.Message}");
            }
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {
        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {
        }

        private async void Settings_Load(object sender, EventArgs e)
        {
            try
            {
                CreateSettingsFiles();

                string settingsf = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings");
                string minimapfile = System.IO.Path.Combine(settingsf, "MiniMap.txt");
                string antiskidfile = System.IO.Path.Combine(settingsf, "AntiSkid.txt");
                string topmostfile = System.IO.Path.Combine(settingsf, "TopMost.txt");
                string autoinjectfile = System.IO.Path.Combine(settingsf, "AutoInject.txt");

                bool minimapcheck = ReadSettingFile(minimapfile);
                bool antiskidcheck = ReadSettingFile(antiskidfile);
                bool topmostcheck = ReadSettingFile(topmostfile);
                bool autoinjectcheck = ReadSettingFile(autoinjectfile);

                siticoneToggleSwitch4.Checked = autoinjectcheck;
                siticoneToggleSwitch3.Checked = minimapcheck;
                siticoneToggleSwitch2.Checked = antiskidcheck;
                siticoneToggleSwitch1.Checked = topmostcheck;

                if (minimapcheck)
                {
                    await InvokeJavaScriptFunctionAsync("ShowMinimap");
                }
                else
                {
                    await InvokeJavaScriptFunctionAsync("HideMinimap");
                }

                if (antiskidcheck)
                {
                    await InvokeJavaScriptFunctionAsync("enableAntiSkid");
                }
                else
                {
                    await InvokeJavaScriptFunctionAsync("disableAntiSkid");
                }

                if (autoinjectcheck)
                {
                    ForlornApi.Api.SetAutoInject(true);
                }
                else
                {
                    ForlornApi.Api.SetAutoInject(false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"err loading settings: {ex.Message}");
            }
        }

        public async Task LoadSettingsAsync()
        {
            try
            {
                CreateSettingsFiles();

                string settingsf = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings");
                string minimapfile = System.IO.Path.Combine(settingsf, "MiniMap.txt");
                string antiskidfile = System.IO.Path.Combine(settingsf, "AntiSkid.txt");
                string topmostfile = System.IO.Path.Combine(settingsf, "TopMost.txt");
                string autoinjectfile = System.IO.Path.Combine(settingsf, "AutoInject.txt");

                bool minimapcheck = ReadSettingFile(minimapfile);
                bool antiskidcheck = ReadSettingFile(antiskidfile);
                bool topmostcheck = ReadSettingFile(topmostfile);
                bool autoinjectcheck = ReadSettingFile(autoinjectfile);

                siticoneToggleSwitch4.Checked = autoinjectcheck;
                siticoneToggleSwitch3.Checked = minimapcheck;
                siticoneToggleSwitch2.Checked = antiskidcheck;
                siticoneToggleSwitch1.Checked = topmostcheck;

                if (minimapcheck)
                {
                    await InvokeJavaScriptFunctionAsync("ShowMinimap");
                }
                else
                {
                    await InvokeJavaScriptFunctionAsync("HideMinimap");
                }

                if (antiskidcheck)
                {
                    await InvokeJavaScriptFunctionAsync("enableAntiSkid");
                }
                else
                {
                    await InvokeJavaScriptFunctionAsync("disableAntiSkid");
                }

                if (autoinjectcheck)
                {
                    ForlornApi.Api.SetAutoInject(true);
                }
                else
                {
                    ForlornApi.Api.SetAutoInject(false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"err loading settings: {ex.Message}");
            }
        }


        private async Task InvokeJavaScriptFunctionAsync(string fn)
        {
            try
            {
                if (scriptinguc == null)
                    return;

                foreach (var vw in scriptinguc.GetAllWebViews())
                {
                    if (vw.CoreWebView2 != null)
                    {
                        await vw.CoreWebView2.ExecuteScriptAsync($"{fn}()");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"err invoking JavaScript function '{fn}': {ex.Message}");
            }
        }

        private async void siticoneToggleSwitch2_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (siticoneToggleSwitch2.Checked)
                {
                    await InvokeJavaScriptFunctionAsync("enableAntiSkid");
                }
                else
                {
                    await InvokeJavaScriptFunctionAsync("disableAntiSkid");
                }
                UpdateSettingFile("AntiSkid.txt", siticoneToggleSwitch2.Checked);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"err updating AntiSkid setting: {ex.Message}");
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {
        }

        private void label8_Click(object sender, EventArgs e)
        {
        }

        private void panel8_Paint(object sender, PaintEventArgs e)
        {
        }

        private void panel7_Paint(object sender, PaintEventArgs e)
        {
        }

        private void panel6_Paint_1(object sender, PaintEventArgs e)
        {
        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {
        }

        private async void siticoneToggleSwitch3_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (siticoneToggleSwitch3.Checked)
                {
                    await InvokeJavaScriptFunctionAsync("ShowMinimap");
                }
                else
                {
                    await InvokeJavaScriptFunctionAsync("HideMinimap");
                }
                UpdateSettingFile("MiniMap.txt", siticoneToggleSwitch3.Checked);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"err updating MiniMap setting: {ex.Message}");
            }
        }

        private void CreateSettingsFiles()
        {
            try
            {
                string settingsf = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings");
                string minimapfile = System.IO.Path.Combine(settingsf, "MiniMap.txt");
                string antiskidfile = System.IO.Path.Combine(settingsf, "AntiSkid.txt");
                string topmostfile = System.IO.Path.Combine(settingsf, "TopMost.txt");
                string autoinjectfile = System.IO.Path.Combine(settingsf, "AutoInject.txt");
                string disableanimsfile = System.IO.Path.Combine(settingsf, "DisableAnimations.txt");
                if (!System.IO.Directory.Exists(settingsf))
                {
                    System.IO.Directory.CreateDirectory(settingsf);
                }
                if (!System.IO.File.Exists(minimapfile))
                {
                    System.IO.File.WriteAllText(minimapfile, "true");
                }
                if (!System.IO.File.Exists(antiskidfile))
                {
                    System.IO.File.WriteAllText(antiskidfile, "false");
                }
                if (!System.IO.File.Exists(topmostfile))
                {
                    System.IO.File.WriteAllText(topmostfile, "false");
                }
                if (!System.IO.File.Exists(autoinjectfile))
                {
                    System.IO.File.WriteAllText(autoinjectfile, "false");
                }
                if (!System.IO.File.Exists(disableanimsfile))
                {
                    System.IO.File.WriteAllText(disableanimsfile, "false");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"err creating settings files: {ex.Message}");
            }
        }

        private bool ReadSettingFile(string fp)
        {
            try
            {
                if (System.IO.File.Exists(fp))
                {
                    string content = System.IO.File.ReadAllText(fp).Trim();
                    return content.Equals("true", StringComparison.OrdinalIgnoreCase);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"err reading setting file '{fp}': {ex.Message}");
            }
            return false;
        }

        private void UpdateSettingFile(string fileName, bool value)
        {
            try
            {
                string settingsf = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings");
                string fp = System.IO.Path.Combine(settingsf, fileName);
                System.IO.File.WriteAllText(fp, value ? "true" : "false");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"err updating setting file '{fileName}': {ex.Message}");
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void siticoneToggleSwitch4_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (siticoneToggleSwitch4.Checked)
                {
                    ForlornApi.Api.SetAutoInject(true);
                }
                else
                {
                    ForlornApi.Api.SetAutoInject(false);
                }
                UpdateSettingFile("AutoInject.txt", siticoneToggleSwitch4.Checked);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"err updating AutoInject setting: {ex.Message}");
            }
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void panel10_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}