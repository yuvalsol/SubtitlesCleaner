# Subtitles Cleaner

Subtitles Cleaner cleans SubRip .srt subtitle files from OCR errors, Hearing-impaired lines and other junk.

The program includes many cleaning rules and several complex cleaning routines. It also has some basic nice-to-have synchronization features.

This program is not, and was not meant to be, a comprehensive subtitles editor.

## Subtitles Cleaner Editor

![Subtitles Cleaner Editor](./Solution%20Items/Images/SubtitlesCleanerEditor.jpg)

1. Subtitles panel. The last column, Clean Text, is the text after Subtitles Cleaner gone over it. It is only a suggestion and it does not take effect until it is copied to the Text column. Double-click on a subtitle with an error will make the error list panel focus on the error.
2. Error list panel. The headers can be sorted by subtitle numbers (#) or by errors. Double-click on an error will make the Subtitles panel focus on the subtitle.
3. Clean subtitles in the subtitles panel.
4. Adjust Timing. Adjust subtitles timing by 2 sync points. The popup will open with the sync points filled with timing of the first and last subtitles. Load From File loads the first and last sync points from another subtitles file.

![Adjust Timing](./Solution%20Items/Images/AdjustTiming.jpg)

5. Reorder subtitles based on their show time. Also, does a little cleanup of empty lines and non-subtitle lines ("Sync by").
6. Balance Lines. Merge short line with preceding long line.
7. Search and replace.
8. Load the original subtitles and discard all changes.
9. Time Calculator. A little utility when you need to calculate time differences and it is too hard to do the math in your head.

![Time Calculator](./Solution%20Items/Images/TimeCalculator.jpg)

10. Hearing-Impaired Detection. Identifies hearing-impaired with only capital letters text or all-case text.
11. Current selected subtitle. Changes can be made in the textbox.
12. Display length of the first 4 lines of the subtitle. Display length means only the text that is shown on the screen, so no `<i>`, `<b>`, `<u>` and no [ASSA Tags](https://www.nikse.dk/subtitleedit/formats/assa-override-tags).
13. Cleaned subtitle.
14. Fix buttons will copy the cleaned subtitle to the subtitle itself. The advance button will also jump to the next subtitle with an error.
15. Set Show Time. Set the specified show time to the selected subtitle and change the timings of all the subtitles below it based on their time differences. When the Interactive Retiming is checked, the show timings will change in the subtitles panel as the show time is changed.
16. Add Time starting from the selected subtitle. Time can positive or negative. The time sign is clickable and change between `+` and `-`.

## Subtitles Cleaner Command

### Clean

Clean subtitles.

```console
SubtitlesCleanerCommand clean [--cleanHICaseInsensitive]
                              [--firstSubtitlesCount n]
                              --path fileOrFolder
                              (--print|--save [--outputFile file] [--outputFolder folder])
                              [--suppressBackupFile]
                              [--suppressErrorFile]
```

By default, Subtitles Cleaner Command identifies hearing-impaired line, which is not in any brackets, when the line is all in capital letters. Use this parameter to clean hearing-impaired lines when they are both upper and lower letters (sentence-like).

```console
--cleanHICaseInsensitive    Clean HI case-insensitive
```

Use this parameter to read the first n subtitles out of the file (not the first number of lines).

```console
--firstSubtitlesCount       Read only the specified first number of subtitles
```

If the path points to a folder, it will clean all the .srt files in that folder.

```console
--path                      Path to subtitle file or folder
```

Outputs the cleaned subtitles to the console.

```console
--print                     Print to console
```

Outputs the cleaned subtitles to a file. By default, Subtitles Cleaner Command will save the cleaned subtitles to the same .srt file, overwriting it. Use `--outputFile` to write to a new subtitle file with different name. use `--outputFolder` to write to a different folder than the original subtitle file.

```console
--save                      Save to file
--outputFile                Output file. If omitted, the program outputs on the original file
--outputFolder              Output folder. If omitted, the program outputs in the original folder
```

Subtitles Cleaner Command saves a backup file, of the original file, with extension .bak.srt. This parameter suppresses the backup file.

```console
--suppressBackupFile        Do not create backup file of the original subtitles
```

Subtitles Cleaner Command creates an error file with extension .error.srt. This file includes all the **_possible_** errors the program encountered but didn't cleaned up. These are meant to be viewed by a human eye and fix accordingly. This parameter suppresses the error file.

```console
--suppressErrorFile         Do not create error file with possible errors
```

**Examples:**

Clean subtitle file

```console
SubtitlesCleanerCommand clean --path "C:\My Documents\Subtitle.srt" --save
```

Clean subtitle file and save results in another folder

```console
SubtitlesCleanerCommand clean --path "C:\My Documents\Subtitle.srt" --save --outputFolder "C:\My Documents\Subtitles"
```

Clean hearing-impaired case-insensitive

```console
SubtitlesCleanerCommand clean --cleanHICaseInsensitive --path "C:\My Documents\Subtitle.srt" --save
```

Clean subtitle file and suppress backup & error files

```console
SubtitlesCleanerCommand clean --path "C:\My Documents\Subtitle.srt" --save --suppressBackupFile --suppressErrorFile
```

Clean all subtitle files in the folder

```console
SubtitlesCleanerCommand clean --path "C:\My Documents\Subtitles" --save
```

Print to console

```console
SubtitlesCleanerCommand clean --path "C:\My Documents\Subtitle.srt" --print
```

### Add Time

Add time to subtitles.

```console
SubtitlesCleanerCommand addTime --timeAdded (+00:00:00,000|-00:00:00,000)
                                [--subtitleNumber n]
                                [--firstSubtitlesCount n]
                                --path fileOrFolder
                                (--print|--save [--outputFile file] [--outputFolder folder])
                                [--suppressBackupFile]
```

```console
--timeAdded              Added time to subtitles
```

```console
--subtitleNumber         Start operation from specified subtitle. If omitted, starts with first subtitle
```

### Set Show Time

Move subtitles to show time.

```console
SubtitlesCleanerCommand setShowTime --showTime 00:00:00,000
                                    [--subtitleNumber n]
                                    [--firstSubtitlesCount n]
                                    --path fileOrFolder
                                    (--print|--save [--outputFile file] [--outputFolder folder])
                                    [--suppressBackupFile]
```

```console
--showTime               Show time
```

```console
--subtitleNumber         Start operation from specified subtitle. If omitted, starts with first subtitle
```

### Adjust Timing

Adjust subtitles timing by 2 sync points.

```console
SubtitlesCleanerCommand adjustTiming --firstShowTime 00:00:00,000
                                     --lastShowTime 00:00:00,000
                                     [--firstSubtitlesCount n]
                                     --path fileOrFolder
                                     (--print|--save [--outputFile file] [--outputFolder folder])
                                     [--suppressBackupFile]
```

```console
--firstShowTime          First subtitle's show time
```

```console
--lastShowTime           Last subtitle's show time
```

### Reorder

Reorder subtitles based on their show time. Also, does a little cleanup of empty lines and non-subtitle lines ("Sync by").

```console
SubtitlesCleanerCommand reorder --path fileOrFolder
                                (--print|--save)
                                [--suppressBackupFile]
```

If the path points to a folder, it will reorder all the .srt files in that folder.

```console
--path                      Path to subtitle file or folder
```

### Balance Lines

Merge short line with preceding long line.

```console
SubtitlesCleanerCommand balanceLines --path fileOrFolder
                                     (--print|--save)
                                     [--suppressBackupFile]
```

If the path points to a folder, it will balance the lines of all the .srt files in that folder.

```console
--path                      Path to subtitle file or folder
```
