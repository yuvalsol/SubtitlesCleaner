using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using WeCantSpell.Hunspell;

namespace SubtitlesCleaner.Library
{
    public class MisspelledLine
    {
        public string Line { get; private set; }
        public int LineIndex { get; private set; }
        public List<MisspelledWordWithLineLocation> MisspelledWords { get; private set; }

        public MisspelledLine(string line, int lineIndex, List<MisspelledWordWithLineLocation> misspelledWords)
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
        public string[] Suggestions { get; private set; }

        public MisspelledWord(string word, string[] suggestions)
        {
            Word = word;
            Suggestions = suggestions;
        }

        public MisspelledWord(MisspelledWord misspelledWord)
            : this(misspelledWord.Word, misspelledWord.Suggestions)
        { }

        public override string ToString()
        {
            return Word;
        }
    }

    public class MisspelledWordWithLineLocation : MisspelledWord
    {
        public MisspelledLine MisspelledLine { get; private set; }
        public int Index { get; private set; }
        public int Length { get; private set; }
        public int SelectionStart { get; set; }
        public int SelectionLength { get { return Length; } }

        public MisspelledWordWithLineLocation(string word, string[] suggestions, int index, int length)
            : base(word, suggestions)
        {
            Init(index, length);
        }

        public MisspelledWordWithLineLocation(MisspelledWord misspelledWord, int index, int length)
            : base(misspelledWord)
        {
            Init(index, length);
        }

        private void Init(int index, int length)
        {
            Index = index;
            Length = length;
        }

        internal void SetMisspelledLine(MisspelledLine misspelledLine)
        {
            MisspelledLine = misspelledLine;
        }
    }

    public static class DictionaryHelper
    {
        #region Loading

        static DictionaryHelper()
        {
            var assembly = Assembly.GetExecutingAssembly();
            LoadDictionary(assembly);
            LoadNames(assembly);
        }

        private static WordList dictionary;

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

        private static readonly HashSet<string> names = new HashSet<string>();

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
                            string word = (sr.ReadLine() ?? string.Empty).Trim();
                            if (string.IsNullOrEmpty(word) == false)
                                names.Add(word);
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

        public static bool IsName(string word)
        {
            return names.Contains(FixWord(word));
        }

        public static string[] GetSuggestions(string word, int maxSuggestions = 5)
        {
            return dictionary.Suggest(
                FixWord(word),
                new QueryOptions() { MaxSuggestions = maxSuggestions }
            ).ToArray();
        }

        #endregion

        #region Ignored Words

        private static readonly HashSet<string> ignoredWords = new HashSet<string>();

        public static void AddIgnoredWord(string word)
        {
            ignoredWords.Add(word);
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
                {
                    var arrMisspelledWords = misspelledWords.ToList();
                    var misspelledLine = new MisspelledLine(line, lineIndex, arrMisspelledWords);

                    foreach (var misspelledWord in arrMisspelledWords)
                    {
                        misspelledWord.SelectionStart = misspelledWord.Index + lines.Take(lineIndex).Sum(l => l.Length + 1);
                        misspelledWord.SetMisspelledLine(misspelledLine);
                    }

                    yield return misspelledLine;
                }

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

                if (wordsSpelledCorrectly.Contains(word) || ignoredWords.Contains(word) || names.Contains(word))
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
                    var suggestions = dictionary.Suggest(
                        word,
                        new QueryOptions() { MaxSuggestions = 5 }
                    ).ToArray();

                    var misspelledWord = new MisspelledWord(word, suggestions);
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