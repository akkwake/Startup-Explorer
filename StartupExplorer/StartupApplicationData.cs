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

namespace StartupExplorer
{
    /// <summary>
    /// Contains data about a single application found in one of the startup groups.
    /// Has methods to change whether this specific application is enabled to run on startup, or remove it from startup entirely.
    /// </summary>
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
        /// True if this application belongs to a group that requires administrator privilages.
        /// </summary>
        public readonly bool requiresAdminPrivilages;

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


        public StartupApplicationData(string name, string path, string origin, StartupGroup startupGroup, AppStartupState state, bool requiresAdminPrivilages)
        {
            this.name = name;
            this.rawPath = path;
            this.groupPath = origin;
            this.startupGroup = startupGroup;
            this.state = state;
            this.requiresAdminPrivilages = requiresAdminPrivilages;

            if (!isShortcut)
                SplitPathAndArguments(out executablePath, out arguments);
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
            string[] pathAndArgs = new string[2];
            if (fullPath.IndexOf('"') == 0)
            {
                //error when there is a character '"' but no arguments
                pathAndArgs[0] = fullPath.TrimStart('"');
                int index = pathAndArgs[0].IndexOf('"');
                pathAndArgs[1] = pathAndArgs[0].Substring(index, pathAndArgs[0].Length - index);
                pathAndArgs[0] = pathAndArgs[0].Substring(0, index);
                pathAndArgs[1] = pathAndArgs[1].TrimStart('"');
                pathAndArgs[1] = pathAndArgs[1].TrimStart();
            }
            //This returns the path and argument if Path doesn't start with ' " '.
            else
            {
                pathAndArgs[0] = fullPath;
                int index;
                //Check indexof('/' and '-')
                string[] argDelimiters = new string[2] { " -", " /" };
                foreach (string s in argDelimiters)
                {
                    //psakse apla apo lastindexof('.') teleia mexri keno
                    if ((index = pathAndArgs[0].IndexOf(s, StringComparison.Ordinal)) > 0)
                    {
                        pathAndArgs[0] = pathAndArgs[0].Substring(0, index);
                        pathAndArgs[1] = fullPath.Substring(index, fullPath.Length - index);
                    }
                }

            }

            path = pathAndArgs[0];
            args = pathAndArgs[1];
        }


        /// <summary>
        ///Splits the application path and arguments. Every application has its own arguments format so this method is slightly convoluted.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="path"></param>
        /// <param name="args"></param>
        public void SplitPathAndArguments(out string path, out string args)
        {
            //SplitPathAndArguments(this.path, out path, out args);
            string[] pathAndArgs = new string[2];
            if (rawPath.IndexOf('"') == 0)
            {
                //error when there is a character '"' but no arguments
                pathAndArgs[0] = rawPath.TrimStart('"');
                int index = pathAndArgs[0].IndexOf('"');
                pathAndArgs[1] = pathAndArgs[0].Substring(index, pathAndArgs[0].Length - index);
                pathAndArgs[0] = pathAndArgs[0].Substring(0, index);
                pathAndArgs[1] = pathAndArgs[1].TrimStart('"');
                pathAndArgs[1] = pathAndArgs[1].TrimStart();
            }
            //This returns the path and argument if Path doesn't start with ' " '.
            else
            {
                pathAndArgs[0] = rawPath;
                int index;
                //Check indexof('/' and '-')
                string[] argDelimiters = new string[2] { " -", " /" };
                foreach (string s in argDelimiters)
                {
                    //psakse apla apo lastindexof('.') teleia mexri keno
                    if ((index = pathAndArgs[0].IndexOf(s, StringComparison.Ordinal)) > 0)
                    {
                        pathAndArgs[0] = pathAndArgs[0].Substring(0, index);
                        pathAndArgs[1] = rawPath.Substring(index, rawPath.Length - index);
                    }
                }

            }

            path = pathAndArgs[0];
            args = pathAndArgs[1];
        }

    }
}
