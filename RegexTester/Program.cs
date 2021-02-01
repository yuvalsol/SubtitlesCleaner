using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RegexTester
{
    class Program
    {
        static void Main(string[] args)
        {
            string input = "< <i>It's something big.</i> >";
            PrintInput(input);

            Regex regexAngleBracketLeft = new Regex(@"<(?!/?i>)", RegexOptions.Compiled);
            PrintMatches(regexAngleBracketLeft.Matches(input));

            Regex regexAngleBracketRight = new Regex(@"(?<!</?i)>", RegexOptions.Compiled);
            PrintMatches(regexAngleBracketRight.Matches(input));
        }

        private static void PrintMatches(MatchCollection matches)
        {
            foreach (Match match in matches)
            {
                Console.WriteLine(match.Value);
                Console.WriteLine("I: {0}, L: {1}", match.Index, match.Length);
                Console.WriteLine();
            }
        }

        private static void PrintInput(string input)
        {
            if (input.Length > 10)
            {
                for (int i = 0; i < input.Length; i++)
                {
                    if (i % 10 == 0 && i / 10 > 0)
                        Console.Write(i / 10);
                    else
                        Console.Write(" ");
                }
                Console.WriteLine();
            }

            for (int i = 0; i < input.Length; i++)
                Console.Write(i % 10);
            Console.WriteLine();

            Console.WriteLine(input);
            Console.WriteLine();
        }
    }
}
