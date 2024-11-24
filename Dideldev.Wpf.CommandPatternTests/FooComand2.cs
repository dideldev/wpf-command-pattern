namespace Dideldev.Wpf.CommandPattern.Tests
{
    /// <summary>
    /// Mock command implementation to change <see cref="FooContext.Value"/>.
    /// </summary>
    /// <param name="newValue"></param>
    /// <param name="oldValue"></param>
    public class FooComand2 : Command<FooContext>
    {
        public int  IntNewValue { get; set; } = 0;
        public int IntOldValue { get; set; } = 0;

        public FooComand2() { }

        public FooComand2(int newValue)
        {
            this.IntNewValue = newValue;
        }

        public override void Do(FooContext? Context)
        {
            if (Context == null) return;
            IntOldValue = Context.IntValue;
            Context.IntValue = IntNewValue;
        }


        public override void Undo(FooContext? Context)
        {
            if (Context == null) return;
            Context.IntValue = IntOldValue;
        }

    }
}