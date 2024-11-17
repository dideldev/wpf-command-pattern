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
        private System.Windows.Media.Color oldValue = default;

        /// <summary> New value to be set.</summary>
        private System.Windows.Media.Color newValue = default;

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
        public SetBackgroundCommand(Model context, System.Windows.Media.Color newValue, string? propertyName = null)
        {
            this.oldValue = context!.Background;
            this.newValue = newValue;
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

            context.Background = this.newValue;
        }

        /// <summary>
        /// Read the bytes on a stream that contains the values to be set to this instance props.
        /// </summary>
        /// <param name="br"></param>
        public override void ReadParameterBytes(BinaryReader br)
        {
            this.oldValue = MediaColorExtensions.GetColorFromBytes(br);
            this.newValue = MediaColorExtensions.GetColorFromBytes(br);
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

            context.Background = this.oldValue;
        }

        /// <summary>
        /// Writes the bytes of this instance props to a stream.
        /// </summary>
        /// <param name="br"></param>
        public override void WriteParameterBytes(BinaryWriter bw)
        {
            oldValue.WriteBytes(bw);
            newValue.WriteBytes(bw);
        }

        public override string ToString()
        {
            return $"SetBackground({newValue})";
        }
    }
}
