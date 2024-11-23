using Dideldev.Wpf.CommandPattern.Demo.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dideldev.Wpf.CommandPattern.Demo.Commands
{
    /// <summary>
    /// Command to set <see cref="Model.Background"/> property.
    /// </summary>
    public class SetBackgroundCommand : Command<Model>
    {
        /// <summary> Value model has before set it to <see cref="newValue"/>.</summary>
        public System.Windows.Media.Color OldValue { get; set; } = default;

        /// <summary> New value to be set.</summary>
        public System.Windows.Media.Color NewValue { get; set; } = default;

        /// <summary>
        /// Initializes a new instance of <see cref="SetBackgroundCommand"/>.
        /// </summary>
        public SetBackgroundCommand() : base() { }

        /// <summary>
        /// Initializes a new instance of <see cref="SetBackgroundCommand"/> with the given parameters..
        /// </summary>
        /// <param name="model">Context where the property value are changed.</param>
        /// <param name="newValue">Value to be set to the context. </param>
        /// <param name="propertyName">Property name of the viewmodel that will be raised after Undo this change.</param>
        public SetBackgroundCommand(System.Windows.Media.Color newValue, string? propertyName = null)
        {
            this.NewValue = newValue;
            if (propertyName != null)
                this.PropertyNames = [propertyName];
        }

        /// <summary>
        /// Changes the <see cref="Model.Background"/> property to <see cref="newValue"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="NullReferenceException">If there is no context.</exception>
        public override void Do(Model? context)
        {
            if (context == null)
                throw new NullReferenceException(nameof(context));

            this.OldValue = this.NewValue;
            context.Background = this.NewValue;
        }


        /// <summary>
        /// Changes the <see cref="Model.Background"/> property to <see cref="oldValue"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="NullReferenceException">If there is no context.</exception>
        public override void Undo(Model? context)
        {
            if (context == null)
                throw new NullReferenceException(nameof(context));

            context.Background = this.OldValue;
        }

        public override string ToString()
        {
            return $"SetBackground({NewValue})";
        }
    }
}
