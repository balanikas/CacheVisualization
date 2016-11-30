using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Timers;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.Host.HttpListener;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.StaticFiles;
using Owin;

namespace CacheVisualization
{
    public sealed class CacheWatch : IDisposable
    {
        Timer _timer;
        IDisposable _webApp;
        readonly CacheConversion _cacheConversion;
        readonly string _workingDirectory;
        CacheWriter _cacheWriter;

        public CacheWatch()
        {
            _cacheConversion = new CacheConversion();
            _workingDirectory = Path.Combine(Path.GetTempPath(), "cachevisualization");
        }

        public void AddConversion(string cacheKey, Func<object, string> conversion)
        {
            _cacheConversion.Add(cacheKey, conversion);
        }

        public void Start()
        {
            Start(new CacheWatchOptions());
        }

        public void Start(CacheWatchOptions options)
        {
            if (_timer != null)
            {
                throw new InvalidOperationException("Cache watch is already started.");
            }

            CopyVisualizationFiles();
           
            StartWebServer(options.Url);

            _cacheWriter = new CacheWriter(_workingDirectory, options, _cacheConversion);

            _timer = new Timer {AutoReset = false, Enabled = true, Interval = options.UpdateInterval};
              
            _timer.Elapsed += OnTimedEvent;
        }

        public void Stop()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }

            _webApp?.Dispose();
        }
        
        public void Dispose()
        {
            Stop();
        }

        private void StartWebServer(string url)
        {
            //workaround so that context has all assemblies needed in /bin
            var stupid = typeof(OwinHttpListener);
            if (stupid != null) { }

            var fileServerOptions = new FileServerOptions
            {
                EnableDirectoryBrowsing = true,
                FileSystem = new PhysicalFileSystem(_workingDirectory),
                StaticFileOptions = { ServeUnknownFileTypes = true }
            };

            _webApp = WebApp.Start(url, builder => builder.UseFileServer(fileServerOptions));
            
            Process.Start(url);
        }

        void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            _cacheWriter.Write();
            _timer.Start();
        }

        void CopyVisualizationFiles()
        {
            ClearWorkingDirectory();
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CacheVisualization.frontend.index.html"))
            {
                using (var fileStream = File.Create(Path.Combine(_workingDirectory, "index.html")))
                {
                    stream.CopyTo(fileStream);
                }
            }
        }

        void ClearWorkingDirectory()
        {
            var directoryInfo = new DirectoryInfo(_workingDirectory);

            foreach (var file in directoryInfo.GetFiles())
            {
                file.Delete();
            }
            foreach (var directory in directoryInfo.GetDirectories())
            {
                directory.Delete(true);
            }
        }
    }
}
