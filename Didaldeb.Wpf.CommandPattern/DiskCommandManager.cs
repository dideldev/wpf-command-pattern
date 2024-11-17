using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dideldev.Wpf.CommandPattern
{
    /// <summary>
    /// Command manager that keeps a fixed amount of Commands in memmory and stores the rest on disk.
    /// </summary>
    /// <remarks>
    /// ** IMPORTANT NOTE **
    /// Everty command used by this manager MUST have a parameterless constructor, as it need to be instanciated
    /// wiith <see cref="Activator.CreateInstanceFrom(string, string)"/>.
    /// </remarks>
    /// <typeparam name="T">Tpye of the context where the comands operates.</typeparam>
    public class DiskCommandManager<T> : ICommandManager<T>
        where T : class
    {
        /// <summary>
        ///  Configuration of the <see cref="DiskCommandManagerConfig"/>. Folders, files, limits, etc. 
        /// </summary>
        /// <remarks>
        /// Must be read only. 
        /// Once the class has created some files it is expected to load files with the same configuration.
        /// </remarks>
        private readonly DiskCommandManagerConfig config = new();

        /// <summary>
        /// Context where the commands opeartes and make changes.
        /// </summary>
        private readonly T? context;

        /// <summary>
        /// Contains the last chunk of executed commands, previous to the chunk we are currently using. 
        /// TODO: Set as private. public for demo.
        /// </summary>
        private List<Command<T>> PreviousExecutedCommands { get; set; } = [];

        /// <summary>
        /// Contains the current chunk of commands. The commands with an index lesser or equal to <see cref="LastExecutedCommandIndex"/>
        /// are executed. If there are any command on a greater index, that means taht command has been undone.         
        /// TODO: Set as private. Internal for debug.
        /// /// </summary>
        private List<Command<T>> CurrentCommands { get; set; } = [];

        /// <summary>
        /// Contains the next chunk of undone commands.         
        /// TODO: Set as private. Internal for debug.
        /// /// </summary>
        private List<Command<T>> NextUndoneCommands { get; set; } = [];

        /// <summary>
        /// Index of the last executed command
        /// TODO: Set as private. Internal for debug.
        /// </summary>
        private int LastExecutedCommandIndex { get; set; } = -1;

        /// <summary>
        /// Manager to save and load command list files.
        /// </summary>
        private readonly DiskCommandManagerFileManager<T> fileManager;

        /// <summary>
        /// Initializes a new instance of <see cref="DiskCommandManager{T}"/>.
        /// </summary>
        /// <param name="context">Context where the commands operates.</param>
        /// <param name="config">Configuration of the command manager behavieur.</param>
        public DiskCommandManager(T? context = null, DiskCommandManagerConfig? config = null)
        {
            this.context = context;
            this.config = config ?? this.config;

            fileManager = new DiskCommandManagerFileManager<T>(
                this.config.Folder,
                this.config.ExecPrefix,
                this.config.UndoPrefix);

            if (!this.config.PreserveFolderState && Directory.Exists(this.config.Folder))
                fileManager.DeleteAllFiles();
        }

        /// <summary>
        /// Gets the current contest that is being used by the commands. 
        /// </summary>
        public T? Context { get { return this.context; } }

        /// <summary>
        /// Gets the configuration of the command manager. 
        /// </summary>
        public DiskCommandManagerConfig Config => config;

        /// <summary>
        /// Perform the action of the given command and stores it so it can be undone.
        /// </summary>
        /// <param name="cmd"></param>
        public void Do(Command<T> cmd)
        {
            cmd.Do(context);

            if (NextUndoneCommands.Count > 0)
            {
                NextUndoneCommands.Clear();
                fileManager.DeleteUndoFiles();
            }

            // Desplazar listas si procede
            if (LastExecutedCommandIndex + 1 >= config.ListSize)
            {
                if (PreviousExecutedCommands.Count > 0)
                {
                    fileManager.SaveNewExecutedFile(PreviousExecutedCommands);
                }

                PreviousExecutedCommands = CurrentCommands;
                CurrentCommands = [];
                NextUndoneCommands = [];
                LastExecutedCommandIndex = 0;
            }

            if (LastExecutedCommandIndex < CurrentCommands.Count - 1)
            {
                // If the last written is not the the last one on the list, remove the undone commands.                 
                CurrentCommands.RemoveRange(LastExecutedCommandIndex + 1, CurrentCommands.Count - LastExecutedCommandIndex - 1);
            }

           

            // Add the executed command.
            CurrentCommands.Add(cmd);
            LastExecutedCommandIndex = CurrentCommands.Count - 1;

        }

        /// <summary>
        /// Rollbacks the action done by the last executed command.
        /// </summary>
        /// <returns>The command that has been undone. It may have information relevant to the view model or the view.</returns>
        public Command<T>? Undo()
        {
            // Undo last exexuted command.
            if (LastExecutedCommandIndex < 0 || LastExecutedCommandIndex > CurrentCommands.Count)
                return null;

            Command<T> cmd = CurrentCommands[LastExecutedCommandIndex];
            cmd.Undo(context);

            // Update pointer
            LastExecutedCommandIndex--;

            // If the pointer is out of range of the current list, update list and pointer. 
            if (LastExecutedCommandIndex < 0 && PreviousExecutedCommands.Count > 0)
            {
                if (NextUndoneCommands.Count > 0)
                {
                    fileManager.SaveNewUndoneFile(NextUndoneCommands);
                }

                NextUndoneCommands = CurrentCommands;
                CurrentCommands = PreviousExecutedCommands;
                PreviousExecutedCommands = fileManager.LoadLastExecutedFile();

                LastExecutedCommandIndex = CurrentCommands.Count - 1;
            }

            return cmd;
        }

        /// <summary>
        /// Do the action of the last undone command.
        /// </summary>
        /// <returns>The command that has been undone. It may have information relevant to the view model or the view.</returns>
        public Command<T>? Redo()
        {
            if (LastExecutedCommandIndex + 1 >= CurrentCommands.Count)
            {
                if (NextUndoneCommands.Count == 0)
                    // Nothing to redo. 
                    return null;

                // Load next undone commands
                if (PreviousExecutedCommands.Count > 0)
                    fileManager.SaveNewExecutedFile(PreviousExecutedCommands);
                PreviousExecutedCommands = CurrentCommands;
                CurrentCommands = NextUndoneCommands;
                NextUndoneCommands = fileManager.LoadNextUndoneFile();

                LastExecutedCommandIndex = -1;
            }

            LastExecutedCommandIndex++;

            Command<T>? cmd = CurrentCommands[LastExecutedCommandIndex];
            cmd?.Do(context);

            return cmd;
        }
    }
}
