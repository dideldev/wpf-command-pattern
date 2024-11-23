using System.IO;
using System.Runtime.Remoting;

namespace Dideldev.Wpf.CommandPattern
{
    /// <summary>
    /// Base class to implement a serialize method for a list of commands. So it can be saved to disk and loaded afterwards. 
    /// </summary>
    /// <typeparam name="T">Context where the commands operate.</typeparam>
    public abstract class CommandFileSerializer
    {
        
        /// <summary>
        /// Load a file which contains a list of commands.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public abstract List<Command> LoadFile(string path);

        /// <summary>
        /// Save a list of commands to a file. 
        /// </summary>
        /// <param name="commands"></param>
        /// <param name="path"></param>
        public abstract void SaveFile(IEnumerable<Command> commands, string path);

        /// <summary>
        /// Instanciate a command given its type and assembly. 
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="IOException"></exception>
        protected static Command InstanciateCommand(string assembly, string type)
        {
            ObjectHandle? hndl = Activator.CreateInstance(assembly, type);
            if (hndl?.Unwrap() is not Command cmd)
            {
                throw new InvalidOperationException("Type of Command not defined in current assembly or could not be instanciated.");
            }
            return cmd;
        }
    }
}
