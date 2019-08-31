using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NoMansTrade.Core.Model;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace NoMansTrade.Core.Serialization
{
    public static class JsonSerialization
    {
        public static JsonSerializerSettings Settings { get; } = new JsonSerializerSettings()
        {
            ContractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            }
        };

        static JsonSerialization()
        {
            Settings.Converters.Add(new ObservableCollectionJsonConverter());
        }

        public static async Task<Location[]> Parse(Stream data)
        {
            using var reader = new StreamReader(data, Encoding.UTF8, leaveOpen: true);
            var json = await reader.ReadToEndAsync();

            return JsonConvert.DeserializeObject<Location[]>(json, Settings);
        }

        public static void Store(IEnumerable<Location> data, Stream stream)
        {
            using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);

            var test = JsonConvert.SerializeObject(data);

            writer.Write(test);
        }
    }
}
