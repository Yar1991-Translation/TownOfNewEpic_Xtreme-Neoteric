using AmongUs.GameOptions;
using System.Linq;
using TONEX.Roles.Core;
using TONEX.Roles.Core.Interfaces;
using TONEX.Roles.Core.Interfaces.GroupAndRole;

namespace TONEX.Roles.Neutral;

public sealed class Opportunist : RoleBase, IAdditionalWinner, INeutralKiller
{
    public static readonly SimpleRoleInfo RoleInfo =
       SimpleRoleInfo.Create(
            typeof(Opportunist),
            player => new Opportunist(player),
            CustomRoles.Opportunist,
         () => OptionCanKill.GetBool() ? RoleTypes.Impostor : RoleTypes.Crewmate,
            CustomRoleTypes.Neutral,
            50100,
           SetupOptionItem,
            "op|Ͷ�C��|Ͷ��",
            "#00ff00",
           true
           
        );
    public Opportunist(PlayerControl player)
    : base(
        RoleInfo,
        player
    )
    {
        
    }
    private static OptionItem OptionKillCooldown;
    public static OptionItem OptionCanKill;
    public static OptionItem OptionCanVent;
    public bool IsNK { get; private set; } = OptionCanKill.GetBool();
    public bool IsNE { get; private set; } = false;
    public SchrodingerCat.TeamType SchrodingerCatChangeTo => SchrodingerCat.TeamType.Opportunist;
    private static void SetupOptionItem()
    {
        OptionCanKill = BooleanOptionItem.Create(RoleInfo, 15, GeneralOption.CanKill, true, false);
        OptionKillCooldown = FloatOptionItem.Create(RoleInfo, 10, GeneralOption.KillCooldown, new(2.5f, 180f, 2.5f), 20f, false, OptionCanKill)
            .SetValueFormat(OptionFormat.Seconds);
        OptionCanVent = BooleanOptionItem.Create(RoleInfo, 11, GeneralOption.CanVent, true, false, OptionCanKill);
    }
    public float CalculateKillCooldown() => OptionKillCooldown.GetFloat();
    public bool CanUseKillButton() => OptionCanKill.GetBool();
    public bool CanUseSabotageButton() => false;
    public bool CanUseImpostorVentButton() => OptionCanVent.GetBool();
    public override void ApplyGameOptions(IGameOptions opt) => opt.SetVision(false);

    public bool CheckWin(ref CustomRoles winnerRole, ref CountTypes winnerCountType)
    {
        var win = false;
        foreach (var player in Main.AllPlayerControls.Where(p => p.Is(CustomRoles.SchrodingerCat)))
        {
            if (CustomWinnerHolder.WinnerIds.Contains(player.PlayerId) && (player.GetRoleClass() as SchrodingerCat).Team == SchrodingerCat.TeamType.Opportunist)
                win = true;
        }
        return Player.IsAlive()|| win;
    }
}
