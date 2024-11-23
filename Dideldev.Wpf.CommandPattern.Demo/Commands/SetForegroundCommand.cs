using Dideldev.Wpf.CommandPattern.Demo.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Dideldev.Wpf.CommandPattern.Demo.Commands
{
    /// <summary>
    /// Command to set <see cref="Model.Foreground"/> property.
    /// </summary>
    public class SetForegroundCommand : Command<Model>
    {
        /// <summary> Value model has before set it to <see cref="newValue"/>.</summary>
        public System.Windows.Media.Color OldValue { get; set; } = default;

        /// <summary> New value to be set.</summary>
        public System.Windows.Media.Color NewValue { get; set; } = default;

        /// <summary>
        /// Initializes a new instance of <see cref="SetForegroundCommand"/>.
        /// </summary>
        public SetForegroundCommand() : base() { }

        /// <summary>
        /// Initializes a new instance of <see cref="SetForegroundCommand"/> with the given parameters. 
        /// </summary>
        /// <param name="model">Context where the property value are changed.</param>
        /// <param name="newValue">Value to be set to the context. </param>
        /// <param name="propertyName">Property name of the viewmodel that will be raised after Undo this change.</param>
        public SetForegroundCommand(Color newValue, string? propertyName = null)
        {
            this.NewValue = newValue;
            if (propertyName != null)
                this.PropertyNames = [propertyName];
        }

        /// <summary>
        /// Change the value of <see cref="Model.Foreground"/> to <see cref="newValue"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="NullReferenceException">If there is no context.</exception>
        public override void Do(Model? context)
        {
            if (context == null) throw new NullReferenceException(nameof(context));
            this.OldValue = context.Foreground;
            context.Foreground = this.NewValue;
        }

        /// <summary>
        /// Change the value of <see cref="Model.Foreground"/> to <see cref="oldValue"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="NullReferenceException">If there is no context.</exception>
        public override void Undo(Model? context)
        {
            if (context == null) throw new NullReferenceException(nameof(context));
            context.Foreground = this.OldValue;
        }

        public override string ToString()
        {
            return $"SetForeground({NewValue})";
        }
    }
}
