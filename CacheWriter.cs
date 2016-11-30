using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace CacheVisualization
{
    class CacheWriter
    {
        private readonly string _workingDirectory;
        private readonly CacheWatchOptions _options;
        private readonly CacheConversion _cacheConversion;
        private readonly ChartData _chartData;
        private readonly CacheReader _cacheReader;
        static readonly Random _random = new Random();

        public CacheWriter(
            string workingDirectory, 
            CacheWatchOptions options,
            CacheConversion cacheConversion)
        {
            _cacheConversion = cacheConversion;
            _workingDirectory = workingDirectory;
            _options = options;
            _cacheReader = new CacheReader(_options.CachePaths, _options.ExcludedCachePaths);
            _chartData = new ChartData();
        }
        
        public void Write()
        {
            _chartData.Labels.Add(DateTime.Now.ToLongTimeString());

            var cache = _options.Mode == WatchMode.All ? _cacheReader.GetCache() : _cacheReader.GetGroupedCache();

            foreach (var cachedItem in cache)
            {
                var dataSet = _chartData.DataSets.SingleOrDefault(x => x.Name == cachedItem.Key);
                if (dataSet == null)
                {
                    dataSet = new CacheDataSet();
                    dataSet.Name = cachedItem.Key;

                    dataSet.Data.AddRange(Enumerable.Repeat(0L, _chartData.Labels.Count - 1));
                    dataSet.Data.Add(cachedItem.Value.Count);

                    foreach (var o in cachedItem.Value)
                    {
                        dataSet.Values.Add(_cacheConversion.GetValue(cachedItem.Key, o));
                    }

                    var color = GetRandomConsoleColor();
                    dataSet.BackgroundColor = color;
                    dataSet.BorderColor = color;
                    _chartData.DataSets.Add(dataSet);
                }
                else
                {
                    dataSet.Data.Add(cachedItem.Value.Count);

                    dataSet.Values.Clear();
                    foreach (var o in cachedItem.Value)
                    {
                        dataSet.Values.Add(_cacheConversion.GetValue(cachedItem.Key, o));
                    }
                }
            }

            for (var i = 0; i < _chartData.DataSets.Count; i++)
            {
                if (!_chartData.DataSets[i].Data.Any())
                {
                    _chartData.DataSets.RemoveAt(i);
                }
            }

            if (_chartData.Labels.Count > _options.CacheSnapshotCount) 
            {
                _chartData.Labels.RemoveAt(0);
                foreach (var dataSet in _chartData.DataSets)
                {
                    dataSet.Data.RemoveAt(0);
                }
            }

            foreach (var dataSet in _chartData.DataSets)
            {
                dataSet.Label = dataSet.Name + $" ({dataSet.Values.Count})";
            }

            WriteChartData(_chartData);
        }

        void WriteChartData(ChartData chartData)
        {
            File.WriteAllText(Path.Combine(_workingDirectory, "data.json"), JsonConvert.SerializeObject(chartData, Formatting.Indented));
        }

        string GetRandomConsoleColor()
        {
            var consoleColors = new string[] { "green", "red", "blue", "brown", "darkgreen", "orange", "plum", "slateblue", "teal", "mediumaquamarine", "cadetblue", "gold", "deepskyblue" };
            return consoleColors[_random.Next(consoleColors.Length)];
        }
    }
}
