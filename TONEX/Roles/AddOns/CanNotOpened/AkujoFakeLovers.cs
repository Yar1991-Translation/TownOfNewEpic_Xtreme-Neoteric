using HarmonyLib;
using Hazel;
using System.Collections.Generic;
using System.Linq;
using TONEX.Attributes;
using UnityEngine;
using TONEX.Roles.Core;
using static TONEX.Translator;
using static TONEX.Utils;
using System.Text;
using InnerNet;
using static UnityEngine.GraphicsBuffer;
using TONEX.Roles.AddOns.Common;

namespace TONEX.Roles.AddOns.CanNotOpened;
public static class AkujoFakeLovers
{
    private static readonly int Id = 75_1_2_2000;
    private static List<byte> playerIdList = new();

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
    public static void GetSubRolesText(bool intro, bool disableColor, List<CustomRoles> SubRoles, ref StringBuilder sb)
    {
        if (intro && SubRoles.Contains(CustomRoles.AkujoFakeLovers))
        {
            var RoleText = disableColor ? GetRoleName(CustomRoles.AkujoLovers) : ColorString(GetRoleColor(CustomRoles.AkujoLovers), GetRoleName(CustomRoles.AkujoLovers));
            sb.Append($"{ColorString(Color.white, " + ")}{RoleText}");
        }
    }
    public static void TargetMarks(PlayerControl seer, PlayerControl target, ref StringBuilder targetMark)
    {
        if (seer.IsAlive())
        {
            if (seer.Is(CustomRoles.AkujoFakeLovers) && (target.Is(CustomRoles.Akujo) &&  seer == target))
            {
                targetMark.Append($"<color={GetRoleColorCode(CustomRoles.AkujoLovers)}>❤</color>");
            }
        }
        if (!seer.IsAlive() || !CustomRoles.Akujo.IsExist())
        {
            if (seer.Is(CustomRoles.AkujoFakeLovers) && (target.Is(CustomRoles.Akujo) && seer == target))
            {
                targetMark.Append($"<color={GetRoleColorCode(CustomRoles.AkujoFakeLovers)}>_♡_</color>");
            }
        }
    }
    public static bool CanKnowOthers(PlayerControl seer, PlayerControl seen)
    {
        if (seer.Is(CustomRoles.AkujoFakeLovers) && seen.Is(CustomRoles.Akujo) || seer.Is(CustomRoles.Akujo) && seen.Is(CustomRoles.AkujoFakeLovers) && AkujoLovers.AkujoLoverKnowRoles.GetBool())
            return true;
        return false;
    }
    public static void MeetingngStartNotifyOthers(ref StringBuilder sb, CustomRoles role)
    {
        if (role==(CustomRoles.AkujoFakeLovers))
            sb.Append($"\n\n" + GetString($"AkujoLovers") + Utils.GetRoleDisplaySpawnMode(CustomRoles.AkujoLovers) + GetString($"AkujoLoversInfoLong"));

    }
    public static void MeetingHud(bool isLover, PlayerControl seer, PlayerControl target, ref StringBuilder sb)
    {
        if (seer.IsAlive())
        {
            if (seer.Is(CustomRoles.AkujoFakeLovers) && (target.Is(CustomRoles.Akujo) && seer == target) && !seer.Data.IsDead && !isLover)
                sb.Append(Utils.ColorString(Utils.GetRoleColor(CustomRoles.AkujoLovers), "❤"));
        }
        if (!seer.IsAlive() || !CustomRoles.Akujo.IsExist())
        {
            if (seer.Is(CustomRoles.AkujoFakeLovers) && (target.Is(CustomRoles.Akujo) && seer == target))
                sb.Append(Utils.ColorString(Utils.GetRoleColor(CustomRoles.AkujoFakeLovers), "_♡_"));
        }
        
    }
    public static void Marks(PlayerControl __instance, ref StringBuilder Mark)
    {
        if (PlayerControl.LocalPlayer.IsAlive())
        {
            if (PlayerControl.LocalPlayer.Is(CustomRoles.AkujoFakeLovers) && (__instance.Is(CustomRoles.Akujo) && PlayerControl.LocalPlayer == __instance))
            {
                Mark.Append($"<color={GetRoleColorCode(CustomRoles.AkujoLovers)}>❤</color>");
            }
        }
        if (!PlayerControl.LocalPlayer.IsAlive() || !CustomRoles.Akujo.IsExist())
        {
            if (PlayerControl.LocalPlayer.Is(CustomRoles.AkujoFakeLovers) && (__instance.Is(CustomRoles.Akujo) && PlayerControl.LocalPlayer == __instance))
            {
                Mark.Append($"<color={GetRoleColorCode(CustomRoles.AkujoFakeLovers)}>_♡_</color>");
            }
        }
    }
}
