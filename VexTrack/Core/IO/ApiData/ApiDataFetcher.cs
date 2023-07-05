using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Documents;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VexTrack.Core.Helper;
using VexTrack.Core.Model;
using VexTrack.Core.Model.Templates;
using VexTrack.Core.Model.WPF;

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

        // Fetch Agents and AgentRoles

        var (agents, agentRoles) = FetchAgentsAndRoles();
        if (agents.Count == 0 || agentRoles.Count == 0) return -3;
        
        // Fetch Contracts and Gears

        var (contracts, gears) = FetchContractsAndGears();
        if (contracts.Count == 0 || gears.Count == 0) return -4;
        
        // Fetch Cosmetics

        var cosmetics = FetchCosmetics();
        
        // Apply fetched data
        
        Model.ApiData.SetData(version, maps, gameModes, agents, agentRoles, contracts, gears, cosmetics);
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
    //  Agents
    // ================================

    private static (List<Agent>, List<AgentRole>) FetchAgentsAndRoles()
    {
        List<Agent> agents = new();
        List<AgentRole> agentRoles = new();

        var agentsResponse = ApiHelper.Request("https://valorant-api.com/v1/agents?isPlayableCharacter=true");
        if (agentsResponse.Count == 0) return (agents, agentRoles);

        var agentsData = agentsResponse.Value<JArray>("data");

        foreach (var jTokenAgent in agentsData)
        {
            var agent = (JObject)jTokenAgent;
            var uuid = (string)agent["uuid"];
            var name = (string)agent["displayName"];
            var description = (string)agent["description"];

            var iconPath = ApiHelper.DownloadImage((string)agent["displayIcon"], Constants.AgentsIconFolder, uuid);
            var portraitPath = ApiHelper.DownloadImage((string)agent["fullPortrait"], Constants.AgentsPortraitFolder, uuid);
            var killFeedPortraitPath = ApiHelper.DownloadImage((string)agent["killfeedPortrait"], Constants.AgentsKillFeedPortraitFolder, uuid);
            var backgroundPath = ApiHelper.DownloadImage((string)agent["background"], Constants.AgentsBackgroundFolder, uuid);

            var isBaseContent = (bool)agent["isBaseContent"];

            // Gradient colors
            var gradientColorsToken = agent["backgroundGradientColors"];
            if (gradientColorsToken == null) continue;

            List<string> gradientColors = new();

            foreach (var jTokenGradientColor in (JArray)gradientColorsToken)
            {
                gradientColors.Add((string)jTokenGradientColor);
            }
            
            // Role
            var role = (JObject)agent["role"];
            if (role == null) return (agents, agentRoles);
            
            var roleUuid = (string)role["uuid"];
            if (!agentRoles.Exists(ar => ar.Uuid == roleUuid))
            {
                var roleName = (string)role["displayName"];
                var roleDescription = (string)role["description"];
                var roleIconPath = ApiHelper.DownloadImage((string)role["displayIcon"], Constants.AgentRolesIconFolder, roleUuid);
                
                agentRoles.Add(new AgentRole(roleUuid, roleName, roleDescription, roleIconPath));
            }

            // Abilities
            var abilitiesToken = agent["abilities"];
            if (abilitiesToken == null) continue;

            List<AgentAbility> abilities = new();

            foreach (var jTokenAbility in (JArray)abilitiesToken)
            {
                var ability = (JObject)jTokenAbility;
                var abilityName = (string)ability["displayName"];
                var abilityDescription = (string)ability["description"];
                var abilitySlot = (string)ability["slot"];
                
                var abilityIconPath = ApiHelper.DownloadImage((string)ability["displayIcon"], Constants.AgentAbilitiesIconFolder, uuid + "-" + abilitySlot);
                abilities.Add(new AgentAbility(abilityName, abilityDescription, abilitySlot, abilityIconPath));
            }
            
            // Add agent to list
            agents.Add(new Agent(uuid, name, description, iconPath, portraitPath, killFeedPortraitPath, backgroundPath, gradientColors, isBaseContent, roleUuid, abilities));
        }

        return (agents, agentRoles);
    }

    
    
    // ================================
    //  Contracts
    // ================================

    private static (List<ContractTemplate>, List<GearTemplate>) FetchContractsAndGears()
    {
        List<ContractTemplate> contracts = new();
        List<GearTemplate> gears = new();
        
        var contractsResponse = ApiHelper.Request("https://valorant-api.com/v1/contracts");
        if (contractsResponse.Count == 0) return (contracts, gears);

        var contractsData = contractsResponse.Value<JArray>("data");
        
        foreach (var jTokenContract in contractsData)
        {
            var contract = (JObject)jTokenContract;
            var uuid = (string)contract["uuid"];
            var name = (string)contract["displayName"];

            // Fix names being in FULL CAPS and inconsistent spaces
            name = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(name.ToLower());
            name = name.Replace(" : ", ": ");
            name = name.Replace("Riotx", "Riot x");

            var content = (JObject)contract["content"];
            if(content == null) continue;
            
            var type = (string)content["relationType"];
            var relUuid = (string)content["relationUuid"];

            var chapters = content["chapters"];
            if (chapters == null) continue;

            List<GoalTemplate> goals = new();

            foreach (var jTokenChapter in (JArray)chapters)
            {
                var chapter = (JObject)jTokenChapter;
                var isEpilogue = (bool)chapter["isEpilogue"];
                var levels = chapter["levels"];
                if (levels == null) continue;

                foreach (var jTokenLevel in (JArray)levels)
                {
                    var level = (JObject)jTokenLevel;
                    var xp = (int)level["xp"];
                    var doughCost = (int)level["doughCost"];
                    var canBuyDough = (bool)level["isPurchasableWithDough"];
                    var vpCost = (int)level["vpCost"];
                    var canBuyVp = (bool)level["isPurchasableWithVP"];
                    
                    var reward = (JObject)level["reward"];
                    if(reward == null) continue;

                    var rewardUuid = (string)reward["uuid"];
                    var rewardType = (string)reward["type"];
                    var rewardAmount = (int)reward["amount"];

                    var rewardObj = new Reward(rewardUuid, rewardType, rewardAmount, true);
                    goals.Add(new GoalTemplate(new List<Reward> {rewardObj}, canBuyDough, doughCost, xp, canBuyVp, vpCost, isEpilogue));
                }
                
                var freeRewards = chapter["freeRewards"];
                if (freeRewards != null && freeRewards.Type != JTokenType.Null)
                {
                    foreach (var jTokenReward in (JArray)freeRewards)
                    {
                        var reward = (JObject)jTokenReward;
                        var rewardUuid = (string)reward["uuid"];
                        var rewardType = (string)reward["type"];
                        var rewardAmount = (int)reward["amount"];

                        var rewardObj = new Reward(rewardUuid, rewardType, rewardAmount, false);
                        goals.Last().Rewards.Add(rewardObj); // Free Rewards are always added to the last level of chapter
                    }
                }
            }

            if (string.IsNullOrEmpty(type)) // Edge case for "PLAY TO UNLOCK AGENTS"
            {
                contracts.Add(new ContractTemplate(uuid, name, "Event", -1, -1, goals));
                continue;
            }
            
            if (type == "Agent") // Agents
            {
                gears.Add(new GearTemplate(uuid, name, relUuid, goals));
            }
            else // Seasons and Events
            {
                var baseUrl = type == "Season" ? "https://valorant-api.com/v1/seasons/" : type == "Event" ? "https://valorant-api.com/v1/events/" : "";
                if(baseUrl == "") continue;

                var relationResponse = ApiHelper.Request(baseUrl + relUuid);
                if (relationResponse.Count == 0) continue;

                var relationData = relationResponse.Value<JObject>("data");

                var startTime = (DateTimeOffset)relationData["startTime"];
                var endTime = (DateTimeOffset)relationData["endTime"];

                var startTimestamp = startTime.ToUnixTimeSeconds();
                var endTimestamp = endTime.ToUnixTimeSeconds();
                    
                contracts.Add(new ContractTemplate(uuid, name, type, startTimestamp, endTimestamp, goals));
            }
        }

        return (contracts, gears);
    }

    
    
    // ================================
    //  Cosmetics
    // ================================

    private static List<Cosmetic> FetchCosmetics()
    {
        List<Cosmetic> cosmetics = new();
        return cosmetics;
    }
}