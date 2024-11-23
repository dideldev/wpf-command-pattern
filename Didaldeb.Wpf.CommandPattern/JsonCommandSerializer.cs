using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Dideldev.Wpf.CommandPattern
{

    public class JsonCommandSerializer : CommandFileSerializer
    {
        private const string KEY_ASSEMBLY = "$Assembly";
        private const string KEY_TYPE = "$Type";

        public override List<Command> LoadFile(string path)
        {
            string jsonString = File.ReadAllText(path, Encoding.UTF8);
            List<Command> commands = [];
            JsonArray jArr = JsonArray.Parse(jsonString)!.AsArray();
            foreach (JsonNode? jNode in jArr)
            {
                Command? cmd = DeserializeCommand(jNode!.ToJsonString());
                commands.Add(cmd);
            }

            return commands;
        }

        public override void SaveFile(IEnumerable<Command> commands, string path)
        {
            JsonArray jArr = new JsonArray();
            foreach (Command cmd in commands)
            {
                JsonObject jComm = SerializeCommand(cmd);
                jArr.Add(jComm);
            }

            string jsonString = jArr.ToJsonString();
            File.WriteAllText(path, jsonString, Encoding.UTF8);
        }

        private JsonObject SerializeCommand(Command command)
        {
            string jsonString = JsonSerializer.Serialize(command, command.GetType());
            JsonObject? jo = JsonNode.Parse(jsonString)!.AsObject();

            string? type = command.GetType().FullName;
            string? assemblyName = Assembly.GetAssembly(command.GetType())?.FullName ?? null;

            jo.Add(KEY_ASSEMBLY, assemblyName);
            jo.Add(KEY_TYPE, type);

            return jo;
        }

        private Command DeserializeCommand(string jsonString)
        {
            JsonObject? jo = JsonNode.Parse(jsonString)!.AsObject();
            string assembly = (string)jo[KEY_ASSEMBLY]!;
            string type = (string)jo[KEY_TYPE]!;

            Command? cmd = InstanciateCommand(assembly, type);
            object? cmd2 = JsonSerializer.Deserialize(jsonString, cmd.GetType())!;
            return (cmd2 as Command)!;
        }
    }
}
