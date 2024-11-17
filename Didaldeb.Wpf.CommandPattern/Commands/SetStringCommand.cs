using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Dideldev.Wpf.CommandPattern.Commands
{
    public class SetStringCommand<T> : SetPropertyCommand<T, string>
        where T : class
    {
        public SetStringCommand() { }

        public SetStringCommand(string propName, string oldValue, string newValue) 
            : base(propName, oldValue, newValue) 
        {
            
        }

        public override void ReadParameterBytes(BinaryReader br)
        {
            this.PropertyName = this.PropertyNames![0];
            this.OldValue = br.ReadString();
            this.NewValue = br.ReadString();
        }

        public override void WriteParameterBytes(BinaryWriter bw)
        {
            bw.Write((string)OldValue!);
            bw.Write((string)NewValue!);
        }
    }
}
