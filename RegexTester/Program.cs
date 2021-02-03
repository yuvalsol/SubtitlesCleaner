using System;
using System.Text.RegularExpressions;

namespace RegexTester
{
    class Program
    {
        static void Main(string[] args)
        {
            string input = "Morn. Your morn's awkward. My morn. morning. Morning.";
            RegexHelper.PrintInput(input);

            Regex regex = new Regex(@"\b(?i:m)orn\b", RegexOptions.Compiled);
            RegexHelper.PrintIsMatch(input, regex);
        }
    }
}
