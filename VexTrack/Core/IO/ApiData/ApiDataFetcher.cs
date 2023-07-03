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

        // Fetch GameModes

        var gameModes = FetchGameModes();
        if (gameModes.Count == 0) return -2;

        // Apply fetched data
        
        Model.ApiData.SetData(version, maps, gameModes);
        ApiDataSaver.SaveApiData();
        return 0;
    }

    
    
    // ================================
    //  Version
    // ================================
    
    private static string FetchVersion()
    {
        var versionResponse = ApiHelper.Request("https://valorant-api.com/v1/version");
        if (versionResponse.Count == 0) return "";
        
        var version = versionResponse.Value<JObject>("data").Value<string>("version");
        if (Model.ApiData.Version == version) return "";

        return version;
    }

    
    
    // ================================
    //  Maps
    // ================================
    
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

    
    
    // ================================
    //  GameModes
    // ================================
    
    private static List<GameMode> FetchGameModes()
    {
        List<GameMode> gameModes = new();

        var gameModesResponse = ApiHelper.Request("https://valorant-api.com/v1/gamemodes");
        if (gameModesResponse.Count == 0) return gameModes;
        
        var gameModesData = gameModesResponse.Value<JArray>("data");

        foreach (var jTokenGameMode in gameModesData)
        {
            var gameMode = (JObject)jTokenGameMode;
            var uuid = (string)gameMode["uuid"];
            var name = (string)gameMode["displayName"];
            
            var iconPath = ApiHelper.DownloadImage((string)gameMode["displayIcon"], Constants.GameModeIconFolder, uuid);
            
            gameModes.Add(new GameMode(uuid, name, name == "Deathmatch" ? "Placement" : "Score", iconPath));
        }
        
        // Adjust "Standard" game mode to have Competitive and Unrated game modes for selection
        var stdIdx = gameModes.FindIndex(gm => gm.Name == "Standard");
        gameModes[stdIdx].Name = "Unrated";
        gameModes.Insert(stdIdx + 1, new GameMode(Constants.CompetitiveGameModeUuid, "Competitive", "Score", gameModes[stdIdx].IconPath));
        
        // Remove Onboarding
        var onboardingIdx = gameModes.FindIndex(gm => gm.Name == "Onboarding");
        if (onboardingIdx != -1) gameModes.RemoveAt(onboardingIdx);
        
        // Fix Name for Practice
        var practiceIdx = gameModes.FindIndex(gm => gm.Name == "PRACTICE");
        if (practiceIdx != -1) gameModes[practiceIdx].Name = "Practice";
        
        // Add custom GameMode as option
        gameModes.Add(new GameMode("", "Custom", "None", ""));
        return gameModes;
    }

    
    
    // ================================
    //  Contracts
    // ================================

    private static List<ContractTemplate> FetchContracts()
    {
        return new List<ContractTemplate>();
    }
}