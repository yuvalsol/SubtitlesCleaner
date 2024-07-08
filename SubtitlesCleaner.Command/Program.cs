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
                    var parser = new Parser(with =>
                    {
                        with.CaseSensitive = false;
                        with.IgnoreUnknownArguments = true;
                        with.HelpWriter = null;
                    });

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

                            if (options.clean)
                            {
                                Console.WriteLine("SubtitlesCleanerCommand clean --path <fileOrFolder>");
                                Console.WriteLine("                              [--subfolders]");
                                Console.WriteLine("                              [--save [--outputFile <file>] [--outputFolder <folder>]]");
                                Console.WriteLine("                              [--print]");
                                Console.WriteLine("                              [--cleanHICaseInsensitive]");
                                Console.WriteLine("                              [--dictionaryCleaning]");
                                Console.WriteLine("                              [--firstSubtitlesCount <N>]");
                                Console.WriteLine("                              [--suppressBackupFile]");
                                Console.WriteLine("                              [--suppressBackupFileOnSame]");
                                Console.WriteLine("                              [--suppressWarningsFile]");
                                Console.WriteLine("                              [--printCleaning]");
                                Console.WriteLine("                              [--log <logFile>]");
                                Console.WriteLine("                              [--log+ <logFile>]");
                                Console.WriteLine("                              [--csv]");
                                Console.WriteLine("                              [--quiet]");
                                Console.WriteLine("                              [--sequential]");

                                PrintVerbHelp<CleanSubtitlesOptions>("clean");
                            }

                            if (options.cleanEmptyAndNonSubtitles)
                            {
                                Console.WriteLine("SubtitlesCleanerCommand cleanEmptyAndNonSubtitles --path <fileOrFolder>");
                                Console.WriteLine("                                                  [--subfolders]");
                                Console.WriteLine("                                                  [--save [--outputFile <file>] [--outputFolder <folder>]]");
                                Console.WriteLine("                                                  [--print]");
                                Console.WriteLine("                                                  [--firstSubtitlesCount <N>]");
                                Console.WriteLine("                                                  [--suppressBackupFile]");
                                Console.WriteLine("                                                  [--suppressBackupFileOnSame]");
                                Console.WriteLine("                                                  [--printCleaning]");
                                Console.WriteLine("                                                  [--log <logFile>]");
                                Console.WriteLine("                                                  [--log+ <logFile>]");
                                Console.WriteLine("                                                  [--csv]");
                                Console.WriteLine("                                                  [--quiet]");
                                Console.WriteLine("                                                  [--sequential]");

                                PrintVerbHelp<CleanEmptyAndNonSubtitlesOptions>("cleanEmptyAndNonSubtitles");
                            }

                            if (options.addTime)
                            {
                                Console.WriteLine("SubtitlesCleanerCommand addTime --timeAdded <+00:00:00,000|-00:00:00,000>");
                                Console.WriteLine("                                --path <fileOrFolder>");
                                Console.WriteLine("                                [--subfolders]");
                                Console.WriteLine("                                [--save [--outputFile <file>] [--outputFolder <folder>]]");
                                Console.WriteLine("                                [--print]");
                                Console.WriteLine("                                [--subtitleNumber <N>]");
                                Console.WriteLine("                                [--firstSubtitlesCount <N>]");
                                Console.WriteLine("                                [--suppressBackupFile]");
                                Console.WriteLine("                                [--suppressBackupFileOnSame]");
                                Console.WriteLine("                                [--log <logFile>]");
                                Console.WriteLine("                                [--log+ <logFile>]");
                                Console.WriteLine("                                [--csv]");
                                Console.WriteLine("                                [--quiet]");
                                Console.WriteLine("                                [--sequential]");

                                PrintVerbHelp<AddTimeOptions>("addTime");
                            }

                            if (options.setShowTime)
                            {
                                Console.WriteLine("SubtitlesCleanerCommand setShowTime --showTime <00:00:00,000>");
                                Console.WriteLine("                                    --path <fileOrFolder>");
                                Console.WriteLine("                                    [--subfolders]");
                                Console.WriteLine("                                    [--save [--outputFile <file>] [--outputFolder <folder>]]");
                                Console.WriteLine("                                    [--print]");
                                Console.WriteLine("                                    [--subtitleNumber <N>]");
                                Console.WriteLine("                                    [--firstSubtitlesCount <N>]");
                                Console.WriteLine("                                    [--suppressBackupFile]");
                                Console.WriteLine("                                    [--suppressBackupFileOnSame]");
                                Console.WriteLine("                                    [--log <logFile>]");
                                Console.WriteLine("                                    [--log+ <logFile>]");
                                Console.WriteLine("                                    [--csv]");
                                Console.WriteLine("                                    [--quiet]");
                                Console.WriteLine("                                    [--sequential]");

                                PrintVerbHelp<SetShowTimeOptions>("setShowTime");
                            }

                            if (options.adjustTiming)
                            {
                                Console.WriteLine("SubtitlesCleanerCommand adjustTiming --firstShowTime <00:00:00,000>");
                                Console.WriteLine("                                     --lastShowTime <00:00:00,000>");
                                Console.WriteLine("                                     --path <fileOrFolder>");
                                Console.WriteLine("                                     [--subfolders]");
                                Console.WriteLine("                                     [--save [--outputFile <file>] [--outputFolder <folder>]]");
                                Console.WriteLine("                                     [--print]");
                                Console.WriteLine("                                     [--firstSubtitlesCount <N>]");
                                Console.WriteLine("                                     [--suppressBackupFile]");
                                Console.WriteLine("                                     [--suppressBackupFileOnSame]");
                                Console.WriteLine("                                     [--log <logFile>]");
                                Console.WriteLine("                                     [--log+ <logFile>]");
                                Console.WriteLine("                                     [--csv]");
                                Console.WriteLine("                                     [--quiet]");
                                Console.WriteLine("                                     [--sequential]");

                                PrintVerbHelp<AdjustTimingOptions>("adjustTiming");
                            }

                            if (options.reorder)
                            {
                                Console.WriteLine("SubtitlesCleanerCommand reorder --path <fileOrFolder>");
                                Console.WriteLine("                                [--subfolders]");
                                Console.WriteLine("                                [--save]");
                                Console.WriteLine("                                [--print]");
                                Console.WriteLine("                                [--suppressBackupFile]");
                                Console.WriteLine("                                [--suppressBackupFileOnSame]");
                                Console.WriteLine("                                [--log <logFile>]");
                                Console.WriteLine("                                [--log+ <logFile>]");
                                Console.WriteLine("                                [--csv]");
                                Console.WriteLine("                                [--quiet]");
                                Console.WriteLine("                                [--sequential]");

                                PrintVerbHelp<ReorderOptions>("reorder");
                            }

                            if (options.balanceLines)
                            {
                                Console.WriteLine("SubtitlesCleanerCommand balanceLines --path <fileOrFolder>");
                                Console.WriteLine("                                     [--subfolders]");
                                Console.WriteLine("                                     [--save]");
                                Console.WriteLine("                                     [--print]");
                                Console.WriteLine("                                     [--suppressBackupFile]");
                                Console.WriteLine("                                     [--suppressBackupFileOnSame]");
                                Console.WriteLine("                                     [--log <logFile>]");
                                Console.WriteLine("                                     [--log+ <logFile>]");
                                Console.WriteLine("                                     [--csv]");
                                Console.WriteLine("                                     [--quiet]");
                                Console.WriteLine("                                     [--sequential]");

                                PrintVerbHelp<BalanceLinesOptions>("balanceLines");
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
                                var helpText = HelpText.AutoBuild(parserResult, h =>
                                {
                                    h.Heading =
                                        "Subtitles Cleaner Command, Version" + " " +
                                        Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
                                    h.Copyright = string.Empty;
                                    h.AdditionalNewLineAfterOption = false;
                                    h.MaximumDisplayWidth = 120;
                                    h.AddNewLineBetweenHelpSections = true;
                                    return HelpText.DefaultParsingErrorsHandler(parserResult, h);
                                }, e => e, verbsIndex: true);

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
                Console.Error.WriteLine("_________________________________________");
                string errorMessage = UnhandledException(ex);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    while (ex != null)
                    {
                        Console.Error.WriteLine(string.Format("{0}\n{1}\n_________________________________________", ex.GetType().ToString(), ex.Message));
                        ex = ex.InnerException;
                    }
                }
                else
                {
                    Console.Error.WriteLine(errorMessage);
                    Console.Error.WriteLine("_________________________________________");
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

        private static void PrintVerbHelp<TSharedOptions>(string verb)
            where TSharedOptions : SharedOptions
        {
            var parser = new Parser(with =>
            {
                with.CaseSensitive = false;
                with.IgnoreUnknownArguments = true;
                with.HelpWriter = null;
            });

            var parserResult = parser.ParseArguments<TSharedOptions>(new string[] { "--help", verb });

            parserResult.WithNotParsed(errors =>
            {
                var helpText = HelpText.AutoBuild(parserResult, h =>
                {
                    h.Heading = string.Empty;
                    h.Copyright = string.Empty;
                    h.AdditionalNewLineAfterOption = false;
                    h.MaximumDisplayWidth = 120;
                    h.AddNewLineBetweenHelpSections = true;
                    return HelpText.DefaultParsingErrorsHandler(parserResult, h);
                }, e => e);

                Console.WriteLine(helpText);
            });
        }

        private static string UnhandledException(Exception ex)
        {
            try
            {
                return
                    string.Format("Unhandled Error - {0} {1}",
                        Assembly.GetExecutingAssembly().GetName().Name,
                        Assembly.GetExecutingAssembly().GetName().Version.ToString(3)) + Environment.NewLine +
                    ex.GetUnhandledExceptionErrorWithApplicationTerminationMessage();
            }
            catch
            {
                return null;
            }
        }
    }
}
