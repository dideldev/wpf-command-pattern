using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dideldev.Wpf.CommandPattern.Commands
{
    /// <summary>
    /// Generic implementation of Do/Undo for a Set like command.
    /// </summary>
    /// <typeparam name="T">Type of the context where the property are going to be set.</typeparam>
    /// <typeparam name="S">Type of the proeperty to be set.</typeparam>
    public abstract class SetPropertyCommand<T,S>  : Command<T>
        where T : class 
    {
        public string PropertyName { get; protected set; } = "";
        public S? OldValue { get; protected set; } = default;
        public S? NewValue { get; protected set; } = default;

        /// <summary>
        /// Initializes a new instance of <see cref="SetPressedKeyCommand"/> with the given parameters..
        /// </summary>
        public SetPropertyCommand() : base() { }


        /// <summary>
        /// Initializes a new instance of <see cref="SetPressedKeyCommand"/> with the given parameters..
        /// </summary>
        /// <param name="model">Context where the property value are changed.</param>
        /// <param name="newValue">Value to be set to the context. </param>
        /// <param name="propertyName">Property name of the viewmodel that will be raised after Undo this change.</param>
        public SetPropertyCommand(string propName, S oldValue, S newValue, string[]? aditionalProps = null)
        {
            this.PropertyName = propName;
            this.OldValue = oldValue;
            this.NewValue = newValue;

            this.PropertyNames = [propName];
            if (aditionalProps != null)
                this.PropertyNames.AddRange(aditionalProps);
        }

        /// <summary>
        /// Changes the <see cref="Model.PressedKey"/> property to <see cref="newValue"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="NullReferenceException">If there is no context.</exception>
        public override void Do(T? context)
        {
            if (context == null)
                throw new NullReferenceException(nameof(context));

            SetValue(context, NewValue);
        }

        /// <summary>
        /// Changes the <see cref="Model.PressedKey"/> property to <see cref="oldValue"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="NullReferenceException">If there is no context.</exception>
        public override void Undo(T? context)
        {
            if (context == null)
                throw new NullReferenceException(nameof(context));

            SetValue(context, OldValue);
        }

        public override string ToString()
        {
            return $"({OldValue})=>({NewValue}) ";
        }

        /// <summary>
        /// Set a vlaue to the <see cref="PropertyName"/> of the given context. 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="value"></param>
        /// <exception cref="NullReferenceException">If the property name is empty or could not get the property.</exception>
        private void SetValue(T context, S? value)
        {
            if (this.PropertyName == null)
                throw new NullReferenceException(nameof(this.PropertyName));

            PropertyInfo? propInfo =
                typeof(T).GetProperty(this.PropertyName, BindingFlags.Instance | BindingFlags.Public)
                ?? throw new NullReferenceException($"Property {nameof(this.PropertyName)} not found in tpye{typeof(T)}");

            propInfo.SetValue(context, value);
        }        
    }
}
