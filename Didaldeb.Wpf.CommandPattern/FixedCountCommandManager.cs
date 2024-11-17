using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dideldev.Wpf.CommandPattern
{
    /// <summary>
    /// Command manager that keeps a maximum number of commands on the stack.
    /// </summary>
    /// <remarks>
    /// It is not guaranteed to occupy a limited amount of memory.
    /// Commands may have lot of data to store. 
    /// TransactionCommands may have several commands inside and still count as one.
    /// </remarks>
    /// <typeparam name="T">Type of the context where the commands operate.</typeparam>
    /// <remarks>
    /// Initializes a new instance of <see cref="FixedCountCommandManager{T}"/>.
    /// </remarks>
    /// <param name="maxCount"></param>
    public class FixedCountCommandManager<T>(int maxCount = 1000) : ICommandManager<T>
        where T : class
    {
        /// <summary>
        /// Context where the commands opeartes and make changes.
        /// </summary>
        private T? context;

        /// <summary>
        /// Maximum number of commands stored on a stack.
        /// </summary>
        private readonly int maxCount = maxCount;

        /// <summary>
        /// Last exexuted commands. 
        /// </summary>
        protected CappedStack<Command<T>> executedCommands = new();

        /// <summary>
        /// Last undone commands. 
        /// </summary>
        protected CappedStack<Command<T>> undoneCommands = new();

        /// <summary>
        /// Gets or sets the context where de commands operates.
        /// </summary>
        public T? Context { get => context; set => context = value; }

        /// <summary>
        /// Perform the action of the given command and stores it so it can be undone.
        /// </summary>
        /// <param name="cmd"></param>
        public void Do(Command<T> cmd)
        {
            cmd.Do(context);
            executedCommands.Push(cmd);

            if (undoneCommands.Count > 0)
                undoneCommands.Clear();


            if (this.executedCommands.Count > maxCount)
            {
                // The command is now lost and cannot be undo
                this.executedCommands.Items.RemoveAt(0);
            }
        }

        /// <summary>
        /// Rollbacks the action done by the last executed command.
        /// </summary>
        /// <returns>The command that has been undone. It may have information relevant to the view model or the view.</returns>

        public Command<T>? Undo()
        {
            if (executedCommands.Count > 0)
            {
                Command<T> cmd = executedCommands.Pop();
                cmd.Undo(context);
                undoneCommands.Push(cmd);
                return cmd;
            }
            return null;
        }

        /// <summary>
        /// Do the action of the last undone command.
        /// </summary>
        /// <returns>The command that has been undone. It may have information relevant to the view model or the view.</returns>

        public Command<T>? Redo()
        {
            if (undoneCommands.Count > 0)
            {
                Command<T> cmd = undoneCommands.Pop();
                cmd.Do(context);
                executedCommands.Push(cmd);
                return cmd;
            }
            return null;
        }
    }
}
