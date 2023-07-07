using Newtonsoft.Json.Linq;
using VexTrack.Core.Model.Game;

namespace VexTrack.Core.IO.ApiData;

public static class ApiDataSaver
{
    internal static void SaveApiData()
    {
        JObject cache = new() { { "version", Model.ApiData.Version } };
        
        
        
        //  --------------------------------
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
        
        
        
        //  --------------------------------
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
        
        
        
        //  --------------------------------
        //  Agents
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
                    
                    rewards.Add(reward);
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
                    
                    rewards.Add(reward);
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
        JArray sprays = new();
        foreach (var spray in Model.ApiData.Sprays)
        {
            JObject sprayObj = new()
            {
                { "uuid", spray.Uuid },
                { "name", spray.Name },
                { "iconPath", spray.IconPath },
                { "fullIconPath", spray.FullIconPath },
                { "animationPath", spray.AnimationPath },
            };
            
            sprays.Add(sprayObj);
        }
        cache.Add("sprays", sprays);
        
        
        
        //  --------------------------------
        //  Weapons and Skins
        JObject weaponData = new();
        
        
    }
}