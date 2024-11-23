namespace Dideldev.Wpf.CommandPattern.Tests
{
    /// <summary>
    /// Mock command implementation to change <see cref="FooContext.Value"/>.
    /// </summary>
    /// <param name="newValue"></param>
    /// <param name="oldValue"></param>
    public class FooComand : Command<FooContext>
    {
        public string NewValue { get; set; } = string.Empty;
        public string OldValue { get; set; } = string.Empty;

        public FooComand() { }

        public FooComand(string newValue)
        {
            this.NewValue = newValue;
        }

        public override void Do(FooContext? Context)
        {
            if (Context == null) return;
            OldValue = Context.Value;
            Context.Value = NewValue;
        }


        public override void Undo(FooContext? Context)
        {
            if (Context == null) return;
            Context.Value = OldValue;
        }

    }
}