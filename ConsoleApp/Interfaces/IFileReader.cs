using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Interfaces
{
    public interface IFileReader
    {
        string[] ReadAllLines(string path);
        string[] Split(string line, char delimiter);
    }
}
