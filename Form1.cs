using System;
using System.Diagnostics;
using System.Media;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _2pacalypse
{
    public partial class Form1 : Form
    {
        private SoundPlayer _soundPlayer;
        private bool _isPinging;

        public Form1()
        {
            InitializeComponent();
            _isPinging = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Randomize the label text
            Random rng = new Random();
            label5.Text += rng.Next(15, 75);

            // Set up the background image for label6
            label6.Visible = false;
            label6.Refresh();
            Application.DoEvents();
            Rectangle screenRectangle = RectangleToScreen(ClientRectangle);
            int titleHeight = screenRectangle.Top - Top;
            int Right = screenRectangle.Left - Left;
            Bitmap bmp = new Bitmap(Width, Height);
            DrawToBitmap(bmp, new Rectangle(0, 0, Width, Height));
            Bitmap bmpImage = new Bitmap(bmp);
            bmp = bmpImage.Clone(new Rectangle(label6.Location.X + Right, label6.Location.Y + titleHeight, label6.Width, label6.Height), bmpImage.PixelFormat);
            label6.BackgroundImage = bmp;
            label6.Visible = true;

            // Play the sound loop
            System.IO.Stream _2pacstream = Properties.Resources.keygen;
            _soundPlayer = new SoundPlayer(_2pacstream);
            _soundPlayer.PlayLooping();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (_isPinging)
            {
                MessageBox.Show("Ping is already running!", "2pacalypse", MessageBoxButtons.OK);
                return;
            }

            Regex ipRegex = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");
            MatchCollection ipMatches = ipRegex.Matches(textBox1.Text);

            if (!int.TryParse(textBox2.Text, out int port) || port < 1 || port > 65535)
            {
                MessageBox.Show("Invalid port number! Port must be between 1 and 65535.", "2pacalypse", MessageBoxButtons.OK);
                return;
            }

            if (ipMatches.Count != 1)
            {
                MessageBox.Show("Invalid IP address! Please enter a valid IPv4 address.", "2pacalypse", MessageBoxButtons.OK);
                return;
            }

            string targetIp = ipMatches[0].ToString();
            string targetWithPort = $"{targetIp}:{port}";

            _isPinging = true;
            button1.Enabled = false;

            await Task.Run(() =>
            {
                Ping pingSender = new Ping();
                while (_isPinging)
                {
                    try
                    {
                        PingReply reply = pingSender.Send(targetIp);
                        UpdateStatusLabel(reply, targetWithPort);
                    }
                    catch (Exception ex)
                    {
                        UpdateStatusLabel($"Error: {ex.Message}");
                    }
                }
            });

            button1.Enabled = true;
        }

        private void UpdateStatusLabel(PingReply reply, string targetWithPort)
        {
            label8.BeginInvoke((Action)(() =>
            {
                string statusText;
                if (reply.Status == IPStatus.Success)
                {
                    statusText = $"/bin/cmd.exe $ status: {reply.Status} | roundtrip time: {reply.RoundtripTime}ms | TTL: {reply.Options.Ttl} | size: {reply.Buffer.Length} bytes\n";
                }
                else
                {
                    statusText = $"/bin/cmd.exe $ status: {reply.Status} | host: {targetWithPort}\n";
                }

                label8.Text = label8.Text.Length + statusText.Length <= 1337 ? label8.Text + statusText : statusText;
            }));
        }

        private void UpdateStatusLabel(string errorMessage)
        {
            label8.BeginInvoke((Action)(() =>
            {
                label8.Text = $"/bin/cmd.exe $ error: {errorMessage}\n";
            }));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Pause the sound
            _soundPlayer.Stop();
            button3.Visible = false;
            button2.Visible = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Resume the sound
            _soundPlayer.PlayLooping();
            button2.Visible = false;
            button3.Visible = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Stop pinging
            _isPinging = false;
            button1.Enabled = true;
        }
    }
}