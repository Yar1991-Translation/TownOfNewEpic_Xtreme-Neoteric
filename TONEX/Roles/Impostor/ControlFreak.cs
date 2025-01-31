using AmongUs.GameOptions;
using Hazel;
using System.Collections.Generic;
using System.Linq;
using TONEX.Modules.SoundInterface;
using TONEX.Roles.Core;
using TONEX.Roles.Core.Interfaces.GroupAndRole;
using UnityEngine;
using static TONEX.Translator;

namespace TONEX.Roles.Impostor;
public sealed class ControlFreak : RoleBase, IImpostor
{
    public static readonly SimpleRoleInfo RoleInfo =
        SimpleRoleInfo.Create(
            typeof(ControlFreak),
            player => new ControlFreak(player),
            CustomRoles.ControlFreak,
            () => RoleTypes.Impostor,
            CustomRoleTypes.Impostor,
            2500,
            null,
            "pup|傀儡師|傀儡"
        );
    public ControlFreak(PlayerControl player)
    : base(
        RoleInfo,
        player
    )
    {
        CustomRoleManager.OnFixedUpdateOthers.Add(OnFixedUpdateOthers);
    }
    /// <summary>
    /// Key: ターゲットのPlayerId, Value: パペッティア
    /// </summary>
    private static Dictionary<byte, ControlFreak> Puppets = new(15);
    public bool IsKiller { get; private set; } = false;
    public override void OnDestroy()
    {
        Puppets.Clear();
    }
    private void SendRPC(byte targetId, byte typeId)
    {
        using var sender = CreateSender();

        sender.Writer.Write(typeId);
        sender.Writer.Write(targetId);
    }
    public override void ReceiveRPC(MessageReader reader)
    {
        

        var typeId = reader.ReadByte();
        var targetId = reader.ReadByte();

        switch (typeId)
        {
            case 0: //Dictionaryのクリア
                Puppets.Clear();
                break;
            case 1: //Dictionaryに追加
                Puppets[targetId] = this;
                break;
            case 2: //DictionaryのKey削除
                Puppets.Remove(targetId);
                break;
        }
    }
    public bool OnCheckMurderAsKiller(MurderInfo info)
    {
        var (ControlFreak, target) = info.AttemptTuple;

        Puppets[target.PlayerId] = this;
        SendRPC(target.PlayerId, 1);
        ControlFreak.SetKillCooldownV2();
        ControlFreak.RPCPlayCustomSound("Line");

        Utils.NotifyRoles(SpecifySeer: ControlFreak);
        return false;
    }
    public override void OnReportDeadBody(PlayerControl _, GameData.PlayerInfo __)
    {
        Puppets.Clear();
        SendRPC(byte.MaxValue, 0);
    }
    public static void OnFixedUpdateOthers(PlayerControl puppet)
    {
        if (!AmongUsClient.Instance.AmHost) return;

        if (Puppets.TryGetValue(puppet.PlayerId, out var ControlFreak))
            ControlFreak.CheckPuppetKill(puppet);
    }
    private void CheckPuppetKill(PlayerControl puppet)
    {
        if (!puppet.IsAlive())
        {
            Puppets.Remove(puppet.PlayerId);
            SendRPC(puppet.PlayerId, 2);
        }
        else
        {
            var puppetPos = puppet.transform.position;//puppetの位置
            Dictionary<PlayerControl, float> targetDistance = new();
            foreach (var pc in Main.AllAlivePlayerControls.ToArray())
            {
                if (pc.PlayerId != puppet.PlayerId && !pc.Is(CountTypes.Impostor))
                {
                    var dis = Vector2.Distance(puppetPos, pc.transform.position);
                    targetDistance.Add(pc, dis);
                }
            }
            if (targetDistance.Keys.Count <= 0) return;

            var min = targetDistance.OrderBy(c => c.Value).FirstOrDefault();//一番値が小さい
            var target = min.Key;
            var KillRange = NormalGameOptionsV07.KillDistances[Mathf.Clamp(Main.NormalOptions.KillDistance, 0, 2)];
            if (min.Value <= KillRange && puppet.CanMove && target.CanMove)
            {
                RPC.PlaySoundRPC(Player.PlayerId, Sounds.KillSound);
                target.SetRealKiller(Player);
                puppet.RpcMurderPlayer(target);
                Utils.MarkEveryoneDirtySettings();
                Puppets.Remove(puppet.PlayerId);
                SendRPC(puppet.PlayerId, 2);
                Utils.NotifyRoles();
            }
        }
    }
    public override string GetMark(PlayerControl seer, PlayerControl seen, bool _ = false)
    {
        //seenが省略の場合seer
        seen ??= seer;

        if (!(Puppets.ContainsValue(this) &&
            Puppets.ContainsKey(seen.PlayerId))) return "";

        return Utils.ColorString(RoleInfo.RoleColor, "◆");
    }
    public bool OverrideKillButtonText(out string text)
    {
        text = GetString("ControlFreakOperateButtonText");
        return true;
    }
}