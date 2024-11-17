using System;
using System.Drawing;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Saalad
{
    public partial class ArtificialIntelligence : UserControl
    {
        private bool placeholdercheck = true;
        private static readonly HttpClient client = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30)
        };
        private const string apikey = "AIzaSyDDwbSwIft1CESC-fgPvLX3zF5Z_iFp4r8"; // dont do anything with this or explode >:( realreal ( i dont care ) 
        private const string apiendpoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash-latest:generateContent";

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        public ArtificialIntelligence()
        {
            InitializeComponent();
            SetupControls();
        }

        private void SetupControls()
        {
            richTextBox1.Text = "Write your question here and press enter so the AI can answer you.";
            richTextBox1.ForeColor = Color.Gray;

            richTextBox1.Enter += RemovePlaceholder;
            richTextBox1.Leave += SetPlaceholder;
            richTextBox1.KeyDown += richTextBox1_KeyDown;

            ApplyRoundCorners(panel1, 10);
            ApplyRoundCorners(panel2, 10);
            ApplyRoundCorners(richTextBox1, 10);
            ApplyRoundCorners(richTextBox2, 10);
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
                richTextBox1.Text = "Write your question here and press enter so the AI can answer you.";
                richTextBox1.ForeColor = Color.Gray;
                placeholdercheck = true;
            }
        }

        private async void richTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                string question = richTextBox1.Text;

                if (!placeholdercheck && !string.IsNullOrWhiteSpace(question))
                {
                    await ProcessQuestionAsync(question);
                }
            }
        }

        private async Task ProcessQuestionAsync(string question)
        {
            try
            {
                richTextBox1.ReadOnly = true;
                AppendFormattedText("User:\n", FontStyle.Bold);
                AppendTextToConversation(question + "\n");
                string answer = await GetGeminiResponseAsync(question);
                AppendFormattedText("AI:\n", FontStyle.Bold);
                AppendRichTextAnswer(answer);
            }
            catch (Exception ex)
            {
                AppendFormattedText("AI:\n", FontStyle.Bold);
                AppendTextToConversation("Error: " + ex.Message + "\n");
            }
            finally
            {
                richTextBox1.ReadOnly = false;
                richTextBox1.Text = "";
            }
        }

        private void AppendFormattedText(string text, FontStyle fontStyle)
        {
            int selectstart = richTextBox2.TextLength;
            richTextBox2.AppendText(text);
            richTextBox2.Select(selectstart, text.Length);
            richTextBox2.SelectionFont = new Font(richTextBox2.Font, fontStyle);
            richTextBox2.SelectionLength = 0;
        }

        private void AppendRichTextAnswer(string answer)
        {
            bool insidecodebl = false;
            var lines = answer.Split('\n');

            foreach (var line in lines)
            {
                if (line.StartsWith("```lua"))
                {
                    insidecodebl = true;
                    richTextBox2.SelectionFont = new Font("Consolas", 10, FontStyle.Regular);
                    AppendTextToConversation("\n");
                }
                else if (line.StartsWith("```"))
                {
                    insidecodebl = false;
                    richTextBox2.SelectionFont = richTextBox2.Font;
                    AppendTextToConversation("\n");
                }
                else if (insidecodebl)
                {
                    AppendTextToConversation(line + "\n");
                }
                else if (line.StartsWith("**") && line.EndsWith("**"))
                {
                    AppendFormattedText(line.Trim('*') + "\n", FontStyle.Bold);
                }
                else if (line.Contains("*") && !line.StartsWith("**"))
                {
                    AppendFormattedText(line + "\n", FontStyle.Italic);
                }
                else
                {
                    AppendTextToConversation(line + "\n");
                }
            }
        }

        private void AppendTextToConversation(string text)
        {
            richTextBox2.AppendText(text);
            richTextBox2.ScrollToCaret();
        }


        public async Task<string> GetGeminiResponseAsync(string text)
        {
            try
            {
                var rb = new
                {
                    contents = new[]
                    {
                new
                {
                    parts = new[]
                    {
                        new { text }
                    }
                }
            }
                };

                var json = JsonConvert.SerializeObject(rb);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var rqurl = $"{apiendpoint}?key={apikey}";
                client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
                client.DefaultRequestHeaders.Clear();
                var response = await client.PostAsync(rqurl, content);

                if (response.IsSuccessStatusCode)
                {
                    var rjson = await response.Content.ReadAsStringAsync();
                    var jsonr = JObject.Parse(rjson);
                    var candidates = jsonr["candidates"] as JArray;
                    if (candidates != null && candidates.Count > 0)
                    {
                        var candidate = candidates[0];
                        var contentobj = candidate["content"];
                        var parts = contentobj?["parts"] as JArray;
                        if (parts != null && parts.Count > 0)
                        {
                            var part = parts[0];
                            var rtext = part["text"]?.ToString();
                            return rtext ?? "No response text found.";
                        }
                    }

                    return "Invalid response format."; // if you're checking the code dis usually happens when you asked something the ai cant answer like "Let's fuck" lol
                }
                else
                {
                    return $"HTTP Error: {response.ReasonPhrase}";
                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        private void ApplyRoundCorners(Control control, int radius)
        {
            IntPtr ptr = CreateRoundRectRgn(0, 0, control.Width, control.Height, radius, radius);
            control.Region = Region.FromHrgn(ptr);
        }

        private void ArtificialIntelligence_Load(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
