using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartupExplorer
{
    /// <summary>
    /// A simple way to look at it is that there are a bunch of `groups` of applications running on startup.
    /// Each group has a path; either a folder, or a registry key.
    /// Each group also has a key in the registry that determines whether each application will actually run.
    /// The database contains data to look through each group separately and a dictionary to help sort through them.
    /// </summary>
    internal static class Database
    {
        /// <summary>
        /// Dictionary containing all startup group data sorted using the StartupGroup enumerator.
        /// </summary>
        public static readonly Dictionary<StartupGroup, GroupData> Group = new Dictionary<StartupGroup, GroupData>();

        public const string registry_startup_path = "Software\\Microsoft\\Windows\\CurrentVersion\\Run";
        public const string registry_startup_approved_path = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\StartupApproved\\Run";
        public const string registry_startup32_path = "Software\\WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Run";
        public const string registry_startup32_approved_path = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\StartupApproved\\Run32";
        public const string folder_startup_approved_path = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\StartupApproved\\StartupFolder";
        
        public const string registry_runonce_path = "Software\\Microsoft\\Windows\\CurrentVersion\\RunOnce";

        /// <summary>
        /// Key values in the startup approved paths, have this value when the application is set to run on startup.
        /// </summary>
        public static readonly byte[] bEnabled = new byte[]
        {
                            0x02,0x00,0x00,0x00,
                            0x00,0x00,0x00,0x00,
                            0x00,0x00,0x00,0x00,
        };

        /// <summary>
        /// Not enabled.
        /// </summary>
        public static readonly byte[] bDisabled = new byte[]
        {
                            0x03,0x00,0x00,0x00,
                            0x00,0x00,0x00,0x00,
                            0x00,0x00,0x00,0x00,
        };


        static Database()
        {
            //Create the data containers
            GroupData folder_user = new GroupDataFolder(StartupGroup.StartMenuUser,
                Environment.GetFolderPath(Environment.SpecialFolder.Startup),
                folder_startup_approved_path,
                RegistryHive.CurrentUser, RegistryView.Default,
                false);

            GroupData folder_common = new GroupDataFolder(StartupGroup.StartMenuCommon,
                Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup),
                folder_startup_approved_path,
                RegistryHive.LocalMachine, RegistryView.Registry64,
                true);

            GroupData registry_hkcu = new GroupDataRegistry(StartupGroup.HKCU,
                registry_startup_path,
                registry_startup_approved_path,
                RegistryHive.CurrentUser, RegistryView.Default,
                false);

            GroupData registry_hklm32 = new GroupDataRegistry(StartupGroup.HKLM32,
                registry_startup32_path,
                registry_startup32_approved_path,
                RegistryHive.LocalMachine, RegistryView.Registry64,
                true);

            GroupData registry_hklm64 = new GroupDataRegistry(StartupGroup.HKLM64,
                registry_startup_path,
                registry_startup_approved_path,
                RegistryHive.LocalMachine, RegistryView.Registry64,
                true);


            //Add to dictionary
            Group.Add(StartupGroup.StartMenuUser, folder_user);
            Group.Add(StartupGroup.StartMenuCommon, folder_common);
            Group.Add(StartupGroup.HKCU, registry_hkcu);
            Group.Add(StartupGroup.HKLM32, registry_hklm32);
            Group.Add(StartupGroup.HKLM64, registry_hklm64);
        }
    }
}
