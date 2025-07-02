using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;

namespace RemoteAccessHelper
{
    /// <summary>
    /// Main form for the Remote Access Helper application.
    /// Displays network information and triggers a harmless prank after a delay.
    /// </summary>
    public partial class MainForm : Form
    {
        private System.Windows.Forms.Timer rdpTimer;
        private ConfigurationForm? configurationForm;

        public MainForm()
        {
            InitializeComponent();
            SetupForm();
            DisplayNetworkInfo();
            StartRdpTimer();
        }

        /// <summary>
        /// Sets up the main form appearance and properties
        /// </summary>
        private void SetupForm()
        {
            this.Text = "Remote Access Helper v1.0";
            this.Size = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Icon = SystemIcons.Information;

            // Create and configure the main label
            Label infoLabel = new Label
            {
                Text = "Remote Desktop Assistant\n\nAnalyzing system configuration...",
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                AutoSize = false
            };

            this.Controls.Add(infoLabel);
        }

        /// <summary>
        /// Displays network information including local IP and RDP status
        /// </summary>
        private void DisplayNetworkInfo()
        {
            try
            {
                string localIP = GetLocalIPAddress();
                bool rdpEnabled = IsRDPEnabled();
                int rdpPort = 3389; // Default RDP port

                string statusText = $"Remote Desktop Assistant\n\n" +
                                   $"System Analysis Complete:\n\n" +
                                   $"Local IP Address: {localIP}\n" +
                                   $"RDP Port: {rdpPort}\n" +
                                   $"RDP Status: {(rdpEnabled ? "Enabled" : "Disabled")}\n\n" +
                                   $"RDP available at: {localIP}:{rdpPort}\n\n";

                if (!rdpEnabled)
                {
                    statusText += "⚠️  WARNING: Remote Desktop is disabled!\n\n" +
                                 "To enable RDP:\n" +
                                 "1. Open System Properties\n" +
                                 "2. Go to Remote tab\n" +
                                 "3. Check 'Allow remote connections'\n" +
                                 "4. Click Apply and OK\n\n";
                }

                statusText += "Initializing remote access protocols...";

                // Update the label text
                if (this.Controls.Count > 0 && this.Controls[0] is Label label)
                {
                    label.Text = statusText;
                }

                // Also display in console for debugging
                Console.WriteLine($"RDP available at: {localIP}:{rdpPort}");
                if (!rdpEnabled)
                {
                    Console.WriteLine("WARNING: RDP is disabled on this system.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error analyzing system: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Gets the local IP address of the machine
        /// </summary>
        /// <returns>Local IP address as string</returns>
        private string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "127.0.0.1"; // Fallback to localhost
        }

        /// <summary>
        /// Checks if Remote Desktop is enabled on the system
        /// </summary>
        /// <returns>True if RDP is enabled, false otherwise</returns>
        private bool IsRDPEnabled()
        {
            try
            {
                IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
                IPEndPoint[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpListeners();
                
                return tcpConnInfoArray.Any(endpoint => endpoint.Port == 3389);
            }
            catch
            {
                return true;
            }
        }

        private void StartRdpTimer()
        {
            rdpTimer = new System.Windows.Forms.Timer();
            rdpTimer.Interval = 3000; 
            rdpTimer.Tick += RdpTimer_Tick;
            rdpTimer.Start();
        }

        private void RdpTimer_Tick(object? sender, EventArgs e)
        {
            rdpTimer.Stop();
            rdpTimer.Dispose();
            
            // Hide the main form
            this.Hide();
            
            configurationForm = new ConfigurationForm();
            configurationForm.Show();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            rdpTimer?.Stop();
            rdpTimer?.Dispose();
            configurationForm?.Close();
            base.OnFormClosing(e);
        }
    }
} 