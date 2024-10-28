using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Windows.Forms;

namespace Launcher {
    static class Program {

        public static bool UsingDarkMode;

        private static bool DoUpdate(FileInfo file) {
            try {
                string thisExe = file.Name;
                string thisFolder = file.DirectoryName;
                file.MoveTo($@"{thisFolder}\Launcher.exe.bak");
                File.Copy($@"{Properties.Resources.CanonicalLocation}Launcher.exe", $@"{thisFolder}\{thisExe}");
                return true;
            }
            catch {
                return false;
            }
        }

        private static bool CheckForUpdate(Assembly assembly, FileInfo file) {
            if (System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName != "med.ds.osd.mil")
                return false;
            string folder = file.DirectoryName;
            if (File.Exists($@"{folder}\Launcher.exe.bak")) {
                File.Delete($@"{folder}\Launcher.exe.bak");
            }
            Version version = assembly.GetName().Version;
            try {
                var canonicalVersion = FileVersionInfo.GetVersionInfo($@"{Properties.Resources.CanonicalLocation}Launcher.exe");
                if (canonicalVersion.ProductMajorPart > version.Major)
                    return DoUpdate(file);
                if (canonicalVersion.ProductMinorPart > version.Minor)
                    return DoUpdate(file);
                if (canonicalVersion.ProductBuildPart > version.Build)
                    return DoUpdate(file);
                if (canonicalVersion.ProductPrivatePart > version.Revision)
                    return DoUpdate(file);
            } catch (FileNotFoundException) {
                Debug.WriteLine("Canonical location not available");
            }
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
            string thisFolder = thisFile.DirectoryName;
#if EMPTY
            string jsonFile = thisFolder + @"\launcher.json";
            File.Delete(jsonFile);
#endif
#if LOCAL
            if (!File.Exists($@"{thisFolder}\Microsoft.WindowsAPICodePack.Shell.dll"))
                File.Copy($@"{Properties.Resources.CanonicalLocation}Microsoft.WindowsAPICodePack.dll", $@"{thisFolder}\Microsoft.WindowsAPICodePack.dll");
            if (!File.Exists($@"{thisFolder}\Microsoft.WindowsAPICodePack.Shell.dll"))
                File.Copy($@"{Properties.Resources.CanonicalLocation}Microsoft.WindowsAPICodePack.Shell.dll", $@"{thisFolder}\Microsoft.WindowsAPICodePack.Shell.dll");
            if (FileVersionInfo.GetVersionInfo($@"{Properties.Resources.CanonicalLocation}Newtonsoft.Json.dll").ProductVersion != FileVersionInfo.GetVersionInfo($@"{thisFolder}\Newtonsoft.Json.dll").ProductVersion)
                File.Copy($@"{Properties.Resources.CanonicalLocation}Newtonsoft.Json.dll", $@"{thisFolder}\Newtonsoft.Json.dll");
            if (CheckForUpdate(thisAssembly, thisFile)) {
                MessageBox.Show("A new version has been detected.  Program will now restart.");
                Process.Start(staticExe);
                Application.Exit();
                return;
            }
#endif

            using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                       @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize")) {
                var registryValueObject = key?.GetValue("AppsUseLightTheme");
                UsingDarkMode = (int?)registryValueObject == 0;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(thisAssembly.GetName().Version.ToString()));
        }
    }
}
