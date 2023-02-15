using System;
using System.Reflection;
using System.Text;
using CommandLine;
using CommandLine.Text;

namespace SubtitlesCleaner.Command
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.OutputEncoding = Encoding.UTF8;

                var subtitlesHandler = new SubtitlesHandler();

                if (SubtitlesHandler.IsProduction)
                {
                    var parser = new Parser(with => with.HelpWriter = null);
                    var parserResult = parser.ParseArguments<
                        CleanSubtitlesOptions,
                        CleanEmptyAndNonSubtitlesOptions,
                        AddTimeOptions,
                        SetShowTimeOptions,
                        AdjustTimingOptions,
                        ReorderOptions,
                        BalanceLinesOptions,
                        UsageOptions>(args);

                    parserResult
                        .WithParsed<CleanSubtitlesOptions>(subtitlesHandler.CleanSubtitles)
                        .WithParsed<CleanEmptyAndNonSubtitlesOptions>(subtitlesHandler.CleanEmptyAndNonSubtitles)
                        .WithParsed<AddTimeOptions>(subtitlesHandler.AddTime)
                        .WithParsed<SetShowTimeOptions>(subtitlesHandler.SetShowTime)
                        .WithParsed<AdjustTimingOptions>(subtitlesHandler.AdjustTiming)
                        .WithParsed<ReorderOptions>(subtitlesHandler.Reorder)
                        .WithParsed<BalanceLinesOptions>(subtitlesHandler.BalanceLines)
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
                                Console.WriteLine("SubtitlesCleanerCommand clean --path <fileOrFolder>");
                                Console.WriteLine("                              [--save [--outputFile <file>] [--outputFolder <folder>]]");
                                Console.WriteLine("                              [--print]");
                                Console.WriteLine("                              [--cleanHICaseInsensitive]");
                                Console.WriteLine("                              [--firstSubtitlesCount <N>]");
                                Console.WriteLine("                              [--suppressBackupFile]");
                                Console.WriteLine("                              [--suppressErrorFile]");
                                Console.WriteLine("                              [--printCleaning]");
                                Console.WriteLine("                              [--log <logFile>]");
                                Console.WriteLine("                              [--log+ <logFile>]");
                                Console.WriteLine("                              [--csv]");
                                Console.WriteLine("                              [--quiet]");
                                Console.WriteLine();
                            }

                            if (options.cleanEmptyAndNonSubtitles)
                            {
                                Console.WriteLine("Clean Empty And Non-Subtitles");
                                Console.WriteLine("SubtitlesCleanerCommand cleanEmptyAndNonSubtitles --path <fileOrFolder>");
                                Console.WriteLine("                                                  [--save [--outputFile <file>] [--outputFolder <folder>]]");
                                Console.WriteLine("                                                  [--print]");
                                Console.WriteLine("                                                  [--firstSubtitlesCount <N>]");
                                Console.WriteLine("                                                  [--suppressBackupFile]");
                                Console.WriteLine("                                                  [--printCleaning]");
                                Console.WriteLine("                                                  [--log <logFile>]");
                                Console.WriteLine("                                                  [--log+ <logFile>]");
                                Console.WriteLine("                                                  [--csv]");
                                Console.WriteLine("                                                  [--quiet]");
                                Console.WriteLine();
                            }

                            if (options.addTime)
                            {
                                Console.WriteLine("Add Time");
                                Console.WriteLine("SubtitlesCleanerCommand addTime --timeAdded <+00:00:00,000|-00:00:00,000>");
                                Console.WriteLine("                                --path <fileOrFolder>");
                                Console.WriteLine("                                [--save [--outputFile <file>] [--outputFolder <folder>]]");
                                Console.WriteLine("                                [--print]");
                                Console.WriteLine("                                [--subtitleNumber <N>]");
                                Console.WriteLine("                                [--firstSubtitlesCount <N>]");
                                Console.WriteLine("                                [--suppressBackupFile]");
                                Console.WriteLine("                                [--log <logFile>]");
                                Console.WriteLine("                                [--log+ <logFile>]");
                                Console.WriteLine("                                [--csv]");
                                Console.WriteLine("                                [--quiet]");
                                Console.WriteLine();
                            }

                            if (options.setShowTime)
                            {
                                Console.WriteLine("Set Show Time");
                                Console.WriteLine("SubtitlesCleanerCommand setShowTime --showTime <00:00:00,000>");
                                Console.WriteLine("                                    --path <fileOrFolder>");
                                Console.WriteLine("                                    [--save [--outputFile <file>] [--outputFolder <folder>]]");
                                Console.WriteLine("                                    [--print]");
                                Console.WriteLine("                                    [--subtitleNumber <N>]");
                                Console.WriteLine("                                    [--firstSubtitlesCount <N>]");
                                Console.WriteLine("                                    [--suppressBackupFile]");
                                Console.WriteLine("                                    [--log <logFile>]");
                                Console.WriteLine("                                    [--log+ <logFile>]");
                                Console.WriteLine("                                    [--csv]");
                                Console.WriteLine("                                    [--quiet]");
                                Console.WriteLine();
                            }

                            if (options.adjustTiming)
                            {
                                Console.WriteLine("Adjust Timing");
                                Console.WriteLine("SubtitlesCleanerCommand adjustTiming --firstShowTime <00:00:00,000>");
                                Console.WriteLine("                                     --lastShowTime <00:00:00,000>");
                                Console.WriteLine("                                     --path <fileOrFolder>");
                                Console.WriteLine("                                     [--save [--outputFile <file>] [--outputFolder <folder>]]");
                                Console.WriteLine("                                     [--print]");
                                Console.WriteLine("                                     [--firstSubtitlesCount <N>]");
                                Console.WriteLine("                                     [--suppressBackupFile]");
                                Console.WriteLine("                                     [--log <logFile>]");
                                Console.WriteLine("                                     [--log+ <logFile>]");
                                Console.WriteLine("                                     [--csv]");
                                Console.WriteLine("                                     [--quiet]");
                                Console.WriteLine();
                            }

                            if (options.reorder)
                            {
                                Console.WriteLine("Reorder");
                                Console.WriteLine("SubtitlesCleanerCommand reorder --path <fileOrFolder>");
                                Console.WriteLine("                                [--save]");
                                Console.WriteLine("                                [--print]");
                                Console.WriteLine("                                [--suppressBackupFile]");
                                Console.WriteLine("                                [--log <logFile>]");
                                Console.WriteLine("                                [--log+ <logFile>]");
                                Console.WriteLine("                                [--csv]");
                                Console.WriteLine("                                [--quiet]");
                                Console.WriteLine();
                            }

                            if (options.balanceLines)
                            {
                                Console.WriteLine("Balance Lines");
                                Console.WriteLine("SubtitlesCleanerCommand balanceLines --path <fileOrFolder>");
                                Console.WriteLine("                                     [--save]");
                                Console.WriteLine("                                     [--print]");
                                Console.WriteLine("                                     [--suppressBackupFile]");
                                Console.WriteLine("                                     [--log <logFile>]");
                                Console.WriteLine("                                     [--log+ <logFile>]");
                                Console.WriteLine("                                     [--csv]");
                                Console.WriteLine("                                     [--quiet]");
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
                    subtitlesHandler.Debug();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("_________________________________________");
                string errorMessage = UnhandledException(ex);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    while (ex != null)
                    {
                        Console.WriteLine(string.Format("{0}\n{1}\n_________________________________________", ex.GetType().ToString(), ex.Message));
                        ex = ex.InnerException;
                    }
                }
                else
                {
                    Console.WriteLine(errorMessage);
                    Console.WriteLine("_________________________________________");
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

        private static string UnhandledException(Exception ex)
        {
            try
            {
                return
                    string.Format("Unhandled Error - {0} {1}",
                        Assembly.GetExecutingAssembly().GetName().Name,
                        Assembly.GetExecutingAssembly().GetName().Version.ToString(3)) + Environment.NewLine +
                    ex.GetUnhandledExceptionErrorMessage();
            }
            catch
            {
                return null;
            }
        }
    }
}
