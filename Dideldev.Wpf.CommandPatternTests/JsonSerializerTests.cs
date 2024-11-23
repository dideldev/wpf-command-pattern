using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dideldev.Wpf.CommandPattern.Tests
{
    [TestFixture]
    public class JsonSerializerTests
    {
        [Test]
        public void SerializeTest()
        {
            string path = "jFile.json";
            List<FooComand> list = [
                new FooComand("Hello"),
                new FooComand(" "),
                new FooComand("World"),
                new FooComand("!")
            ];

            FooContext context = new();
            foreach (FooComand foo in list)
            {
                foo.Do(context);
            }
            JsonCommandSerializer serializer = new();
            serializer.SaveFile(list, path);

            List<Command> list2 = serializer.LoadFile(path);
            list2.Should().HaveCount(list.Count);
            List<Command<FooContext>> list3 = list2.Cast<Command<FooContext>>().ToList();
            list2.Should().BeEquivalentTo(list);
            list3.Should().BeEquivalentTo(list);
        }
    }
}
