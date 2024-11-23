using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dideldev.Wpf.CommandPattern
{
    /// <summary>
    /// Configuration for a <see cref="DiskCommandManager{T}"/> instance.
    /// </summary>
    public struct DiskCommandManagerConfig()
    {
        /// <summary>
        /// Folder where the command files are stored. 
        /// </summary>
        public string Folder { get; set; } = "temp/rollbacks";

        /// <summary>
        /// Prefix for the files that contains commands that have been undone.
        /// </summary>
        public string UndoPrefix { get; set; } = "U";

        /// <summary>
        /// Prefix for the files that contains commands that have been executed. 
        /// </summary>
        public string ExecPrefix { get; set; } = "E";

        /// <summary>
        /// Maximum size of each one of the three lists the <see cref="DiskCommandManager{T}"/> uses.
        /// </summary>
        public int ListSize { get; set; } = 1000;

        /// <summary>
        /// It indicates to not delete files when initializing so the previous executed / undone commands reamins. 
        /// </summary>
        public bool PreserveFolderState { get; set; } = false;

        /// <summary>
        /// Gets or sets the serializer used to save and load Command files. 
        /// </summary>
        public CommandFileSerializer Serializer{ get; set; } = new JsonCommandSerializer();
    }

}
