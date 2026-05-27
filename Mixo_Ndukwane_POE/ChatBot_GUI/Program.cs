using System;
using System.Windows.Forms;

namespace ChatBot_GUI
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // Catch any unhandled UI-thread exception and show it instead of
            // silently killing the app — this keeps the window open.
            Application.ThreadException += (s, e) =>
            {
                MessageBox.Show(
                    "An unexpected error occurred:\n\n" + e.Exception.Message +
                    "\n\nThe application will continue running.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            };

            // Also catch non-UI thread exceptions
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                var ex = e.ExceptionObject as Exception;
                MessageBox.Show(
                    "A critical error occurred:\n\n" + (ex?.Message ?? e.ExceptionObject.ToString()),
                    "Critical Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            };

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
