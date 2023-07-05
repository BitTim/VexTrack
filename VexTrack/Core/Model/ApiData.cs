using System.Collections.Generic;
using VexTrack.Core.Model.Templates;
using VexTrack.Core.Model.WPF;

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
    public static List<Cosmetic> Cosmetics;

    public static void SetData(string version, List<Map> maps, List<GameMode> gameModes, List<Agent> agents, List<AgentRole> agentRoles, List<ContractTemplate> contractTemplates, List<GearTemplate> gearTemplates, List<Cosmetic> cosmetics)
    {
        Version = version;
        Maps = maps;
        GameModes = gameModes;
        Agents = agents;
        AgentRoles = agentRoles;
        ContractTemplates = contractTemplates;
        GearTemplates = gearTemplates;
        Cosmetics = cosmetics;
    }
}