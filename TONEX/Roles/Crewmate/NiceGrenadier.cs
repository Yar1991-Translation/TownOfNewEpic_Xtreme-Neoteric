using AmongUs.GameOptions;
using System.Collections.Generic;
using System.Linq;
using TONEX.Modules.SoundInterface;
using TONEX.Roles.Core;
using Hazel;
using UnityEngine;
using static TONEX.Translator;

namespace TONEX.Roles.Crewmate;
public sealed class NiceGrenadier : RoleBase
{
    public static readonly SimpleRoleInfo RoleInfo =
        SimpleRoleInfo.Create(
            typeof(NiceGrenadier),
            player => new NiceGrenadier(player),
            CustomRoles.NiceGrenadier,
         () => Options.UsePets.GetBool() ? RoleTypes.Crewmate : RoleTypes.Engineer,
            CustomRoleTypes.Crewmate,
            22000,
            SetupOptionItem,
            "gr|擲雷兵|掷雷|闪光弹",
            "#3c4a16"
        );
    public NiceGrenadier(PlayerControl player)
    : base(
        RoleInfo,
        player
    )
    {
        //CustomRoleManager.SuffixOthers.Add(GetSuffixOthers);
        CustomRoleManager.MarkOthers.Add(GetSuffixOthers);
        Blinds = new();
    }

    static OptionItem OptionSkillCooldown;
    static OptionItem OptionSkillDuration;
    static OptionItem OptionCanAffectNeutral;
    static OptionItem OptionSkillRange;
    static List<byte> Blinds;
    private static void SendRPC_SyncList()
    {
        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetNiceGraList, SendOption.Reliable, -1);
        writer.Write(Blinds.Count);
        for (int i = 0; i < Blinds.Count; i++)
            writer.Write(Blinds[i]);
        AmongUsClient.Instance.FinishRpcImmediately(writer);
    }
    public static void ReceiveRPC_SyncList(MessageReader reader)
    {
        int count = reader.ReadInt32();
        Blinds = new();
        for (int i = 0; i < count; i++)
            Blinds.Add(reader.ReadByte());
    }
    enum OptionName
    {
        NiceGrenadierSkillCooldown,
        NiceGrenadierSkillDuration,
        NiceGrenadierCanAffectNeutral,
        NiceGrenadierSkillRange,
    }
    public static bool IsBlinding(PlayerControl target)
    {
        if (Blinds.Contains(target.PlayerId) && target.IsAlive())
            return true;
        return false;
    }

    private long BlindingStartTime;
    private long MadBlindingStartTime;
    public long UsePetCooldown;
    private static void SetupOptionItem()
    {
        OptionSkillCooldown = FloatOptionItem.Create(RoleInfo, 10, OptionName.NiceGrenadierSkillCooldown, new(2.5f, 180f, 2.5f), 20f, false)
            .SetValueFormat(OptionFormat.Seconds);
        OptionSkillDuration = FloatOptionItem.Create(RoleInfo, 11, OptionName.NiceGrenadierSkillDuration, new(2.5f, 180f, 2.5f), 20f, false)
            .SetValueFormat(OptionFormat.Seconds);
        OptionCanAffectNeutral = BooleanOptionItem.Create(RoleInfo, 13, OptionName.NiceGrenadierCanAffectNeutral, false, false);
        OptionSkillRange = FloatOptionItem.Create(RoleInfo, 14, OptionName.NiceGrenadierSkillRange, new(0f, 50f, 2.5f), 10f, false)
            .SetValueFormat(OptionFormat.Multiplier);
    }
    public override void Add()
    {
        BlindingStartTime = -1;
        MadBlindingStartTime = -1;
        if (Options.UsePets.GetBool()) UsePetCooldown = Utils.GetTimeStamp();
    }
    public override void OnGameStart()
    {
        if (Options.UsePets.GetBool()) UsePetCooldown = Utils.GetTimeStamp();
    }
    public override void ApplyGameOptions(IGameOptions opt)
    {
        AURoleOptions.EngineerCooldown = OptionSkillCooldown.GetFloat();
        AURoleOptions.EngineerInVentMaxTime = 1f;
    }
    public override bool GetAbilityButtonText(out string text)
    {
        text = GetString("NiceGrenadierVetnButtonText");
        return true;
    }
    public override bool GetPetButtonText(out string text)
    {
        text = GetString("NiceGrenadierVetnButtonText");
        return !(UsePetCooldown != -1);
    }
    public override bool OnEnterVent(PlayerPhysics physics, int ventId)
    {
        if (Player.Is(CustomRoles.Madmate))
        {
            MadBlindingStartTime = Utils.GetTimeStamp();
            foreach (var pc in Main.AllAlivePlayerControls.Where(x => !x.IsImpTeam()))
            {

                OnBlinding(pc);
                pc.DisableAct();
            }
        }
        else
        {
            BlindingStartTime = Utils.GetTimeStamp();
            foreach (var pc in Main.AllAlivePlayerControls.Where(x => x.IsImpTeam() || (x.IsNeutral() && OptionCanAffectNeutral.GetBool())))
            {
                OnBlinding(pc);
                pc.DisableAct();
            }
        }
        SendRPC_SyncList();
        if (!Player.IsModClient()) Player.RpcProtectedMurderPlayer();
        Player.RPCPlayCustomSound("FlashBang");
        Player.Notify(GetString("NiceGrenadierSkillInUse"), OptionSkillDuration.GetFloat());
        return true;
    }
    void OnBlinding(PlayerControl pc)
    {
        var posi = Player.transform.position;
        var diss = Vector2.Distance(posi, pc.transform.position);
        if (pc != Player && diss <= OptionSkillRange.GetFloat())
        {
            if (pc.IsModClient())
            {
                pc.RPCPlayCustomSound("FlashBang");

            }
            Blinds.Add(pc.PlayerId);
            
            //pc.Notify("<size=1000><color=#ffffff>●</color></size>", OptionSkillDuration.GetInt());
        }
    }
    public override void OnUsePet()
    {
        if (!Options.UsePets.GetBool()) return;
        if (UsePetCooldown != -1)
        {
            var cooldown = UsePetCooldown + (long)OptionSkillCooldown.GetFloat() - Utils.GetTimeStamp();
            Player.Notify(string.Format(GetString("ShowUsePetCooldown"), cooldown, 1f));
            return;
        }
        UsePetCooldown = Utils.GetTimeStamp();
        if (Player.Is(CustomRoles.Madmate))
        {
            MadBlindingStartTime = Utils.GetTimeStamp();
            foreach (var pc in Main.AllAlivePlayerControls.Where(x => !x.IsImpTeam()))
            {

                OnBlinding(pc);
            }
        }
        else
        {
            BlindingStartTime = Utils.GetTimeStamp();
            foreach (var pc in Main.AllAlivePlayerControls.Where(x => x.IsImpTeam() || (x.IsNeutral() && OptionCanAffectNeutral.GetBool())))
            {
                OnBlinding(pc);
            }
        }
        SendRPC_SyncList();
        if (!Player.IsModClient()) Player.RpcProtectedMurderPlayer();
        Player.RPCPlayCustomSound("FlashBang");
        Player.Notify(GetString("NiceGrenadierSkillInUse"), OptionSkillDuration.GetFloat());
        return;
    }
    public override void OnFixedUpdate(PlayerControl player)
    {
        if (!AmongUsClient.Instance.AmHost) return;
        var now = Utils.GetTimeStamp();
        if (BlindingStartTime != -1 && BlindingStartTime + (long)OptionSkillDuration.GetFloat() < now)
        {
            BlindingStartTime = -1;
            Player.RpcProtectedMurderPlayer();
            Blinds.Clear();
            SendRPC_SyncList();
            Player.Notify(GetString("NiceGrenadierSkillStop"));
            Utils.MarkEveryoneDirtySettings();
        }
        if (MadBlindingStartTime != -1 && MadBlindingStartTime + (long)OptionSkillDuration.GetFloat() < now)
        {
            MadBlindingStartTime = -1;
            Player.RpcProtectedMurderPlayer();
            Blinds.Clear();
            SendRPC_SyncList();
            Player.Notify(GetString("NiceGrenadierSkillStop"));
            Utils.MarkEveryoneDirtySettings();
        }
        if (UsePetCooldown + (long)OptionSkillCooldown.GetFloat() < now && UsePetCooldown != -1 && Options.UsePets.GetBool())
        {
            UsePetCooldown = -1;
            player.RpcProtectedMurderPlayer();
            player.Notify(string.Format(GetString("PetSkillCanUse")));
        }
    }
    public override void OnExileWrapUp(GameData.PlayerInfo exiled, ref bool DecidedWinner)
    {
        Player.RpcResetAbilityCooldown();
    }
    public override void AfterMeetingTasks()
    {
        UsePetCooldown = Utils.GetTimeStamp();
    }
    public override void OnStartMeeting()
    {
        MadBlindingStartTime = -1;
        BlindingStartTime = -1;
    }
    public static string GetSuffixOthers(PlayerControl seer, PlayerControl seen = null, bool isForMeeting = false)
    {
        seen ??= seer;
        if (IsBlinding(seer))
            return "<size=1000><color=#ffffff>●</color></size>";
        return "";
    }

    public override bool GetAbilityButtonSprite(out string buttonName)
    {
        buttonName = "Gangstar";
        return true;
    }
    public override bool GetPetButtonSprite(out string buttonName)
    {
        buttonName = "Gangstar";
        return !(UsePetCooldown != -1);
    }
}