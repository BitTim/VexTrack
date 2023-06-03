using Newtonsoft.Json.Linq;
using VexTrack.Core.Helper;

namespace VexTrack.Core.IO.ApiData;

public static class ApiDataFetcher
{
    internal static void FetchApiData()
    {
        // Version checking
        
        var versionResponse = ApiHelper.Request("https://valorant-api.com/v1/version");
        var versionStatus = versionResponse.Value<int>("status");
        if (versionStatus != 200) return;

        var version = versionResponse.Value<JObject>("data").Value<string>("version");
        if (Model.ApiData.Version == version) return;
        
        
        
        // Continue fetching

        
        
        // Apply fetched data
        
        Model.ApiData.SetData(version);
        ApiDataSaver.SaveApiData();
    }
}