﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NoMansTrade.Core.Model;
using System;
using System.Collections.ObjectModel;

namespace NoMansTrade.Core.Serialization
{
    internal class ObservableCollectionJsonConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsAssignableFrom(typeof(ObservableCollection<Item>));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            var items = JArray.Load(reader).ToObject<Item[]>();

            foreach (var item in items)
            {
                ((ObservableCollection<Item>)existingValue).Add(item);
            }

            return existingValue;
        }

        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            writer.WriteStartArray();
            writer.WriteEndArray();
        }
    }
}
