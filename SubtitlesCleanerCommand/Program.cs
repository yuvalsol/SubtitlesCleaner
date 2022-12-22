using System;
using System.Reflection;
using System.Text;
using CommandLine;
using CommandLine.Text;

namespace SubtitlesCleanerCommand
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.OutputEncoding = Encoding.UTF8;

                if (SubtitlesHandler.IsProduction)
                {
                    var parser = new Parser(with => with.HelpWriter = null);
                    var parserResult = parser.ParseArguments<
                        CleanOptions,
                        AddTimeOptions,
                        SetShowTimeOptions,
                        AdjustTimingOptions,
                        ReorderOptions,
                        BalanceLinesOptions>(args);

                    parserResult
                        .WithParsed<CleanOptions>(SubtitlesHandler.Clean)
                        .WithParsed<AddTimeOptions>(SubtitlesHandler.AddTime)
                        .WithParsed<SetShowTimeOptions>(SubtitlesHandler.SetShowTime)
                        .WithParsed<AdjustTimingOptions>(SubtitlesHandler.AdjustTiming)
                        .WithParsed<ReorderOptions>(SubtitlesHandler.Reorder)
                        .WithParsed<BalanceLinesOptions>(SubtitlesHandler.BalanceLines)
                        .WithNotParsed(errors =>
                        {
                            if (errors.IsVersion())
                            {
                                Console.WriteLine(
                                    "Subtitles Cleaner Command, Version" + " " +
                                    Assembly.GetExecutingAssembly().GetName().Version.ToString(3)
                                );
                                Console.WriteLine();
                            }
                            else if (errors.IsHelp())
                            {
                                Console.WriteLine(
                                    "Subtitles Cleaner Command, Version" + " " +
                                    Assembly.GetExecutingAssembly().GetName().Version.ToString(3)
                                );
                                Console.WriteLine();
                                Console.WriteLine("USAGE:");
                                Console.WriteLine();
                                Console.WriteLine("Clean");
                                Console.WriteLine("SubtitlesCleanerCommand.exe clean [--cleanHICaseInsensitive]");
                                Console.WriteLine("                                  [--firstSubtitlesCount n]");
                                Console.WriteLine("                                  --path fileOrFolderPath");
                                Console.WriteLine("                                  (--print|--save [--outFile file] [--outPath path])");
                                Console.WriteLine();
                                Console.WriteLine("Add Time");
                                Console.WriteLine("SubtitlesCleanerCommand.exe addTime --timeAdded (+00:00:00,000|-00:00:00,000)");
                                Console.WriteLine("                                    [--subtitleNumber n]");
                                Console.WriteLine("                                    [--firstSubtitlesCount n]");
                                Console.WriteLine("                                    --path fileOrFolderPath");
                                Console.WriteLine("                                    (--print|--save [--outFile file] [--outPath path])");
                                Console.WriteLine();
                                Console.WriteLine("Set Show Time");
                                Console.WriteLine("SubtitlesCleanerCommand.exe setShowTime --showTime 00:00:00,000");
                                Console.WriteLine("                                        [--subtitleNumber n]");
                                Console.WriteLine("                                        [--firstSubtitlesCount n]");
                                Console.WriteLine("                                        --path fileOrFolderPath");
                                Console.WriteLine("                                        (--print|--save [--outFile file] [--outPath path])");
                                Console.WriteLine();
                                Console.WriteLine("Adjust Timing");
                                Console.WriteLine("SubtitlesCleanerCommand.exe adjustTiming --firstShowTime 00:00:00,000");
                                Console.WriteLine("                                         --lastShowTime 00:00:00,000");
                                Console.WriteLine("                                         [--firstSubtitlesCount n]");
                                Console.WriteLine("                                         --path fileOrFolderPath");
                                Console.WriteLine("                                         (--print|--save [--outFile file] [--outPath path])");
                                Console.WriteLine();
                                Console.WriteLine("Reorder");
                                Console.WriteLine("SubtitlesCleanerCommand.exe reorder --path fileOrFolderPath (--print|--save)");
                                Console.WriteLine();
                                Console.WriteLine("Balance Lines");
                                Console.WriteLine("SubtitlesCleanerCommand.exe balanceLines --path fileOrFolderPath (--print|--save)");
                                Console.WriteLine();
                            }
                            else
                            {
                                HelpText helpText = HelpText.AutoBuild(parserResult, h =>
                                {
                                    h.Heading =
                                        "Subtitles Cleaner Command, Version" + " " +
                                        Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
                                    h.Copyright = string.Empty;
                                    h.AdditionalNewLineAfterOption = false;
                                    h.MaximumDisplayWidth = 120;
                                    return HelpText.DefaultParsingErrorsHandler(parserResult, h);
                                }, e => e);

                                Console.WriteLine(helpText);
                            }
                        });
                }
                else
                {
                    SubtitlesHandler.Debug();
                }
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
