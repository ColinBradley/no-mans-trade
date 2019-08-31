using NoMansTrade.App.Support;
using System;
using System.ComponentModel;

namespace NoMansTrade.App.ViewModels
{
    internal class Image
    {
        public Image(string filePath)
        {
            this.FilePath = filePath;
            this.Date = System.IO.File.GetLastWriteTimeUtc(filePath);

            this.IsAnalyzing.PropertyChanged += this.UpdateStateMessage;
            this.IsAnalyzed.PropertyChanged += this.UpdateStateMessage;
        }

        public string FilePath { get; }

        public DateTime Date { get; }

        public ObservableProperty<bool> IsAnalyzing { get; } = new ObservableProperty<bool>(false);

        public ObservableProperty<bool> IsAnalyzed { get; } = new ObservableProperty<bool>(false);

        public ObservableProperty<string> StateMessage { get; } = new ObservableProperty<string>("Awaiting Analysis");

        private void UpdateStateMessage(object sender, PropertyChangedEventArgs e)
        {
            if (this.IsAnalyzing.Value)
            {
                this.StateMessage.Value = "Analyzing...";
                return;
            }
            if (this.IsAnalyzed.Value)
            {
                this.StateMessage.Value = "Analyzed ✔";
            }
        }
    }
}