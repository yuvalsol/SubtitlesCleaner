using System;

namespace SubtitlesApp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                new SubtitlesHandler().Run(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine("_________________________________________");
                while (ex != null)
                {
                    Console.WriteLine(string.Format("{0}\n{1}\n_________________________________________", ex.GetType().ToString(), ex.Message));
                    ex = ex.InnerException;
                }
            }
            finally
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    Console.WriteLine("Press any key to continue . . .");
                    Console.ReadKey(true);
                }
            }
        }
    }
}
