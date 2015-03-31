using System;
using System.Threading;
using System.Windows.Forms;
using Cesto.Internal;
using Cesto.Logging;

namespace Cesto.WinForms
{
    /// <summary>
    /// Windows Forms Application Context which handles uncaught exceptions and
    /// displays useful diagnostics.
    /// </summary>
    /// <typeparam name="T">Form type for the application's main form.</typeparam>
    public class SelfHostApp<T> : ApplicationContext where T : Form, new()
    {
        private Exception _exception;

        public SelfHostApp()
        {
            try
            {
                var mainForm = new T();
                mainForm.FormClosed += MainFormOnFormClosed;
                Application.ThreadException += ApplicationOnThreadException;
                MainForm = mainForm;
            }
            catch (Exception ex)
            {
                // Exception has been thrown attempting to create the main form
                // prior to initialization of the processing loop.
                // Set non-zero exit code to indicate failure.
                Environment.ExitCode = 1;

                // If running in interactive mode, attempt to display the error form.
                if (Environment.UserInteractive)
                {
                    // This is where we rely on UnexpectedErrorForm having few
                    // dependencies and a high chance of being able to display.
                    MainForm = new UnexpectedErrorForm(ex);
                }
            }
        }

        // Called when the main form closes. If an exception has been previously thrown
        // and not caught by anything else, the error display form will show.
        private void MainFormOnFormClosed(object sender, FormClosedEventArgs args)
        {
            if (Environment.UserInteractive)
            {
                if (_exception != null)
                {
                    MainForm = new UnexpectedErrorForm(_exception);
                    MainForm.Show();
                }
            }
        }

        // Event handler for uncaught exceptions.
        private void ApplicationOnThreadException(object sender, ThreadExceptionEventArgs args)
        {
            // Only create the logger when needed, because logging gets initialized at
            // a later point in the program than most of the code in this class is run.
            var log = InternalLogManager.GetLogger(GetType());
            log.Error("Unexpected exception: " + args.Exception.Message, args.Exception);
            // remember the exception 
            _exception = args.Exception;
            MainForm.Close();
        }
    }
}
