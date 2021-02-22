using System;
using System.Text.RegularExpressions;
using SubtitlesCL;

namespace RegexTester
{
    class Program
    {
        static void Main(string[] args)
        {
            string input = @"house no later than 5:30.";
            RegexHelper.PrintInput(input);

            PrintHasErrors(input);

            RegexHelper.PrintIsMatch(input, SubtitlesHelper.regexColonStartLine);
            RegexHelper.PrintIsMatch(input, SubtitlesHelper.regexColonStartLineExclude);
        }

        private static void PrintHasErrors(string input)
        {
            bool hasErrors = SubtitlesHelper.HasErrors(input);
            Console.WriteLine(hasErrors ? "Has Errors" : "No Errors");
            Console.WriteLine();
        }
    }
}
