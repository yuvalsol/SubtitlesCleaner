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
                        BalanceLinesOptions,
                        UsageOptions>(args);

                    parserResult
                        .WithParsed<CleanOptions>(SubtitlesHandler.Clean)
                        .WithParsed<AddTimeOptions>(SubtitlesHandler.AddTime)
                        .WithParsed<SetShowTimeOptions>(SubtitlesHandler.SetShowTime)
                        .WithParsed<AdjustTimingOptions>(SubtitlesHandler.AdjustTiming)
                        .WithParsed<ReorderOptions>(SubtitlesHandler.Reorder)
                        .WithParsed<BalanceLinesOptions>(SubtitlesHandler.BalanceLines)
                        .WithParsed<UsageOptions>(options =>
                        {
                            Console.WriteLine(
                                "Subtitles Cleaner Command, Version" + " " +
                                Assembly.GetExecutingAssembly().GetName().Version.ToString(3)
                            );

                            Console.WriteLine();
                            Console.WriteLine("USAGE:");
                            Console.WriteLine();

                            if (options.clean)
                            {
                                Console.WriteLine("Clean");
                                Console.WriteLine("SubtitlesCleanerCommand clean [--cleanHICaseInsensitive]");
                                Console.WriteLine("                              [--firstSubtitlesCount n]");
                                Console.WriteLine("                              --path fileOrFolder");
                                Console.WriteLine("                              (--print|--save [--outputFile file] [--outputFolder folder])");
                                Console.WriteLine("                              [--suppressBackupFile]");
                                Console.WriteLine("                              [--suppressErrorFile]");
                                Console.WriteLine();
                            }

                            if (options.addTime)
                            {
                                Console.WriteLine("Add Time");
                                Console.WriteLine("SubtitlesCleanerCommand addTime --timeAdded (+00:00:00,000|-00:00:00,000)");
                                Console.WriteLine("                                [--subtitleNumber n]");
                                Console.WriteLine("                                [--firstSubtitlesCount n]");
                                Console.WriteLine("                                --path fileOrFolder");
                                Console.WriteLine("                                (--print|--save [--outputFile file] [--outputFolder folder])");
                                Console.WriteLine("                                [--suppressBackupFile]");
                                Console.WriteLine();
                            }

                            if (options.setShowTime)
                            {
                                Console.WriteLine("Set Show Time");
                                Console.WriteLine("SubtitlesCleanerCommand setShowTime --showTime 00:00:00,000");
                                Console.WriteLine("                                    [--subtitleNumber n]");
                                Console.WriteLine("                                    [--firstSubtitlesCount n]");
                                Console.WriteLine("                                    --path fileOrFolder");
                                Console.WriteLine("                                    (--print|--save [--outputFile file] [--outputFolder folder])");
                                Console.WriteLine("                                    [--suppressBackupFile]");
                                Console.WriteLine();
                            }

                            if (options.adjustTiming)
                            {
                                Console.WriteLine("Adjust Timing");
                                Console.WriteLine("SubtitlesCleanerCommand adjustTiming --firstShowTime 00:00:00,000");
                                Console.WriteLine("                                     --lastShowTime 00:00:00,000");
                                Console.WriteLine("                                     [--firstSubtitlesCount n]");
                                Console.WriteLine("                                     --path fileOrFolder");
                                Console.WriteLine("                                     (--print|--save [--outputFile file] [--outputFolder folder])");
                                Console.WriteLine("                                     [--suppressBackupFile]");
                                Console.WriteLine();
                            }

                            if (options.reorder)
                            {
                                Console.WriteLine("Reorder");
                                Console.WriteLine("SubtitlesCleanerCommand reorder --path fileOrFolder");
                                Console.WriteLine("                                (--print|--save)");
                                Console.WriteLine("                                [--suppressBackupFile]");
                                Console.WriteLine();
                            }

                            if (options.balanceLines)
                            {
                                Console.WriteLine("Balance Lines");
                                Console.WriteLine("SubtitlesCleanerCommand balanceLines --path fileOrFolder");
                                Console.WriteLine("                                     (--print|--save)");
                                Console.WriteLine("                                     [--suppressBackupFile]");
                                Console.WriteLine();
                            }
                        })
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
                                    h.AddPreOptionsLine("Execute 'SubtitlesCleanerCommand usage --clean' to print usage for clean");
                                    h.AddNewLineBetweenHelpSections = true;
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
                Console.ReadKey(true);
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
