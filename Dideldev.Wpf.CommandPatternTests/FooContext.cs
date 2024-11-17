namespace Dideldev.Wpf.CommandPatternTests
{
    /// <summary>
    /// Mock class to test commands changing its value;
    /// </summary>
    /// <param name="value"></param>
    public class FooContext(string value = "")
    {
        public string Value { get; set; } = value;
    }
}
