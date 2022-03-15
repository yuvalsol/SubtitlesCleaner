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

            FindAndReplace far = 
                new FindAndReplace(new Regex(@"(?<Lyrics>[^ ♪])(?<Suffix>♪+(?:</i>)?)$"), "${Lyrics} ${Suffix}", SubtitleError.Missing_Spaces);

            string line = @"Lyrics♪</i>";
            RegexHelper.PrintInput(line);

            RegexHelper.PrintIsMatch(line, far.Regex);

            Console.WriteLine(far.CleanLine(line));
        }

        private static void PrintHasErrors(string input)
        {
            bool hasErrors = SubtitlesHelper.HasErrors(input);
            Console.WriteLine(hasErrors ? "Has Errors" : "No Errors");
            Console.WriteLine();
        }
    }
}
