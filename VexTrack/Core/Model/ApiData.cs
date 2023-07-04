using System.Collections.Generic;
using VexTrack.Core.Model.Templates;

namespace VexTrack.Core.Model;

public abstract class ApiData
{
    public static string Version;
    public static List<Map> Maps;
    public static List<GameMode> GameModes;
    public static List<ContractTemplate> ContractTemplates;
    public static List<ContractTemplate> GearTemplates;
    public static List<Cosmetic> Cosmetics;

    public static void SetData(string version, List<Map> maps, List<GameMode> gameModes, List<ContractTemplate> contractTemplates, List<ContractTemplate> gearTemplates, List<Cosmetic> cosmetics)
    {
        Version = version;
        Maps = maps;
        GameModes = gameModes;
        ContractTemplates = contractTemplates;
        GearTemplates = gearTemplates;
        Cosmetics = cosmetics;
    }
}