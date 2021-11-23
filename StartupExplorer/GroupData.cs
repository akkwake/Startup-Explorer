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
    /// Contains all data required to look through a specific startup location, as well as methods that look through this group and return data about the applications in it.
    /// Has 2 subclasses, one for the start menu folders (user and common) and another for the registry keys.
    /// </summary>
    public abstract class GroupData
    {
        protected readonly StartupGroup group;
        /// <summary>
        /// This is the path to the key, or the folder that holds the path to the executable that will run on startup.
        /// </summary>
        public readonly string path;
        /// <summary>
        /// This is the path to the key that determines whether a startup application is enabled.
        /// </summary>
        public readonly string approvedPath;

        protected readonly RegistryHive registryHive;
        protected readonly RegistryView registryView;

        /// <summary>
        /// If this group requires admin privileges to modify, this should be true.
        /// </summary>
        public readonly bool requiresAdminPrivileges;


        public GroupData(StartupGroup group, string path, string approvedPath, RegistryHive regHive, RegistryView regView, bool requiresAdminPrivileges)
        {
            this.group = group;
            this.path = path;
            this.approvedPath = approvedPath;
            this.registryHive = regHive;
            this.registryView = regView;
            this.requiresAdminPrivileges = requiresAdminPrivileges;
        }


        /// <summary>
        /// Returns all relevant application data in this specific group.
        /// </summary>
        /// <returns></returns>
        public abstract StartupApplicationData[] GetApplications();

        /// <summary>
        /// Adds a value to the corresponding registry key or creates a shortcut in the corresponding startup folder.
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="appPath"></param>
        public abstract void AddAppToStartup(string appName, string appPath);

        /// <summary>
        /// Removes a value from the corresponding registry key or deletes a shortcut in the corresponding startup folder.
        /// </summary>
        /// <param name="appName"></param>
        public abstract void RemoveAppFromStartup(string appName);

        /// <summary>
        /// Sets the state of a startup application that belongs to this specific group.
        /// Setting to `NotFound` deletes the value from the `startup approved` key, and WILL run on startup.
        /// Setting to `Disabled` sets the key value to `Database.bDisabled`. The app will remain in the registry but will be flagged to NOT run on startup.
        /// Setting to `Enable` sets the key value to `Database.bEnabled`.
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="newState"></param>
        public void SetAppState(string appName, AppStartupState newState)
        {
            RegistryKey key = GetStateKey(true);
            if (key == null)
                return;

            switch (newState)
            {
                case AppStartupState.NotFound:
                    key.DeleteValue(appName, false);
                    break;
                case AppStartupState.Disabled:
                    key.DeleteValue(appName, false);
                    key.SetValue(appName, Database.bDisabled);
                    break;
                case AppStartupState.Enabled:
                    key.DeleteValue(appName, false);
                    key.SetValue(appName, Database.bEnabled);
                    break;
            }
            key.Close();
        }

        /// <summary>
        /// Returns the key that determines whether the applications in this group are enabled or disabled.
        /// </summary>
        /// <param name="writable"></param>
        /// <returns></returns>
        protected RegistryKey GetStateKey(bool writable)
        {
            RegistryKey stateKey;
            
            using (RegistryKey keyView = RegistryKey.OpenBaseKey(registryHive, registryView))
                stateKey = keyView.OpenSubKey(approvedPath, writable);

            return stateKey;
        }

        /// <summary>
        /// Looks in the `approvedPath` key. Returns `NotFound` if there's no value for the application. Otherwise it's pretty self-explanatory.
        /// </summary>
        /// <param name="appName"></param>
        /// <returns></returns>
        protected AppStartupState GetApplicationState(string appName)
        {
            //Get approvedPath key
            RegistryKey key = GetStateKey(false);
            AppStartupState state = AppStartupState.NotFound;

            if (key == null)
                return state;

            //Get all values names in the key
            string[] stateEntries = key.GetValueNames();

            for (int i = 0; i < stateEntries.Length; i++)
            {
                //Find the `appName` value
                if (appName.Equals(stateEntries[i]))
                {
                    //Set state
                    if (Database.bEnabled.SequenceEqual((byte[])(key.GetValue(stateEntries[i]))))
                    {
                        state = AppStartupState.Enabled;
                    }
                    else
                    {
                        state = AppStartupState.Disabled;
                    }
                    break;
                }
            }
            key.Close();
            return state;
        }
    }
}
