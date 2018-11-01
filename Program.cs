using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;


namespace SystemeDarkhast
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string[] args = Environment.GetCommandLineArgs();

            SingleInstanceApplication application = new SingleInstanceApplication();
            application.Run(args);
        }


        class SingleInstanceApplication : WindowsFormsApplicationBase
        {

            // Must call base constructor to ensure correct initial 
            // WindowsFormsApplicationBase configuration
            public SingleInstanceApplication()
            {

                // This ensures the underlying single-SDI framework is employed, 
                // and OnStartupNextInstance is fired
                this.IsSingleInstance = true;
            }

            protected override void OnCreateMainForm()
            {
                this.MainForm = new LoginForm();
            }


        }

    }
}
