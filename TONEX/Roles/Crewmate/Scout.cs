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
   }
    private void SendRPC()
    {
        using var sender = CreateSender();
        sender.Writer.Write(ScoutLimit);
    }
    public override void ReceiveRPC(MessageReader reader)
    {
        ScoutLimit = reader.ReadInt32();
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
            var imp = 0;
            var neu = 0;
            var crew = 0;
            foreach (var player in Main.AllPlayerControls)
            {

                if (!player.IsModClient()) player.KillFlash();
                if (!player.IsAlive() || Pelican.IsEaten(player.PlayerId)) continue;
                
                if (Vector2.Distance(killer.transform.position, player.transform.position) <= Radius.GetFloat())
                {
                    if (player.IsImpTeam())
                    {
                       Player.RpcProtectedMurderPlayer(player);
                       imp ++;
                    }
                    else if(player.GetCustomRole().IsNeutral())
                    {
                        Player.RpcProtectedMurderPlayer(player);
                        neu++;
                    }
                    else if (player.GetCustomRole().IsCrewmate())
                    {
                        Player.RpcProtectedMurderPlayer(player);
                        crew += 1;
                    }
                }
            }
            Player.Notify(string.Format(GetString("ScoutICNLimit"), imp, crew,neu));
            Player.SetKillCooldownV2();
            SendRPC();
        }
        info.CanKill = false;
        return false;
    }
    public override string GetProgressText(bool comms = false) => Utils.ColorString(CanUseKillButton() ? Utils.GetRoleColor(CustomRoles.Scout) : Color.gray, $"({ScoutLimit})");
}
