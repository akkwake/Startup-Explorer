using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartupExplorer
{
    /// <summary>
    /// Finds startup applications, or adds applications to any startup group.
    /// </summary>
    public static class Explorer
    {
        /// <summary>
        /// Returns all startup applications found, sorted by group.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<StartupGroup, StartupApplicationData[]> GetAll()
        {
            Dictionary<StartupGroup, StartupApplicationData[]> dict = new Dictionary<StartupGroup, StartupApplicationData[]>();

            Array groups = Enum.GetValues(typeof(StartupGroup));

            foreach (StartupGroup group in groups)
            {
                StartupApplicationData[] apps = Database.Group[group].GetApplications();
                dict.Add(group, apps);
            }

            return dict;
        }

        /// <summary>
        /// Returns all enabled startup applications found, sorted by group.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<StartupGroup, StartupApplicationData[]> GetAllEnabled()
        {
            Dictionary<StartupGroup, StartupApplicationData[]> dict = new Dictionary<StartupGroup, StartupApplicationData[]>();

            Array groups = Enum.GetValues(typeof(StartupGroup));

            List<StartupApplicationData> enabledAppsInGroup = new List<StartupApplicationData>();

            foreach (StartupGroup group in groups)
            {
                StartupApplicationData[] appsInGroup = Database.Group[group].GetApplications();
                foreach(StartupApplicationData app in appsInGroup)
                {
                    if (app.state == AppStartupState.Enabled || app.state == AppStartupState.NotFound)
                        enabledAppsInGroup.Add(app);
                }
                
                dict.Add(group, enabledAppsInGroup.ToArray());
                enabledAppsInGroup.Clear();
            }

            return dict;
        }

        /// <summary>
        /// Returns all startup applications found in a specific group.
        /// </summary>
        /// <param name="startupGroup"></param>
        /// <returns></returns>
        public static StartupApplicationData[] GetGroup(StartupGroup startupGroup)
        {
            StartupApplicationData[] appData = Database.Group[startupGroup].GetApplications();
            
            return appData;
        }


        /// <summary>
        /// Returns all enabled startup applications found in a specific group.
        /// </summary>
        /// <param name="startupGroup"></param>
        /// <returns></returns>
        public static List<StartupApplicationData> GetGroupEnabled(StartupGroup startupGroup)
        {
            StartupApplicationData[] allEntries = Database.Group[startupGroup].GetApplications();
            List<StartupApplicationData> enabledEntries = new List<StartupApplicationData>();

            foreach (StartupApplicationData app in allEntries)
            {
                if (app.state == AppStartupState.Enabled || app.state == AppStartupState.NotFound)
                    enabledEntries.Add(app);
            }

            return enabledEntries;
        }

        /// <summary>
        /// Returns the GroupData object for the specified StartupGroup.
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public static GroupData GetGroupData(StartupGroup group)
        {
            return Database.Group[group];
        }
        
        /// <summary>
        /// Adds a program to windows startup in the specified startup group. 
        /// `appName` can be whatever.
        /// `appPath` requires the executable file as well (ex. C:\Foo.exe).
        /// If the `startupGroup` chosen is one of the folders it creates a shortcut to it, otherwise it adds a value to the corresponding registry key.
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="appPath"></param>
        /// <param name="startupGroup"></param>
        public static void AddToStartup(string appName, string appPath, StartupGroup startupGroup)
        {
            Database.Group[startupGroup].AddAppToStartup(appName, appPath);
        }
    }
}
