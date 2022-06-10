using System.Collections.Generic;
using System.Linq;
using MyWorldHub;
using Newtonsoft.Json;

namespace AssetSystem.Reference.Json
{
    [JsonObject]
    public class JsonMasterBundle : IMasterBundle
    {
        [JsonProperty]
        public string MainReferenceId { get; set; }

        [JsonProperty]
        public Dictionary<string,JsonObjectReference> Objects { get; set; }

        [JsonProperty]
        public Dictionary<string,JsonPropertyReference> Resources { get; set; }

        public string GetMainReferenceId() => MainReferenceId;

        private Dictionary<string, IObjectReference> _typedCache;

        public IDictionary<string, IObjectReference> GetObjectReferences()
        {
            return _typedCache ??= Objects
                .Select(kv => new KeyValuePair<string,IObjectReference>(kv.Key,kv.Value))
                .ToDictionary();
        }

        public IDictionary<string, IResourceReference> GetResourceReferences()
        {
            var converter = ServiceLocator.Resolve<IReferenceConverter>();
            var dict = new Dictionary<string, IResourceReference>();
            foreach (var (key, value) in Resources)
            {
                var (result,resource) = converter.ConvertPropertyToResource(value);
                if (result)
                {
                    dict[key] = resource;
                }
            }
            return dict;
        }

        public string Encode()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}