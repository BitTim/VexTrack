using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using VexTrack.Core.Helper;
using VexTrack.Core.Model;

namespace VexTrack.Core.IO.ApiData;

public static class ApiDataFetcher
{
    internal static int FetchApiData()
    {
        // Version checking

        var version = FetchVersion();
        if (string.IsNullOrEmpty(version)) return 0;

        // Fetch Maps

        var maps = FetchMaps();
        if (maps.Count == 0) return -1;

        
        
        
        
        // Apply fetched data
        
        Model.ApiData.SetData(version, maps);
        ApiDataSaver.SaveApiData();
        return 0;
    }

    private static string FetchVersion()
    {
        var versionResponse = ApiHelper.Request("https://valorant-api.com/v1/version");
        if (versionResponse.Count == 0) return "";
        
        var version = versionResponse.Value<JObject>("data").Value<string>("version");
        if (Model.ApiData.Version == version) return "";

        return version;
    }

    private static List<Map> FetchMaps()
    {
        List<Map> maps = new();

        var mapsResponse = ApiHelper.Request("https://valorant-api.com/v1/maps");
        if (mapsResponse.Count == 0) return maps;

        var mapsData = mapsResponse.Value<JArray>("data");

        foreach (var jTokenMap in mapsData)
        {
            var map = (JObject)jTokenMap;
            var uuid = (string)map["uuid"];
            var name = (string)map["displayName"];

            var listViewImagePath = ApiHelper.DownloadImage((string)map["listViewIcon"], Constants.MapListViewImageFolder, uuid);
            var splashImagePath = ApiHelper.DownloadImage((string)map["splash"], Constants.MapSplashImageFolder, uuid);
            
            maps.Add(new Map(uuid, name, listViewImagePath, splashImagePath));
        }

        // Add empty map as option
        maps.Add(new Map("", "None", "", "")); 
        return maps;
    }
}