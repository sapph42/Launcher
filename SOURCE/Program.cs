using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Launcher {
    static class Program {

        /// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(Assembly.GetExecutingAssembly().GetName().Version.ToString()));
        }
    }
}
