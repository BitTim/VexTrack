using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using VexTrack.Core.Model.Game;

namespace VexTrack.Core.IO.ApiData;

public static class ApiDataLoader
{
    internal static int LoadApiData()
    {
        if (!File.Exists(Constants.CachePath) || File.ReadAllText(Constants.CachePath) == "") return 0;
        
        var rawJson = File.ReadAllText(Constants.CachePath);
        var cache = JObject.Parse(rawJson);
        
        // If newer version exists, don't load cache
        var version = cache.Value<string>("version");
        var remoteVersion = ApiDataFetcher.FetchVersion();
        if (version != remoteVersion && !string.IsNullOrEmpty(remoteVersion)) return 0;

        // Maps
        var maps = LoadMaps(cache);
        if (maps.Count == 0) return -1;
        
        //Model.ApiData.SetData();
        Model.ApiData.Version = version;

        return 0;
    }

    
    
    // ================================
    //  Maps
    // ================================

    private static List<Map> LoadMaps(JObject cache)
    {
        List<Map> maps = new();
        foreach (var map in cache.Value<JArray>("maps"))
        {
            var uuid = map.Value<string>("uuid");
            var name = map.Value<string>("name");
            var listViewImagePath = map.Value<string>("listViewImagePath");
            var splashImagePath = map.Value<string>("splashImagePath");
            
            
            maps.Add(new Map(uuid, name, listViewImagePath, splashImagePath));
        }

        return maps;
    }
}