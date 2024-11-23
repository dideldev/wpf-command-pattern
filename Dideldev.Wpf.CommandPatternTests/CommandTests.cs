using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dideldev.Wpf.CommandPattern.Tests
{

    [TestFixture()]
    public class CommandTests
    {

        public void CommandTest()
        {
            new FooComand().Should().NotBeNull();
            FooComand fc = new("New");
            fc.NewValue.Should().Be("New");
            fc.OldValue.Should().Be("Old");
        }

        [Test()]
        public void DoUndoTest()
        {
            FooContext context = new("0");
            FooComand command1 = new("1");
            FooComand command2 = new("2");
            FooComand command3 = new("3");

            context.Value.Should().Be("0");
            command1.Do(context);
            context.Value.Should().Be("1");
            command2.Do(context);
            context.Value.Should().Be("2");
            command3.Do(context);
            context.Value.Should().Be("3");
            command3.Undo(context);
            context.Value.Should().Be("2");
            command2.Undo(context);
            context.Value.Should().Be("1");
            command1.Undo(context);
            context.Value.Should().Be("0");
        }

        [Test()]
        public void WriteReadParameterBytesTest()
        {
            FooComand c1 = new("New");
            FooComand c2 = new();

            using MemoryStream ms = new();
            using BinaryWriter bw = new BinaryWriter(ms, Encoding.ASCII);
            using BinaryReader br = new(ms, Encoding.ASCII);
            /*
            c1.WriteParameterBytes(bw);

            ms.Seek(0, SeekOrigin.Begin);

            c2.ReadParameterBytes(br);

            c1.NewValue.Should().Be(c2.NewValue);
            c1.OldValue.Should().Be(c2.OldValue);*/
        }
    }
}