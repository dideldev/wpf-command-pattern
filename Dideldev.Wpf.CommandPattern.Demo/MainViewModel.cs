using Dideldev.Wpf.CommandPattern.Demo.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using Dideldev.Wpf.CommandPattern;

namespace Dideldev.Wpf.CommandPattern.Demo
{

    /// <summary>
    /// View model to show the behavieur of <see cref="DiskCommandManager{T}"/>.
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Model that is changed by the commands.
        /// </summary>
        private Model model;

        /// <summary>
        /// Manager that manage Undo/Redo actions of our commands.
        /// </summary>        
        DiskCommandManager<Model> cmdManager;

        /// <summary>
        /// Implementation of <see cref="INotifyPropertyChanged.PropertyChanged"/>.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary> Commands on the previous chunk of executed commands.</summary>
        public ObservableCollection<DebugCommandItem> ListPrevious { get; set; } = [];

        /// <summary> Commands on the current chunk of executed commands.</summary>
        /// <remarks> This commands may be executed or undone.</remarks>
        public ObservableCollection<DebugCommandItem> ListCurrent { get; set; } = [];

        /// <summary> Commands on the next chunk of undone commands.</summary>
        public ObservableCollection<DebugCommandItem> ListNext { get; set; } = [];

        /// <summary>
        /// List of executed command files the <see cref="CommandManager"/>> has generated.
        /// </summary>
        public ObservableCollection<string> ExecFilesOnFolder { get; set; } = [];

        /// <summary>
        /// List of undone commands files the <see cref="CommandManager"/>> has generated.
        /// </summary>
        public ObservableCollection<string> UndoFilesOnFolder { get; set; } = [];

        /// <summary>
        /// List of colors to feed our combobox.
        /// </summary>
        public Dictionary<Brush, string> AvailableColors => new()
        {
            {Brushes.Black,nameof(Colors.Black)},
            {Brushes.White,nameof(Colors.White)},
            {Brushes.Red,nameof(Colors.Red)},
            {Brushes.Green,nameof(Colors.Green)},
            {Brushes.Blue,nameof(Colors.Blue)},
            {Brushes.Yellow,nameof(Colors.Yellow)},
            {Brushes.Orange,nameof(Colors.Orange)},
            {Brushes.Purple,nameof(Colors.Purple)},
        };

        /// <summary>
        /// Initializes a new instance of <see cref="MainViewModel"/>.
        /// </summary>
        public MainViewModel()
        {
            this.model = new Model()
            {
                PressedKey = "A",
                Background = Colors.White,
                Foreground = Colors.Black,
            };

            NotifyChange(nameof(PressedKey));
            NotifyChange(nameof(Background));
            NotifyChange(nameof(Foreground));

            cmdManager = new DiskCommandManager<Model>(model, new DiskCommandManagerConfig
            {
                // Limit the size to a small value to see how files are being loaded and saved. 
                // For a real proyect this should be higher.
                ListSize = 10
            });
        }

        /// <summary>
        /// Get or sets the last pressed key. 
        /// </summary>
        public string PressedKey
        {
            get => this.model.PressedKey;
            set
            {
                if (this.model.PressedKey == value)
                    return;

                cmdManager.Do(new SetPressedKeyCommand(value,nameof(PressedKey)));
                NotifyChange();
                UpdateLists();
            }
        }

        /// <summary>
        /// Get or sets the background of the PressedKey text. 
        /// </summary>
        public Brush Background
        {
            get
            {
                return AvailableColors.Keys.Where(k => k.ToString() == this.model.Background.ToString()).FirstOrDefault()
                    ?? AvailableColors.Keys.First();
            }
            set
            {
                System.Windows.Media.Color newColor = ((SolidColorBrush)value).Color;
                if (this.model.Background == newColor)
                    return;
                cmdManager.Do(new SetBackgroundCommand(newColor, nameof(Background)));
                NotifyChange();
                UpdateLists();
            }
        }

        /// <summary>
        /// Get or sets the foreground of the PressedKey text. 
        /// </summary>
        public Brush Foreground
        {
            get
            {
                return AvailableColors.Keys.Where(k => k.ToString() == this.model.Foreground.ToString()).FirstOrDefault()
                    ?? AvailableColors.Keys.First();
            }
            set
            {
                System.Windows.Media.Color newColor = ((SolidColorBrush)value).Color;
                if (this.model.Foreground == newColor)
                    return;
                cmdManager.Do(new SetForegroundCommand(newColor, nameof(Foreground)));
                NotifyChange();
                UpdateLists();
            }
        }

        /// <summary>
        /// Listen to the control/windows keys.
        /// </summary>
        public void PressKey(bool ctrlPressed, Key key)
        {
            if (ctrlPressed)
            {
                CheckUndoRedoKey(key);
                return;
            }

            PressedKey = key.ToString();
        }

        /// <summary>
        /// Chekc if Undo o Redo shortcuts have been pressed. 
        /// </summary>
        /// <param name="key"></param>
        private void CheckUndoRedoKey(Key key)
        {
            Command<Model>? cmd;
            switch (key)
            {
                case Key.Z:
                    cmd = cmdManager.Undo();
                    UpdateLists();
                    break;
                case Key.Y:
                    cmd = cmdManager.Redo();
                    UpdateLists();
                    break;
                default:
                    return;
            }

            if (cmd == null || cmd.PropertyNames == null)
                return;

            foreach (string prop in cmd.PropertyNames)
            {
                NotifyChange(prop);
            }
        }

        /// <summary>
        /// Read the list of the <see cref="CommandManager"/>.
        /// </summary>
        private void UpdateLists()
        {
            if (cmdManager == null)
                return;

            /*
             * Ok, this region looks ugly. 
             * The lists that the command manager uses are private for a reason, and build a demo app to see what's going on 
             * inside the class is not a reason to make it public. 
             * 
             * So reflection is used to get those values, given that we now how those properties are defined. 
             * 
             * Moreover, this is very inefficient as we are reading 3 full list every time a command is Do/Undo..
             */

            List<Command<Model>> listPreviousCommands = [];
            List<Command<Model>> listCurrentCommands = [];
            List<Command<Model>> listUndoneCommands = [];
            int cIndex = 0;

            PropertyInfo? propPreviousCommands = cmdManager.GetType().GetProperty("PreviousExecutedCommands", BindingFlags.Instance | BindingFlags.NonPublic);
            PropertyInfo? propCurrentCommands = cmdManager.GetType().GetProperty("CurrentCommands", BindingFlags.Instance | BindingFlags.NonPublic);
            PropertyInfo? propUndoneCommands = cmdManager.GetType().GetProperty("NextUndoneCommands", BindingFlags.Instance | BindingFlags.NonPublic);
            PropertyInfo? propIndex = cmdManager.GetType().GetProperty("LastExecutedCommandIndex", BindingFlags.Instance | BindingFlags.NonPublic);

            listPreviousCommands = (List<Command<Model>>?)propPreviousCommands?.GetValue(cmdManager) ?? [];
            listCurrentCommands = (List<Command<Model>>?)propCurrentCommands?.GetValue(cmdManager) ?? [];
            listUndoneCommands = (List<Command<Model>>?)propUndoneCommands?.GetValue(cmdManager) ?? [];
            cIndex = (int?)propIndex?.GetValue(cmdManager) ?? 0;

            UpdateList(ListPrevious, listPreviousCommands);
            UpdateList(ListCurrent, listCurrentCommands, cIndex);
            UpdateList(ListNext, listUndoneCommands);
            NotifyChange(nameof(ListPrevious));
            NotifyChange(nameof(ListCurrent));
            NotifyChange(nameof(ListNext));

            ExecFilesOnFolder.Clear();
            UndoFilesOnFolder.Clear();

            if (!Directory.Exists(cmdManager.Config.Folder))
                return;

            foreach (var file in Directory.EnumerateFiles(cmdManager.Config.Folder, $"{cmdManager.Config.ExecPrefix}*"))
            {
                ExecFilesOnFolder.Add(Path.GetFileName(file));
            }

            
            foreach (var file in Directory.EnumerateFiles(cmdManager.Config.Folder, $"{cmdManager.Config.UndoPrefix}*"))
            {
                UndoFilesOnFolder.Add(Path.GetFileName(file));
            }
        }

        /// <summary>
        /// Fill a list exposed by the view model with the commands on a list. 
        /// </summary>
        /// <param name="debugList"></param>
        /// <param name="cmdList"></param>
        /// <param name="selectedItem"></param>
        private static void UpdateList(ObservableCollection<DebugCommandItem> debugList, List<Command<Model>> cmdList, int selectedItem = -2)
        {
            debugList.Clear();
            string?[] cmds = cmdList.Select(c => c.ToString()).Reverse().ToArray();
            for (int i = 0; i < cmds.Length; i++)
            {
                DebugCommandItem item = new()
                {
                    Name = cmds[i] ?? ""
                };

                if (selectedItem >= -1)
                {
                    int reversedIndex = cmds.Length - 1 - i;

                    item.LastExecuted = reversedIndex == selectedItem;
                    item.Undone = reversedIndex > selectedItem;
                }

                debugList.Add(item);
            }
        }

        protected void NotifyChange([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
