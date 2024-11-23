using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dideldev.Wpf.CommandPattern
{
    /// <summary>
    /// Base class for a command file manager defining save an load methods.
    /// </summary>
    /// <typeparam name="T">Type of the context where the commands operates.</typeparam>
    public class CommandFileManager
    {

        /// <summary>
        /// Create a new instance of <see cref="CommandFileManager{T}"/>.
        /// </summary>
        /// <param name="folder">Folder where the files are stored. </param>
        /// <param name="execPrefix">Prefix that identifies the files which contains executed commands.</param>
        /// <param name="undoPrefix">Prefix that identifies the files which contains undone commands.</param>
        public CommandFileManager()
        {

        }

        /// <summary>
        /// Get or sets the folder where the files are stored.
        /// </summary>
        public string Folder { get; set; } = "temp/rollbacks";

        /// <summary>
        /// Get or sets the prefix that identifies the files which contains executed commands
        /// </summary>
        public string ExecPrefix { get; set; } = "E";

        /// <summary>
        /// Get or sets the prefix that identifies the files which contains undone commands
        /// </summary>
        public string UndoPrefix { get; set; } = "U";

        /// <summary>
        /// Seraizlier use to create and load files containing commands.
        /// </summary>
        public CommandFileSerializer Serializer { get; set; } = new JsonCommandSerializer();

        /// <summary>
        /// Delete all files in <see cref="folder"/> which starts with
        /// <see cref="ExecPrefix"/> or <see cref="UndoPrefix"/>.
        /// </summary>
        internal void DeleteAllFiles()
        {
            DeleteExecFiles();
            DeleteUndoFiles();
        }

        /// <summary>
        /// Delete all files in <see cref="folder"/> which starts with <see cref="ExecPrefix"/>.
        /// </summary>
        internal void DeleteExecFiles()
        {
            if (!Directory.Exists(Folder))
                return;

            foreach (var file in Directory.EnumerateFiles(Folder, $"{ExecPrefix}*"))
            {
                File.Delete(file);
            }
        }

        /// <summary>
        /// Delete all files in <see cref="folder"/> which starts with <see cref="UndoPrefix"/>.
        /// </summary>
        internal void DeleteUndoFiles()
        {
            if (!Directory.Exists(Folder))
                return;
            foreach (var file in Directory.EnumerateFiles(Folder, $"{UndoPrefix}*"))
            {
                File.Delete(file);
            }
        }

        /// <summary>
        /// Get the commands in the last create Executed Commands File.
        /// </summary>
        /// <returns>
        /// A list with the commands found in the file.
        /// If no file is found it returns an empty list. 
        /// </returns>
        internal List<Command> LoadLastExecutedFile()
        {
            string? file = Directory.EnumerateFiles(Folder, $"{ExecPrefix}*")
                .OrderBy(f => f)
                .LastOrDefault();

            if (file == null)
                return [];

            return OpenCommandFile(file, deleteAfterRead: true);
        }

        /// <summary>
        /// Get the commands in the last created Undone Commands File; 
        /// </summary>
        /// <returns>
        /// A list with the commands found in the file.
        /// If no file is found it returns an empty list. 
        /// </returns>
        internal List<Command> LoadNextUndoneFile()
        {
            string? file = Directory.EnumerateFiles(Folder, $"{UndoPrefix}*")
                .OrderBy(f => f)
                .LastOrDefault();

            if (file == null)
                return [];

            return OpenCommandFile(file, deleteAfterRead: true);
        }

        /// <summary>
        /// Open a file, read the commands in it and return them as a list.  
        /// </summary>
        /// <param name="path">Path of the commands file to be read.</param>
        /// <param name="deleteAfterRead">Once the file is read, deletes the file from disk.</param>
        /// <returns></returns>
        internal List<Command> OpenCommandFile(string path, bool deleteAfterRead = true)
        {
            List<Command>  list = Serializer.LoadFile(path);

            if(deleteAfterRead)
                File.Delete(path);

            return list;
        }

        /// <summary>
        /// save a list of commands to a file. 
        /// </summary>
        /// <param name="commands">List of commands to save.</param>
        /// <param name="path">Path of the file where commands are going to be stored.</param>
        /// <exception cref="NullReferenceException">
        /// The file should have, at least, the version of this assembly that generated the file.
        /// </exception>
        internal void SaveCommandFile(List<Command> commands, string path)
        {
            Serializer.SaveFile(commands, path);
        }

        /// <summary>
        /// Generate the next available name for the file and save the executed commands on a file with that name.
        /// </summary>
        /// <param name="list">List of executed commands to be written in the file.</param>
        internal void SaveNewExecutedFile(List<Command> list)
        {
            Directory.CreateDirectory(Folder);

            string? newFile = Directory.EnumerateFiles(Folder, $"{ExecPrefix}*")
              .OrderBy(f => f)
              .LastOrDefault();

            int num = 1;

            if (newFile != null)
            {
                string name = Path.GetFileNameWithoutExtension(newFile);
                name = name[ExecPrefix.Length..];
                num = int.Parse(name) + 1;
            }

            newFile = Path.Combine(Folder, $"{ExecPrefix}{num}");

            SaveCommandFile(list, newFile);
        }

        /// <summary>
        /// Generate the next available name for the file and save the undone commands on a file with that name.
        /// </summary>
        /// <param name="list">List of undone commands to be written in the file.</param>
        internal void SaveNewUndoneFile(List<Command> list)
        {
            string? newFile = Directory.EnumerateFiles(Folder, $"{UndoPrefix}*")
              .OrderBy(f => f)
              .LastOrDefault();
            int num = 1;

            if (newFile != null)
            {
                string name = Path.GetFileNameWithoutExtension(newFile);
                name = name[UndoPrefix.Length..];
                num = int.Parse(name) + 1;
            }

            newFile = Path.Combine(Folder, $"{UndoPrefix}{num}");

            SaveCommandFile(list, newFile);
        }

    }
}
