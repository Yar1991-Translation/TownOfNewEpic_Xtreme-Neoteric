using AmongUs.GameOptions;
using Hazel;
using TONEX.Roles.Core;
using TONEX.Roles.Core.Interfaces;
using TONEX.Roles.Core.Interfaces.GroupAndRole;
using static TONEX.Translator;

namespace TONEX.Roles.Neutral;
public sealed class SharpShooter : RoleBase, INeutralKiller
{
    public static readonly SimpleRoleInfo RoleInfo =
       SimpleRoleInfo.Create(
           typeof(SharpShooter),
           player => new SharpShooter(player),
           CustomRoles.SharpShooter,
           () => RoleTypes.Impostor,
           CustomRoleTypes.Neutral,
           94_1_2_0100,
           SetupOptionItem,
           "ss|神射|神社",
           "#000855",
           true,
           true,
           countType: CountTypes.SharpShooter
       );
    public SharpShooter(PlayerControl player)
    : base(
        RoleInfo,
        player,
        () => HasTask.False
    )
    {
    }

    static OptionItem OptionKillDistance;
    enum OptionName
    {
        SharpShooterKillDistance,
    }
    public bool IsNK { get; private set; } = true;
    public SchrodingerCat.TeamType SchrodingerCatChangeTo => SchrodingerCat.TeamType.SharpShooter;

    private static void SetupOptionItem()
    {
        OptionKillDistance = IntegerOptionItem.Create(RoleInfo, 16, OptionName.SharpShooterKillDistance, new(1, 15, 1), 1, false)
            .SetValueFormat(OptionFormat.Multiplier);
    }
    public override void ApplyGameOptions(IGameOptions opt)
    {
        AURoleOptions.KillDistance = (int)((PlayerState.GetByPlayerId(Player.PlayerId)?.GetKillCount(true) ?? 0)) * OptionKillDistance.GetInt();
    }
    public static void SetHudActive(HudManager __instance, bool _) => __instance.SabotageButton.ToggleVisible(false);
    public bool CanUseSabotageButton() => false;
}