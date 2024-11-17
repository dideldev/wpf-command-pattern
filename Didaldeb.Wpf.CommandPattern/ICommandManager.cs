using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dideldev.Wpf.CommandPattern
{
    public interface ICommandManager<T> 
        where T : class
    {
        /// <summary>
        /// Get or sets the context where the Commands operate.
        /// </summary>
        public T? Context { get; }

        /// <summary>
        /// Execute a command.
        /// </summary>
        /// <param name="cmd">Command to ve executed.</param>
        public void Do(Command<T> cmd);

        /// <summary>
        /// Undo the last comand executed using <see cref="Do(Command)"/> or <see cref="Redo"/>.
        /// </summary>
        /// <returns></returns>
        public Command<T>? Undo();

        /// <summary>
        /// Redo the last command executed using <see cref="Undo"/>
        /// </summary>
        /// <returns></returns>
        public Command<T>? Redo();

    }
}
