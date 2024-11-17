namespace Dideldev.Wpf.CommandPattern.Tests
{
    /// <summary>
    /// Mock command implementation to change <see cref="FooContext.Value"/>.
    /// </summary>
    /// <param name="newValue"></param>
    /// <param name="oldValue"></param>
    public class FooComand(string newValue = "", string oldValue = "") : Command<FooContext>
    {
        public string NewValue { get; set; } = newValue;
        public string OldValue { get; set; } = oldValue;

        public override void Do(FooContext? Context)
        {
            if (Context == null) return;
            Context.Value = NewValue;
        }

        public override void ReadParameterBytes(BinaryReader br)
        {
            NewValue = br.ReadString();
            OldValue = br.ReadString();
        }

        public override void Undo(FooContext? Context)
        {
            if (Context == null) return;
            Context.Value = OldValue;
        }

        public override void WriteParameterBytes(BinaryWriter bw)
        {
            bw.Write(NewValue);
            bw.Write(OldValue);
        }
    }
}