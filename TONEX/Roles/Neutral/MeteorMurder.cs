using AmongUs.GameOptions;
using static TONEX.Translator;
using TONEX.Roles.Core;
using UnityEngine;
using MS.Internal.Xml.XPath;
using static UnityEngine.GraphicsBuffer;
using TONEX.Roles.Neutral;
using System.Collections.Generic;
using Hazel;
using static Il2CppSystem.Net.Http.Headers.Parser;
using TONEX.Modules;
using TONEX.Roles.Core.Interfaces.GroupAndRole;
using TONEX.Roles.Core.Interfaces;
using System.Linq;

namespace TONEX.Roles.Neutral;

public sealed class MeteorMurder : RoleBase, INeutralKiller
{
    public static readonly SimpleRoleInfo RoleInfo =
        SimpleRoleInfo.Create(
            typeof(MeteorMurder),
            player => new MeteorMurder(player),
            CustomRoles.MeteorMurder,
            () => RoleTypes.Impostor,
            CustomRoleTypes.Neutral,
            75_1_2_0100,
            null,
            "Frisk|MeteorMurder|SFBF!Frisk",
             "#C0EAFF",
            true,
            true,
            countType: CountTypes.MeteorMurder
        );
    public MeteorMurder(PlayerControl player)
    : base(
        RoleInfo,
        player,
        () => HasTask.False
    )
    {
    }
    public bool IsNK { get; private set; } = true;
    public bool IsNE { get; private set; } = true;
    #region 全局变量
    public int LOVE;
    public int LVOverFlow;
    public int Shield;
    #endregion
    #region RPC相关
    private void SendRPC()
    {
        using var sender = CreateSender();
        sender.Writer.Write(LOVE);
        sender.Writer.Write(LVOverFlow);
        sender.Writer.Write(Shield);

    }
    public override void ReceiveRPC(MessageReader reader)
    {

            LOVE = reader.ReadInt32();
            LVOverFlow = reader.ReadInt32();
        Shield = reader.ReadInt32();

    }
    #endregion
    public bool CanUseKillButton() => true;
    public bool CanUseSabotageButton() => false;
    public bool CanUseImpostorVentButton() => false;
    public float CalculateKillCooldown() => 25f;
    public override bool OnCheckMurderAsTarget(MurderInfo info)
    {
        if (info.IsSuicide) return true;

        if (Shield >0)
        {
            Shield--;
            SendRPC();
            return false;
        }
        return true;
    }
    public bool OnCheckMurderAsKiller(MurderInfo info)
    {
        if (info.IsSuicide) return true;
        var (killer, target) = info.AttemptTuple;
        if (LOVE<=16)
        {
            LOVE = 20;
            LVOverFlow += LVOverFlow - 16;
        }
        else
        {
            LOVE += 4;
        }
        Shield = LOVE +LVOverFlow;
        SendRPC();
        return true;
    }
    
    public override string GetProgressText(bool comms = false)
    {
        Color color = Utils.GetRoleColor(CustomRoles.MeteorMurder);
        if (LOVE >= 10 && LOVE<20)
            color = Color.red;
        if (LOVE == 20)
            color = Palette.Purple;
        return Utils.ColorString(color, $"(LV{LOVE})"  +$"HP{Shield+1}");
    }
  
    public override bool OnCheckReportDeadBody(PlayerControl reporter, GameData.PlayerInfo target)
    {
        if (Is(reporter) && target == null)
        {
            if (LOVE <= 16)
            {
                LOVE = 20;
                LVOverFlow += LVOverFlow - 16;
            }
            else
            {
                LOVE += 4;
            }
            Shield = LOVE + LVOverFlow;
            SendRPC();
        }
            return true;
    }
}
