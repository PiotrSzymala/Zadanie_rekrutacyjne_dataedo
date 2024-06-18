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
        public void WriteLine(string message) => Console.WriteLine(message);
        public void SetForegroundColor(ConsoleColor color) => Console.ForegroundColor = color;
        public void ResetColor() => Console.ResetColor();
        public void ReadKey() => Console.ReadKey();
    }
}
