using AmongUs.GameOptions;
using static TONEX.Translator;
using TONEX.Roles.Core;
using MS.Internal.Xml.XPath;
using TONEX.Roles.Core.Interfaces.GroupAndRole;

namespace TONEX.Roles.Ghost.Impostor;
public sealed class EvilAngle : RoleBase, IImpostor
{

    public static readonly SimpleRoleInfo RoleInfo =
        SimpleRoleInfo.Create(
            typeof(EvilAngle),
            player => new EvilAngle(player),
            CustomRoles.EvilAngle,
            () => RoleTypes.GuardianAngel,
            CustomRoleTypes.Impostor,
            94_1_5_0100,
           null,
            "eg|邪恶守护",
            ctop: true
        );
    public EvilAngle(PlayerControl player)
    : base(
        RoleInfo,
        player
    )
    {
        Maxi = OptionTryMax.GetInt();
    }
    static OptionItem EnableEvilAngle;
    static OptionItem OptionProbability;
    static OptionItem OptionKiilCooldown;
    static OptionItem OptionTryMax;
    int Maxi;
    public static void SetupOptionItem()
    {
        EnableEvilAngle = BooleanOptionItem.Create(94_1_5_0110, "EnableEvilAngle", false, TabGroup.ImpostorRoles, false)
            .SetHeader(true)
            .SetGameMode(CustomGameMode.Standard);
        OptionProbability = IntegerOptionItem.Create(94_1_5_0111, "KillProbability", new(0, 100, 5), 40, TabGroup.ImpostorRoles, false)
            .SetValueFormat(OptionFormat.Percent)
            .SetParent(EnableEvilAngle);
        OptionKiilCooldown = FloatOptionItem.Create(94_1_5_0112,"KillCooldown", new(0, 100, 5), 40, TabGroup.ImpostorRoles, false)
            .SetValueFormat(OptionFormat.Seconds)
            .SetParent(EnableEvilAngle);
        OptionTryMax = IntegerOptionItem.Create(94_1_5_0113, "OptionTryMax", new(0, 100, 5), 40, TabGroup.ImpostorRoles, false)
            .SetValueFormat(OptionFormat.Times)
            .SetParent(EnableEvilAngle);
    }
    public override bool CanUseAbilityButton() => true;
    public override bool GetAbilityButtonText(out string text)
    {
        text = GetString("KillButtonText");
        return true;
    }
    public override bool GetAbilityButtonSprite(out string buttonName)
    {
        buttonName= "KillButton";
        return true;
    }
    public override bool OnProtectPlayer(PlayerControl target)
    {
        if (Player.IsAlive() ||Maxi <=0) return false;
        Maxi --;
        if (IRandom.Instance.Next(0, 100) < OptionProbability.GetInt())
        {
            target.Notify(string.Format(GetString("KillForEvilAngle")), 2f);
            Player.RpcTeleport(target.GetTruePosition());
            RPC.PlaySoundRPC(Player.PlayerId, Sounds.KillSound);
            Player.RpcMurderPlayerV2(target);
            target.SetRealKiller(Player);
        }
        return false;
    }
    public override void ApplyGameOptions(IGameOptions opt)
    {
        AURoleOptions.GuardianAngelCooldown = OptionKiilCooldown.GetFloat();
    }
}
