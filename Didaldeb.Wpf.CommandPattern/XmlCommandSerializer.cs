using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.ConstrainedExecution;

namespace Dideldev.Wpf.CommandPattern
{
    /// <summary>
    /// Xml Serializer for <see cref="CommandFile"/>.
    /// </summary>
    public class XmlCommandSerializer : CommandFileSerializer
    {

        /// <summary>
        /// Xml serializer. 
        /// </summary>
        private XmlSerializer serializer;

        /// <summary>
        /// Initializes a new instance of <see cref="XmlSerializer"/> that serializes the types given. 
        /// </summary>
        /// <param name="types"></param>
        public XmlCommandSerializer(IEnumerable<Type>? types = null) {

            XmlAttributeOverrides aor = new ();
            XmlAttributes listAttribs = new ();            

            if (types != null) {
                foreach (var type in types)
                {
                    listAttribs.XmlElements.Add(new XmlElementAttribute(type.Name, type));
                } 
            }
            aor.Add(typeof(CommandFile), "Items", listAttribs);
            serializer = new (typeof(CommandFile), aor);

        }

        /// <summary>
        /// Load a command file and return its command list. 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public override List<Command> LoadFile(string path)
        {
            using TextReader reader = new StreamReader(path);
            CommandFile file = (CommandFile)serializer.Deserialize(reader)!;
            return file.Items;
        }
        
        /// <summary>
        /// Save a list of commands to disk as a xml document.
        /// </summary>
        /// <param name="commands"></param>
        /// <param name="path"></param>
        public override void SaveFile(IEnumerable<Command> commands, string path)
        {
            CommandFile file = new() {
                Items = commands.ToList()
            };            
            
            using TextWriter writer = new StreamWriter(path);
            serializer.Serialize(writer, file);
        }

    }
}
