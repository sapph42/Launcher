using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Launcher {
    static class Program {

        private static bool DoUpdate(FileInfo file) {
            try {
                string thisExe = file.Name;
                string thisFolder = file.DirectoryName;
                file.MoveTo($@"{thisFolder}\Launcher.exe.bak");
                File.Copy(Properties.Resources.CanonicalLocation, $@"{thisFolder}\{thisExe}");
                return true;
            }
            catch {
                return false;
            }
        }

        private static bool CheckForUpdate(Assembly assembly, FileInfo file) {
            string folder = file.DirectoryName;
            if (File.Exists($@"{folder}\Launcher.exe.bak")) {
                File.Delete($@"{folder}\Launcher.exe.bak");
            }
            Version version = assembly.GetName().Version;
            var canonicalVersion = FileVersionInfo.GetVersionInfo(Properties.Resources.CanonicalLocation);
            if (canonicalVersion.ProductMajorPart > version.Major) 
                return DoUpdate(file);
            if (canonicalVersion.ProductMinorPart > version.Minor)
                return DoUpdate(file);
            if (canonicalVersion.ProductBuildPart > version.Build)
                return DoUpdate(file);
            if (canonicalVersion.ProductPrivatePart > version.Revision)
                return DoUpdate(file);
            return false;
        }

        /// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
        static void Main() {
            Assembly thisAssembly = Assembly.GetExecutingAssembly();
            FileInfo thisFile = new FileInfo(thisAssembly.Location);
            string staticExe = thisFile.FullName;
            if (CheckForUpdate(thisAssembly, thisFile)) {
                MessageBox.Show("A new version has been detected.  Program will now restart.");
                Process.Start(staticExe);
                Application.Exit();
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(thisAssembly.GetName().Version.ToString()));
        }
    }
}
