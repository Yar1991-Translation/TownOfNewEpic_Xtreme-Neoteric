using AmongUs.GameOptions;
using UnityEngine;
using Hazel;
using TONEX.Roles.Core;
using static TONEX.Translator;
using static UnityEngine.GraphicsBuffer;
using System.Collections.Generic;
using TONEX.Roles.Core.Interfaces.GroupAndRole;
using TONEX.Modules.SoundInterface;
using TONEX.Roles.Neutral;

namespace TONEX.Roles.Crewmate;
public sealed class Scout : RoleBase, IKiller
{
    public static readonly SimpleRoleInfo RoleInfo =
        SimpleRoleInfo.Create(
            typeof(Scout),
            player => new Scout(player),
            CustomRoles.Scout,
            () => RoleTypes.Impostor,
            CustomRoleTypes.Crewmate,
            94_1_2_0200,
            SetupOptionItem,
            "sc",
            "#0099FF",
            true
        );
    public Scout(PlayerControl player)
    : base(
        RoleInfo,
        player,
        () => HasTask.False
    )
    {
    }

    static OptionItem Cooldown;
    static OptionItem Limits;
    static OptionItem Radius;
    enum OptionName
    {
        ProphetCooldown,
        ProphetLimit,
        ScoutRadius,
    }
    private int ScoutLimit;
    private int Impostors;
         private int Crewmates;
    private int Neutrals;
    public bool IsKiller { get; private set; } = false;
    private static void SetupOptionItem()
    {
        Cooldown = FloatOptionItem.Create(RoleInfo, 10, GeneralOption.SkillCooldown, new(2.5f, 180f, 2.5f), 30f, false)
            .SetValueFormat(OptionFormat.Seconds);
        Limits = IntegerOptionItem.Create(RoleInfo, 11, GeneralOption.SkillLimit, new(1, 180, 1), 3, false)
            .SetValueFormat(OptionFormat.Times);
        Radius = FloatOptionItem.Create(RoleInfo, 12, OptionName.ScoutRadius, new(0.5f, 10f, 0.5f), 1.5f, false)
            .SetValueFormat(OptionFormat.Multiplier);
    }
    public override void Add()
    {
        ScoutLimit = Limits.GetInt();
        Impostors = 0;
        Crewmates = 0;
        Neutrals = 0;
   }
    private void SendRPC()
    {
        using var sender = CreateSender();
        sender.Writer.Write(ScoutLimit);
        sender.Writer.Write(Crewmates);
        sender.Writer.Write(Impostors);
        sender.Writer.Write(Neutrals);
    }
    public override void ReceiveRPC(MessageReader reader)
    {
        ScoutLimit = reader.ReadInt32();
        Impostors = reader.ReadInt32();
        Crewmates = reader.ReadInt32();
        Neutrals = reader.ReadInt32();
    }
    public float CalculateKillCooldown() => CanUseKillButton() ? Cooldown.GetFloat() : 255f;
    public bool CanUseKillButton() => Player.IsAlive() && ScoutLimit >= 1;
    public bool CanUseSabotageButton() => false;
    public bool CanUseImpostorVentButton() => false;
    public override void ApplyGameOptions(IGameOptions opt) => opt.SetVision(false);
    public bool OnCheckMurderAsKiller(MurderInfo info)
    {
        var (killer, target) = info.AttemptTuple;
        if (ScoutLimit >= 1)
        {
            ScoutLimit -= 1;
            SendRPC();
            foreach (var player in Main.AllPlayerControls)
            {

                if (!player.IsModClient()) player.KillFlash();
                if (!player.IsAlive() || Pelican.IsEaten(player.PlayerId)) continue;
                if (Vector2.Distance(killer.transform.position, player.transform.position) <= Radius.GetFloat())
                {
                    if (player.GetCustomRole().IsImpostor())
                    {
                       Player.RpcProtectedMurderPlayer(player);
                        Impostors += 1;
                    }
                    else if(player.GetCustomRole().IsNeutral())
                    {
                        Player.RpcProtectedMurderPlayer(player);
                        Neutrals += 1;
                    }
                    else if (player.GetCustomRole().IsCrewmate())
                    {
                        Player.RpcProtectedMurderPlayer(player);
                        Crewmates += 1;
                    }
                }
            }
            SendRPC();
        }
        info.CanKill = false;
        return false;
    }
    public override string GetProgressText(bool comms = false) => Utils.ColorString(CanUseKillButton() ? Utils.GetRoleColor(CustomRoles.Scout) : Color.gray, $"({ScoutLimit})");
    public override string GetSuffix(PlayerControl seer, PlayerControl seen = null, bool isForMeeting = false)
    {
        seen ??= seer;
        if (!GameStates.IsInTask || isForMeeting || !Is(seer) || !Is(seen)) return "";;

        string suffix = "";
       suffix = (string.Format(GetString("ScoutICNLimit"), Impostors,Crewmates,Neutrals));

        return suffix;
    }
}
