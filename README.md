# wpf-command-pattern

Implementation of Command Design Pattern for WPF projects.

It offers some Command managers to manage the memmory in a way that better suits the proyect. 
* CommandManager: Standard implementation of two executed/undone commands stacks.
* CappedCountCommandManager: Like CommandManager but with maximum executed commands stored in the stack.
* DiskCommandManager: Is able to store commands on disk. It sets a maximum size for the stacks in memory and saves on disk the ones that are not needed, loading them when it has to.

![Demo image](Others/Demo.gif)

## Use

First create some commands:
```c#
public class SetStringToValue : Command<FooContext>
{
    public string NewValue { get; set; } = string.Empty;
    public string OldValue { get; set; } = string.Empty;

    public SetStringToValue() { }

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
```

This commands must have a parameterless constructor and public setters to its properties, as it will be instanciated by the serializer and then filled with the values of its properties. 

Declare a manager: 
```c#
FooContext context= new FooContext();
DiskCommandManager<FooContext> manager = new (context);
```

Then invoke commands through the manager:

```c#
// Foocamnd adds a 
SetStringToValue setHello = new("Hello");
SetStringToValue setSpace = new(" ");
SetStringToValue setWorld = new("World");
SetStringToValue setExclamation = new("!");

manager.Do(setHello) // Context.Value now is "Hello"
manager.Do(setSpace) // Context.Value now is " "
manager.Do(setWorld) // Context.Value now is "World"
manager.Do(setExclamation) // Context.Value now is "!"
manager.Undo() // Context.Value now is "World"
manager.Undo() // Context.Value now is " "
manager.Redo() // Context.Value now is "World"
manager.Redo() // Context.Value now is "!"
manager.Redo() // Context.Value now is "!", no more commands to redo
```

## Command to file serializers

You can change the behaviour of the command manager giving a configuration to its constructor. 
The default serializer is a JsonCommandSerailizer, but it can be changed. 

```c#
DiskCommandManagerConfig cfg = new(){
    Folder = "temp/rollbacks",
    ListSize = 1000;
    Serializer = new XmlCommandSerializer([
        typeof(FooCommand1),
        typeof(FooCommand2),
        typeof(FooCommand3),
    ]);
}

DiskCommandManager<FooContext> manager = new (context, config);
```
NOTE: The xmlSerializer (currently) must know the types to be serialized and deserialized so the Polymorphism works. Im working to add this dynamicaly using Reflection.
