using NoMansTrade.Core.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NoMansTrade.Core.Serialization
{
    internal class ObservableCollectionJsonConverter : JsonConverter<ICollection<Item>>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsAssignableFrom(typeof(ObservableCollection<Item>));
        }

        public override ICollection<Item> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var items = new ObservableCollection<Item>();

            
            return items;
        }

        public override void Write(Utf8JsonWriter writer, ICollection<Item> value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value.ToArray(), options);
        }
    }
}
