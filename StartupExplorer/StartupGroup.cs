using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartupExplorer
{
    /// <summary>
    /// Enumeration of startup groups. Note that there are more groups not included here (or in the database).
    /// </summary>
    public enum StartupGroup
    {
        /// <summary>
        /// User startup folder in start menu.
        /// </summary>
        StartMenuUser,
        /// <summary>
        /// Common startup folder in start menu.
        /// Requires administrator privilages to modify.
        /// </summary>
        StartMenuCommon,
        /// <summary>
        /// Current User.
        /// </summary>
        HKCU,
        /// <summary>
        /// Local Machine x32 (wow6432 node).
        /// Requires administrator privilages to modify.
        /// </summary>
        HKLM32,
        /// <summary>
        /// Local Machine x64.
        /// Requires administrator privilages to modify.
        /// </summary>
        HKLM64
        
    }
}
