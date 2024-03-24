using Hazel;
using System.Collections.Generic;
using System.Linq;
using TONEX.Attributes;
using TONEX.Modules.SoundInterface;
using TONEX.Roles.Core;
using UnityEngine;
using static TONEX.Options;
using static UnityEngine.GraphicsBuffer;

namespace TONEX.Roles.AddOns.Common;
public static class Nihility
{
    private static readonly int Id = 75_1_2_0400;
    private static List<byte> playerIdList = new();
    public static void SetupCustomOption()
    {
        SetupAddonOptions(Id, TabGroup.Addons, CustomRoles.Nihility);
        AddOnsAssignData.Create(Id + 10, CustomRoles.Nihility, true, true, true);
    }
    [GameModuleInitializer]
    public static void Init()
    {
        playerIdList = new();
    }
    public static void Add(byte playerId)
    {
        playerIdList.Add(playerId);
    }
    public static bool IsEnable => playerIdList.Count > 0;
    public static bool IsThisRole(byte playerId) => playerIdList.Contains(playerId);
    public static bool OnCheckMurderPlayerOthers_Nihility(MurderInfo info)
    {

        var (killer, target) = info.AttemptTuple;
        if (!killer.IsNeutralEvil() && target.Is(CustomRoles.Nihility))
        {

                killer.Notify(Translator.GetString("Nihility"));
                return false;
            
        }
        return true;
    }
}
