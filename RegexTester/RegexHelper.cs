using System;
using System.Text.RegularExpressions;

namespace RegexTester
{
    public static class RegexHelper
    {
        public static void PrintInput(string input)
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

        public static void PrintMatches(string input, Regex regex)
        {
            foreach (Match match in regex.Matches(input))
            {
                Console.WriteLine(match.Value);
                Console.WriteLine("Index: {0}, Length: {1}", match.Index, match.Length);

                Console.Write(input.Substring(0, match.Index));

                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.Red;
                Console.Write(input.Substring(match.Index, match.Length));
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.BackgroundColor = ConsoleColor.Black;

                Console.Write(input.Substring(match.Index + match.Length));
                Console.WriteLine();

                Console.WriteLine();
            }
        }

        public static bool PrintIsMatch(string input, params Regex[] regexes)
        {
            return PrintIsMatch(input, false, regexes);
        }

        public static bool PrintIsMatch(string input, bool isShowOnlySuccess, params Regex[] regexes)
        {
            bool isAnyMatch = false;

            foreach (Regex regex in regexes)
            {
                bool isMatch = regex.IsMatch(input);
                isAnyMatch |= isMatch;
                if (isShowOnlySuccess == false || isMatch)
                {
                    Console.WriteLine("Regex: {0}", regex.ToString());
                    Console.WriteLine("Is Match: {0}", isMatch);
                    Console.WriteLine();
                    if (isMatch)
                        PrintMatches(input, regex);
                }
            }

            return isAnyMatch;
        }
    }
}
