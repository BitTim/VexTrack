using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using VexTrack.Core.Helper;
using VexTrack.Core.Model.Game;
using VexTrack.Core.Model.Game.Agent;
using VexTrack.Core.Model.Game.Cosmetic;
using VexTrack.Core.Model.Game.Cosmetic.Weapon;
using VexTrack.Core.Model.Game.Weapon;
using VexTrack.Core.Model.Game.Weapon.Stats;
using VexTrack.Core.Model.Templates;

namespace VexTrack.Core.IO.ApiData;

public static class ApiDataFetcher
{
    internal static int FetchApiData()
    {
        // Version checking

        var version = FetchVersion();
        if (string.IsNullOrEmpty(version)) return 0;
        
        // Backup and Clear Assets Folder
        
        BackupAndClearAssets();
        
        // Fetch Maps

        var maps = FetchMaps();
        if (maps.Count == 0)
        {
            RestoreAssetsBackup();
            return -1;
        }

        // Fetch GameModes

        var gameModes = FetchGameModes();
        if (gameModes.Count == 0)
        {
            RestoreAssetsBackup();
            return -2;
        }

        // Fetch Agents and AgentRoles

        var (agents, agentRoles) = FetchAgentsAndRoles();
        if (agents.Count == 0 || agentRoles.Count == 0)
        {
            RestoreAssetsBackup();
            return -3;
        }

        // Fetch Contracts and Gears

        var (contracts, gears) = FetchContractsAndGears();
        if (contracts.Count == 0 || gears.Count == 0)
        {
            RestoreAssetsBackup();
            return -4;
        }

        // Fetch Buddies

        var buddies = FetchBuddies();
        if (buddies.Count == 0)
        {
            RestoreAssetsBackup();
            return -5;
        }

        // Fetch Currencies

        var currencies = FetchCurrencies();
        if (currencies.Count == 0)
        {
            RestoreAssetsBackup();
            return -6;
        }

        // Fetch Player Cards

        var playerCards = FetchPlayerCards();
        if (playerCards.Count == 0)
        {
            RestoreAssetsBackup();
            return -7;
        }

        // Fetch Player Titles

        var playerTitles = FetchPlayerTitles();
        if (playerTitles.Count == 0)
        {
            RestoreAssetsBackup();
            return -8;
        }

        // Fetch Sprays

        var sprays = FetchSprays();
        if (sprays.Count == 0)
        {
            RestoreAssetsBackup();
            return -9;
        }

        // Fetch Weapons, Weapon Skins, Weapon Skin Chromas and Weapon Skin Levels

        var (weapons, weaponSkins, weaponSkinChromas, weaponSkinLevels) = FetchWeaponsAndSkins();
        if (weapons.Count == 0 || weaponSkins.Count == 0 || weaponSkinChromas.Count == 0 || weaponSkinLevels.Count == 0)
        {
            RestoreAssetsBackup();
            return -10;
        }

        // Apply fetched data
        
        Model.ApiData.SetData(version, maps, gameModes, agents, agentRoles, contracts, gears, weapons, buddies, currencies, playerCards, playerTitles, sprays, weaponSkins, weaponSkinChromas, weaponSkinLevels);
        ApiDataSaver.SaveApiData();
        DeleteAssetsBackup(); // Delete Backup since new data was downloaded successfully
        return 0;
    }


    // ================================
    //  Assets Folder
    // ================================

    private static void DeleteAssetsBackup()
    {
        if(Directory.Exists(Constants.AssetBackupFolder)) Directory.Delete(Constants.AssetBackupFolder, true);
    }

    private static void BackupAndClearAssets()
    {
        DeleteAssetsBackup();
        Directory.Move(Constants.AssetFolder, Constants.AssetBackupFolder);
        if(Directory.Exists(Constants.AssetFolder)) Directory.Delete(Constants.AssetFolder, true);
    }

    private static void RestoreAssetsBackup()
    {
        if(Directory.Exists(Constants.AssetFolder)) Directory.Delete(Constants.AssetFolder, true);
        Directory.Move(Constants.AssetBackupFolder, Constants.AssetFolder);
    }


    // ================================
    //  Version
    // ================================
    
    public static string FetchVersion()
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

        foreach (var map in mapsData)
        {
            var uuid = map.Value<string>("uuid");
            var name = map.Value<string>("displayName");

            var listViewImagePath = ApiHelper.DownloadImage(map.Value<string>("listViewIcon"), Constants.MapListViewImageFolder, uuid);
            var splashImagePath = ApiHelper.DownloadImage(map.Value<string>("splash"), Constants.MapSplashImageFolder, uuid);
            
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

        foreach (var gameMode in gameModesData)
        {
            var uuid = gameMode.Value<string>("uuid");
            var name = gameMode.Value<string>("displayName");
            
            var iconPath = ApiHelper.DownloadImage(gameMode.Value<string>("displayIcon"), Constants.GameModeIconFolder, uuid);
            
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

        foreach (var agent in agentsData)
        {
            var uuid = agent.Value<string>("uuid");
            var name = agent.Value<string>("displayName");
            var description = agent.Value<string>("description");

            var iconPath = ApiHelper.DownloadImage(agent.Value<string>("displayIcon"), Constants.AgentIconFolder, uuid);
            var portraitPath = ApiHelper.DownloadImage(agent.Value<string>("fullPortrait"), Constants.AgentPortraitFolder, uuid);
            var killFeedPortraitPath = ApiHelper.DownloadImage(agent.Value<string>("killfeedPortrait"), Constants.AgentKillFeedPortraitFolder, uuid);
            var backgroundPath = ApiHelper.DownloadImage(agent.Value<string>("background"), Constants.AgentBackgroundFolder, uuid);

            var isBaseContent = agent.Value<bool>("isBaseContent");

            // Gradient colors
            var gradientColorsToken = agent["backgroundGradientColors"];
            if (gradientColorsToken == null) continue;

            List<string> gradientColors = new();

            foreach (var jTokenGradientColor in (JArray)gradientColorsToken)
            {
                gradientColors.Add((string)jTokenGradientColor);
            }
            
            // Role
            var role = agent.Value<JObject>("role");
            if (role == null) return (agents, agentRoles);
            
            var roleUuid = role.Value<string>("uuid");
            if (!agentRoles.Exists(ar => ar.Uuid == roleUuid))
            {
                var roleName = role.Value<string>("displayName");
                var roleDescription = role.Value<string>("description");
                var roleIconPath = ApiHelper.DownloadImage(role.Value<string>("displayIcon"), Constants.AgentRoleIconFolder, roleUuid);
                
                agentRoles.Add(new AgentRole(roleUuid, roleName, roleDescription, roleIconPath));
            }

            // Abilities
            var abilitiesToken = agent["abilities"];
            if (abilitiesToken == null) continue;

            List<AgentAbility> abilities = new();

            foreach (var ability in (JArray)abilitiesToken)
            {
                var abilityName = ability.Value<string>("displayName");
                var abilityDescription = ability.Value<string>("description");
                var abilitySlot = ability.Value<string>("slot");
                
                var abilityIconPath = ApiHelper.DownloadImage(ability.Value<string>("displayIcon"), Constants.AgentAbilityIconFolder, uuid + "-" + abilitySlot);
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
        
        foreach (var contract in contractsData)
        {
            var uuid = contract.Value<string>("uuid");
            var name = contract.Value<string>("displayName");

            // Fix names being in FULL CAPS and inconsistent spaces
            name = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(name.ToLower());
            name = name.Replace(" : ", ": ");
            name = name.Replace("Riotx", "Riot x");

            var content = contract.Value<JObject>("content");
            if(content == null) continue;
            
            var type = content.Value<string>("relationType");
            var relUuid = content.Value<string>("relationUuid");

            var chapters = content["chapters"];
            if (chapters == null) continue;

            List<GoalTemplate> goals = new();

            foreach (var chapter in (JArray)chapters)
            {
                var isEpilogue = chapter.Value<bool>("isEpilogue");
                var levels = chapter["levels"];
                if (levels == null) continue;

                foreach (var level in (JArray)levels)
                {
                    var xp = level.Value<int>("xp");
                    var doughCost = level.Value<int>("doughCost");
                    var canBuyDough = level.Value<bool>("isPurchasableWithDough");
                    var vpCost = level.Value<int>("vpCost");
                    var canBuyVp = level.Value<bool>("isPurchasableWithVP");
                    
                    var reward = level.Value<JObject>("reward");
                    if(reward == null) continue;

                    var rewardUuid = reward.Value<string>("uuid");
                    var rewardType = reward.Value<string>("type");
                    var rewardAmount = reward.Value<int>("amount");

                    var rewardObj = new Reward(rewardUuid, rewardType, rewardAmount, true);
                    goals.Add(new GoalTemplate(new List<Reward> {rewardObj}, canBuyDough, doughCost, xp, canBuyVp, vpCost, isEpilogue));
                }
                
                var freeRewards = chapter["freeRewards"];
                if (freeRewards != null && freeRewards.Type != JTokenType.Null)
                {
                    foreach (var reward in (JArray)freeRewards)
                    {
                        var rewardUuid = reward.Value<string>("uuid");
                        var rewardType = reward.Value<string>("type");
                        var rewardAmount = reward.Value<int>("amount");

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

                var startTime = (DateTimeOffset)relationData.Value<DateTime>("startTime");
                var endTime = (DateTimeOffset)relationData.Value<DateTime>("endTime");

                var startTimestamp = startTime.ToUnixTimeSeconds();
                var endTimestamp = endTime.ToUnixTimeSeconds();
                    
                contracts.Add(new ContractTemplate(uuid, name, type, startTimestamp, endTimestamp, goals));
            }
        }

        return (contracts, gears);
    }

    
    
    // ================================
    //  Buddies
    // ================================

    private static List<Buddy> FetchBuddies()
    {
        List<Buddy> buddies = new();

        var buddiesResponse = ApiHelper.Request("https://valorant-api.com/v1/buddies");
        if(buddiesResponse.Count == 0) return buddies;

        var buddiesData = buddiesResponse.Value<JArray>("data");

        foreach (var buddy in buddiesData)
        {
            var uuid = buddy.Value<string>("uuid");
            var name = buddy.Value<string>("displayName");
            var iconPath = ApiHelper.DownloadImage(buddy.Value<string>("displayIcon"), Constants.BuddyIconFolder, uuid);

            buddies.Add(new Buddy(uuid, name, iconPath));
        }

        return buddies;
    }

    
    
    // ================================
    //  Currencies
    // ================================
    
    private static List<Currency> FetchCurrencies()
    {
        List<Currency> currencies = new();

        var currenciesResponse = ApiHelper.Request("https://valorant-api.com/v1/currencies");
        if (currenciesResponse.Count == 0) return currencies;

        var currenciesData = currenciesResponse.Value<JArray>("data");

        foreach (var currency in currenciesData)
        {
            var uuid = currency.Value<string>("uuid");
            var name = currency.Value<string>("displayName");
            var iconPath = ApiHelper.DownloadImage(currency.Value<string>("displayIcon"), Constants.CurrencyIconFolder, uuid);
            var largeIconPath = ApiHelper.DownloadImage(currency.Value<string>("largeIcon"), Constants.CurrencyLargeIconFolder, uuid);
            
            currencies.Add(new Currency(uuid, name, iconPath, largeIconPath));
        }
        
        return currencies;
    }

    
    
    // ================================
    //  Player Cards
    // ================================

    private static List<PlayerCard> FetchPlayerCards()
    {
        List<PlayerCard> playerCards = new();

        var playerCardsResponse = ApiHelper.Request("https://valorant-api.com/v1/playercards");
        if (playerCardsResponse.Count == 0) return playerCards;

        var playerCardsData = playerCardsResponse.Value<JArray>("data");

        foreach (var playerCard in playerCardsData)
        {
            var uuid = playerCard.Value<string>("uuid");
            var name = playerCard.Value<string>("displayName");
            var iconPath = ApiHelper.DownloadImage(playerCard.Value<string>("displayIcon"), Constants.PlayerCardIconFolder, uuid);
            var smallArtPath = ApiHelper.DownloadImage(playerCard.Value<string>("smallArt"), Constants.PlayerCardSmallArtFolder, uuid);
            var wideArtPath = ApiHelper.DownloadImage(playerCard.Value<string>("wideArt"), Constants.PlayerCardWideArtFolder, uuid);
            var largeArtPath = ApiHelper.DownloadImage(playerCard.Value<string>("largeArt"), Constants.PlayerCardLargeArtFolder, uuid);
            
            playerCards.Add(new PlayerCard(uuid, name, iconPath, smallArtPath, wideArtPath, largeArtPath));
        }
        
        return playerCards;
    }

    
    
    // ================================
    //  Player Titles
    // ================================

    private static List<PlayerTitle> FetchPlayerTitles()
    {
        List<PlayerTitle> playerTitles = new();

        var playerTitlesResponse = ApiHelper.Request("https://valorant-api.com/v1/playertitles");
        if (playerTitlesResponse.Count == 0) return playerTitles;

        var playerTitlesData = playerTitlesResponse.Value<JArray>("data");

        foreach (var playerTitle in playerTitlesData)
        {
            var uuid = playerTitle.Value<string>("uuid");
            var name = playerTitle.Value<string>("displayName");
            var titleText = playerTitle.Value<string>("titleText");
            
            playerTitles.Add(new PlayerTitle(uuid, name, titleText));
        }
        
        return playerTitles;
    }

    
    
    // ================================
    //  Sprays
    // ================================

    private static List<Spray> FetchSprays()
    {
        List<Spray> sprays = new();

        var spraysResponse = ApiHelper.Request("https://valorant-api.com/v1/sprays");
        if (spraysResponse.Count == 0) return sprays;

        var spraysData = spraysResponse.Value<JArray>("data");

        foreach (var spray in spraysData)
        {
            var uuid = spray.Value<string>("uuid");
            var name = spray.Value<string>("displayName");
            var iconPath = ApiHelper.DownloadImage(spray.Value<string>("displayIcon"), Constants.SprayIconFolder, uuid);
            var fullIconPath = ApiHelper.DownloadImage(spray.Value<string>("fullTransparentIcon"), Constants.SprayFullIconFolder, uuid);
            var animationPath = ApiHelper.DownloadImage(spray.Value<string>("animationGif"), Constants.SprayAnimationFolder, uuid);
            
            sprays.Add(new Spray(uuid, name, iconPath, fullIconPath, animationPath));
        }

        return sprays;
    }

    
    
    // ================================
    //  Weapons and Skins
    // ================================
    
    private static (List<Weapon> weapons, List<WeaponSkin> weaponSkins, List<WeaponSkinChroma> weaponSkinChromas, List<WeaponSkinLevel> weaponSkinLevels) FetchWeaponsAndSkins()
    {
        List<Weapon> weapons = new();
        List<WeaponSkin> allSkins = new();
        List<WeaponSkinChroma> allChromas = new();
        List<WeaponSkinLevel> allLevels = new();

        var weaponsResponse = ApiHelper.Request("https://valorant-api.com/v1/weapons");
        if (weaponsResponse.Count == 0) return (weapons, allSkins, allChromas, allLevels);

        var weaponsData = weaponsResponse.Value<JArray>("data");

        foreach (var weapon in weaponsData)
        {
            // General Weapon data
            var uuid = weapon.Value<string>("uuid");
            var name = weapon.Value<string>("displayName");
            var category = weapon.Value<string>("category").Replace("EEquippableCategory::", ""); // Convert class names to readable names
            var defaultSkinUuid = weapon.Value<string>("defaultSkinUuid");
            var iconPath = ApiHelper.DownloadImage(weapon.Value<string>("displayIcon"), Constants.WeaponIconPath, uuid);
            var killStreamIconPath = ApiHelper.DownloadImage(weapon.Value<string>("killStreamIcon"), Constants.WeaponKillStreamIconPath, uuid);

            var shopCost = -1;
            var shopData = weapon.Value<JObject>("shopData");
            if (shopData != null) shopCost = shopData.Value<int>("cost");

            // Weapon stats
            WeaponStats stats = null;
            var rawStats = weapon.Value<JObject>("weaponStats");
            if (rawStats != null)
            {
                var fireRate = rawStats.Value<float>("fireRate");
                var magazineSize = rawStats.Value<int>("magazineSize");
                var runSpeedMultiplier = rawStats.Value<float>("runSpeedMultiplier");
                var equipTimeSeconds = rawStats.Value<float>("equipTimeSeconds");
                var reloadTimeSeconds = rawStats.Value<float>("reloadTimeSeconds");
                var firstBulletAccuracy = rawStats.Value<float>("firstBulletAccuracy");
                var shotgunPelletCount = rawStats.Value<int>("shotgunPelletCount");

                var wallPenetration = rawStats.Value<string>("wallPenetration").Replace("EWallPenetrationDisplayType::", ""); // Convert class names to readable names
                var feature = (rawStats.Value<string>("feature") ?? "").Replace("EWeaponStatsFeature::", ""); // Convert class names to readable names
                var fireMode = (rawStats.Value<string>("fireMode") ?? "").Replace("EWeaponFireModeDisplayType::", ""); // Convert class names to readable names
                var altFireMode = (rawStats.Value<string>("altFireType") ?? "").Replace("EWeaponAltFireDisplayType::", ""); // Convert class names to readable names

                WeaponAdsStats adsStats = null;
                var rawAdsStats = rawStats.Value<JObject>("adsStats");
                if (rawAdsStats != null)
                {
                    var adsZoomMultiplier = rawAdsStats.Value<float>("zoomMultiplier");
                    var adsFireRate = rawAdsStats.Value<float>("fireRate");
                    var adsRunSpeedMultiplier = rawAdsStats.Value<float>("runSpeedMultiplier");
                    var adsBurstCount = rawAdsStats.Value<int>("burstCount");
                    var adsFirstBulletAccuracy = rawAdsStats.Value<float>("firstBulletAccuracy");
                    
                    adsStats = new WeaponAdsStats(adsZoomMultiplier, adsFireRate, adsRunSpeedMultiplier, adsBurstCount, adsFirstBulletAccuracy);
                }
                
                WeaponAltShotgunStats altShotgunStats = null;
                var rawAltShotgunStats = rawStats.Value<JObject>("altShotgunStats");
                if (rawAltShotgunStats != null)
                {
                    var altShotgunShotgunPelletCount = rawAltShotgunStats.Value<int>("shotgunPelletCount");
                    var altShotgunBurstRate = rawAltShotgunStats.Value<float>("burstRate");

                    altShotgunStats = new WeaponAltShotgunStats(altShotgunShotgunPelletCount, altShotgunBurstRate);
                }
                
                WeaponAirBurstStats airBurstStats = null;
                var rawAirBurstStats = rawStats.Value<JObject>("airBurstStats");
                if (rawAirBurstStats != null)
                {
                    var airBurstShotgunPelletCount = rawAirBurstStats.Value<int>("shotgunPelletCount");
                    var airBurstBurstDistance = rawAirBurstStats.Value<float>("burstDistance");

                    airBurstStats = new WeaponAirBurstStats(airBurstShotgunPelletCount, airBurstBurstDistance);
                }

                List<WeaponDamageRange> damageRanges = new();
                var rawDamageRanges = rawStats.Value<JArray>("damageRanges");

                foreach (var damageRange in rawDamageRanges)
                {
                    var rangeStartMeters = damageRange.Value<int>("rangeStartMeters");
                    var rangeEndMeters = damageRange.Value<int>("rangeEndMeters");
                    var headDamage = damageRange.Value<float>("headDamage");
                    var bodyDamage = damageRange.Value<float>("bodyDamage");
                    var legDamage = damageRange.Value<float>("legDamage");
                    
                    damageRanges.Add(new WeaponDamageRange(rangeStartMeters, rangeEndMeters, headDamage, bodyDamage, legDamage));
                }

                stats = new WeaponStats(fireRate, magazineSize, runSpeedMultiplier, equipTimeSeconds, reloadTimeSeconds,
                    firstBulletAccuracy, shotgunPelletCount, wallPenetration, feature, fireMode, altFireMode, adsStats,
                    altShotgunStats, airBurstStats, damageRanges);
            }

            // Weapon Skins
            List<string> skinUuids = new();
            var rawSkins = weapon.Value<JArray>("skins");
            foreach (var rawSkin in rawSkins)
            {
                var skinUuid = rawSkin.Value<string>("uuid");
                var skinName = rawSkin.Value<string>("displayName");
                var skinIconPath = ApiHelper.DownloadImage(rawSkin.Value<string>("displayIcon"), Constants.WeaponSkinIconPath, skinUuid);
                var skinWallpaperPath = ApiHelper.DownloadImage(rawSkin.Value<string>("wallpaper"), Constants.WeaponSkinWallpaperPath, skinUuid);
                
                // Skin Chromas
                List<string> chromaUuids = new();
                var rawChromas = rawSkin.Value<JArray>("chromas");
                foreach (var rawChroma in rawChromas)
                {
                    var chromaUuid = rawChroma.Value<string>("uuid");
                    var chromaName = rawChroma.Value<string>("displayName");
                    var chromaIconPath = ApiHelper.DownloadImage(rawChroma.Value<string>("displayIcon"), Constants.WeaponSkinChromaIconPath, chromaUuid);
                    var chromaFullRenderPath = ApiHelper.DownloadImage(rawChroma.Value<string>("fullRender"), Constants.WeaponSkinChromaFullRenderPath, chromaUuid);
                    var chromaSwatchPath = ApiHelper.DownloadImage(rawChroma.Value<string>("swatch"), Constants.WeaponSkinChromaSwatchPath, chromaUuid);
                    
                    var chroma = new WeaponSkinChroma(chromaUuid, chromaName, skinUuid, chromaIconPath, chromaFullRenderPath, chromaSwatchPath);
                    chromaUuids.Add(chroma.Uuid);
                    allChromas.Add(chroma);
                }
                
                // Skin Levels
                List<string> levelUuids = new();
                var rawLevels = rawSkin.Value<JArray>("levels");
                foreach (var rawLevel in rawLevels)
                {
                    var levelUuid = rawLevel.Value<string>("uuid");
                    var levelName = rawLevel.Value<string>("displayName");
                    var levelLevelItem = (rawLevel.Value<string>("levelItem") ?? "").Replace("EEquippableSkinLevelItem::", ""); // Convert class names to readable names
                    var levelIconPath = ApiHelper.DownloadImage(rawLevel.Value<string>("displayIcon"), Constants.WeaponSkinLevelIconPath, levelUuid);

                    var level = new WeaponSkinLevel(levelUuid, levelName, skinUuid, levelLevelItem, levelIconPath);
                    levelUuids.Add(level.Uuid);
                    allLevels.Add(level);
                }

                var skin = new WeaponSkin(skinUuid, skinName, uuid, skinIconPath, skinWallpaperPath, chromaUuids, levelUuids);
                skinUuids.Add(skin.Uuid);
                allSkins.Add(skin);
            }
            
            weapons.Add(new Weapon(uuid, name, category, defaultSkinUuid, iconPath, killStreamIconPath, shopCost, stats, skinUuids));
        }
        
        return (weapons, allSkins, allChromas, allLevels);
    }
}