using System.Collections.Generic;
using Newtonsoft.Json;

namespace CacheVisualization
{
    class CacheDataSet
    {
        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonIgnore]
        public string Name { get; set; }

        [JsonProperty("data")]
        public List<long> Data { get; set; } = new List<long>();

        [JsonProperty("values")]
        public List<string> Values { get; set; } = new List<string>();

        [JsonProperty("fill")]
        public bool Fill { get; }

        [JsonProperty("backgroundColor")]
        public string BackgroundColor { get; set; }

        [JsonProperty("borderColor")]
        public string BorderColor { get; set; }

        [JsonProperty("hidden")]
        public bool Hidden { get; set; } = false;
    }
}