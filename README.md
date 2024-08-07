![Subtitles Cleaner Editor](./Solution%20Items/Images/SubtitlesCleanerEditor.gif "Subtitles Cleaner Editor")

# Subtitles Cleaner

Subtitles Cleaner cleans English SubRip .srt subtitle files from OCR errors, Hearing-impaired lines and other junk.

The program includes many cleaning rules and several complex cleaning routines. It also has some basic nice-to-have synchronization features.

For a comprehensive subtitles editor use [Subtitle Edit](https://www.nikse.dk/subtitleedit "Subtitle Edit").

Subtitles Cleaner requires .NET Framework 4.8 Runtime.

## Subtitles Cleaner Editor

![Subtitles Cleaner Editor](./Solution%20Items/Images/SubtitlesCleanerEditor.jpg "Subtitles Cleaner Editor")

1. Subtitles panel. The last column, Clean Text, is the text after Subtitles Cleaner gone over it. It is only a suggestion and it does not take effect until it is copied to the Text column. Double-click on a subtitle with an error will make the error list panel focus on the error. Context menu offers copy of texts and subtitles.

2. Error list panel. The headers can be sorted by subtitle numbers (#) or by errors. Double-click on an error will make the Subtitles panel focus on the subtitle. Context menu offers fixing all errors of the same type.

3. Clean all subtitles in the subtitles panel.

4. Quick Actions. Perform quick selective fixes to subtitles.

![Quick Actions](./Solution%20Items/Images/QuickActions.jpg "Quick Actions")

5. Adjust Timing. Adjust subtitles timing by 2 sync points. The popup will open with the sync points filled with timing of the first and last subtitles. Load From File loads the first and last sync points from another subtitles file.

![Adjust Timing](./Solution%20Items/Images/AdjustTiming.jpg "Adjust Timing")

6. Reorder subtitles based on their show time.

7. Balance Lines. Merge short line with long line, or first line with its continuation in the second line.

8. Search and replace.

9. Load the original subtitles and discard all previous changes.

10. Time Calculator. A little utility when you need to calculate time differences and it is too hard to do the math in your head.

![Time Calculator](./Solution%20Items/Images/TimeCalculator.jpg "Time Calculator")

11. Hearing-Impaired Detection. Identifies hearing-impaired with only capital letters text or all-case text.

12. Enable English (Hunspell en-US) dictionary for cleaning misspelled words. Copyright and credits, for the dictionary, are spelled out in [README_en_US.txt](SubtitlesCleaner.Library/Dictionaries/README_en_US.txt). The file is located in the same folder as the dictionary.

13. Selected subtitle. Changes can be made in the textbox.

14. Cleaned subtitle.

15. Fix buttons will copy the cleaned subtitle to the subtitle itself. The advance button will also jump to the next subtitle with an error.

16. Set Show Time. Set the specified show time to the selected subtitle and change the timings of all the subtitles below it based on their time differences. When the Interactive Retiming is checked, the show timings will change in the subtitles panel as the show time is changed.

17. Add Time starting from the selected subtitle. Time can be positive or negative. The time sign is clickable and changes between `+` and `-`.

18. Sync Errors & Subtitles. When checked, clicking on an error or a subtitle will also focus on the other one.

## Subtitles Cleaner Command

### Clean

Clean subtitles.

```console
SubtitlesCleanerCommand.exe clean --path <fileOrFolder>
                                  [--subfolders]
                                  [--save [--outputFile <file>] [--outputFolder <folder>]]
                                  [--print]
                                  [--cleanHICaseInsensitive]
                                  [--dictionaryCleaning]
                                  [--firstSubtitlesCount <N>]
                                  [--suppressBackupFile]
                                  [--suppressBackupFileOnSame]
                                  [--suppressWarningsFile]
                                  [--printCleaning]
                                  [--log <logFile>]
                                  [--log+ <logFile>]
                                  [--csv]
                                  [--quiet]
                                  [--sequential]
```

If the path points to a folder, it will clean all the subtitle files in that folder.

```console
--path <fileOrFolder>       Path to subtitle file or folder.
```

If the path points to a folder and `subfolders` is enabled, Subtitles Cleaner Command will get all the subtitle files in `path` and any subfolder under it.

```console
--subfolders                Include all subfolders under path.
```

Outputs the cleaned subtitles to a file. By default, Subtitles Cleaner Command will save the cleaned subtitles to the same subtitle file, overwriting it. Use `outputFile` switch to write to a new subtitle file with different name. use `outputFolder` switch to write to a different folder than the original subtitle file.

```console
--save                      Save to file.
--outputFile <file>         Output file. If omitted, the program outputs on the original file.
--outputFolder <folder>     Output folder. If omitted, the program outputs in the original folder.
```

Outputs the cleaned subtitles to the console.

```console
--print                     Print to console.
```

By default, Subtitles Cleaner Command identifies hearing-impaired line, which is not in any brackets, when the line is all in capital letters. Use this parameter to clean hearing-impaired lines when they are both upper and lower letters (sentence-like).

```console
--cleanHICaseInsensitive    Clean HI case-insensitive.
```

Enable English dictionary (Hunspell dictionary) for cleaning misspelled words. This feature increases the cleaning time.

```console
--dictionaryCleaning    Clean misspelled words with English dictionary.
```

Use this parameter to read the first N subtitles out of the file (not the first number of lines).

```console
--firstSubtitlesCount <N>   Read only the specified first number of subtitles.
```

Subtitles Cleaner Command saves a backup file, of the original file, with extension .bak.srt. This parameter suppresses the backup file.

```console
--suppressBackupFile        Do not create backup file of the original subtitles file.
```

Suppress creating a backup file if the subtitles after processing are the same as the original subtitles.

```console
--suppressBackupFileOnSame  Do not create backup file if processing results the same file.
```

Subtitles Cleaner Command creates a warnings file with extension .warnings.txt. This file includes all the **_possible_** errors the program encountered but didn't cleaned up. These are meant to be viewed by a human eye and fix accordingly. This parameter suppresses the warnings file.

```console
--suppressWarningsFile      Do not create warnings file.
```

Print to console the cleaning process.

```console
--printCleaning             Print to console what the cleaning process does.
```

The program outputs informative messages about what it does, like reading and saving files, cleaning subtitles, etc. By default, it will print them to the console. The following switches control where and how these informative messages show.

Write informative messages to a log file. If a folder path is specified instead of a file path, the program will name its own log file and create it in that folder.

```console
--log <logFile>             Write informative messages to log file. Overwrites existing log file.
--log+ <logFile>            Write informative messages to log file. Appends to existing log file.
```

Write informative messages in a comma-separated values. Use in conjunction with .csv file extension for the log file `--log logFile.csv`.

```console
--csv                       Write informative messages in a comma-separated values.
```

Quiet mode will not print any informative messages, to console or log file.

```console
--quiet                     Do not write informative messages.
```

By default, the program handles multiple files concurrently. The `sequential` switch forces the program to process each file one at a time. Sequential processing takes considerable less resources from the CPU and memory. Concurrent processing will use as much CPU as it can. Sequential processing will take significant longer to process multiple files.

```console
--sequential                Process subtitle files in sequential order, one after another, instead of concurrently.
```

Help screen for ```clean```.

```console
SubtitlesCleanerCommand.exe clean --help
```

**Examples:**

Clean subtitle file.

```console
SubtitlesCleanerCommand.exe clean --path "C:\My Documents\Subtitle.srt" --save
```

Clean subtitle file and save results in another folder.

```console
SubtitlesCleanerCommand.exe clean --path "C:\My Documents\Subtitle.srt" --save --outputFolder "C:\My Documents\Subtitles"
```

Clean hearing-impaired case-insensitive.

```console
SubtitlesCleanerCommand.exe clean --cleanHICaseInsensitive --path "C:\My Documents\Subtitle.srt" --save
```

Clean subtitle file and suppress backup & warnings files.

```console
SubtitlesCleanerCommand.exe clean --path "C:\My Documents\Subtitle.srt" --save --suppressBackupFile --suppressWarningsFile
```

Clean subtitle file and suppress warnings file. Create backup file if the cleaned subtitles are not the same as the original subtitles.

```console
SubtitlesCleanerCommand.exe clean --path "C:\My Documents\Subtitle.srt" --save --suppressBackupFileOnSame --suppressWarningsFile
```

Clean all subtitle files in the folder.

```console
SubtitlesCleanerCommand.exe clean --path "C:\My Documents\Subtitles" --save
```

Print to console.

```console
SubtitlesCleanerCommand.exe clean --path "C:\My Documents\Subtitle.srt" --print
```

Print to console the cleaned subtitles and the cleaning process. Very useful when tracking cleaning errors.

```console
SubtitlesCleanerCommand.exe clean --path "C:\My Documents\Subtitle.srt" --print --printCleaning
```

![Print Cleaning](./Solution%20Items/Images/PrintCleaning.jpg "Print Cleaning")

Clean all subtitle files in the folder. Write informative messages to text log file log_clean.txt.

```console
SubtitlesCleanerCommand.exe clean --path "C:\My Documents\Subtitles" --save --log "C:\My Documents\Subtitles\log_clean.txt"
```

Clean all subtitle files in the folder. Write informative messages to csv log file log_clean.csv. If the file already exists, append to it.

```console
SubtitlesCleanerCommand.exe clean --path "C:\My Documents\Subtitles" --save --log+ "C:\My Documents\Subtitles\log_clean.csv" --csv
```

Clean all subtitle files in the folder. Don't write informative messages.

```console
SubtitlesCleanerCommand.exe clean --path "C:\My Documents\Subtitles" --save --quiet
```

Clean all subtitle files in the folder, one after the other, in sequential order. Don't write informative messages.

```console
SubtitlesCleanerCommand.exe clean --path "C:\My Documents\Subtitles" --save --quiet --sequential
```

### Clean Empty And Non-Subtitles

Clean empty lines and non-subtitles.

```console
SubtitlesCleanerCommand.exe cleanEmptyAndNonSubtitles --path <fileOrFolder>
                                                      [--subfolders]
                                                      [--save [--outputFile <file>] [--outputFolder <folder>]]
                                                      [--print]
                                                      [--firstSubtitlesCount <N>]
                                                      [--suppressBackupFile]
                                                      [--suppressBackupFileOnSame]
                                                      [--printCleaning]
                                                      [--log <logFile>]
                                                      [--log+ <logFile>]
                                                      [--csv]
                                                      [--quiet]
                                                      [--sequential]
```

Help screen for ```cleanEmptyAndNonSubtitles```.

```console
SubtitlesCleanerCommand.exe cleanEmptyAndNonSubtitles --help
```

### Add Time

Add time to subtitles.

```console
SubtitlesCleanerCommand.exe addTime --timeAdded <+00:00:00,000|-00:00:00,000>
                                    --path <fileOrFolder>
                                    [--subfolders]
                                    [--save [--outputFile <file>] [--outputFolder <folder>]]
                                    [--print]
                                    [--subtitleNumber <N>]
                                    [--firstSubtitlesCount <N>]
                                    [--suppressBackupFile]
                                    [--suppressBackupFileOnSame]
                                    [--log <logFile>]
                                    [--log+ <logFile>]
                                    [--csv]
                                    [--quiet]
                                    [--sequential]
```

```console
--timeAdded <Time>         Added time to subtitles.
--timeAdded +00:00:00,000  Shift subtitle timings forwards.
--timeAdded -00:00:00,000  Shift subtitle timings backwards.
```

```console
--subtitleNumber <N>     Start operation from specified subtitle. If omitted, starts with first subtitle.
```

Help screen for ```addTime```.

```console
SubtitlesCleanerCommand.exe addTime --help
```

### Set Show Time

Move subtitles to show time.

```console
SubtitlesCleanerCommand.exe setShowTime --showTime <00:00:00,000>
                                        --path <fileOrFolder>
                                        [--subfolders]
                                        [--save [--outputFile <file>] [--outputFolder <folder>]]
                                        [--print]
                                        [--subtitleNumber <N>]
                                        [--firstSubtitlesCount <N>]
                                        [--suppressBackupFile]
                                        [--suppressBackupFileOnSame]
                                        [--log <logFile>]
                                        [--log+ <logFile>]
                                        [--csv]
                                        [--quiet]
                                        [--sequential]
```

```console
--showTime <Time>        Show time.
```

```console
--subtitleNumber <N>     Start operation from specified subtitle. If omitted, starts with first subtitle.
```

Help screen for ```setShowTime```.

```console
SubtitlesCleanerCommand.exe setShowTime --help
```

### Adjust Timing

Adjust subtitles timing by 2 sync points.

```console
SubtitlesCleanerCommand.exe adjustTiming --firstShowTime <00:00:00,000>
                                         --lastShowTime <00:00:00,000>
                                         --path <fileOrFolder>
                                         [--subfolders]
                                         [--save [--outputFile <file>] [--outputFolder <folder>]]
                                         [--print]
                                         [--firstSubtitlesCount <N>]
                                         [--suppressBackupFile]
                                         [--suppressBackupFileOnSame]
                                         [--log <logFile>]
                                         [--log+ <logFile>]
                                         [--csv]
                                         [--quiet]
                                         [--sequential]
```

```console
--firstShowTime <Time>   First subtitle's show time.
--lastShowTime <Time>    Last subtitle's show time.
```

Help screen for ```adjustTiming```.

```console
SubtitlesCleanerCommand.exe adjustTiming --help
```

### Reorder

Reorder subtitles based on their show time.

```console
SubtitlesCleanerCommand.exe reorder --path <fileOrFolder>
                                    [--subfolders]
                                    [--save]
                                    [--print]
                                    [--suppressBackupFile]
                                    [--suppressBackupFileOnSame]
                                    [--log <logFile>]
                                    [--log+ <logFile>]
                                    [--csv]
                                    [--quiet]
                                    [--sequential]
```

If the path points to a folder, it will reorder all the subtitle files in that folder.

```console
--path <fileOrFolder>       Path to subtitle file or folder.
```

Help screen for ```reorder```.

```console
SubtitlesCleanerCommand.exe reorder --help
```

### Balance Lines

Merge short line with long line, or first line with its continuation in the second line.

```console
SubtitlesCleanerCommand.exe balanceLines --path <fileOrFolder>
                                         [--subfolders]
                                         [--save]
                                         [--print]
                                         [--suppressBackupFile]
                                         [--suppressBackupFileOnSame]
                                         [--log <logFile>]
                                         [--log+ <logFile>]
                                         [--csv]
                                         [--quiet]
                                         [--sequential]
```

If the path points to a folder, it will balance the lines of all the subtitle files in that folder.

```console
--path <fileOrFolder>       Path to subtitle file or folder.
```

Help screen for ```balanceLines```.

```console
SubtitlesCleanerCommand.exe balanceLines --help
```

## Acknowledgments

[Cleaning-brush icons created by Umeicon - Flaticon](https://www.flaticon.com/free-icons/cleaning-brush "cleaning-brush icons")
