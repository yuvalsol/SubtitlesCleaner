using System;
using System.Text;
using System.Text.RegularExpressions;
using SubtitlesCL;

namespace RegexTester
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            string input = @"""Forty-two errors""?";
            RegexHelper.PrintInput(input);

            PrintHasErrors(input);

            RegexHelper.PrintIsMatch(input, SubtitlesHelper.regexDoubleQuateAndQuestionMark);
        }

        private static void PrintHasErrors(string input)
        {
            bool hasErrors = SubtitlesHelper.HasErrors(input);
            Console.WriteLine(hasErrors ? "Has Errors" : "No Errors");
            Console.WriteLine();
        }
    }
}
