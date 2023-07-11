using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace VexTrack.Core.IO.ApiData;

public static class ApiDataSaver
{
    internal static void SaveApiData()
    {
        JObject cache = new() { { "version", Model.ApiData.Version } };
        
        
        
        //  --------------------------------
        //  Maps
        //  --------------------------------
        
        JArray maps = new();
        foreach (var map in Model.ApiData.Maps)
        {
            JObject mapObj = new()
            {
                { "uuid", map.Uuid },
                { "name", map.Name },
                { "type", map.Type },
                { "listViewImagePath", map.ListViewImagePath },
                { "splashImagePath", map.SplashImagePath }
            };
            
            maps.Add(mapObj);
        }
        cache.Add("maps", maps);
        
        
        
        //  --------------------------------
        //  GameModes
        //  --------------------------------
        
        JArray gameModes = new();
        foreach (var gameMode in Model.ApiData.GameModes)
        {
            JObject gameModeObj = new()
            {
                { "uuid", gameMode.Uuid },
                { "name", gameMode.Name },
                { "mapType", gameMode.MapType },
                { "scoreType", gameMode.ScoreType },
                { "iconPath", gameMode.IconPath }
            };
            
            gameModes.Add(gameModeObj);
        }
        cache.Add("gameModes", gameModes);
        
        
        
        //  --------------------------------
        //  Agents
        //  --------------------------------
        
        JObject agentData = new();
        
        JArray agentRoles = new();
        foreach (var agentRole in Model.ApiData.AgentRoles)
        {
            JObject agentRoleObj = new()
            {
                { "uuid", agentRole.Uuid },
                { "name", agentRole.Name },
                { "description", agentRole.Description },
                { "iconPath", agentRole.IconPath }
            };
            
            agentRoles.Add(agentRoleObj);
        }
        agentData.Add("roles", agentRoles);
        
        JArray agents = new();
        foreach (var agent in Model.ApiData.Agents)
        {
            JObject agentObj = new()
            {
                { "uuid", agent.Uuid },
                { "name", agent.Name },
                { "description", agent.Description },
                { "iconPath", agent.IconPath },
                { "portraitPath", agent.PortraitPath },
                { "killFeedPortraitPath", agent.KillFeedPortraitPath },
                { "backgroundPath", agent.BackgroundPath },
                { "isBaseContent", agent.IsBaseContent },
                { "roleUuid", agent.RoleUuid }
            };

            // Gradient
            JArray gradientColors = new();
            foreach (var color in agent.GradientColors)
            {
                gradientColors.Add(color);
            }
            agentObj.Add("gradientColors", gradientColors);

            // Abilities
            JArray abilities = new();
            foreach (var ability in agent.Abilities)
            {
                JObject abilityObj = new()
                {
                    { "name", ability.Name },
                    { "description", ability.Description },
                    { "slot", ability.Slot },
                    { "iconPath", ability.IconPath }
                };
                
                abilities.Add(abilityObj);
            }
            agentObj.Add("abilities", abilities);
            agents.Add(agentObj);
        }
        agentData.Add("agents", agents);
        cache.Add("agentData", agentData);
        
        
        
        //  --------------------------------
        //  Contracts
        //  --------------------------------
        
        JArray contracts = new();
        foreach (var contract in Model.ApiData.ContractTemplates)
        {
            JObject contractObj = new()
            {
                { "uuid", contract.Uuid },
                { "name", contract.Name },
                { "type", contract.Type },
                { "startTimestamp", contract.StartTimestamp },
                { "endTimestamp", contract.EndTimestamp }
            };
            
            // Goals
            JArray goals = new();
            foreach (var goal in contract.Goals)
            {
                JObject goalObj = new()
                {
                    { "doughCost", goal.DoughCost },
                    { "xpTotal", goal.XpTotal },
                    { "vpCost", goal.VpCost },
                    { "canBuyDough", goal.CanBuyDough },
                    { "canBuyVp", goal.CanBuyVp },
                    { "isEpilogue", goal.IsEpilogue },
                };
                
                // Rewards
                JArray rewards = new();
                foreach (var reward in goal.Rewards)
                {
                    JObject rewardsObj = new()
                    {
                        { "cosmeticUuid", reward.CosmeticUuid },
                        { "cosmeticType", reward.CosmeticType },
                        { "amount", reward.Amount },
                        { "isPremium", reward.IsPremium }
                    };
                    
                    rewards.Add(rewardsObj);
                }
                goalObj.Add("rewards", rewards);
                goals.Add(goalObj);
            }
            contractObj.Add("goals", goals);
            contracts.Add(contractObj);
        }
        cache.Add("contracts", contracts);
        
        
        
        //  --------------------------------
        //  Gears
        //  --------------------------------
        
        JArray gears = new();
        foreach (var gear in Model.ApiData.GearTemplates)
        {
            JObject gearObj = new()
            {
                { "uuid", gear.Uuid },
                { "name", gear.Name },
                { "agentUuid", gear.AgentUuid }
            };
            
            // Goals
            JArray goals = new();
            foreach (var goal in gear.Goals)
            {
                JObject goalObj = new()
                {
                    { "doughCost", goal.DoughCost },
                    { "xpTotal", goal.XpTotal },
                    { "vpCost", goal.VpCost },
                    { "canBuyDough", goal.CanBuyDough },
                    { "canBuyVp", goal.CanBuyVp },
                    { "isEpilogue", goal.IsEpilogue },
                };
                
                // Rewards
                JArray rewards = new();
                foreach (var reward in goal.Rewards)
                {
                    JObject rewardsObj = new()
                    {
                        { "cosmeticUuid", reward.CosmeticUuid },
                        { "cosmeticType", reward.CosmeticType },
                        { "amount", reward.Amount },
                        { "isPremium", reward.IsPremium }
                    };
                    
                    rewards.Add(rewardsObj);
                }
                goalObj.Add("rewards", rewards);
                goals.Add(goalObj);
            }
            gearObj.Add("goals", goals);
            gears.Add(gearObj);
        }
        cache.Add("gears", gears);
        
        
        
        //  --------------------------------
        //  Buddies
        //  --------------------------------
        
        JArray buddies = new();
        foreach (var buddy in Model.ApiData.Buddies)
        {
            JObject buddyObj = new()
            {
                { "uuid", buddy.Uuid },
                { "name", buddy.Name },
                { "iconPath", buddy.IconPath }
            };
            
            buddies.Add(buddyObj);
        }
        cache.Add("buddies", buddies);
        
        
        
        //  --------------------------------
        //  Currencies
        //  --------------------------------
        
        JArray currencies = new();
        foreach (var currency in Model.ApiData.Currencies)
        {
            JObject currencyObj = new()
            {
                { "uuid", currency.Uuid },
                { "name", currency.Name },
                { "iconPath", currency.IconPath },
                { "largeIconPath", currency.LargeIconPath },
            };
            
            currencies.Add(currencyObj);
        }
        cache.Add("currencies", currencies);
        
        
        
        //  --------------------------------
        //  Player Cards
        //  --------------------------------
        
        JArray playerCards = new();
        foreach (var playerCard in Model.ApiData.PlayerCards)
        {
            JObject playerCardObj = new()
            {
                { "uuid", playerCard.Uuid },
                { "name", playerCard.Name },
                { "iconPath", playerCard.IconPath },
                { "smallArtPath", playerCard.SmallArtPath },
                { "wideArtPath", playerCard.WideArtPath },
                { "largeArtPath", playerCard.LargeArtPath }
            };
            
            playerCards.Add(playerCardObj);
        }
        cache.Add("playerCards", playerCards);
        
        
        
        //  --------------------------------
        //  Player Titles
        //  --------------------------------
        
        JArray playerTitles = new();
        foreach (var playerTitle in Model.ApiData.PlayerTitles)
        {
            JObject playerTitleObj = new()
            {
                { "uuid", playerTitle.Uuid },
                { "name", playerTitle.Name },
                { "titleText", playerTitle.TitleText }
            };
            
            playerTitles.Add(playerTitleObj);
        }
        cache.Add("playerTitles", playerTitles);
        
        
        
        //  --------------------------------
        //  Sprays
        //  --------------------------------
        
        JArray sprays = new();
        foreach (var spray in Model.ApiData.Sprays)
        {
            JObject sprayObj = new()
            {
                { "uuid", spray.Uuid },
                { "name", spray.Name },
                { "iconPath", spray.IconPath },
                { "fullIconPath", spray.FullIconPath },
                { "animationPath", spray.AnimationPath }
            };
            
            sprays.Add(sprayObj);
        }
        cache.Add("sprays", sprays);
        
        
        
        //  --------------------------------
        //  Weapons and Skins
        //  --------------------------------
        
        JObject weaponData = new();

        // Chromas
        JArray chromas = new();
        foreach (var chroma in Model.ApiData.WeaponSkinChromas)
        {
            JObject chromaObj = new()
            {
                { "uuid", chroma.Uuid },
                { "name", chroma.Name },
                { "parentUuid", chroma.ParentUuid },
                { "iconPath", chroma.IconPath },
                { "fullRenderPath", chroma.FullRenderPath },
                { "swatchPath", chroma.SwatchPath }
            };
            
            chromas.Add(chromaObj);
        }
        weaponData.Add("chromas", chromas);
        
        // Levels
        JArray levels = new();
        foreach (var level in Model.ApiData.WeaponSkinLevels)
        {
            JObject levelObj = new()
            {
                { "uuid", level.Uuid },
                { "name", level.Name },
                { "parentUuid", level.ParentUuid },
                { "levelItem", level.LevelItem },
                { "iconPath", level.IconPath }
            };
            
            levels.Add(levelObj);
        }
        weaponData.Add("levels", levels);
        
        // Skins
        JArray skins = new();
        foreach (var skin in Model.ApiData.WeaponSkins)
        {
            JObject skinObj = new()
            {
                { "uuid", skin.Uuid },
                { "name", skin.Name },
                { "parentUuid", skin.ParentUuid },
                { "iconPath", skin.IconPath },
                { "wallpaperPath", skin.WallpaperPath }
            };

            JArray chromaUuids = new();
            foreach (var chromaUuid in skin.ChromaUuids)
            {
                chromaUuids.Add(chromaUuid);
            }
            skinObj.Add("chromaUuids", chromaUuids);

            JArray levelUuids = new();
            foreach (var levelUuid in skin.LevelUuids)
            {
                levelUuids.Add(levelUuid);
            }
            skinObj.Add("levelUuids", levelUuids);
            
            skins.Add(skinObj);
        }
        weaponData.Add("skins", skins);
        
        // Weapons
        JArray weapons = new();
        foreach (var weapon in Model.ApiData.Weapons)
        {
            JObject weaponObj = new()
            {
                { "uuid", weapon.Uuid },
                { "name", weapon.Name },
                { "category", weapon.Category },
                { "defaultSkinUuid", weapon.DefaultSkinUuid },
                { "iconPath", weapon.IconPath },
                { "killStreamIconPath", weapon.KillStreamIconPath },
                { "shopCost", weapon.ShopCost }
            };

            JObject stats = null;
            if (weapon.Stats != null)
            {
                stats = new JObject
                {
                    { "fireRate", weapon.Stats.FireRate },
                    { "magazineSize", weapon.Stats.MagazineSize },
                    { "runSpeedMultiplier", weapon.Stats.RunSpeedMultiplier },
                    { "equipTimeSeconds", weapon.Stats.EquipTimeSeconds },
                    { "reloadTimeSeconds", weapon.Stats.ReloadTimeSeconds },
                    { "firstBulletAccuracy", weapon.Stats.FirstBulletAccuracy },
                    { "shotgunPelletCount", weapon.Stats.ShotgunPelletCount },
                    { "wallPenetration", weapon.Stats.WallPenetration },
                    { "feature", weapon.Stats.Feature },
                    { "fireMode", weapon.Stats.FireMode },
                    { "altFireMode", weapon.Stats.AltFireMode }
                };

                JObject adsStats = null;
                if (weapon.Stats.AdsStats != null)
                {
                    adsStats = new JObject
                    {
                        { "zoomMultiplier", weapon.Stats.AdsStats?.ZoomMultiplier },
                        { "fireRate", weapon.Stats.AdsStats?.FireRate },
                        { "runSpeedMultiplier", weapon.Stats.AdsStats?.RunSpeedMultiplier },
                        { "burstCount", weapon.Stats.AdsStats?.BurstCount },
                        { "firstBulletAccuracy", weapon.Stats.AdsStats?.FirstBulletAccuracy }
                    };
                }
                stats.Add("adsStats", adsStats);

                JObject altShotgunStats = null;
                if (weapon.Stats.AltShotgunStats != null)
                {
                    altShotgunStats = new JObject
                    {
                        { "shotgunPelletCount", weapon.Stats.AltShotgunStats?.ShotgunPelletCount },
                        { "burstRate", weapon.Stats.AltShotgunStats?.BurstRate }
                    };
                }
                stats.Add("altShotgunStats", altShotgunStats);

                JObject airBurstStats = null;
                if (weapon.Stats.AirBurstStats != null)
                {
                    airBurstStats = new JObject
                    {
                        { "shotgunPelletCount", weapon.Stats.AirBurstStats?.ShotgunPelletCount },
                        { "burstDistance", weapon.Stats.AirBurstStats?.BurstDistance }
                    };
                }
                stats.Add("airBurstStats", airBurstStats);
                
                JArray damageRanges = new();
                foreach (var damageRange in weapon.Stats.DamageRanges)
                {
                    JObject damageRangeObj = new()
                    {
                        { "rangeStartMeters", damageRange.RangeStartMeters },
                        { "rangeEndMeters", damageRange.RangeEndMeters },
                        { "headDamage", damageRange.HeadDamage },
                        { "bodyDamage", damageRange.BodyDamage },
                        { "legDamage", damageRange.LegDamage }
                    };
                
                    damageRanges.Add(damageRangeObj);
                }
                stats.Add("damageRanges", damageRanges);
            }
            weaponObj.Add("stats", stats);

            JArray skinUuids = new();
            foreach (var skinUuid in weapon.SkinUuids)
            {
                skinUuids.Add(skinUuid);
            }
            weaponObj.Add("skinUuids", skinUuids);
            
            weapons.Add(weaponObj);
        }
        weaponData.Add("weapons", weapons);
        cache.Add("weaponData", weaponData);
        
        
        
        //  --------------------------------
        //  Saving to File
        //  --------------------------------
        
        if (!File.Exists(Constants.CachePath))
        {
            var sep = Constants.CachePath.LastIndexOf("/", StringComparison.Ordinal);

            Directory.CreateDirectory(Constants.CachePath[..sep]);
            File.CreateText(Constants.CachePath).Close();
        }

        File.WriteAllText(Constants.CachePath, cache.ToString());
    }
}