using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dideldev.Wpf.CommandPattern.Demo.Commands
{
    public class SetPressedKeyCommand : Command<Model>
    {
        public string OldValue { get; set; } = string.Empty;
        public string NewValue { get; set; } = string.Empty;

        /// <summary>
        /// Initializes a new instance of <see cref="SetPressedKeyCommand"/> with the given parameters..
        /// </summary>
        public SetPressedKeyCommand() : base() { }


        /// <summary>
        /// Initializes a new instance of <see cref="SetPressedKeyCommand"/> with the given parameters..
        /// </summary>
        /// <param name="model">Context where the property value are changed.</param>
        /// <param name="newValue">Value to be set to the context. </param>
        /// <param name="propertyName">Property name of the viewmodel that will be raised after Undo this change.</param>
        public SetPressedKeyCommand( string newValue, string? propertyName = null)
        {
            this.NewValue = newValue;
            if (propertyName != null)
                this.PropertyNames = [propertyName];
        }

        /// <summary>
        /// Changes the <see cref="Model.PressedKey"/> property to <see cref="newValue"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="NullReferenceException">If there is no context.</exception>
        public override void Do(Model? context)
        {
            if (context == null)
                throw new NullReferenceException(nameof(context));

            this.OldValue = context.PressedKey;
            context.PressedKey = this.NewValue;
        }


        /// <summary>
        /// Changes the <see cref="Model.PressedKey"/> property to <see cref="oldValue"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="NullReferenceException">If there is no context.</exception>
        public override void Undo(Model? context)
        {
            if (context == null)
                throw new NullReferenceException(nameof(context));

            context.PressedKey = this.OldValue;
        }

        public override string ToString()
        {
            return $"Key({NewValue})";
        }
    }
    }
