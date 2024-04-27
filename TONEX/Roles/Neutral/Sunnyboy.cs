using AmongUs.GameOptions;
using System.Linq;
using TONEX.Roles.Core;
using TONEX.Roles.Core.Interfaces;
using System;
using System.Collections.Generic;
using TONEX.Roles.Core.Interfaces.GroupAndRole;
using Hazel;

namespace TONEX.Roles.Neutral;

public sealed class Sunnyboy : RoleBase,  INeutral, IAdditionalWinner
{
    public static readonly SimpleRoleInfo RoleInfo =
       SimpleRoleInfo.Create(
            typeof(Sunnyboy),
            player => new Sunnyboy(player),
            CustomRoles.Sunnyboy,
         () =>  RoleTypes.Scientist,
            CustomRoleTypes.Neutral,
            75_1_2_1000,
            null,
            "sb|Ñô¹â|Ñô¹â¿ªÀÊ",
            "#ff9902",
           true
           #if RELEASE
,
            Hidden: true
#endif

        );
    public Sunnyboy(PlayerControl player)
    : base(
        RoleInfo,
        player
    )
    {
        
    }
    public bool IsNE { get; private set; } = false;
    public override void ApplyGameOptions(IGameOptions opt) => opt.SetVision(false);

    public bool CheckWin(ref CustomRoles winnerRole, ref CountTypes winnerCountType)
    {
        return !Player.IsAlive();
    }
}
