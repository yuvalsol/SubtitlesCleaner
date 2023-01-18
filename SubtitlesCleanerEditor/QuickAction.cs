﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SubtitlesCleanerLibrary;

namespace SubtitlesCleanerEditor
{
    public delegate QuickActionResult DoQuickActionHandler(List<Subtitle> subtitles, bool isPreview);

    public class QuickAction : INotifyPropertyChanged
    {
        public QuickAction(string action, DoQuickActionHandler @do)
        {
            Action = action;
            Do = @do;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public string Action { get; set; }

        private string result;
        public string Result
        {
            get
            {
                return result;
            }

            set
            {
                result = value;
                OnPropertyChanged();
            }
        }

        public DoQuickActionHandler Do { get; set; }
    }

    public class QuickActionResult
    {
        public bool Succeeded { get; set; }
        public string ResultMessage { get; set; }
        public List<PreviewSubtitle> Preview { get; set; }
    }

    public class PreviewSubtitle
    {
        public int SubtitleNumber { get; set; }
        public Subtitle OriginalSubtitle { get; set; }
        public Subtitle CleanedSubtitle { get; set; }
    }
}
