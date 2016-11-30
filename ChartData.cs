using System.Collections.Generic;
using Newtonsoft.Json;

namespace CacheVisualization
{
    class ChartData
    {
        [JsonProperty("labels")]
        public List<string> Labels { get; set; } = new List<string>();

        [JsonProperty("datasets")]
        public List<CacheDataSet> DataSets { get; set; } = new List<CacheDataSet>();
    }
}