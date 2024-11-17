using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dideldev.Wpf.CommandPattern
{
    /// <summary>
    /// Basic command manager with stacks for executed and undone commands. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CommandManager<T> : ICommandManager<T>
        where T : class
    {
        /// <summary>
        /// Context where the commands opeartes and make changes.
        /// </summary>
        T? context;

        /// <summary>
        /// Stack of executed commands.
        /// </summary>
        private readonly Stack<Command<T>> executedCommands = new();

        /// <summary>
        /// Stack of executed commands that have been undone.
        /// </summary>
        private readonly Stack<Command<T>> undoneCommands = new();

        /// <summary>
        /// Initializes a new instance of <see cref="CommandManager{T}"/>.
        /// </summary>
        public CommandManager()
        {

        }

        /// <summary>
        /// Initializes a new instance of <see cref="CommandManager{T}"/> with the given context. 
        /// </summary>
        /// <param name="context"></param>
        public CommandManager(T context)
        {
            this.context = context;
        }

        /// <summary>
        /// Get or sets the context where the commands opeartes and make changes.
        /// </summary>
        public T? Context { get => context; set => context = value; }

        /// <summary>
        /// Perform the action of the given command and stores it in a stack so it can be undone.
        /// </summary>
        /// <param name="cmd"></param>
        public void Do(Command<T> cmd)
        {
            cmd.Do(context);
            executedCommands.Push(cmd);
            if (undoneCommands.Count > 0)
            {
                undoneCommands.Clear();
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
