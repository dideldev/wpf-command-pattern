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

    /// <summary>
    /// Implementation of <see cref="CommandManager{T}"/> defining the type of <see cref="Command.Context"/>.
    /// </summary>
    /// <remarks>
    /// <typeparam name="T">Type of the context where the Do/Undo actions operate.</typeparam>
    public abstract class Command<T>
        where T : class
    {
        /// <summary>
        /// Get or sets the list of properties that might change so the view model may 
        /// notify it.
        /// </summary>
        public List<string>? PropertyNames { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="Command"/> with a null context. 
        /// </summary>
        public Command() : base() { }

        /// <summary>
        /// Perform some action that affects the given context. 
        /// </summary>
        /// <param name="Context"></param>
        public abstract void Do(T? Context);

        /// <summary>
        /// Undo some previously done via <see cref="Do(T?)"/>.
        /// </summary>
        /// <param name="Context"></param>
        public abstract void Undo(T? Context);

        /// <summary>
        /// Write the bytes(serialization) of the parameters of the command into a stream.
        /// be saved and restored from a file
        /// </summary>
        /// <returns></returns>
        public abstract void WriteParameterBytes(BinaryWriter bw);

        /// <summary>
        /// Read the bytes(serialization) of the parameters of the command from a stream.
        /// </summary>
        /// <param name="br"></param>
        public abstract void ReadParameterBytes(BinaryReader br);

        /// <summary>
        /// Read an instance of this command from a stream.
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        /// <exception cref="IOException"></exception>
        internal static Command<T> ReadBytes(BinaryReader br)
        {
            string assemblyName = br.ReadString();
            string typeName = br.ReadString();

            ObjectHandle? hndl = Activator.CreateInstance(assemblyName, typeName);
            if (hndl?.Unwrap() is not Command<T> cmd)
            {
                throw new IOException("Type of Command not defined in current assembly.");
            }

            int propertyNamesCount = br.ReadInt32();
            if (propertyNamesCount > 0)
            {
                cmd.PropertyNames = [];
                for (int i = 0; i < propertyNamesCount; i++)
                {
                    cmd.PropertyNames.Add(br.ReadString());
                }
            }

            cmd.ReadParameterBytes(br);

            return cmd;
        }

        /// <summary>
        /// Write (serialize) this instance into a binary file that can be restered afterwards.
        /// </summary>
        /// <param name="bw"></param>
        /// <exception cref="Exception"></exception>
        internal static void WriteBytes(BinaryWriter bw, Command<T> cmd)
        {
            Type myType = cmd.GetType();
            string? assembly = Assembly.GetAssembly(myType)?.FullName ?? null;
            string? typeName = myType.FullName;

            if (assembly == null)
                throw new Exception($"Could not load the name of the assembly where {myType} is located.");

            if (typeName == null)
                throw new Exception($"Could not get the full name of {myType}.");

            bw.Write(assembly);
            bw.Write(typeName);
            if (cmd.PropertyNames != null)
            {
                bw.Write(cmd.PropertyNames.Count);
                for (int i = 0; i < cmd.PropertyNames.Count; i++)
                {
                    bw.Write(cmd.PropertyNames[i]);
                }
            }
            else
            {
                bw.Write(0);
            }

            cmd.WriteParameterBytes(bw);
        }
    }
}

