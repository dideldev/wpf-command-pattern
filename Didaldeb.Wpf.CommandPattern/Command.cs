using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Dideldev.Wpf.CommandPattern
{
    public abstract class Command
    {
        /// <summary>
        /// Get or sets the list of properties that might change so the view model may 
        /// notify it.
        /// </summary>
        public List<string> PropertyNames { get; set; } = [];

        /// <summary>
        /// Gets the full name of the current command type. 
        /// </summary>
        internal string? TypeName => this.GetType().FullName;

        /// <summary>
        /// Gets the assembly name of the current command type. 
        /// </summary>
        internal string? AssemblyName => Assembly.GetAssembly(this.GetType())?.FullName ?? null;

        /// <summary>
        /// Initializes a new instance of <see cref="Command"/>.
        /// </summary>
        public Command()
        {

        }

        /// <summary>
        /// Perform some action that affects the given context. 
        /// </summary>
        /// <param name="Context"></param>
        public abstract void Do(object context);

        /// <summary>
        /// Undo some previously done via <see cref="Do(T?)"/>.
        /// </summary>
        /// <param name="Context"></param>
        public abstract void Undo(object context);
    }

    /// <summary>
    /// Implementation of <see cref="Command"/> defining the type of <see cref="Command.Context"/>.
    /// </summary>
    /// <remarks>
    /// <typeparam name="T">Type of the context where the Do/Undo actions operate.</typeparam>
    public abstract class Command<T> : Command
    where T : class
    {
        /// <summary>
        /// Initializes a new instance of <see cref="Command"/> with a null context. 
        /// </summary>
        public Command() : base() {
           
        }

        /// <summary>
        /// Perform some action that affects the given context. 
        /// It must match the type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="Context"></param>
        public override void Do(object context)
        {
            if (context != null && context is not T)
                throw new ArgumentException($"The given context is not of type {typeof(T)}. Found {context.GetType()}. ");

            Do((T?)context);
        }        

        /// <summary>
        /// Perform some action that affects the given context. 
        /// </summary>
        /// <param name="Context"></param>
        public abstract void Do(T? Context);

        /// <summary>
        /// Undo some previously done command."/>.
        /// </summary>
        /// <param name="Context"></param>
        public override void Undo(object context)
        {
            if (context != null && context is not T)
                throw new ArgumentException($"The given context is not of type {typeof(T)}. Found {context.GetType()}. ");

            Undo((T?)context);
        }

        /// <summary>
        /// Undo some previously done command via <see cref="Do(T?)"/>.
        /// </summary>
        /// <param name="Context"></param>
        public abstract void Undo(T? Context);
    }
}

