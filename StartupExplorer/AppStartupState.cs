using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartupExplorer
{
    /// <summary>
    /// Enumeration of all possible states of an application already existing in a startup group.
    /// </summary>
    public enum AppStartupState
    {
        /// <summary>
        /// No entry in the startup approved path.
        /// </summary>
        NotFound,
        /// <summary>
        /// Key value is not 02 00 00 00 00 00 00 00 00 00 00 00.
        /// </summary>
        Disabled,
        /// <summary>
        /// Key value is 02 00 00 00 00 00 00 00 00 00 00 00.
        /// </summary>
        Enabled
    }
}
