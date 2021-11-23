using IWshRuntimeLibrary;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using File = System.IO.File;

namespace StartupExplorer
{
    /// <summary>
    /// GroupData for the start menu folders.
    /// </summary>
    class GroupDataFolder : GroupData
    {
        public GroupDataFolder(StartupGroup group, string path, string approvedPath, RegistryHive regHive, RegistryView regView, bool requiresAdminPrivileges) : base(group, path, approvedPath, regHive, regView, requiresAdminPrivileges)
        { }
        

        public override StartupApplicationData[] GetApplications()
        {
            List<FileInfo> files = new List<FileInfo>();
            DirectoryInfo dir = new DirectoryInfo(path);

            //Enumerate files in directory and remove "desktop.ini"
            files.AddRange(dir.EnumerateFiles());
            for (int i = 0; i < files.Count; i++)
            {
                if (files[i].Name.Equals("desktop.ini"))
                {
                    files.RemoveAt(i);
                    break;
                }
            }

            //Initialize array
            StartupApplicationData[] applicationData = new StartupApplicationData[files.Count];
            for (int i = 0; i < files.Count; i++)
            {
                //Get application state
                AppStartupState state = GetApplicationState(files[i].Name);
                StartupApplicationData appData = new StartupApplicationData(files[i].Name, files[i].FullName, files[i].DirectoryName, group, state, requiresAdminPrivileges);
                applicationData[i] = appData;
            }

            return applicationData;
        }

        // Creates a shortcut called `appName`.lnk at `appPath` directory.
        public override void AddAppToStartup(string appName, string appPath)
        {
            WshShell shellClass = new WshShell();
            string link = this.path + "\\" + appName + ".lnk";
            IWshShortcut shortcut = (IWshShortcut)shellClass.CreateShortcut(link);
            shortcut.TargetPath = appPath;
            shortcut.IconLocation = appPath;
            shortcut.Save();
        }


        // Deletes the shortcut.
        public override void RemoveAppFromStartup(string appPath)
        {
            if (File.Exists(appPath))
                File.Delete(appPath);
        }
    }
}
