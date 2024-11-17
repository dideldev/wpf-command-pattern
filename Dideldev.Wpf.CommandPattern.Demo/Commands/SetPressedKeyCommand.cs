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
        private string OldValue { get; set; } = string.Empty;
        private string NewValue { get; set; } = string.Empty;

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
        public SetPressedKeyCommand(string oldValue, string newValue, string? propertyName = null)
        {
            this.OldValue = oldValue;
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

            context.PressedKey = this.NewValue;
        }

        /// <summary>
        /// Read the bytes on a stream that contains the values to be set to this instance props.
        /// </summary>
        /// <param name="br"></param>
        public override void ReadParameterBytes(BinaryReader br)
        {
            OldValue = br.ReadString();
            NewValue = br.ReadString();
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

        /// <summary>
        /// Writes the bytes of this instance props to a stream.
        /// </summary>
        /// <param name="br"></param>
        public override void WriteParameterBytes(BinaryWriter bw)
        {
            bw.Write(OldValue);
            bw.Write(NewValue);
        }

        public override string ToString()
        {
            return $"Key({NewValue})";
        }
    }
}
