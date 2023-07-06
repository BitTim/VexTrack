using Newtonsoft.Json.Linq;
using VexTrack.Core.Model.Game;

namespace VexTrack.Core.IO.ApiData;

public static class ApiDataSaver
{
    internal static void SaveApiData()
    {
        JObject cache = new() { { "version", Model.ApiData.Version } };
        
        
        
        //  Maps
        JArray maps = new();
        foreach (var map in Model.ApiData.Maps)
        {
            JObject mapObj = new()
            {
                { "uuid", map.Uuid },
                { "name", map.Name },
                { "listViewImagePath", map.ListViewImagePath },
                { "splashImagePath", map.SplashImagePath }
            };
            
            maps.Add(mapObj);
        }
        cache.Add("maps", maps);
        
        
        
        //  GameModes
        JArray gameModes = new();
        foreach (var gameMode in Model.ApiData.GameModes)
        {
            JObject gameModeObj = new()
            {
                { "uuid", gameMode.Uuid },
                { "name", gameMode.Name },
                { "scoreType", gameMode.ScoreType },
                { "iconPath", gameMode.IconPath }
            };
            
            gameModes.Add(gameModeObj);
        }
        cache.Add("gameModes", gameModes);
    }
}