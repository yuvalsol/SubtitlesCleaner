namespace SubtitlesCleaner.Command
{
    internal sealed class DebugOptions
    {
        private DebugOptions() { }

        public static DebugOptions Instance
        {
            get { return SingletonCreator.instance; }
        }

        private class SingletonCreator
        {
            static SingletonCreator() { }
            internal static readonly DebugOptions instance = new DebugOptions();
        }

        public bool PrintCleaning { get; private set; } = SubtitlesHandler.PrintCleaning;
        public bool CleanHICaseInsensitive { get; private set; } = SubtitlesHandler.CleanHICaseInsensitive;
        public int? FirstSubtitlesCount { get; private set; } = SubtitlesHandler.FirstSubtitlesCount;
    }
}
