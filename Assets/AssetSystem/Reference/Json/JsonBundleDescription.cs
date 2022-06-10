using Newtonsoft.Json;

namespace AssetSystem.Reference.Json
{
    [JsonObject]
    public class JsonBundleDescription : IBundleDescription
    {
        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty]
        public string BundleId { get; set; }
        
        [JsonProperty]
        public string AuthorId { get; set; }
        
        [JsonProperty]
        public long CreatedTime { get; set; }
        
        [JsonProperty]
        public long UpdatedTime { get; set; }
    }
}