using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Dideldev.Wpf.CommandPattern.Demo
{
    /// <summary>
    /// Simple model with some properties to be changed by commands. 
    /// </summary>
    public class Model
    {
        /// <summary> Text of the last pressed key. </summary>
        public string PressedKey { get; set; }

        /// <summary> Foreground of the text.</summary>
        public Color Foreground { get; set; }

        /// <summary> Background of the text.</summary>
        public Color Background { get; set; }

        /// <summary> Initializes a new instance of <see cref="Model"/>.</summary>
        public Model()
        {
            PressedKey = string.Empty;
            Foreground = Colors.Black;
            Background = Colors.White;
        }
    }
}
