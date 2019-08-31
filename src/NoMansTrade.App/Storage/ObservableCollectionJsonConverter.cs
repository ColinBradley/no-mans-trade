using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NoMansTrade.Core.Model;
using SkiaSharp;
using System;
using System.Collections.ObjectModel;

namespace NoMansTrade.Core.Serialization
{
    internal class SKRectIJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsAssignableFrom(typeof(SKRectI));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var values = JArray.Load(reader).ToObject<int[]>();

            return new SKRectI(values[0], values[1], values[2], values[3]);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var native = (SKRectI)value;
            writer.WriteStartArray();
            writer.WriteValue(native.Left);
            writer.WriteValue(native.Top);
            writer.WriteValue(native.Right);
            writer.WriteValue(native.Bottom);
            writer.WriteEndArray();
        }
    }
}
