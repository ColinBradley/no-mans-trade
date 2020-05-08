using System;
using System.Collections.Generic;
using NoMansTrade.App.Support;
using NoMansTrade.Core.Model;
using SkiaSharp;

namespace NoMansTrade.App.Storage
{
    internal class ApplicationConfiguration
    {
        public string ApiEndPoint { get; set; } = "";

        public string ApiKey { get; set; } = "";

        public bool AutoScanNewImages { get; set; } = false;

        public string ScanDirectory { get; set; } = "";

        public LocationCollection Locations { get; set; } = new LocationCollection();

        public string[] AnalyzedNames { get; set; } = Array.Empty<string>();

        public SKRectI LocationRectangle { get; set; } = new SKRectI(50, 50, 100, 100);

        public SKRectI ItemsRectangle { get; set; } = new SKRectI(60, 60, 110, 110);
        
    }
}