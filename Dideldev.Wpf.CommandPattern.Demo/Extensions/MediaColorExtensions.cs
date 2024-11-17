using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Dideldev.Wpf.CommandPattern.Demo.Extensions
{
    public static class MediaColorExtensions
    {
        /// <summary>
        /// Writes on a binarystream the bytes to serialize the color value.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="bw"></param>
        public static void WriteBytes(this System.Windows.Media.Color color, BinaryWriter bw)
        {
            bw.Write(color.A);
            bw.Write(color.R);
            bw.Write(color.G);
            bw.Write(color.B);
        }

        /// <summary>
        /// Gets the color defined by ther ARGB bytes on a binary reader.
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        public static System.Windows.Media.Color GetColorFromBytes(BinaryReader br)
        {
            byte a = br.ReadByte();
            byte r = br.ReadByte();
            byte g = br.ReadByte();
            byte b = br.ReadByte();
            return System.Windows.Media.Color.FromArgb(a, r, g, b);
        }

    }
}
