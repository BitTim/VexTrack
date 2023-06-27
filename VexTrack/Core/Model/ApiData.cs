using System.Collections.Generic;
using VexTrack.Core.Model.Presets;

namespace VexTrack.Core.Model;

public abstract class ApiData
{
    public static string Version;
    public static List<Map> Maps;
    public static List<GameMode> GameModes;
    public static List<ContractPreset> Contracts;

    public static void SetData(string version, List<Map> maps, List<GameMode> gameModes)
    {
        Version = version;
        Maps = maps;
        GameModes = gameModes;
    }
}