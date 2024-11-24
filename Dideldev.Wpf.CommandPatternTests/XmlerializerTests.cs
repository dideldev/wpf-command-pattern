using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Dideldev.Wpf.CommandPattern.Tests
{
    [TestFixture]
    public class XmlSerializerTests
    {
        [Test]
        public void SerializeTest()
        {
            string path = "xmlCmdFile.xml";
            List<Command> list = [
                new FooComand2(0),
                new FooComand("Hello"),
                new FooComand(" "),
                new FooComand("World"),
                new FooComand("!"),
                new FooComand2(1)
            ];

            FooContext context = new();
            foreach (Command foo in list)
            {
                foo.Do(context);
            }
            XmlCommandSerializer serializer = new([
                typeof(FooComand),
                typeof(FooComand2)]);

            serializer.SaveFile(list, path);

            List<Command> list2 = serializer.LoadFile(path);
            list2.Should().HaveCount(list.Count);
            List<Command<FooContext>> list3 = list2.Cast<Command<FooContext>>().ToList();
            list2.Should().BeEquivalentTo(list);
            list3.Should().BeEquivalentTo(list);
        }
    }
}
