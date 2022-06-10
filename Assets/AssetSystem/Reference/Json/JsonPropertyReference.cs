using System.Collections.Generic;
using Newtonsoft.Json;

namespace AssetSystem.Reference.Json
{
    [JsonObject]
    public class JsonPropertyReference : IPropertyReference
    {
        [JsonProperty]
        public string Type { get; set; }

        [JsonProperty]
        public Dictionary<string, string> Data { get; set; }

        public string GetPropertyType() => Type;

        public IDictionary<string, string> GetProperties()
        {
            return Data;
        }
    }
}