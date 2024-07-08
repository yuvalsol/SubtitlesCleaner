using System;
using System.Collections.Generic;
using System.IO;
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
            public string Line { get; private set; }
            public int LineIndex { get; private set; }
            public IEnumerable<MisspelledWordWithLineLocation> MisspelledWords { get; private set; }

            public MisspelledLine(string line, int lineIndex, IEnumerable<MisspelledWordWithLineLocation> misspelledWords)
            {
                Line = line;
                LineIndex = lineIndex;
                MisspelledWords = misspelledWords;
            }

            public override string ToString()
            {
                return Line;
            }
        }

        public class MisspelledWord
        {
            public string Word { get; private set; }
            public string Suggestion { get; private set; }

            public MisspelledWord(string word)
            {
                Word = word;

                Suggestion = dictionary.Suggest(word, new QueryOptions()
                {
                    MaxSuggestions = 1
                }).FirstOrDefault();
            }

            public MisspelledWord(MisspelledWord misspelledWord)
            {
                Word = misspelledWord.Word;
                Suggestion = misspelledWord.Suggestion;
            }

            public override string ToString()
            {
                return Word;
            }
        }

        public class MisspelledWordWithLineLocation : MisspelledWord
        {
            public int Index { get; private set; }
            public int Length { get; private set; }

            public MisspelledWordWithLineLocation(string word, int index, int length) : base(word)
            {
                Index = index;
                Length = length;
            }

            public MisspelledWordWithLineLocation(MisspelledWord misspelledWord, int index, int length) : base(misspelledWord)
            {
                Index = index;
                Length = length;
            }
        }

        private static WordList dictionary;
        private static readonly HashSet<string> names = new HashSet<string>();

        #region Loading

        static DictionaryHelper()
        {
            var assembly = Assembly.GetExecutingAssembly();
            LoadDictionary(assembly);
            LoadNames(assembly);
        }

        private static void LoadDictionary(Assembly assembly)
        {
            using (var dictionaryStream = assembly.GetManifestResourceStream("SubtitlesCleaner.Library.Dictionaries.en_US.dic"))
            {
                using (var affixStream = assembly.GetManifestResourceStream("SubtitlesCleaner.Library.Dictionaries.en_US.aff"))
                {
                    dictionary = WordList.CreateFromStreams(dictionaryStream, affixStream);
                }
            }
        }

        private static void LoadNames(Assembly assembly)
        {
            LoadNamesFromResource("SubtitlesCleaner.Library.Dictionaries.names.txt", assembly);

            // https://en.wikipedia.org/wiki/Category:English_given_names
            LoadNamesFromResource("SubtitlesCleaner.Library.Dictionaries.english_given_names.txt", assembly);

            // https://en.wiktionary.org/wiki/Category:Old_English_given_names
            LoadNamesFromResource("SubtitlesCleaner.Library.Dictionaries.old_english_given_names.txt", assembly);

            // https://en.wikipedia.org/wiki/List_of_Game_of_Thrones_characters
            // https://en.wikipedia.org/wiki/List_of_House_of_the_Dragon_characters
            // https://www.ign.com/wikis/game-of-thrones/Locations
            // https://gameofthrones.fandom.com/wiki/Dragon
            LoadNamesFromResource("SubtitlesCleaner.Library.Dictionaries.game_of_thrones_names.txt", assembly);

            LoadNamesFromResource("SubtitlesCleaner.Library.Dictionaries.custom_words.txt", assembly);
        }

        private static void LoadNamesFromResource(string resourceName, Assembly assembly)
        {
            try
            {
                using (var namesStream = assembly.GetManifestResourceStream(resourceName))
                {
                    using (var sr = new StreamReader(namesStream))
                    {
                        while (sr.Peek() != -1)
                        {
                            string line = (sr.ReadLine() ?? string.Empty).Trim();
                            if (string.IsNullOrEmpty(line) == false)
                                names.Add(line);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to load names from resource: " + resourceName, ex);
            }
        }

        #endregion

        #region Testing

        public static bool CheckWord(string word)
        {
            return dictionary.Check(FixWord(word));
        }

        public static IEnumerable<string> GetSuggestions(string word, int? maxSuggestions = null)
        {
            if (maxSuggestions != null)
            {
                return dictionary.Suggest(FixWord(word), new QueryOptions()
                {
                    MaxSuggestions = maxSuggestions.Value
                });
            }
            else
            {
                return dictionary.Suggest(FixWord(word));
            }
        }

        #endregion

        public static IEnumerable<MisspelledLine> GetMisspelledLines(IEnumerable<string> lines)
        {
            if (lines.IsNullOrEmpty())
                yield break;

            int lineIndex = 0;
            foreach (string line in lines)
            {
                var misspelledWords = GetMisspelledWords(line);
                if (misspelledWords.HasAny())
                    yield return new MisspelledLine(line, lineIndex, misspelledWords);

                lineIndex++;
            }
        }

        private static readonly Dictionary<string, MisspelledWord> wordsMisspelled = new Dictionary<string, MisspelledWord>();
        private static readonly HashSet<string> wordsSpelledCorrectly = new HashSet<string>();

        public static readonly Regex regexDictionarySplitToWords = new Regex(@"[A-ZÀ-Ýa-zà-ÿ-'&]{2,}", RegexOptions.Compiled);

        private static IEnumerable<MisspelledWordWithLineLocation> GetMisspelledWords(string line)
        {
            if (string.IsNullOrEmpty(line))
                yield break;

            foreach (Match match in regexDictionarySplitToWords.Matches(line))
            {
                string word = match.Value;
                int index = match.Index;
                // get length before FixWord(). the word and the length might change
                int length = match.Length;

                word = FixWord(word);

                if (wordsSpelledCorrectly.Contains(word) || names.Contains(word))
                {
                    continue;
                }
                else if (wordsMisspelled.ContainsKey(word))
                {
                    yield return new MisspelledWordWithLineLocation(wordsMisspelled[word], index, length);
                }
                else if (dictionary.Check(word))
                {
                    wordsSpelledCorrectly.Add(word);
                }
                else
                {
                    var misspelledWord = new MisspelledWord(word);
                    wordsMisspelled.Add(word, misspelledWord);
                    yield return new MisspelledWordWithLineLocation(misspelledWord, index, length);
                }
            }
        }

        public static readonly Regex regexDictionaryContractions = new Regex(@"'(?:d|em|ll|re|s)$", RegexOptions.Compiled);
        public static readonly Regex regexDictionaryIng = new Regex(@"in'$", RegexOptions.Compiled);
        public static readonly Regex regexDictionaryPluralPossessive = new Regex(@"s'$", RegexOptions.Compiled);

        private static string FixWord(string word)
        {
            if (regexDictionaryContractions.IsMatch(word))
                word = regexDictionaryContractions.Replace(word, string.Empty);

            if (regexDictionaryIng.IsMatch(word))
                word = regexDictionaryIng.Replace(word, "ing");

            if (regexDictionaryPluralPossessive.IsMatch(word))
                word = regexDictionaryPluralPossessive.Replace(word, "s");

            return word;
        }
    }
}