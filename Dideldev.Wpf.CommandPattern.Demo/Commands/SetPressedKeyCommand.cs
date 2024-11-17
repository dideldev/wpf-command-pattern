using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dideldev.Wpf.CommandPattern.Commands;

namespace Dideldev.Wpf.CommandPattern.Demo.Commands
{
    public class SetPressedKeyCommand : SetStringCommand<Model>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="SetForegroundCommand"/>.
        /// </summary>
        public SetPressedKeyCommand() : base() { }

        /// <summary>
        /// Initializes a new instance of <see cref="SetForegroundCommand"/> with the given parameters. 
        /// </summary>
        /// <param name="model">Context where the property value are changed.</param>
        /// <param name="newValue">Value to be set to the context. </param>
        /// <param name="propertyName">Property name of the viewmodel that will be raised after Undo this change.</param>
        public SetPressedKeyCommand(Model model, string newValue) 
            : base(nameof(model.PressedKey), model.PressedKey, newValue)
        {

        }
    }
}
