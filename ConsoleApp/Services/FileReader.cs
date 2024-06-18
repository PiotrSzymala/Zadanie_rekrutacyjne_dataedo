using ConsoleApp.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Services
{
    public class FileReader : IFileReader
    {
        /// <summary>
        /// Reads all lines from the specified path.
        /// </summary>
        /// <param name="path">The file path to read from.</param>
        /// <returns>An array of strings, each representing a line in the file.</returns>
        public string[] ReadAllLines(string path)
        {
            try
            {
                return File.ReadAllLines(path);
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to read from the file: {path}", ex);
            }
        }

        /// <summary>
        /// Splits a single string line using the specified delimiter.
        /// </summary>
        /// <param name="line">The string to split.</param>
        /// <param name="delimiter">The character to use as a delimiter.</param>
        /// <returns>An array of strings that contains the substrings of the line that are delimited by the specified character.</returns>
        public string[] Split(string line, char delimiter)
        {
            return line.Split(delimiter);
        }
    }
}
