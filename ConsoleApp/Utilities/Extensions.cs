using System;

namespace ConsoleApp.Utilities
{
    public static class Extensions
    {
        public static string Clear(this string input)
        {
            return input.Trim().Replace(Environment.NewLine, "");
        }
    }
}
