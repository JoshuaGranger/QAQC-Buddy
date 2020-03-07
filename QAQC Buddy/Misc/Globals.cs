using System;
using System.IO;
using System.Windows;

namespace QAQC_Buddy.Misc
{
    public class Globals
    {
        public static string PathExe = AppDomain.CurrentDomain.BaseDirectory;
        public static string PathFiles = Path.GetFullPath(Path.Combine(PathExe, "..", "Output Files\\"));
        public static string PathConfig = Path.GetFullPath(Path.Combine(PathExe, "..", "Configuration\\"));
        public static string PathCover = PathConfig + "Cover.xlsx";
        public static string PathGold = PathConfig + "gold.json";

        public static void ShowMsg(string text, string caption, MessageBoxButton mbbutton, MessageBoxImage mbimage)
        {
            MessageBox.Show(text, caption, mbbutton, mbimage);

            try
            {
                // Check that the folder exists
                if (!System.IO.Directory.Exists(Globals.PathFiles))
                    System.IO.Directory.CreateDirectory(Globals.PathFiles);

                // Assemble output filename and path
                string targetFile = $"log_{DateTime.Now.ToString("yyyyMMddHHmmss")}_{Environment.UserName.ToString()}.txt";
                string targetPath = PathFiles + targetFile;

                // Create new log file and add message
                if (!File.Exists(targetPath))
                {
                    using (StreamWriter sw = File.CreateText(targetPath))
                        sw.WriteLine(text);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"While trying to write the error to a logfile, another error was experienced.\n\nError:\n{e.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        public static void CheckPaths()
        {
            if (PathExe.ToLower().Contains("desktop"))
            {
                MessageBox.Show("It appears that you copied the application to the Desktop. This will not work. Please make a shortcut" +
                    " to the application instead of copying and pasting the application to your Desktop.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);

                // Shut down... somehow...
                System.Windows.Application.Current.Shutdown();
                Environment.Exit(1);
            }
        }
    }
}
