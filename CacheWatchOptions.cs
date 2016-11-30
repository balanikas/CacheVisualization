using System.Collections.Generic;

namespace CacheVisualization
{
    public class CacheWatchOptions
    {
        public string Url { get; set; } = "http://localhost:8080/";
        public int UpdateInterval { get; set; } = 5000;
        public HashSet<string> ExcludedCachePaths { get; set; } = new HashSet<string>();
        public HashSet<string> CachePaths { get; set; } = new HashSet<string>();
        public int CacheSnapshotCount { get; set; } = 20;
        public WatchMode Mode { get; set; } = WatchMode.All;
    }
}