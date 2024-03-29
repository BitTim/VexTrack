﻿using System.Collections.Generic;
using VexTrack.Core.Model.Game;
using VexTrack.Core.Model.Game.Agent;
using VexTrack.Core.Model.Game.Cosmetic;
using VexTrack.Core.Model.Game.Cosmetic.Weapon;
using VexTrack.Core.Model.Game.Templates;
using VexTrack.Core.Model.Game.Weapon;

namespace VexTrack.Core.Model;

public static class ApiData
{
    public static string Version;
    public static List<Map> Maps;
    public static List<GameMode> GameModes;
    public static List<Agent> Agents;
    public static List<AgentRole> AgentRoles;
    public static List<ContractTemplate> ContractTemplates;
    public static List<GearTemplate> GearTemplates;
    public static List<Weapon> Weapons;
    
    public static List<Buddy> Buddies;
    public static List<Currency> Currencies;
    public static List<PlayerCard> PlayerCards;
    public static List<PlayerTitle> PlayerTitles;
    public static List<Spray> Sprays;
    public static List<WeaponSkin> WeaponSkins;
    public static List<WeaponSkinChroma> WeaponSkinChromas;
    public static List<WeaponSkinLevel> WeaponSkinLevels;
    
    public static void SetData(string version, List<Map> maps, List<GameMode> gameModes, List<Agent> agents,
        List<AgentRole> agentRoles, List<ContractTemplate> contractTemplates, List<GearTemplate> gearTemplates,
        List<Weapon> weapons, List<Buddy> buddies, List<Currency> currencies, List<PlayerCard> playerCards,
        List<PlayerTitle> playerTitles, List<Spray> sprays, List<WeaponSkin> weaponSkins,
        List<WeaponSkinChroma> weaponSkinChromas, List<WeaponSkinLevel> weaponSkinLevels)
    {
        Version = version;
        Maps = maps;
        GameModes = gameModes;
        Agents = agents;
        AgentRoles = agentRoles;
        ContractTemplates = contractTemplates;
        GearTemplates = gearTemplates;
        Weapons = weapons;

        Buddies = buddies;
        Currencies = currencies;
        PlayerCards = playerCards;
        PlayerTitles = playerTitles;
        Sprays = sprays;
        WeaponSkins = weaponSkins;
        WeaponSkinChromas = weaponSkinChromas;
        WeaponSkinLevels = weaponSkinLevels;
    }

    public static Cosmetic GetCosmetic(string type, string uuid)
    {
        switch (type)
        {
            case "EquippableCharmLevel":
                return Buddies.Find(b => b.Uuid == uuid);
            
            case "Currency":
                return Currencies.Find(c => c.Uuid == uuid);
            
            case "PlayerCard":
                return PlayerCards.Find(pc => pc.Uuid == uuid);
            
            case "Title":
                return PlayerTitles.Find(pt => pt.Uuid == uuid);
            
            case "Spray":
                return Sprays.Find(s => s.Uuid == uuid);
            
            case "WeaponSkin":
                return WeaponSkins.Find(ws => ws.Uuid == uuid);
            
            case "EquippableSkinChroma":
                return WeaponSkinChromas.Find(wsc => wsc.Uuid == uuid);
            
            case "EquippableSkinLevel":
                return WeaponSkinLevels.Find(wsl => wsl.Uuid == uuid);
            
            default:
                return new Cosmetic("", "Invalid Type", "Error");
        }
    }
}