using AmongUs.GameOptions;
using HarmonyLib;
using System.Linq;
using TONEX.Modules.SoundInterface;
using TONEX.Roles.Core;
using TONEX.Roles.Core.Interfaces.GroupAndRole;
using TONEX.Roles.Crewmate;
using UnityEngine;
using static TONEX.Translator;

namespace TONEX.Roles.Impostor;
public sealed class EvilGrenadier : RoleBase, IImpostor
{
    public static readonly SimpleRoleInfo RoleInfo =
        SimpleRoleInfo.Create(
            typeof(EvilGrenadier),
            player => new EvilGrenadier(player),
            CustomRoles.EvilGrenadier,
         () => Options.UsePets.GetBool() ? RoleTypes.Impostor : RoleTypes.Shapeshifter,
            CustomRoleTypes.Impostor,
            75_1_2_0500,
            SetupOptionItem,
            "gr|擲雷兵|掷雷|闪光弹"
        );
    public EvilGrenadier(PlayerControl player)
    : base(
        RoleInfo,
        player
    )
    { }

    static OptionItem OptionSkillCooldown;
    static OptionItem OptionSkillDuration;
    static OptionItem OptionSkillRange;
    enum OptionName
    {
        NiceGrenadierSkillCooldown,
        NiceGrenadierSkillDuration,
        NiceGrenadierSkillRange,
    }

    private long BlindingStartTime;
    public long UsePetCooldown;
    private static void SetupOptionItem()
    {
        OptionSkillCooldown = FloatOptionItem.Create(RoleInfo, 10, OptionName.NiceGrenadierSkillCooldown, new(2.5f, 180f, 2.5f), 20f, false)
            .SetValueFormat(OptionFormat.Seconds);
        OptionSkillDuration = FloatOptionItem.Create(RoleInfo, 11, OptionName.NiceGrenadierSkillDuration, new(2.5f, 180f, 2.5f), 20f, false)
            .SetValueFormat(OptionFormat.Seconds);
        OptionSkillRange = FloatOptionItem.Create(RoleInfo, 14, OptionName.NiceGrenadierSkillRange, new(0f, 50f, 2.5f), 10f, false)
            .SetValueFormat(OptionFormat.Multiplier);
    }
    public override void Add()
    {
        BlindingStartTime = -1;
        if (Options.UsePets.GetBool()) UsePetCooldown = Utils.GetTimeStamp();
    }
    public override void OnGameStart()
    {
        if (Options.UsePets.GetBool()) UsePetCooldown = Utils.GetTimeStamp();
    }
    public override bool GetAbilityButtonText(out string text)
    {
        text = GetString("EvilGrenadierVetnButtonText");
        return true;
    }
    public override bool GetPetButtonText(out string text)
    {
        text = GetString("EvilGrenadierVetnButtonText");
        return !(UsePetCooldown != -1);
    }
    public override bool OnCheckShapeshift(PlayerControl target, ref bool animate)
    {
        foreach (var pc in Main.AllAlivePlayerControls.Where(x => !x.IsImp() && !x.Is(CustomRoles.Madmate)))
        {
            OnBlinding(pc);
        }
        Player.RPCPlayCustomSound("FlashBang");
        foreach (var pc in Main.AllAlivePlayerControls.Where(x => x.IsImp() || x.Is(CustomRoles.Madmate)))
            pc.Notify(GetString("NiceGrenadierSkillInUse"), OptionSkillDuration.GetFloat());
        return false;
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
            pc.Notify("<size=100><color=#ffffff>●</color></size>", OptionSkillDuration.GetInt());
        }
    }
    public static bool IsBlinding(PlayerControl target)
    {
        foreach (var pc in Main.AllAlivePlayerControls.Where(x => x.Is(CustomRoles.EvilGrenadier)))
        {
            if (pc.GetRoleClass() is not EvilGrenadier roleClass) continue;
            if (roleClass.BlindingStartTime != -1)
            {
                if (!target.IsImp() && !target.Is(CustomRoles.Madmate))
                    return true;
            }
        }
        return false;
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
        foreach (var pc in Main.AllAlivePlayerControls.Where(x => !x.IsImp() && !x.Is(CustomRoles.Madmate)))
        {
            OnBlinding(pc);
        }
        Player.RPCPlayCustomSound("FlashBang");
        foreach (var pc in Main.AllAlivePlayerControls.Where(x => x.IsImp() || x.Is(CustomRoles.Madmate)))
            pc.Notify(GetString("NiceGrenadierSkillInUse"), OptionSkillDuration.GetFloat());
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
    public override void AfterMeetingTasks()
    {
        UsePetCooldown = Utils.GetTimeStamp();
    }
    public override void ApplyGameOptions(IGameOptions opt)
    {
        AURoleOptions.ShapeshifterLeaveSkin = false;
        AURoleOptions.ShapeshifterCooldown = OptionSkillDuration.GetFloat();
        AURoleOptions.ShapeshifterDuration = 1f;
    }
    public override void OnStartMeeting()
    {
        BlindingStartTime = -1;
    }
}