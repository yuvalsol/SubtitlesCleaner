using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RegexTester
{
    class Program
    {
        private static readonly Regex regexBrackets = new Regex(@"[\({\[~\]}\)]", RegexOptions.Compiled);
        private static readonly Regex regexAngleBracketLeft = new Regex(@"<(?!/?i>)", RegexOptions.Compiled);
        private static readonly Regex regexAngleBracketRight = new Regex(@"(?<!</?i)>", RegexOptions.Compiled);
        private static readonly Regex regexColonStartLine = new Regex(@"^[A-ZÁ-Úa-zá-ú0-9#\-'.]+:", RegexOptions.Compiled);
        private static readonly Regex regexColon = new Regex(@"[A-ZÁ-Úa-zá-ú0-9#\-'.]+:\s", RegexOptions.Compiled);
        // Course 1 can
        private static readonly Regex regexOneInsteadOfI = new Regex(@"[A-ZÁ-Úa-zá-ú]\s+(1)\s+[A-ZÁ-Úa-zá-ú]", RegexOptions.Compiled);
        // a/b
        private static readonly Regex regexSlash = new Regex(@"[A-ZÁ-Úa-zá-ú]/[A-ZÁ-Úa-zá-ú]", RegexOptions.Compiled);
        // " / " -> " I "
        private static readonly Regex regexSlashInsteadOfI = new Regex(@"\s+/\s+", RegexOptions.Compiled);
        // replace with new line
        private static readonly Regex regexMissingSpace = new Regex(@"[!?][A-ZÁ-Úa-zá-ú]", RegexOptions.Compiled);

        private static readonly Regex regexHIWithoutBracket = new Regex(@"^[A-ZÁ-Ú]+$", RegexOptions.Compiled);

        private const string HIChars = @"A-ZÁ-Ú0-9 #\-'.";
        private static readonly Regex regexHIFullLineWithoutBrackets = new Regex(@"^[" + HIChars + @"]+$", RegexOptions.Compiled);

        static void Main(string[] args)
        {
            string input = "I...";
            PrintInput(input);

            PrintIsMatch(input, true,
                regexBrackets,
                regexAngleBracketLeft,
                regexAngleBracketRight,
                regexColonStartLine,
                regexColon,
                regexOneInsteadOfI,
                regexSlash,
                regexSlashInsteadOfI,
                regexMissingSpace,
                regexHIWithoutBracket,
                regexHIFullLineWithoutBrackets
            );
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

        private static void PrintMatches(MatchCollection matches)
        {
            foreach (Match match in matches)
            {
                Console.WriteLine(match.Value);
                Console.WriteLine("I: {0}, L: {1}", match.Index, match.Length);
                Console.WriteLine();
            }
        }

        private static bool PrintIsMatch(string input, params Regex[] regexes)
        {
            return PrintIsMatch(input, false, regexes);
        }

        private static bool PrintIsMatch(string input, bool isShowOnlySuccess, params Regex[] regexes)
        {
            bool isAnyMatch = false;

            foreach (Regex regex in regexes)
            {
                bool isMatch = regex.IsMatch(input);
                isAnyMatch |= isMatch;
                if (isShowOnlySuccess == false || isMatch)
                {
                    Console.WriteLine(regex.ToString());
                    Console.WriteLine("Is Match: {0}", isMatch);
                    Console.WriteLine();
                }
            }

            return isAnyMatch;
        }
    }
}
