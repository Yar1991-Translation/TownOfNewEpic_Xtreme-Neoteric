using System.Collections.Generic;
using System.Text;
using TONEX.Attributes;
using TONEX.Roles.Core;
using UnityEngine;
using static TONEX.Options;
using static TONEX.Translator;
using static TONEX.Utils;

namespace TONEX.Roles.AddOns.Common;
public static class Neptune
{
    private static readonly int Id = 80600;
    private static Color RoleColor = GetRoleColor(CustomRoles.Neptune);
    private static List<byte> playerIdList = new();

    public static void SetupCustomOption()
    {
        SetupAddonOptions(Id, TabGroup.OtherRoles, CustomRoles.Neptune);
        AddOnsAssignData.Create(Id + 10, TabGroup.OtherRoles, CustomRoles.Neptune, true, true, true);
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

    public static void MeetingngStartNotifyOthers(ref StringBuilder sb, CustomRoles role )
    {
        if (CustomRoles.Neptune.IsExist() && (role is not CustomRoles.GM and not CustomRoles.Neptune))
            sb.Append($"\n\n" + GetString($"Lovers") + Utils.GetRoleDisplaySpawnMode(CustomRoles.Lovers) + GetString($"LoversInfoLong"));

    }
    public static void GetSubRolesText(bool intro, bool disableColor ,List<CustomRoles>SubRoles, ref StringBuilder sb)
    {
        if (intro && !SubRoles.Contains(CustomRoles.Lovers) && !SubRoles.Contains(CustomRoles.Neptune) && CustomRoles.Neptune.IsExist())
        {
            var RoleText = disableColor ? GetRoleName(CustomRoles.Lovers) : ColorString(GetRoleColor(CustomRoles.Lovers), GetRoleName(CustomRoles.Lovers));
            sb.Append($"{ColorString(Color.white, " + ")}{RoleText}");
        }
    }
    public static void TargetMarks(PlayerControl seer, PlayerControl target, ref StringBuilder targetMark)
    {
        if (target.Is(CustomRoles.Neptune) || seer.Is(CustomRoles.Neptune))
        {
            targetMark.Append($"<color={GetRoleColorCode(CustomRoles.Lovers)}>♡</color>");
        }
    }
    public static void Intro(ref IntroCutscene __instance)
    {
        if (!PlayerControl.LocalPlayer.Is(CustomRoles.Lovers) && !PlayerControl.LocalPlayer.Is(CustomRoles.Neptune) && CustomRoles.Neptune.IsExist() && !PlayerControl.LocalPlayer.Is(CustomRoles.Mini))
            __instance.RoleBlurbText.text += "\n" + Utils.ColorString(Utils.GetRoleColor(CustomRoles.Lovers), GetString($"{CustomRoles.Lovers}Info"));

        
    }
    public static void MeetingHud(bool isLover, PlayerControl seer, PlayerControl target, ref StringBuilder sb)
    {
        if ((seer.Is(CustomRoles.Neptune) || target.Is(CustomRoles.Neptune)) && !seer.Data.IsDead && !isLover)
            sb.Append(Utils.ColorString(Utils.GetRoleColor(CustomRoles.Lovers), "♡"));
        else if (seer == target && CustomRoles.Neptune.IsExist() && !isLover)
            sb.Append(Utils.ColorString(Utils.GetRoleColor(CustomRoles.Lovers), "♡"));
    }
    public static void Marks(PlayerControl __instance, ref StringBuilder Mark)
    {
        if ((__instance.Is(CustomRoles.Neptune) || PlayerControl.LocalPlayer.Is(CustomRoles.Neptune)) && CustomRoles.Neptune.IsExist())
        {
            Mark.Append($"<color={Utils.GetRoleColorCode(CustomRoles.Lovers)}>♡</color>");
        }
        else if (__instance == PlayerControl.LocalPlayer && CustomRoles.Neptune.IsExist())
        {
            Mark.Append($"<color={Utils.GetRoleColorCode(CustomRoles.Lovers)}>♡</color>");
        }
    }
}