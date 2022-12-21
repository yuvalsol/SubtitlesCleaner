using System;
using System.Text;
using System.Text.RegularExpressions;
using SubtitlesCleanerLibrary;

namespace RegexTester
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            FindAndReplace far = new FindAndReplace(
                new Regex(@"(?<Lyrics>[^ ♪])(?<Suffix>♪+(?:</i>)?)$"), 
                "${Lyrics} ${Suffix}", 
                SubtitleError.Missing_Spaces
            );

            string input = @"Lyrics♪</i>";
            Console.WriteLine("Input:");
            RegexHelper.PrintInput(input);

            Console.WriteLine("Is Match With Regex:");
            RegexHelper.PrintIsMatch(input, far.Regex);

            Console.WriteLine("Has Errors:");
            bool hasErrors = SubtitlesHelper.HasErrors(input);
            Console.WriteLine(hasErrors ? "Has Errors" : "No Errors");
            Console.WriteLine();

            Console.WriteLine("Clean Input:");
            string cleanInput = far.CleanLine(input);
            Console.WriteLine(cleanInput);
        }
    }
}
