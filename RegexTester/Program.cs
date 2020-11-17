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
            //string input = "-[laughs]: Mmm.";
            //string input = "- : Mmm.";
            string input = "<i>   -    :    Mmm.";

            Regex regexHIPrefix_Z = new Regex(@"^(?:\s*<i>)?\s*-\s*:\s*", RegexOptions.Compiled);

            Console.WriteLine(input);
            Console.WriteLine(regexHIPrefix_Z.IsMatch(input));
            Console.WriteLine(regexHIPrefix_Z.Replace(input, string.Empty));

        }
    }
}
