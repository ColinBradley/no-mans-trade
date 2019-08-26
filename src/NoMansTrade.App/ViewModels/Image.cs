using NoMansTrade.App.Support;
using System;

namespace NoMansTrade.App.ViewModels
{
    internal class Image
    {
        public Image(string filePath)
        {
            this.FilePath = filePath;
            this.Date = System.IO.File.GetLastWriteTimeUtc(filePath);
        }

        public string FilePath { get; }

        public DateTime Date { get; }

        public ObservableProperty<bool> IsAnalyzed = new ObservableProperty<bool>(false);
    }
}