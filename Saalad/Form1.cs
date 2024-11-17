using Bunifu.Framework.UI;
using Saalad.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Saalad
{
    public partial class Form1 : Form
    {
        private Settings settinsuc;
        private Scripting scriptinuc;
        [DllImport("gdi32.dll", SetLastError = true)]
        static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        private Timer animtimer;
        private Timer reverseanimtimer;
        private float progress = 0f;
        private int totframe;

        private Scripting scripting;
        private ArtificialIntelligence ai;
        private Settings settings;
        private Credits credits;
        private ScriptHub scripthub;
        private Misc misc;
        private ScriptsUC scripts;

        private Point scriptingloc = new Point(-1, 52);
        private Point scripthubloc = new Point(-1, 99);
        private Point miscloc = new Point(-1, 141);
        private Point settingsloc = new Point(-1, 344);
        private Point scriptsloc = new Point(-1, 188);
        private Point ailoc = new Point(-1, 235);
        private Timer statuscheckinject;
        public Form1()
        {
            InitializeComponent();
            settinsuc = new Settings(this, scriptinuc);
            this.Controls.Add(settinsuc);
            settinsuc.Dock = DockStyle.Fill;
            InitializeSettings();
            customthings();
            round(panel1, 10);
            round(panel2, 10);
            round(panel3, 10);
            round(panel6, 3);
            round(bunifuImageButton3, 10);
            round(bunifuImageButton6, 10);
            round(bunifuImageButton5, 10);
            round(bunifuImageButton4, 6);
            round(this, 20);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(0, 0);

            animtimer = new Timer();
            animtimer.Interval = 1000 / 120;
            totframe = (int)(200f / animtimer.Interval);
            animtimer.Tick += anim;

            reverseanimtimer = new Timer();
            reverseanimtimer.Interval = 1000 / 120;
            reverseanimtimer.Tick += reverse;
            InitializeButtonLocations();

            statuscheckinject = new Timer();
            statuscheckinject.Interval = 1000;
            statuscheckinject.Tick += new EventHandler(CheckInjectionStatus);
            statuscheckinject.Start();
        }

        private void CheckInjectionStatus(object sender, EventArgs e)
        {
            if (scripting.injected)
            {
                pictureBox1.Visible = true;  
            }
            else
            {
                pictureBox1.Visible = false;
            }
        }

        private async void InitializeSettings()
        {
            if (settinsuc != null)
            {
                await settinsuc.LoadSettingsAsync();
            }
        }

        private Timer currentanimtimer;
        private void PanelAnimShit(Point targetloc)
        {
            if (currentanimtimer != null)
            {
                currentanimtimer.Stop();
                currentanimtimer.Dispose();
            }

            currentanimtimer = new Timer();
            currentanimtimer.Interval = 5;

            Point start = panel6.Location;
            int distanceX = targetloc.X - start.X;
            int distanceY = targetloc.Y - start.Y;

            currentanimtimer.Tick += (s, e) =>
            {
                int stepX = distanceX / 10;
                int stepY = distanceY / 10;
                panel6.Location = new Point(panel6.Location.X + stepX, panel6.Location.Y + stepY);
                if ((distanceX > 0 && panel6.Location.X >= targetloc.X) ||
                    (distanceX < 0 && panel6.Location.X <= targetloc.X))
                {
                    panel6.Location = new Point(targetloc.X, panel6.Location.Y);
                }

                if ((distanceY > 0 && panel6.Location.Y >= targetloc.Y) ||
                    (distanceY < 0 && panel6.Location.Y <= targetloc.Y))
                {
                    panel6.Location = new Point(panel6.Location.X, targetloc.Y);
                }
                if (panel6.Location == targetloc)
                {
                    currentanimtimer.Stop();
                    currentanimtimer.Dispose();
                }
            };

            currentanimtimer.Start();
        }

        private void customthings()
        {
            scripting = new Scripting(this)
            {
                Location = new Point(44, 36),
                Size = new Size(642, 344),
                Visible = false
            };
            this.Controls.Add(scripting);
            settings = new Settings(this, scripting)
            {
                Location = new Point(45, 37),
                Size = new Size(642, 344),
                Visible = false
            };
            this.Controls.Add(settings);
            credits = new Credits
            {
                Location = new Point(45, 37),
                Size = new Size(642, 344),
                Visible = false
            };
            this.Controls.Add(credits);
            scripthub = new ScriptHub(scripting)
            {
                Location = new Point(45, 37),
                Size = new Size(642, 344),
                Visible = false
            };
            this.Controls.Add(scripthub);
            misc = new Misc
            {
                Location = new Point(45, 37),
                Size = new Size(642, 344),
                Visible = false
            };
            this.Controls.Add(misc);
            scripts = new ScriptsUC(scripting)
            {
                Location = new Point(45, 37),
                Size = new Size(642, 344),
                Visible = false
            };
            this.Controls.Add(scripts);
            ai = new ArtificialIntelligence
            {
                Location = new Point(45, 37),
                Size = new Size(642, 344),
                Visible = false
            };
            this.Controls.Add(ai);
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            string appdir = AppDomain.CurrentDomain.BaseDirectory;
            string scriptsfpath = Path.Combine(appdir, "Scripts");
            if (!Directory.Exists(scriptsfpath))
            {
                Directory.CreateDirectory(scriptsfpath);
            }
            if (scripting != null)
            {
                scripting.BringToFront();
                scripting.Visible = true;
            }
            animtimer.Start();

            await Task.Delay(400);

            if (scripting != null && settings != null)
            {
                scripting.Visible = false;
                settings.BringToFront();
                settings.Visible = true;
            }

            await Task.Delay(400);

            if (scripting != null && settings != null)
            {
                settings.Visible = false;
                scripting.BringToFront();
                scripting.Visible = true;
            }
        }

        private void anim(object sender, EventArgs e)
        {
            progress += 1f / totframe;
            if (progress >= 1f)
            {
                progress = 1f;
                animtimer.Stop();
            }

            int width = (int)(progress * 697);
            int height = (int)(progress * 382);

            int x = (Screen.PrimaryScreen.WorkingArea.Width - width) / 2;
            int y = (Screen.PrimaryScreen.WorkingArea.Height - height) / 2;

            this.Size = new Size(width, height);
            this.Location = new Point(x, y);
        }

        private void reverse(object sender, EventArgs e)
        {
            progress -= 1f / totframe;
            if (progress <= 0f)
            {
                progress = 0f;
                reverseanimtimer.Stop();
            }

            int width = (int)(progress * 697);
            int height = (int)(progress * 382);

            int x = (Screen.PrimaryScreen.WorkingArea.Width - width) / 2;
            int y = (Screen.PrimaryScreen.WorkingArea.Height - height) / 2;

            this.Size = new Size(width, height);
            this.Location = new Point(x, y);
        }

        private void round(Control control, int radius)
        {
            IntPtr ptr = CreateRoundRectRgn(0, 0, control.Width, control.Height, radius, radius);
            control.Region = System.Drawing.Region.FromHrgn(ptr);
        }
        private void closeanim()
        {
            progress = 1f;
            animtimer.Stop();
            reverseanimtimer.Start();
            reverseanimtimer.Tick += (s, e) =>
            {
                if (progress <= 0f)
                {
                    this.Close();
                }
            };
        }

        private void minimizeanim()
        {
            progress = 1f;
            animtimer.Stop();
            reverseanimtimer.Start();

            reverseanimtimer.Tick += (s, e) =>
            {
                if (progress <= 0f)
                {
                    this.WindowState = FormWindowState.Minimized;
                    reverseanimtimer.Stop();
                }
            };
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_RESTORE = 0xF120;

            base.WndProc(ref m);

            if (m.Msg == WM_SYSCOMMAND && (m.WParam.ToInt32() & 0xFFF0) == SC_RESTORE)
            {
                progress = 0f;
                this.Size = new Size(697, 382);
                this.Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2,
                                          (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2);
                reverseanimtimer.Stop();
                animtimer.Start();
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            minimizeanim();
        }

        private void bunifuImageButton2_Click(object sender, EventArgs e)
        {
            closeanim();
        }

        private void SlideInAnim(Control control)
        {
            control.Visible = true;
            control.Location = new Point(this.Width, control.Location.Y);
            Timer slidetimer = new Timer();
            slidetimer.Interval = 5;
            slidetimer.Tick += (s, e) =>
            {
                if (control.Location.X > 44)
                {
                    control.Location = new Point(control.Location.X - 30, control.Location.Y);
                }
                else
                {
                    control.Location = new Point(44, control.Location.Y);
                    slidetimer.Stop();
                }
            };
            slidetimer.Start();
        }

        private bool scriptigon = true;
        private bool settinson = false;
        private bool scripthubon = false;
        private bool miscon = false;
        private bool credson = false;
        private bool scriptson = false;
        private bool aion = false;
        private void ResetAllFlags()
        {
            scriptigon = false;
            settinson = false;
            scripthubon = false;
            miscon = false;
            credson = false;
            aion = false;
            scriptson = false;
        }

        private Dictionary<BunifuImageButton, Point> selectedlocs;
        private Dictionary<BunifuImageButton, Point> unselectedlocs;
        private Dictionary<BunifuImageButton, string> buttonimagekeys = new Dictionary<BunifuImageButton, string>();

        private void InitializeButtonLocations()
        {
            selectedlocs = new Dictionary<BunifuImageButton, Point> {
            {
              bunifuImageButton3,
              new Point(6, 50)
            },
            {
              bunifuImageButton4,
              new Point(6, 97)
            },
            {
              bunifuImageButton5,
              new Point(6, 144)
            },
            {
              bunifuImageButton8,
              new Point(6, 191)
            },
            {
              bunifuImageButton9,
              new Point(6, 238)
            },
            {
              bunifuImageButton6,
              new Point(6, 344)
            }
          };

            unselectedlocs = new Dictionary<BunifuImageButton, Point> {
            {
              bunifuImageButton3,
              new Point(4, 50)
            },
            {
              bunifuImageButton4,
              new Point(4, 97)
            },
            {
              bunifuImageButton5,
              new Point(4, 144)
            },
            {
              bunifuImageButton8,
              new Point(4, 191)
            },
            {
              bunifuImageButton9,
              new Point(4, 238)
            },
            {
              bunifuImageButton6,
              new Point(4, 344)
            }
          };
        }

        private void UpdateButtonStates(BunifuImageButton selectedbutton)
        {
            foreach (var button in selectedlocs.Keys)
            {
                if (button == selectedbutton)
                {
                    button.Location = selectedlocs[button];
                }
                else
                {
                    button.Location = unselectedlocs[button];
                }
            }
        }

        private void UpdateButtonImages(BunifuImageButton selectedbutton)
        {
            buttonimagekeys = new Dictionary<BunifuImageButton, string> {
            {
              bunifuImageButton3,
              "scripting"
            },
            {
              bunifuImageButton4,
              "scripthub"
            },
            {
              bunifuImageButton5,
              "misc"
            },
            {
              bunifuImageButton8,
              "scripts"
            },
            {
              bunifuImageButton9,
              "ai"
            },
            {
              bunifuImageButton6,
              "settings"
            }
          };

            foreach (var button in buttonimagekeys.Keys)
            {
                if (button == selectedbutton)
                {
                    button.Image = (Image)Properties.Resources.ResourceManager.GetObject($"{buttonimagekeys[button]}_selected");
                }
                else
                {
                    button.Image = (Image)Properties.Resources.ResourceManager.GetObject($"{buttonimagekeys[button]}_unselected");
                }
            }
        }


        private void bunifuImageButton3_Click(object sender, EventArgs e) // scripting
        {
            if (!scriptigon)
            {
                ResetAllFlags();
                scriptigon = true;

                if (scripting != null && settings != null && credits != null && scripthub != null && misc != null && scripts != null && ai != null)
                {
                    UpdateButtonImages(bunifuImageButton3);
                    UpdateButtonStates(bunifuImageButton3);
                    panel6.Visible = true;
                    scripting.BringToFront();
                    SlideInAnim(scripting);
                    PanelAnimShit(scriptingloc);
                }
            }
        }

        private void bunifuImageButton6_Click(object sender, EventArgs e) // settings 
        {
            if (!settinson)
            {
                ResetAllFlags();
                settinson = true;

                if (scripting != null && settings != null && credits != null && scripthub != null && misc != null && scripts != null && ai != null)
                {
                    UpdateButtonImages(bunifuImageButton6);
                    UpdateButtonStates(bunifuImageButton6);
                    panel6.Visible = true;
                    settings.BringToFront();
                    SlideInAnim(settings);
                    PanelAnimShit(settingsloc);
                }
            }
        }

        private void bunifuImageButton7_Click(object sender, EventArgs e)
        {
            if (!credson)
            {
                ResetAllFlags();
                credson = true;
                foreach (var button in selectedlocs.Keys)
                {
                    button.Location = unselectedlocs[button];
                }
                foreach (var button in buttonimagekeys.Keys)
                {
                    button.Image = (Image)Properties.Resources.ResourceManager.GetObject($"{buttonimagekeys[button]}_unselected");
                }

                if (scripting != null && settings != null && credits != null && scripthub != null && misc != null && scripts != null && ai != null)
                {
                    panel6.Visible = false;
                    credits.BringToFront();
                    SlideInAnim(credits);
                }
            }
        }


        private void bunifuImageButton4_Click(object sender, EventArgs e) // scripthub
        {
            if (!scripthubon)
            {
                ResetAllFlags();
                scripthubon = true;

                if (scripting != null && settings != null && credits != null && scripthub != null && misc != null && scripts != null && ai != null)
                {

                    UpdateButtonImages(bunifuImageButton4);
                    UpdateButtonStates(bunifuImageButton4);
                    panel6.Visible = true;
                    scripthub.BringToFront();
                    SlideInAnim(scripthub);
                    PanelAnimShit(scripthubloc);
                }
            }
        }

        private void bunifuImageButton5_Click(object sender, EventArgs e) // misc
        {
            if (!miscon)
            {
                ResetAllFlags();
                miscon = true;

                if (scripting != null && settings != null && credits != null && scripthub != null && misc != null && scripts != null && ai != null)
                {
                    UpdateButtonImages(bunifuImageButton5);
                    UpdateButtonStates(bunifuImageButton5);
                    panel6.Visible = true;
                    misc.BringToFront();
                    SlideInAnim(misc);
                    PanelAnimShit(miscloc);
                }
            }
        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void bunifuImageButton8_Click(object sender, EventArgs e) // scripts
        {
            if (!scriptson)
            {
                ResetAllFlags();
                scriptson = true;

                if (scripting != null && settings != null && credits != null && scripthub != null && misc != null && scripts != null && ai != null)
                {

                    UpdateButtonImages(bunifuImageButton8);
                    UpdateButtonStates(bunifuImageButton8);
                    panel6.Visible = true;
                    scripts.BringToFront();
                    SlideInAnim(scripts);
                    PanelAnimShit(scriptsloc);
                }
            }
        }

        private void bunifuImageButton9_Click(object sender, EventArgs e) // ai
        {
            if (!aion)
            {
                ResetAllFlags();
                aion = true;

                if (scripting != null && settings != null && credits != null && scripthub != null && misc != null && scripts != null && ai != null)
                {

                    UpdateButtonImages(bunifuImageButton9);
                    UpdateButtonStates(bunifuImageButton9);
                    panel6.Visible = true;
                    ai.BringToFront();
                    SlideInAnim(ai);
                    PanelAnimShit(ailoc);
                }
            }
        }

        private void panel7_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        public class TransparentPanel : Panel
        {
            public float Transparency { get; set; } = 0.5f;

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);

                using (var brush = new SolidBrush(Color.FromArgb((int)(Transparency * 255), BackColor)))
                {
                    e.Graphics.FillRectangle(brush, ClientRectangle);
                }
            }
        }

        private void panel8_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {

        }
    }
}