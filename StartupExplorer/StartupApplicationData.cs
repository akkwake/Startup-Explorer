using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace StartupExplorer
{
    /// <summary>
    /// Contains data about a single application found in one of the startup groups.
    /// Has methods to change whether this specific application is enabled to run on startup, or remove it from startup entirely.
    /// </summary>
    [Serializable]
    public class StartupApplicationData
    {
        /// <summary>
        /// Display name of the shortcut or key value. Unimportant in registry entries.
        /// </summary>
        public readonly string name;

        /// <summary>
        /// The path of the executable. Includes arguments for registry entries.
        /// </summary>
        public readonly string rawPath;

        /// <summary>
        /// The path of the executable. Does NOT include arguments.
        /// </summary>
        public readonly string executablePath;

        /// <summary>
        /// Arguments for registry entries.
        /// </summary>
        public readonly string arguments;

        /// <summary>
        /// The path to the startup group.
        /// </summary>
        public readonly string groupPath;

        /// <summary>
        /// Startup group enumerator.
        /// </summary>
        public readonly StartupGroup startupGroup;

        /// <summary>
        /// Whether the startup entry corresponding to this object will run on startup.
        /// </summary>
        public AppStartupState state { get; private set; }

        /// <summary>
        /// True if this application belongs to a group that requires administrator privileges to modify.
        /// </summary>
        public readonly bool requiresAdminPrivileges;

        /// <summary>
        /// Returns true if this application is in the start menu folders.
        /// </summary>
        public bool isShortcut
        {
            get
            {
                return ((int)startupGroup < 2);
            }
        }


        public StartupApplicationData(string name, string path, string origin, StartupGroup startupGroup, AppStartupState state, bool requiresAdminPrivileges)
        {
            this.name = name;
            this.rawPath = path;
            this.groupPath = origin;
            this.startupGroup = startupGroup;
            this.state = state;
            this.requiresAdminPrivileges = requiresAdminPrivileges;

            if (!isShortcut)
                SplitPathAndArguments(out executablePath, out arguments);
            else
                executablePath = rawPath;
        }

        /// <summary>
        /// Changes the registry value determining whether this application will actually run on startup.
        /// </summary>
        /// <param name="state"></param>
        public void SetKeyState(bool state)
        {
            AppStartupState newState = state ? AppStartupState.Enabled : AppStartupState.Disabled;

            Database.Group[startupGroup].SetAppState(name, newState);
            this.state = newState;
        }

        /// <summary>
        /// Changes the registry value determining whether this application will actually run on startup.
        /// </summary>
        /// <param name="state"></param>
        public void SetKeyState(AppStartupState state)
        {
            Database.Group[startupGroup].SetAppState(name, state);
            this.state = state;
        }

        /// <summary>
        /// Removes the shortcut or registry entry corresponding to this object.
        /// </summary>
        public void RemoveFromStartup()
        {
            if (isShortcut)
            {
                //Delete shortcut
                Database.Group[startupGroup].RemoveAppFromStartup(this.rawPath);
            }
            else
            {
                //Delete key values corresponding to this object from startup and startup approved paths
                Database.Group[startupGroup].RemoveAppFromStartup(this.name);
            }

            SetKeyState(AppStartupState.NotFound);
        }


        /// <summary>
        /// Splits the application path and arguments. Every application has its own arguments format so this method is slightly convoluted.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="path"></param>
        /// <param name="args"></param>
        public static void SplitPathAndArguments(string fullPath, out string path, out string args)
        {
            //If path is surrounded by quotes
            if (fullPath.IndexOf('"') == 0)
            {
                //Remove first quote
                string tempString = fullPath.TrimStart('"');
                int index = tempString.IndexOf('"');

                //Split at second quote
                path = tempString.Substring(0, index);
                args = tempString.Substring(index + 1, tempString.Length - index - 1).Trim();
            }
            else
            {
                //Check indexof('/' and '-')
                string[] argDelimiters = new string[2] { " -", " /" };

                //If there are no arguments, no split
                int minIndex = fullPath.Length;

                //Split at the smallest index (first argument found while parsing)
                foreach (string delim in argDelimiters)
                {
                    int index = fullPath.IndexOf(delim);
                    if (index != -1)
                        minIndex = Math.Min(index, minIndex);
                }

                path = fullPath.Substring(0, minIndex);
                args = fullPath.Substring(minIndex, fullPath.Length - minIndex).Trim();
            }
        }

        /// <summary>
        ///Splits the application path and arguments. Every application has its own arguments format so this method is slightly convoluted.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="args"></param>
        public void SplitPathAndArguments(out string path, out string args)
        {
            //If path is surrounded by quotes
            if (rawPath.IndexOf('"') == 0)
            {
                //Remove first quote
                string tempString = rawPath.TrimStart('"');
                int index = tempString.IndexOf('"');

                //Split at second quote
                path = tempString.Substring(0, index);
                args = tempString.Substring(index + 1, tempString.Length - index - 1).Trim();
            }
            else
            {
                //Check indexof('/' and '-')
                string[] argDelimiters = new string[2] { " -", " /" };

                //If there are no arguments, no split
                int minIndex = rawPath.Length;

                //Split at the smallest index (first argument found while parsing)
                foreach (string delim in argDelimiters)
                {
                    int index = rawPath.IndexOf(delim);
                    if (index != -1)
                        minIndex = Math.Min(index, minIndex);
                }

                path = rawPath.Substring(0, minIndex);
                args = rawPath.Substring(minIndex, rawPath.Length - minIndex).Trim();
            }
        }

    }
}
