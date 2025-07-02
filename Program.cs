using System.Management;
using System.Runtime.InteropServices;
using System.Security.Principal;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;

namespace RemoteAccessHelper
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            if (!IsRunningAsAdministrator())
            {
                MessageBox.Show(
                    "This application requires administrator privileges to access system network information.\n\n" +
                    "Please run as administrator and try again.",
                    "Administrator Privileges Required",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                 return;
            }
            
            SetRdpLevel(100);
            
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }

        private static bool IsRunningAsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        
        public static void SetRdpLevel(double level)
        {
            try
            {
                var controller = new CoreAudioController();
                IDevice device = controller.DefaultPlaybackDevice;

                if (device != null)
                {
                    device.SetVolumeAsync(level).Wait();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при управлении громкостью: {ex.Message}", "Ошибка аудио", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
} 
