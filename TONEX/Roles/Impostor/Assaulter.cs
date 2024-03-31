using AmongUs.GameOptions;
using Hazel;
using TONEX.Roles.Core;
using TONEX.Roles.Core.Interfaces.GroupAndRole;
using TONEX.Roles.Neutral;
using UnityEngine;
using YamlDotNet.Core;
using static TONEX.Translator;

namespace TONEX.Roles.Impostor;
public sealed class Assaulter : RoleBase, IImpostor
{
    public static readonly SimpleRoleInfo RoleInfo =
        SimpleRoleInfo.Create(
            typeof(Assaulter),
            player => new Assaulter(player),
            CustomRoles.Assaulter,
            () => RoleTypes.Impostor,
            CustomRoleTypes.Impostor,
            75_1_2_2300,
            SetUpOptionItem,
            "sk|嗜血殺手|嗜血"
        );
    public Assaulter(PlayerControl player)
    : base(
        RoleInfo,
        player
    )
    {
        KillCooldown = OptionKillCooldown.GetFloat();
        AssaulterL = AssaulterLimit.GetInt();

    }
    private static OptionItem OptionKillCooldown;
    private static OptionItem AssaulterLimit;
    enum OptionName
    {
        AssaulterLimit
    }
    private static float KillCooldown;
    private int AssaulterL;
    private void SendRPC()
    {
        using var sender = CreateSender();
        sender.Writer.Write(AssaulterL);
    }
    public override void ReceiveRPC(MessageReader reader)
    {

        AssaulterL = reader.ReadInt32();
    }
    private static void SetUpOptionItem()
    {
        OptionKillCooldown = FloatOptionItem.Create(RoleInfo, 10, GeneralOption.KillCooldown, new(2.5f, 180f, 2.5f), 20f, false)
            .SetValueFormat(OptionFormat.Seconds);
        AssaulterLimit = IntegerOptionItem.Create(RoleInfo, 11, OptionName.AssaulterLimit, new(2, 15, 1), 3, false)
            .SetValueFormat(OptionFormat.Layer);
    }
    public float CalculateKillCooldown() => KillCooldown;
    public bool OnCheckMurderAsKiller(MurderInfo info)
    {
        AssaulterL--;
        SendRPC();
        return true;
    }
    public override bool OnCheckMurderAsTarget(MurderInfo info)
    {
        if (AssaulterL >0)
        {
            AssaulterL--;
            SendRPC();
            return false;
        }
        return true;
    }
    public override string GetProgressText(bool comms = false) => Utils.ColorString(Player.IsAlive() ? RoleInfo.RoleColor : Color.gray, $"({AssaulterL})");

}