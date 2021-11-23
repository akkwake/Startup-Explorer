using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartupExplorer
{
    /// <summary>
    /// Enumeration of startup groups.
    /// </summary>
    public enum StartupGroup
    {
        /// <summary>
        /// User startup folder in start menu.
        /// </summary>
        StartMenuUser,
        /// <summary>
        /// Common startup folder in start menu.
        /// Requires administrator privileges to modify.
        /// </summary>
        StartMenuCommon,
        /// <summary>
        /// Current User.
        /// </summary>
        HKCU,
        /// <summary>
        /// Local Machine x32 (wow6432 node).
        /// Requires administrator privileges to modify.
        /// </summary>
        HKLM32,
        /// <summary>
        /// Local Machine x64.
        /// Requires administrator privileges to modify.
        /// </summary>
        HKLM64
        
    }
}
