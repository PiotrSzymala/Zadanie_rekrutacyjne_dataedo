using ConsoleApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Services
{
    public class ConsoleWriter : IConsoleWriter
    {
        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="message">The value to write.</param>
        public void WriteLine(string message) => Console.WriteLine(message);

        /// <summary>
        /// Sets the foreground color of the console.
        /// </summary>
        /// <param name="color">The color to set.</param>
        public void SetForegroundColor(ConsoleColor color) => Console.ForegroundColor = color;

        /// <summary>
        /// Resets the console foreground and background colors to their defaults.
        /// </summary>
        public void ResetColor() => Console.ResetColor();
       
        /// <summary>
        /// Obtains the next character or function key pressed by the user. The pressed key is displayed in the console window.
        /// </summary>
        public void ReadKey() => Console.ReadKey();
    }
}
