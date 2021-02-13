using System;
using System.Text.RegularExpressions;
using SubtitlesCL;

namespace RegexTester
{
    class Program
    {
        static void Main(string[] args)
        {
            string input = "10:30 tonight.";
            RegexHelper.PrintInput(input);

            bool hasErrors = SubtitlesHelper.HasErrors(input);
            Console.WriteLine(hasErrors ? "Has Errors" : "No Errors");
            Console.WriteLine();

            RegexHelper.PrintIsMatch(input, SubtitlesHelper.regexColonStartLine);
            RegexHelper.PrintIsMatch(input, SubtitlesHelper.regexColonStartLineExclude);
        }
    }
}
