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
    /// Saves and load files used by <see cref="DiskCommandManager{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of the context where the commands operate.</typeparam>
    internal class DiskCommandManagerFileManager<T>
        where T : class
    {

        /// <summary>
        /// Create a new instance of <see cref="DiskCommandManagerFileManager{T}"/>.
        /// </summary>
        /// <param name="folder">Folder where the files are stored. </param>
        /// <param name="execPrefix">Prefix that identifies the files which contains executed commands.</param>
        /// <param name="undoPrefix">Prefix that identifies the files which contains undone commands.</param>
        internal DiskCommandManagerFileManager(string folder, string execPrefix, string undoPrefix)
        {
            Folder = folder;
            ExecPrefix = execPrefix;
            UndoPrefix = undoPrefix;
        }

        /// <summary>
        /// Get or sets the folder where the files are stored.
        /// </summary>
        internal string Folder { get; set; } = "temp/rollbacks";

        /// <summary>
        /// Get or sets the prefix that identifies the files which contains executed commands
        /// </summary>
        internal string ExecPrefix { get; set; } = "E";

        /// <summary>
        /// Get or sets the prefix that identifies the files which contains undone commands
        /// </summary>
        internal string UndoPrefix { get; set; } = "U";

        /// <summary>
        /// Delete all files in <see cref="folder"/> which starts with
        /// <see cref="ExecPrefix"/> or <see cref="UndoPrefix"/>.
        /// </summary>
        internal void DeleteAllFiles()
        {
            foreach (var file in Directory.EnumerateFiles(Folder, $"{UndoPrefix}*"))
            {
                File.Delete(file);
            }

            foreach (var file in Directory.EnumerateFiles(Folder, $"{ExecPrefix}*"))
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
        internal List<Command<T>> LoadLastExecutedFile()
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
        internal List<Command<T>> LoadNextUndoneFile()
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
        internal static List<Command<T>> OpenCommandFile(string path, bool deleteAfterRead = true)
        {
            List<Command<T>> list = [];
            using (FileStream fs = new(path, FileMode.Open, FileAccess.Read))
            {
                using BinaryReader br = new(fs, Encoding.Unicode);

                string strVersion = br.ReadString();
                int count = br.ReadInt32();

                for (int i = 0; i < count; i++)
                {
                    Command<T> cmd = Command<T>.ReadBytes(br);
                    list.Add(cmd);
                }
            }

            if (deleteAfterRead)
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
        internal static void SaveCommandFile(List<Command<T>> commands, string path)
        {
            Version? versionInfo = Assembly.GetExecutingAssembly().GetName().Version;
            if (versionInfo == null)
                throw new NullReferenceException("Could not get the version of the executing assembly.");

            using FileStream fs = new(path, FileMode.CreateNew, FileAccess.Write);
            using BinaryWriter bw = new(fs, Encoding.Unicode);

            bw.Flush();
            bw.Write(versionInfo.ToString());
            bw.Write(commands.Count);
            for (int i = 0; i < commands.Count; i++)
            {
                Command<T>.WriteBytes(bw, commands[i]);
            }
        }

        /// <summary>
        /// Generate the next available name for the file and save the executed commands on a file with that name.
        /// </summary>
        /// <param name="list">List of executed commands to be written in the file.</param>
        internal void SaveNewExecutedFile(List<Command<T>> list)
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
        internal void SaveNewUndoneFile(List<Command<T>> list)
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
