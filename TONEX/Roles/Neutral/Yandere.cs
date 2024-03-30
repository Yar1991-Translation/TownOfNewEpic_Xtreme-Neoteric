using AmongUs.GameOptions;
using Hazel;
using System.Collections.Generic;
using System.Linq;
using TONEX.Modules.SoundInterface;
using TONEX.Roles.Core;
using TONEX.Roles.Core.Interfaces;
using TONEX.Roles.Core.Interfaces.GroupAndRole;
using UnityEngine;
using HarmonyLib;
using InnerNet;
using System;
using static UnityEngine.GraphicsBuffer;
using MS.Internal.Xml.XPath;

namespace TONEX.Roles.Neutral;
public sealed class Yandere : RoleBase, INeutralKiller
{
    public static readonly SimpleRoleInfo RoleInfo =
        SimpleRoleInfo.Create(
            typeof(Yandere),
            player => new Yandere(player),
            CustomRoles.Yandere,
            () => RoleTypes.Impostor,
            CustomRoleTypes.Neutral,
            94_1_2_0400,
            SetupOptionItem,
            "ya",
            "#ff25e4",
            true,
            true,
            countType: CountTypes.Yandere
        );
    public Yandere(PlayerControl player)
    : base(
        RoleInfo,
        player,
        () => HasTask.False
    )
    {
    }
    private static OptionItem OptionKillCooldown;
    public bool IsNK { get; private set; } = true;
    enum OptionName
    {

    }
    public PlayerControl TargetId;
    public static List<PlayerControl> Targets = new();
    private static void SetupOptionItem()
    {
        OptionKillCooldown = FloatOptionItem.Create(RoleInfo, 10, GeneralOption.KillCooldown, new(2.5f, 180f, 2.5f), 20f, false)
            .SetValueFormat(OptionFormat.Seconds);

    }
      List<byte> NeedKill;
    public override void Add()
    {
        //ターゲット割り当て
        if (!AmongUsClient.Instance.AmHost) return;
        NeedKill = new();
        Targets = new();
        var playerId = Player.PlayerId;
        var pcList = Main.AllAlivePlayerControls.Where(x => x.PlayerId != Player.PlayerId && x.IsAlive() && !x.Is(CustomRoles.Lovers)).ToList();
        var SelectedTarget = pcList[IRandom.Instance.Next(0, pcList.Count)];
        TargetId = SelectedTarget;
        Targets.Add(SelectedTarget);
        NameColorManager.Add(Player.PlayerId, SelectedTarget.PlayerId, "#ff25e4");
        SendRPC();
    }
    private void SendRPC()
    {
        var sender = CreateSender();
        sender.Writer.Write(TargetId);
        NeedKill.Do(sender.Writer.Write);
    }
    public override void ReceiveRPC(MessageReader reader)
    {
        NeedKill = new();
        for (int i = 0; i < reader.ReadInt32(); i++)
            NeedKill.Add(reader.ReadByte());
    }
    public override string GetMark(PlayerControl seer, PlayerControl seen, bool _ = false)
    {
        //seenが省略の場合seer
        seen ??= seer;

        return TargetId.PlayerId == seen.PlayerId ? Utils.ColorString(RoleInfo.RoleColor, "♡") : "";
    }
    public override string GetSuffix(PlayerControl seer, PlayerControl seen = null, bool isForMeeting = false)
    {
        if (isForMeeting) return "";
        seen ??= seer;
        if (!Is(seer) && !Is(seen)) return "";

        var arrows = "";
        foreach (var targetId in NeedKill) arrows += TargetArrow.GetArrows(seer, targetId);

        return arrows;
    }
    public override void OnFixedUpdate(PlayerControl player)
    {
        if (!AmongUsClient.Instance.AmHost) return;
      ExtendedPlayerControl. CheckDistanceAndDoActions(TargetId.GetTruePosition(), (pc) =>
        {
            if (!NeedKill.Contains(pc.PlayerId))
            {
                NeedKill.Add(pc.PlayerId); 
                TargetArrow.Add(Player.PlayerId,pc.PlayerId);
            }
        }, TargetId);
    }
    public override void OnPlayerDeath(PlayerControl player, CustomDeathReason deathReason, bool isOnMeeting)
    {
        var pc = player;
        if (TargetId==pc)
        {
           Player.RpcMurderPlayerV2(Player);
            NeedKill.Clear();
            SendRPC();
            foreach (var targetId in NeedKill)   TargetArrow.Remove(Player.PlayerId, targetId);
            Utils.NotifyRoles(Player);
        }
    }
    public bool OnCheckMurderAsKiller(MurderInfo info)
    {
        var (killer, target) = info.AttemptTuple;
           if(!NeedKill.Contains(target.PlayerId)) return false;
        return true;
    }
    public float CalculateKillCooldown() => OptionKillCooldown.GetFloat();
    public bool CanUseSabotageButton() => false;
    public bool CanUseImpostorVentButton() => false;
    public override void ApplyGameOptions(IGameOptions opt) => opt.SetVision(false);
    public void ApplySchrodingerCatOptions(IGameOptions option) => ApplyGameOptions(option);
}