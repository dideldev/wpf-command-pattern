using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dideldev.Wpf.CommandPattern.Demo
{
    /// <summary>
    /// Information about a command on some list of <see cref="DiskCommandManager{T}"/>.
    /// </summary>
    public class DebugCommandItem
    {
        public DebugCommandItem()
        {
        }

        /// <summary> Name to be shown on the view. </summary>
        public string Name { get; set; } = "";

        /// <summary> Indicates this command is the last executed (and the next to be undone)</summary>
        public bool LastExecuted { get; set; } = false;

        /// <summary> Indicates this command has been undone already.</summary>
        public bool Undone { get; set; } = false;
    }
}
