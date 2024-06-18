using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Interfaces
{
    public interface IConsoleWriter
    {
        void WriteLine(string message);
        void SetForegroundColor(ConsoleColor color);
        void ResetColor();
        void ReadKey();
    }
}
