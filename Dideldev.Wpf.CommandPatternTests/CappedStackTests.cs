namespace Dideldev.Wpf.CommandPattern.Tests
{
    [TestFixture()]
    public class CappedStackTests
    {
        [Test()]
        public void CappedStackTest()
        {
            CappedStack<int> cappedStack = new(5);
            cappedStack.Should().NotBeNull().And.BeEmpty("No elements pushed yet.");
            cappedStack.MaxCount.Should().Be(5);
        }

        [Test()]
        public void PopAndClearTest()
        {
            CappedStack<int> cappedStack = new(5);
            cappedStack.Push(1);
            cappedStack.Push(2);
            cappedStack.HasItems.Should().BeTrue();
            cappedStack.Pop().Should().Be(2);
            cappedStack.Pop().Should().Be(1);
            cappedStack.Should().BeEmpty();
            cappedStack.HasItems.Should().BeFalse();
            cappedStack.Clear();
            cappedStack.Should().BeEmpty("Just cleared");

            Action popWhenNoItems = () => { cappedStack.Pop(); };
            popWhenNoItems.Should().Throw<InvalidOperationException>();
        }

        [Test()]
        public void PushTest()
        {
            CappedStack<int> cappedStack = new(5);
            for (int i = 0; i < 10; i++)
                cappedStack.Push(i);
            cappedStack.HasItems.Should().BeTrue();
            cappedStack.Count.Should().Be(5);
            cappedStack.Items[0].Should().Be(5);
            cappedStack.Items[1].Should().Be(6);
        }
    }
}