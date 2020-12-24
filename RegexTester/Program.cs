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
            Regex regexItalicsAndHI = new Regex(@"<i>\-\s+</i>", RegexOptions.Compiled);
            string input = "<i>- </i> What?";
            Console.WriteLine(input);
            Console.WriteLine(regexItalicsAndHI.Replace(input, "- "));
        }
    }
}
