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
            Regex regexCapitalLetter = new Regex(@"[A-ZÁ-Ú]", RegexOptions.Compiled);
            string input = "a";
            bool isCapitalLetter = regexCapitalLetter.IsMatch(input);
            Console.WriteLine($"isCapitalLetter: {isCapitalLetter}");
        }
    }
}
