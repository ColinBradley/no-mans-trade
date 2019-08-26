using NoMansTrade.Core.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace NoMansTrade.Core.Serialization
{
    public static class JsonSerialization
    {
        private static readonly JsonSerializerOptions sJsonOptions = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        static JsonSerialization()
        {
            sJsonOptions.Converters.Add(new ObservableCollectionJsonConverter());
        }

        public static async Task<Location[]> Parse(Stream data)
        {
            using var reader = new StreamReader(data, Encoding.UTF8, leaveOpen: true);
            var json = await reader.ReadToEndAsync();

            return JsonSerializer.Deserialize<Location[]>(json, sJsonOptions);
        }

        public static void Store(IEnumerable<Location> data, Stream stream)
        {
            using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);

            var json = JsonSerializer.Serialize(data, sJsonOptions);

            writer.Write(json);
        }
    }
}
