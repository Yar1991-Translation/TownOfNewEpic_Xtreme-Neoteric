using AmongUs.GameOptions;
using static TONEX.Translator;
using TONEX.Roles.Core;
using MS.Internal.Xml.XPath;
using TONEX.Roles.Core.Interfaces.GroupAndRole;

namespace TONEX.Roles.Ghost.Crewmate;
public sealed class InjusticeSpirit : RoleBase
{

    public static readonly SimpleRoleInfo RoleInfo =
        SimpleRoleInfo.Create(
            typeof(InjusticeSpirit),
            player => new InjusticeSpirit(player),
            CustomRoles.InjusticeSpirit,
            () => RoleTypes.GuardianAngel,
            CustomRoleTypes.Crewmate,
            75_1_5_0200,
            null,
            "ijs|冤枉",
            "#eaf108",
            ctop: true
        );
    public InjusticeSpirit(PlayerControl player)
    : base(
        RoleInfo,
        player
    )
    {
    }
    static OptionItem EnableInjusticeSpirit;
    public static OptionItem OptionTaskCount;
    int Maxi;
    public static void SetupOptionItem()
    {
        EnableInjusticeSpirit = BooleanOptionItem.Create(75_1_5_0110, "EnableInjusticeSpirit", false, TabGroup.CrewmateRoles, false)
            .SetHeader(true)
            .SetGameMode(CustomGameMode.Standard);
        OptionTaskCount = IntegerOptionItem.Create(75_1_5_0111, "OptionTaskCount", new(0, 100, 1), 10, TabGroup.CrewmateRoles, false)
            .SetValueFormat(OptionFormat.Pieces)
            .SetParent(EnableInjusticeSpirit);
    }
    public override bool OnCompleteTask(out bool cancel)
    {
        cancel = false;
        return false;
    }
    public override bool CanUseAbilityButton() => false;
    public override bool OnProtectPlayer(PlayerControl target)
    {
        return false;
    }
    public override void ApplyGameOptions(IGameOptions opt)
    {
        AURoleOptions.GuardianAngelCooldown =255f;
    }

}
