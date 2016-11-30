# CacheVisualization
a nuget package that can be used in any .net web app to visualize http runtime cache

# Usage
### Initialize and start
```csharp
var watch = new CacheWatch();
watch.Start();
```
### Conversions
```csharp
//Conversions mean that when a cache key starts with the specified string, 
// we want to show the cached data in a specific way.
//In this example we will show the the name of the cart.
watch.AddConversion("mywebapp:cachekey:cart", o => (o as Cart).Name);
```

### Options
```csharp
watch.Start(new CacheWatchOptions
{
    //this mode will only show the data that is cached using the keys in CachePaths, 
    //WatchMode.All shows everything and is suitable for simple web apps.
    Mode = WatchMode.Specific, 
    Url = "http://localhost:8080/", //cache visualization will run on this host
    UpdateInterval = 3000, //read cache every 3 seconds
    CacheSnapshotCount = 10, //show only last 10 cache reads
    CachePaths = new HashSet<string> //we are only interested in cache data that use these keys
    {
        "mywebapp:cachekey:order",
        "mywebapp:cachekey:cart",
        "customers:
    }
});
```

# Installation
Use http://nuget.org as the nuget feed to download it. Package page is here: https://www.nuget.org/packages/CacheVisualization/.

To create the package locally, clone this repo and create the nuget package:
```
nuget pack CacheVisualization.csproj
```
and then, add it to your project:
```
install-package CacheVisualization
```

# Source code
http://github.com/balanikas/CacheVisualization

