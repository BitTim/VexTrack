using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using VexTrack.Core.Model.Game;
using VexTrack.Core.Model.Game.Agent;
using VexTrack.Core.Model.Game.Cosmetic;
using VexTrack.Core.Model.Game.Cosmetic.Weapon;
using VexTrack.Core.Model.Game.Templates;
using VexTrack.Core.Model.Game.Weapon;
using VexTrack.Core.Model.Game.Weapon.Stats;

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

        // Game Modes
        var gameModes = LoadGameModes(cache);
        if (gameModes.Count == 0) return -2;

        // Agent Data
        var agentRoles = LoadAgentRoles(cache);
        var agents = LoadAgents(cache);
        if (agentRoles.Count == 0 || agents.Count == 0) return -3;
        
        // Contracts and Gears
        var contracts = LoadContracts(cache);
        var gears = LoadGears(cache);
        if (contracts.Count == 0 || gears.Count == 0) return -4;
        
        // Buddies
        var buddies = LoadBuddies(cache);
        if (buddies.Count == 0) return -5;
        
        // Currencies
        var currencies = LoadCurrencies(cache);
        if (buddies.Count == 0) return -6;
        
        // Player Cards
        var playerCards = LoadPlayerCards(cache);
        if (playerCards.Count == 0) return -7;
        
        // Player Titles
        var playerTitles = LoadPlayerTitles(cache);
        if (playerTitles.Count == 0) return -8;
        
        // Sprays
        var sprays = LoadSprays(cache);
        if (sprays.Count == 0) return -9;
        
        // Weapon Data
        var weaponChromas = LoadWeaponChromas(cache);
        var weaponLevels = LoadWeaponLevels(cache);
        var weaponSkins = LoadWeaponSkins(cache);
        var weapons = LoadWeapons(cache);
        if (weaponChromas.Count == 0 || weaponLevels.Count == 0 || weaponSkins.Count == 0 || weapons.Count == 0) return -10;

        // Apply Data
        Model.ApiData.SetData(version, maps, gameModes, agents, agentRoles, contracts, gears, weapons, buddies, currencies, playerCards, playerTitles, sprays, weaponSkins, weaponChromas, weaponLevels);
        return 0;
    }

    
    
    // ================================
    //  Maps
    // ================================

    private static List<Map> LoadMaps(JObject cache)
    {
        List<Map> maps = new();
        foreach (var map in cache.Value<JArray>("maps") ?? new JArray())
        {
            var uuid = map.Value<string>("uuid");
            var name = map.Value<string>("name");
            var type = map.Value<string>("type");
            var listViewImagePath = map.Value<string>("listViewImagePath");
            var splashImagePath = map.Value<string>("splashImagePath");
            
            maps.Add(new Map(uuid, name, type, listViewImagePath, splashImagePath));
        }

        return maps;
    }

    
    
    // ================================
    //  GameModes
    // ================================

    private static List<GameMode> LoadGameModes(JObject cache)
    {
        List<GameMode> gameModes = new();
        foreach (var gameMode in cache.Value<JArray>("gameModes") ?? new JArray())
        {
            var uuid = gameMode.Value<string>("uuid");
            var name = gameMode.Value<string>("name");
            var mapType = gameMode.Value<string>("mapType");
            var scoreType = gameMode.Value<string>("scoreType");
            var iconPath = gameMode.Value<string>("iconPath");
            
            
            gameModes.Add(new GameMode(uuid, name, mapType, scoreType, iconPath));
        }

        return gameModes;
    }

    
    
    // ================================
    //  Agent Data
    // ================================
    
    private static List<AgentRole> LoadAgentRoles(JObject cache)
    {
        List<AgentRole> agentRoles = new();
        foreach (var role in cache.Value<JObject>("agentData")?.Value<JArray>("roles") ?? new JArray())
        {
            var uuid = role.Value<string>("uuid");
            var name = role.Value<string>("name");
            var description = role.Value<string>("description");
            var iconPath = role.Value<string>("iconPath");
            
            agentRoles.Add(new AgentRole(uuid, name, description, iconPath));
        }

        return agentRoles;
    }

    private static List<Agent> LoadAgents(JObject cache)
    {
        List<Agent> agents = new();
        foreach (var agent in cache.Value<JObject>("agentData")?.Value<JArray>("agents") ?? new JArray())
        {
            var uuid = agent.Value<string>("uuid");
            var name = agent.Value<string>("name");
            var description = agent.Value<string>("description");
            var iconPath = agent.Value<string>("iconPath");
            var portraitPath = agent.Value<string>("portraitPath");
            var killFeedPortraitPath = agent.Value<string>("killFeedPortraitPath");
            var backgroundPath = agent.Value<string>("backgroundPath");

            List<string> gradientColors = new();
            foreach (var gradientColor in agent.Value<JArray>("gradientColors"))
            {
                gradientColors.Add((string)gradientColor);
            }

            var isBaseContent = agent.Value<bool>("isBaseContent");
            var roleUuid = agent.Value<string>("roleUuid");

            List<AgentAbility> abilities = new();
            foreach (var ability in agent.Value<JArray>("abilities") ?? new JArray())
            {
                var abilityName = ability.Value<string>("name");
                var abilityDescription = ability.Value<string>("description");
                var abilitySlot = ability.Value<string>("slot");
                var abilityIconPath = ability.Value<string>("iconPath");
                
                abilities.Add(new AgentAbility(abilityName, abilityDescription, abilitySlot, abilityIconPath));
            }

            agents.Add(new Agent(uuid, name, description, iconPath, portraitPath, killFeedPortraitPath, backgroundPath, gradientColors, isBaseContent, roleUuid, abilities));
        }

        return agents;
    }

    
    
    // ================================
    //  Contracts and Gears
    // ================================

    private static List<ContractTemplate> LoadContracts(JObject cache)
    {
        List<ContractTemplate> contracts = new();
        foreach (var contract in cache.Value<JArray>("contracts") ?? new JArray())
        {
            var uuid = contract.Value<string>("uuid");
            var name = contract.Value<string>("name");
            var type = contract.Value<string>("type");
            var startTimestamp = contract.Value<long>("startTimestamp");
            var endTimestamp = contract.Value<long>("endTimestamp");

            List<GoalTemplate> goals = new();
            foreach (var goal in contract.Value<JArray>("goals") ?? new JArray())
            {
                List<Reward> rewards = new();
                foreach (var reward in goal.Value<JArray>("rewards") ?? new JArray())
                {
                    var cosmeticUuid = reward.Value<string>("cosmeticUuid");
                    var cosmeticType = reward.Value<string>("cosmeticType");
                    var amount = reward.Value<int>("amount");
                    var isPremium = reward.Value<bool>("isPremium");
                    
                    rewards.Add(new Reward(cosmeticUuid, cosmeticType, amount, isPremium));
                }

                var doughCost = goal.Value<int>("doughCost");
                var xpTotal = goal.Value<int>("xpTotal");
                var vpCost = goal.Value<int>("vpCost");
                var canBuyDough = goal.Value<bool>("canBuyDough");
                var canBuyVp = goal.Value<bool>("canBuyVp");
                var isEpilogue = goal.Value<bool>("isEpilogue");
                
                goals.Add(new GoalTemplate(rewards, canBuyDough, doughCost, xpTotal, canBuyVp, vpCost, isEpilogue));
            }
            
            contracts.Add(new ContractTemplate(uuid, name, type, startTimestamp, endTimestamp, goals));
        }

        return contracts;
    }
    
    private static List<GearTemplate> LoadGears(JObject cache)
    {
        List<GearTemplate> gears = new();
        foreach (var gear in cache.Value<JArray>("gears") ?? new JArray())
        {
            var uuid = gear.Value<string>("uuid");
            var name = gear.Value<string>("name");
            var agentUuid = gear.Value<string>("agentUuid");

            List<GoalTemplate> goals = new();
            foreach (var goal in gear.Value<JArray>("goals") ?? new JArray())
            {
                List<Reward> rewards = new();
                foreach (var reward in goal.Value<JArray>("rewards") ?? new JArray())
                {
                    var cosmeticUuid = reward.Value<string>("cosmeticUuid");
                    var cosmeticType = reward.Value<string>("cosmeticType");
                    var amount = reward.Value<int>("amount");
                    var isPremium = reward.Value<bool>("isPremium");
                    
                    rewards.Add(new Reward(cosmeticUuid, cosmeticType, amount, isPremium));
                }

                var doughCost = goal.Value<int>("doughCost");
                var xpTotal = goal.Value<int>("xpTotal");
                var vpCost = goal.Value<int>("vpCost");
                var canBuyDough = goal.Value<bool>("canBuyDough");
                var canBuyVp = goal.Value<bool>("canBuyVp");
                var isEpilogue = goal.Value<bool>("isEpilogue");
                
                goals.Add(new GoalTemplate(rewards, canBuyDough, doughCost, xpTotal, canBuyVp, vpCost, isEpilogue));
            }
            
            gears.Add(new GearTemplate(uuid, name, agentUuid, goals));
        }

        return gears;
    }

    
    
    // ================================
    //  Buddies
    // ================================

    private static List<Buddy> LoadBuddies(JObject cache)
    {
        List<Buddy> buddies = new();
        foreach (var buddy in cache.Value<JArray>("buddies") ?? new JArray())
        {
            var uuid = buddy.Value<string>("uuid");
            var name = buddy.Value<string>("name");
            var iconPath = buddy.Value<string>("iconPath");
            
            buddies.Add(new Buddy(uuid, name, iconPath));
        }

        return buddies;
    }

    
    
    // ================================
    //  Currencies
    // ================================

    private static List<Currency> LoadCurrencies(JObject cache)
    {
        List<Currency> currencies = new();
        foreach (var currency in cache.Value<JArray>("currencies") ?? new JArray())
        {
            var uuid = currency.Value<string>("uuid");
            var name = currency.Value<string>("name");
            var iconPath = currency.Value<string>("iconPath");
            var largeIconPath = currency.Value<string>("largeIconPath");
            
            currencies.Add(new Currency(uuid, name, iconPath, largeIconPath));
        }

        return currencies;
    }

    
    
    // ================================
    //  Player Cards
    // ================================

    private static List<PlayerCard> LoadPlayerCards(JObject cache)
    {
        List<PlayerCard> playerCards = new();
        foreach (var playerCard in cache.Value<JArray>("playerCards") ?? new JArray())
        {
            var uuid = playerCard.Value<string>("uuid");
            var name = playerCard.Value<string>("name");
            var iconPath = playerCard.Value<string>("iconPath");
            var smallArtPath = playerCard.Value<string>("smallArtPath");
            var wideArtPath = playerCard.Value<string>("wideArtPath");
            var largeArtPath = playerCard.Value<string>("largeArtPath");
            
            playerCards.Add(new PlayerCard(uuid, name, iconPath, smallArtPath, wideArtPath, largeArtPath));
        }

        return playerCards;
    }

    
    
    // ================================
    //  Player Titles
    // ================================

    private static List<PlayerTitle> LoadPlayerTitles(JObject cache)
    {
        List<PlayerTitle> playerTitles = new();
        foreach (var playerTitle in cache.Value<JArray>("playerTitles") ?? new JArray())
        {
            var uuid = playerTitle.Value<string>("uuid");
            var name = playerTitle.Value<string>("name");
            var titleText = playerTitle.Value<string>("titleText");
            
            playerTitles.Add(new PlayerTitle(uuid, name, titleText));
        }

        return playerTitles;
    }

    
    
    // ================================
    //  Sprays
    // ================================

    private static List<Spray> LoadSprays(JObject cache)
    {
        List<Spray> sprays = new();
        foreach (var spray in cache.Value<JArray>("sprays") ?? new JArray())
        {
            var uuid = spray.Value<string>("uuid");
            var name = spray.Value<string>("name");
            var iconPath = spray.Value<string>("iconPath");
            var fullIconPath = spray.Value<string>("fullIconPath");
            var animationPath = spray.Value<string>("animationPath");
            
            sprays.Add(new Spray(uuid, name, iconPath, fullIconPath, animationPath));
        }

        return sprays;
    }

    
    
    // ================================
    //  Weapon Data
    // ================================

    private static List<WeaponSkinChroma> LoadWeaponChromas(JObject cache)
    {
        List<WeaponSkinChroma> chromas = new();
        foreach (var chroma in cache.Value<JObject>("weaponData")?.Value<JArray>("chromas") ?? new JArray())
        {
            var uuid = chroma.Value<string>("uuid");
            var name = chroma.Value<string>("name");
            var parentUuid = chroma.Value<string>("parentUuid");
            var iconPath = chroma.Value<string>("iconPath");
            var fullRenderPath = chroma.Value<string>("fullRenderPath");
            var swatchPath = chroma.Value<string>("swatchPath");

            chromas.Add(new WeaponSkinChroma(uuid, name, parentUuid, iconPath, fullRenderPath, swatchPath));
        }

        return chromas;
    }

    private static List<WeaponSkinLevel> LoadWeaponLevels(JObject cache)
    {
        List<WeaponSkinLevel> levels = new();
        foreach (var level in cache.Value<JObject>("weaponData")?.Value<JArray>("levels") ?? new JArray())
        {
            var uuid = level.Value<string>("uuid");
            var name = level.Value<string>("name");
            var parentUuid = level.Value<string>("parentUuid");
            var levelItem = level.Value<string>("levelItem");
            var iconPath = level.Value<string>("iconPath");
            
            levels.Add(new WeaponSkinLevel(uuid, name, parentUuid, levelItem, iconPath));
        }

        return levels;
    }

    private static List<WeaponSkin> LoadWeaponSkins(JObject cache)
    {
        List<WeaponSkin> skins = new();
        foreach (var skin in cache.Value<JObject>("weaponData")?.Value<JArray>("skins") ?? new JArray())
        {
            var uuid = skin.Value<string>("uuid");
            var name = skin.Value<string>("name");
            var parentUuid = skin.Value<string>("parentUuid");
            var iconPath = skin.Value<string>("iconPath");
            var wallpaperPath = skin.Value<string>("wallpaperPath");

            List<string> chromaUuids = new();
            foreach (var chromaUuid in skin.Value<JArray>("chromaUuids") ?? new JArray())
            {
                chromaUuids.Add((string)chromaUuid);
            }

            List<string> levelUuids = new();
            foreach (var levelUuid in skin.Value<JArray>("levelUuids") ?? new JArray())
            {
                levelUuids.Add((string)levelUuid);
            }
            
            skins.Add(new WeaponSkin(uuid, name, parentUuid, iconPath, wallpaperPath, chromaUuids, levelUuids));
        }

        return skins;
    }

    private static List<Weapon> LoadWeapons(JObject cache)
    {
        List<Weapon> weapons = new();
        foreach (var weapon in cache.Value<JObject>("weaponData")?.Value<JArray>("weapons") ?? new JArray())
        {
            var uuid = weapon.Value<string>("uuid");
            var name = weapon.Value<string>("name");
            var category = weapon.Value<string>("category");
            var defaultSkinUuid = weapon.Value<string>("defaultSkinUuid");
            var iconPath = weapon.Value<string>("iconPath");
            var killStreamIconPath = weapon.Value<string>("killStreamIconPath");
            var shopCost = weapon.Value<int>("shopCost");

            WeaponStats stats = null;
            var rawStats = weapon.Value<JObject>("stats");
            if (rawStats != null)
            {
                var fireRate = rawStats.Value<float>("fireRate");
                var magazineSize = rawStats.Value<int>("magazineSize");
                var runSpeedMultiplier = rawStats.Value<float>("runSpeedMultiplier");
                var equipTimeSeconds = rawStats.Value<float>("equipTimeSeconds");
                var reloadTimeSeconds = rawStats.Value<float>("reloadTimeSeconds");
                var firstBulletAccuracy = rawStats.Value<float>("firstBulletAccuracy");
                var shotgunPelletCount = rawStats.Value<int>("shotgunPelletCount");

                var wallPenetration = rawStats.Value<string>("wallPenetration");
                var feature = rawStats.Value<string>("feature") ?? "";
                var fireMode = rawStats.Value<string>("fireMode") ?? "";
                var altFireMode = rawStats.Value<string>("altFireType") ?? "";

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
                var rawDamageRanges = rawStats.Value<JArray>("damageRanges") ?? new JArray();

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

            List<string> skinUuids = new();
            foreach (var skinUuid in weapon.Value<JArray>("skinUuids") ?? new JArray())
            {
                skinUuids.Add((string)skinUuid);
            }
            
            weapons.Add(new Weapon(uuid, name, category, defaultSkinUuid, iconPath, killStreamIconPath, shopCost, stats, skinUuids));
        }

        return weapons;
    }
}