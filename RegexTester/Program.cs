using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using SubtitlesCleaner.Library;

namespace RegexTester
{
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
        }

        private static void TestDictionary(string word)
        {
            Console.WriteLine(word);
            bool inDictionary = DictionaryHelper.CheckWord(word);
            Console.WriteLine("in dictionary: " + inDictionary);
            if (inDictionary == false)
            {
                var suggestions = DictionaryHelper.GetSuggestions(word);
                if (suggestions.HasAny())
                {
                    Console.WriteLine("Suggestions:");
                    foreach (var suggestion in suggestions)
                        Console.WriteLine(suggestion);
                }
            }
        }

        private static void TestRegex()
        {
            string input = @"Lyrics♪</i>";

            Regex regex = new Regex(@"(?<Lyrics>[^ ♪])(?<Suffix>♪+(?:</i>)?)$");

            FindAndReplace far = new FindAndReplace(
                regex,
                "${Lyrics} ${Suffix}",
                SubtitleError.Missing_Spaces
            );

            Print(input, far);
        }

        private static void Print(string input, FindAndReplace far)
        {
            Print(input, far.Regex);

            Console.WriteLine("Clean Input:");
            string cleanInput = far.CleanLine(input);
            Console.WriteLine(cleanInput);
            Console.WriteLine();
        }

        private static void Print(string input, Regex regex)
        {
            Console.WriteLine("Input:");
            RegexHelper.PrintInput(input);

            Console.WriteLine("Is Match With Regex:");
            RegexHelper.PrintIsMatch(input, regex);

            Console.WriteLine("Has Warnings:");
            bool hasWarnings = SubtitlesHelper.HasWarnings(input);
            Console.WriteLine(hasWarnings ? "Has Warnings" : "No Warnings");
            Console.WriteLine();
        }
    }
}
