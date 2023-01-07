using System;
using System.Reflection;
using System.Windows.Forms;

namespace SubtitlesCleanerEditor
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
                MessageBox.Show(string.Format("An unhandled error occurred.{0}The application will terminate now.{0}{0}{1}{0}{0}{2}",
                    "\n",
                    ex.Message,
                    ex.GetFormattedStackTrace()
                ), string.Format("Unhandled Error - {0} {1}",
                    Assembly.GetExecutingAssembly().GetName().Name,
                    Assembly.GetExecutingAssembly().GetName().Version.ToString(3)
                ), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch
            {
            }

            Application.Exit();
        }
    }
}
