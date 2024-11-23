using NUnit.Framework;
using Dideldev.Wpf.CommandPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dideldev.Wpf.CommandPatternTests;
using NUnit.Framework.Constraints;

namespace Dideldev.Wpf.CommandPattern.Tests
{
    [TestFixture()]
    public class DiskCommandManagerTests
    {
        FooComand cmd1 = new("1");
        FooComand cmd2 = new("2");
        FooComand cmd3 = new("3");
        FooComand cmd4 = new("4");        

        FooContext context= new FooContext("0");
        DiskCommandManager<FooContext> manager = new (new FooContext());

        [SetUp()]
        public void Setup()
        {
            context.Value = "0";
            manager = new (context);             
        }

        [Test()]
        public void DoUndoRedoTest()
        {
            manager.Do(cmd1);
            context.Value.Should().Be(cmd1.NewValue);
            manager.Do(cmd2);
            context.Value.Should().Be(cmd2.NewValue);
            manager.Do(cmd3);
            context.Value.Should().Be(cmd3.NewValue);
            manager.Do(cmd4);
            context.Value.Should().Be(cmd4.NewValue);

            manager.Undo();
            context.Value.Should().Be(cmd3.NewValue);
            manager.Undo();
            context.Value.Should().Be(cmd2.NewValue);

            manager.Redo().Should().NotBeNull();
            context.Value.Should().Be(cmd3.NewValue);
            manager.Redo().Should().NotBeNull();
            context.Value.Should().Be(cmd4.NewValue);
            
            manager.Redo().Should().BeNull();
            context.Value.Should().Be(cmd4.NewValue);
            
            manager.Undo().Should().NotBeNull();
            manager.Undo().Should().NotBeNull();
            manager.Undo().Should().NotBeNull();
            manager.Undo().Should().NotBeNull();
            manager.Undo().Should().BeNull();
        }

        [Test()]
        public void FileSaveLoadTest()
        {
            manager = new DiskCommandManager<FooContext>(
                context,
                config: new DiskCommandManagerConfig()
                {
                    ListSize = 4
                });

            for (int i = 0; i < 10; i++)
            {
                manager.Do(cmd1);
                context.Value.Should().Be(cmd1.NewValue);
                manager.Do(cmd2);
                context.Value.Should().Be(cmd2.NewValue);
                manager.Do(cmd3);
                context.Value.Should().Be(cmd3.NewValue);
                manager.Do(cmd4);
                context.Value.Should().Be(cmd4.NewValue);
            }

            for (int i = 0; i < 9; i++)
            {
                manager.Undo().Should().NotBeNull();
                context.Value.Should().Be(cmd3.NewValue);
                manager.Undo().Should().NotBeNull();
                context.Value.Should().Be(cmd2.NewValue);
                manager.Undo().Should().NotBeNull();
                context.Value.Should().Be(cmd1.NewValue);
                manager.Undo().Should().NotBeNull();
                context.Value.Should().Be(cmd4.NewValue);
            }

            for (int i = 0; i < 9; i++)
            {
                manager.Redo().Should().NotBeNull();
                context.Value.Should().Be(cmd1.NewValue);
                manager.Redo().Should().NotBeNull();
                context.Value.Should().Be(cmd2.NewValue);
                manager.Redo().Should().NotBeNull();
                context.Value.Should().Be(cmd3.NewValue);
                manager.Redo().Should().NotBeNull();
                context.Value.Should().Be(cmd4.NewValue);
            }
        }
        }
}