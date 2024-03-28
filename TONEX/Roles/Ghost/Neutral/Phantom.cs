using AmongUs.GameOptions;
using static TONEX.Translator;
using TONEX.Roles.Core;
using MS.Internal.Xml.XPath;
using TONEX.Roles.Core.Interfaces.GroupAndRole;

namespace TONEX.Roles.Ghost.Neutral;
public sealed class Phantom : RoleBase
{

    public static readonly SimpleRoleInfo RoleInfo =
        SimpleRoleInfo.Create(
            typeof(Phantom),
            player => new Phantom(player),
            CustomRoles.Phantom,
            () => RoleTypes.GuardianAngel,
            CustomRoleTypes.Crewmate,
            75_1_5_0300,
            null,
            "ijs|冤枉",
            "#65167d",
            ctop: true
        );
    public Phantom(PlayerControl player)
    : base(
        RoleInfo,
        player
    )
    {
    }
    static OptionItem EnablePhantom;
    public static OptionItem OptionTaskCount;
    int Maxi;
    public static void SetupOptionItem()
    {
        EnablePhantom = BooleanOptionItem.Create(75_1_5_0310, "EnablePhantom", false, TabGroup.NeutralRoles, false)
            .SetHeader(true)
            .SetGameMode(CustomGameMode.Standard);
        OptionTaskCount = IntegerOptionItem.Create(75_1_5_0311, "OptionTaskCount", new(0, 100, 1), 10, TabGroup.NeutralRoles, false)
            .SetValueFormat(OptionFormat.Pieces)
            .SetParent(EnablePhantom);
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
