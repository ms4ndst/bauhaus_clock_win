using System;
using System.Windows.Forms;

namespace BauhausScreensaver
{
    /// <summary>
    /// Main entry point for the Bauhaus Clock screensaver.
    /// Handles command-line arguments for preview, configure, and normal modes.
    /// </summary>
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length > 0)
            {
                string firstArgument = args[0].ToLower().Trim();
                string secondArgument = null;

                // Handle arguments like /c:1234567 or /p 1234567
                if (firstArgument.Length > 2)
                {
                    secondArgument = firstArgument.Substring(3);
                    firstArgument = firstArgument.Substring(0, 2);
                }
                else if (args.Length > 1)
                {
                    secondArgument = args[1];
                }

                switch (firstArgument)
                {
                    case "/c": // Configuration mode
                        ShowConfig();
                        break;

                    case "/p": // Preview mode
                        if (secondArgument != null)
                        {
                            IntPtr previewHandle = new IntPtr(long.Parse(secondArgument));
                            ShowPreview(previewHandle);
                        }
                        break;

                    case "/s": // Full screensaver mode
                        ShowScreensaver();
                        break;

                    default: // No valid argument, show screensaver
                        ShowScreensaver();
                        break;
                }
            }
            else
            {
                // No arguments, show screensaver
                ShowScreensaver();
            }
        }

        /// <summary>
        /// Shows the configuration dialog.
        /// </summary>
        static void ShowConfig()
        {
            using (BauhausSettingsForm settingsForm = new BauhausSettingsForm())
            {
                settingsForm.ShowDialog();
            }
        }

        /// <summary>
        /// Shows the screensaver in preview mode within the provided window handle.
        /// </summary>
        /// <param name="previewHandle">Handle to the preview window.</param>
        static void ShowPreview(IntPtr previewHandle)
        {
            using (ScreensaverForm screensaver = new ScreensaverForm(previewHandle))
            {
                Application.Run(screensaver);
            }
        }

        /// <summary>
        /// Shows the screensaver in full-screen mode across all monitors.
        /// </summary>
        static void ShowScreensaver()
        {
            // Show screensaver on all screens
            foreach (Screen screen in Screen.AllScreens)
            {
                ScreensaverForm screensaver = new ScreensaverForm(screen.Bounds);
                screensaver.Show();
            }
            Application.Run();
        }
    }
}
