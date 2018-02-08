namespace ProactiveBot.Utilities
{
    using System;

    public static class StringHelper
    {
        public static bool Equals(string firstString, string secondString)
        {
            firstString = firstString.Trim();
            secondString = secondString.Trim();
            return String.Equals(firstString.Trim(), secondString, StringComparison.OrdinalIgnoreCase);
        }
    }
}