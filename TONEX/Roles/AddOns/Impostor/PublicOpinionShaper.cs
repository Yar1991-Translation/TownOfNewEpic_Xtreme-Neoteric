using System.Collections.Generic;
using TONEX.Attributes;
using TONEX.Roles.Core;
using UnityEngine;
using static TONEX.Options;

namespace TONEX.Roles.AddOns.Impostor;
public static class PublicOpinionShaper
{
    private static readonly int Id = 75_1_2_0700;
    private static Color RoleColor = Utils.GetRoleColor(CustomRoles.PublicOpinionShaper);
    private static List<byte> playerIdList = new();
    public static OptionItem OptionSpeed;

    public static OptionItem OptionTicketsPerKill;
    public static void SetupCustomOption()
    {
        SetupAddonOptions(Id, TabGroup.Addons, CustomRoles.PublicOpinionShaper);
        AddOnsAssignData.Create(Id + 10, CustomRoles.PublicOpinionShaper, false, true, false);
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
}

