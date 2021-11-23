using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartupExplorer
{
    /// <summary>
    /// GroupData for the registry keys.
    /// </summary>
    class GroupDataRegistry : GroupData
    {
        public GroupDataRegistry(StartupGroup group, string path, string approvedPath, RegistryHive regHive, RegistryView regView, bool requiresAdminPrivileges) : base(group, path, approvedPath, regHive, regView, requiresAdminPrivileges)
        { }


        public override StartupApplicationData[] GetApplications()
        {
            StartupApplicationData[] appDataInKey;

            //Get the key containing the applications
            using (RegistryKey key = GetKey())
            {
                string[] entries = key.GetValueNames();

                appDataInKey = new StartupApplicationData[entries.Length];


                //Loop through all values in the key
                string keyName = key.Name;
                for (int i = 0; i < entries.Length; i++)
                {
                    string entryPath = key.GetValue(entries[i]).ToString();

                    //Get state for each value in the key
                    AppStartupState state = GetApplicationState(entries[i]);

                    //Initialize object and update the array
                    StartupApplicationData appData = new StartupApplicationData(entries[i], entryPath, keyName, group, state, requiresAdminPrivileges);
                    appDataInKey[i] = appData;
                }
            }


            return appDataInKey;
        }

        public override void AddAppToStartup(string appName, string appPath)
        {
            using (RegistryKey key = GetKey(true))
                key.SetValue(appName, appPath);
        }

        public override void RemoveAppFromStartup(string appName)
        {
            using (RegistryKey key = GetKey(true)) 
                key.DeleteValue(appName, false);

            using (RegistryKey key = GetStateKey(true))
                key.DeleteValue(appName, false);
        }


        /// <summary>
        /// Gets the key containing all startup applications in this group.
        /// </summary>
        /// <param name="writable"></param>
        /// <returns></returns>
        RegistryKey GetKey(bool writable = false)
        {
            RegistryKey key;
            using (RegistryKey keyView = RegistryKey.OpenBaseKey(registryHive, registryView))
                 key = keyView.OpenSubKey(path, writable);

            return key;
        }

        
    }
}
