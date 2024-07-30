using System;
using System.Collections.Generic;
using System.Linq;
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
                        BalanceLinesOptions>(args);

                    parserResult
                        .WithParsed<CleanSubtitlesOptions>(subtitlesHandler.CleanSubtitles)
                        .WithParsed<CleanEmptyAndNonSubtitlesOptions>(subtitlesHandler.CleanEmptyAndNonSubtitles)
                        .WithParsed<AddTimeOptions>(subtitlesHandler.AddTime)
                        .WithParsed<SetShowTimeOptions>(subtitlesHandler.SetShowTime)
                        .WithParsed<AdjustTimingOptions>(subtitlesHandler.AdjustTiming)
                        .WithParsed<ReorderOptions>(subtitlesHandler.Reorder)
                        .WithParsed<BalanceLinesOptions>(subtitlesHandler.BalanceLines)
                        .WithNotParsed(errors =>
                        {
                            if (errors.IsVersion())
                            {
                                Console.WriteLine(GetVersion());
                                Console.WriteLine();
                            }
                            else if (errors.IsHelp())
                            {
                                string[] preOptionsLines = null;
                                Comparison<ComparableOption> optionComparison = HelpText.RequiredThenAlphaComparison;
                                bool autoHelpAndVersion = false;

                                if (args[0] == "clean")
                                {
                                    preOptionsLines = new string[]
                                    {
                                         VerbHelpText<CleanSubtitlesOptions>()
                                        ,string.Empty
                                        ,"SubtitlesCleanerCommand.exe clean --path <fileOrFolder>"
                                        ,"                                  [--subfolders]"
                                        ,"                                  [--save [--outputFile <file>] [--outputFolder <folder>]]"
                                        ,"                                  [--print]"
                                        ,"                                  [--cleanHICaseInsensitive]"
                                        ,"                                  [--dictionaryCleaning]"
                                        ,"                                  [--firstSubtitlesCount <N>]"
                                        ,"                                  [--suppressBackupFile]"
                                        ,"                                  [--suppressBackupFileOnSame]"
                                        ,"                                  [--suppressWarningsFile]"
                                        ,"                                  [--printCleaning]"
                                        ,"                                  [--log <logFile>]"
                                        ,"                                  [--log+ <logFile>]"
                                        ,"                                  [--csv]"
                                        ,"                                  [--quiet]"
                                        ,"                                  [--sequential]"
                                    };
                                }
                                else if (args[0] == "cleanEmptyAndNonSubtitles")
                                {
                                    preOptionsLines = new string[]
                                    {
                                         VerbHelpText<CleanEmptyAndNonSubtitlesOptions>()
                                        ,string.Empty
                                        ,"SubtitlesCleanerCommand.exe cleanEmptyAndNonSubtitles --path <fileOrFolder>"
                                        ,"                                                      [--subfolders]"
                                        ,"                                                      [--save [--outputFile <file>] [--outputFolder <folder>]]"
                                        ,"                                                      [--print]"
                                        ,"                                                      [--firstSubtitlesCount <N>]"
                                        ,"                                                      [--suppressBackupFile]"
                                        ,"                                                      [--suppressBackupFileOnSame]"
                                        ,"                                                      [--printCleaning]"
                                        ,"                                                      [--log <logFile>]"
                                        ,"                                                      [--log+ <logFile>]"
                                        ,"                                                      [--csv]"
                                        ,"                                                      [--quiet]"
                                        ,"                                                      [--sequential]"
                                    };
                                }
                                else if (args[0] == "addTime")
                                {
                                    preOptionsLines = new string[]
                                    {
                                         VerbHelpText<AddTimeOptions>()
                                        ,string.Empty
                                        ,"SubtitlesCleanerCommand.exe addTime --timeAdded <+00:00:00,000|-00:00:00,000>"
                                        ,"                                    --path <fileOrFolder>"
                                        ,"                                    [--subfolders]"
                                        ,"                                    [--save [--outputFile <file>] [--outputFolder <folder>]]"
                                        ,"                                    [--print]"
                                        ,"                                    [--subtitleNumber <N>]"
                                        ,"                                    [--firstSubtitlesCount <N>]"
                                        ,"                                    [--suppressBackupFile]"
                                        ,"                                    [--suppressBackupFileOnSame]"
                                        ,"                                    [--log <logFile>]"
                                        ,"                                    [--log+ <logFile>]"
                                        ,"                                    [--csv]"
                                        ,"                                    [--quiet]"
                                        ,"                                    [--sequential]"
                                    };
                                }
                                else if (args[0] == "setShowTime")
                                {
                                    preOptionsLines = new string[]
                                    {
                                         VerbHelpText<SetShowTimeOptions>()
                                        ,string.Empty
                                        ,"SubtitlesCleanerCommand.exe setShowTime --showTime <00:00:00,000>"
                                        ,"                                        --path <fileOrFolder>"
                                        ,"                                        [--subfolders]"
                                        ,"                                        [--save [--outputFile <file>] [--outputFolder <folder>]]"
                                        ,"                                        [--print]"
                                        ,"                                        [--subtitleNumber <N>]"
                                        ,"                                        [--firstSubtitlesCount <N>]"
                                        ,"                                        [--suppressBackupFile]"
                                        ,"                                        [--suppressBackupFileOnSame]"
                                        ,"                                        [--log <logFile>]"
                                        ,"                                        [--log+ <logFile>]"
                                        ,"                                        [--csv]"
                                        ,"                                        [--quiet]"
                                        ,"                                        [--sequential]"
                                    };
                                }
                                else if (args[0] == "adjustTiming")
                                {
                                    preOptionsLines = new string[]
                                    {
                                         VerbHelpText<AdjustTimingOptions>()
                                        ,string.Empty
                                        ,"SubtitlesCleanerCommand.exe adjustTiming --firstShowTime <00:00:00,000>"
                                        ,"                                         --lastShowTime <00:00:00,000>"
                                        ,"                                         --path <fileOrFolder>"
                                        ,"                                         [--subfolders]"
                                        ,"                                         [--save [--outputFile <file>] [--outputFolder <folder>]]"
                                        ,"                                         [--print]"
                                        ,"                                         [--firstSubtitlesCount <N>]"
                                        ,"                                         [--suppressBackupFile]"
                                        ,"                                         [--suppressBackupFileOnSame]"
                                        ,"                                         [--log <logFile>]"
                                        ,"                                         [--log+ <logFile>]"
                                        ,"                                         [--csv]"
                                        ,"                                         [--quiet]"
                                        ,"                                         [--sequential]"
                                    };
                                }
                                else if (args[0] == "reorder")
                                {
                                    preOptionsLines = new string[]
                                    {
                                         VerbHelpText<ReorderOptions>()
                                        ,string.Empty
                                        ,"SubtitlesCleanerCommand.exe reorder --path <fileOrFolder>"
                                        ,"                                    [--subfolders]"
                                        ,"                                    [--save]"
                                        ,"                                    [--print]"
                                        ,"                                    [--suppressBackupFile]"
                                        ,"                                    [--suppressBackupFileOnSame]"
                                        ,"                                    [--log <logFile>]"
                                        ,"                                    [--log+ <logFile>]"
                                        ,"                                    [--csv]"
                                        ,"                                    [--quiet]"
                                        ,"                                    [--sequential]"
                                    };
                                }
                                else if (args[0] == "balanceLines")
                                {
                                    preOptionsLines = new string[]
                                    {
                                         VerbHelpText<BalanceLinesOptions>()
                                        ,string.Empty
                                        ,"SubtitlesCleanerCommand.exe balanceLines --path <fileOrFolder>"
                                        ,"                                         [--subfolders]"
                                        ,"                                         [--save]"
                                        ,"                                         [--print]"
                                        ,"                                         [--suppressBackupFile]"
                                        ,"                                         [--suppressBackupFileOnSame]"
                                        ,"                                         [--log <logFile>]"
                                        ,"                                         [--log+ <logFile>]"
                                        ,"                                         [--csv]"
                                        ,"                                         [--quiet]"
                                        ,"                                         [--sequential]"
                                    };
                                }
                                else
                                {
                                    optionComparison = null;
                                    autoHelpAndVersion = true;
                                }

                                Console.WriteLine(HelpText.AutoBuild(parserResult, h =>
                                {
                                    h.Heading = GetVersion();
                                    h.Copyright = string.Empty;
                                    h.AdditionalNewLineAfterOption = false;
                                    h.MaximumDisplayWidth = 120;
                                    h.AddNewLineBetweenHelpSections = true;
                                    h.AddDashesToOption = true;
                                    h.OptionComparison = optionComparison;
                                    h.AutoHelp = autoHelpAndVersion;
                                    h.AutoVersion = autoHelpAndVersion;

                                    if (preOptionsLines.HasAny())
                                        h.AddPreOptionsLines(preOptionsLines);

                                    return h;
                                }, e => e, verbsIndex: true));
                            }
                            else if (errors.HasAny())
                            {
                                Console.WriteLine(GetVersion());
                                Console.WriteLine();

                                bool isNoVerbSelectedError = errors.All(e => e.Tag == ErrorType.NoVerbSelectedError);
                                if (isNoVerbSelectedError)
                                    return;

                                Console.WriteLine("Command Line Parsing Errors:");
                                foreach (Error error in errors)
                                {
                                    string token = error.Tag.ToString();
                                    if (token.EndsWith("Error"))
                                        token = token.Substring(0, token.Length - 5);
                                    token = string.Join(" ", token.CamelCaseWords());

                                    if (error is TokenError tokenError)
                                        Console.WriteLine(token + ": " + tokenError.Token);
                                    else if (error is NamedError namedError)
                                        Console.WriteLine(token + ": " + namedError.NameInfo.NameText);
                                    else
                                        Console.WriteLine(token);
                                }
                                Console.WriteLine();
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

        private static string GetVersion()
        {
            return
                "Subtitles Cleaner Command, Version" + " " +
                Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
        }

        private static string VerbHelpText<T>() where T : SharedOptions
        {
            var attribute = typeof(T).GetCustomAttribute(typeof(VerbAttribute), false);
            return ((VerbAttribute)attribute).HelpText;
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
