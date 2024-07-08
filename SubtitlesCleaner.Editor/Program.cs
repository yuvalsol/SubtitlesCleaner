using System;
using System.Reflection;
using System.Windows.Forms;

namespace SubtitlesCleaner.Editor
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                // UI exceptions
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);

                // Non-UI exceptions
                AppDomain.CurrentDomain.UnhandledException += (sender, e) => UnhandledException((Exception)e.ExceptionObject);

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new SubtitlesCleanerEditorForm(args));
            }
            catch (Exception ex)
            {
                UnhandledException(ex);
            }
        }

        private static void UnhandledException(Exception ex)
        {
            try
            {
                MessageBoxHelper.Show(
                    ex.GetUnhandledExceptionErrorWithApplicationTerminationMessage(),
                    string.Format("Unhandled Error - {0} {1}",
                        Assembly.GetExecutingAssembly().GetName().Name,
                        Assembly.GetExecutingAssembly().GetName().Version.ToString(3)),
                    MessageBoxIcon.Error
                );
            }
            catch
            {
            }

            Application.Exit();
        }
    }
}
