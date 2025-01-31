﻿using System.Text;

namespace TONEX.Roles.Core.Descriptions;

public static class AddonDescription
{
    public static string FullFormatHelpByPlayer(PlayerControl player)
    {
        var builder = new StringBuilder(512);
        var subRoles = player?.GetCustomSubRoles();
        if (CustomRoles.Neptune.IsExist() && !subRoles.Contains(CustomRoles.Lovers) && !player.Is(CustomRoles.GM) && !player.Is(CustomRoles.Neptune))
        {
            subRoles.Add(CustomRoles.Lovers);
        }
        if (player.IsAlive() &&player.Is(CustomRoles.AkujoFakeLovers))
        {
            subRoles.Add(CustomRoles.AkujoLovers);
        }
        if (!(player.IsAlive() || CustomRoles.Akujo.IsExist())&& player.Is(CustomRoles.AkujoFakeLovers))
        {
            subRoles.Add(CustomRoles.AkujoLovers);
        }
        foreach (var subRole in subRoles)
        {
            if (subRoles.IndexOf(subRole) != 0) builder.AppendFormat("<size={0}>\n", BlankLineSize);
            builder.AppendFormat("<size={0}>{1}\n", FirstHeaderSize, Translator.GetRoleString(subRole.ToString()).Color(Utils.GetRoleColor(subRole)));
            builder.AppendFormat("<size={0}>{1}\n", BodySize, Translator.GetString($"{subRole}InfoLong"));
        }

        return builder.ToString();
    }
    public static string FullFormatHelpByRole(CustomRoles subRole)
    {
        var builder = new StringBuilder(512);
        builder.AppendFormat("<size={0}>\n", BlankLineSize);
        builder.AppendFormat("<size={0}>{1}\n", FirstHeaderSize, Translator.GetRoleString(subRole.ToString()).Color(Utils.GetRoleColor(subRole)));
        builder.AppendFormat("<size={0}>{1}\n", BodySize, Translator.GetString($"{subRole}InfoLong"));
        return builder.ToString();
    }
    public const string FirstHeaderSize = "130%";
    public const string SecondHeaderSize = "100%";
    public const string BodySize = "70%";
    public const string BlankLineSize = "30%";
}
