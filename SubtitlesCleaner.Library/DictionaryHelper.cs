using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using WeCantSpell.Hunspell;

namespace SubtitlesCleaner.Library
{
    public static class DictionaryHelper
    {
        public class MisspelledLine
        {
            public string Line { get; internal set; }
            public int LineIndex { get; internal set; }
            public IEnumerable<MisspelledWord> MisspelledWords;
        }

        public class MisspelledWord
        {
            public Match Match { get; internal set; }
            public IEnumerable<string> Suggestions { get; internal set; }

            public string Word { get { return Match.Value; } }
            public int Index { get { return Match.Index; } }
            public int Length { get { return Match.Length; } }
            public bool HasSuggestions { get { return Suggestions.Any(); } }
        }

        public static IEnumerable<MisspelledLine> GetMisspelledLines(IEnumerable<string> lines)
        {
            if (lines.IsNullOrEmpty())
                yield break;

            var assembly = Assembly.GetExecutingAssembly();
            using (var dictionaryStream = assembly.GetManifestResourceStream("SubtitlesCleaner.Library.Dictionaries.en-US.dic"))
            {
                using (var affixStream = assembly.GetManifestResourceStream("SubtitlesCleaner.Library.Dictionaries.en-US.aff"))
                {
                    WordList dictionary = WordList.CreateFromStreams(dictionaryStream, affixStream);

                    int lineIndex = 0;
                    foreach (string line in lines)
                    {
                        var misspelledWords = GetMisspelledWords(line, dictionary);
                        if (misspelledWords.Any())
                        {
                            yield return new MisspelledLine()
                            {
                                Line = line,
                                LineIndex = lineIndex,
                                MisspelledWords = misspelledWords
                            };
                        }

                        lineIndex++;
                    }
                }
            }
        }

        private static readonly Regex regexDictionarySplitToWords = new Regex(@"\b[A-ZÀ-Ýa-zà-ÿ]{2,}\b", RegexOptions.Compiled);

        private static IEnumerable<MisspelledWord> GetMisspelledWords(string line, WordList dictionary)
        {
            if (string.IsNullOrEmpty(line))
                yield break;

            foreach (Match match in regexDictionarySplitToWords.Matches(line))
            {
                var misspelledWord = new MisspelledWord() { Match = match };
                if (dictionary.Check(misspelledWord.Word) == false)
                {
                    misspelledWord.Suggestions = dictionary.Suggest(misspelledWord.Word);
                    yield return misspelledWord;
                }
            }
        }
    }
}