using System.Linq;
using LeagueSharp.Common.Data;
using NechritoRiven.Menus;
using EloBuddy.SDK;

using TargetSelector = PortAIO.TSManager; namespace NechritoRiven.Core
{
    class Usables : Core
    {
        
        public static void CastHydra()
        {
            if (ItemData.Ravenous_Hydra_Melee_Only.GetItem().IsReady())
                ItemData.Ravenous_Hydra_Melee_Only.GetItem().Cast();

            if (ItemData.Titanic_Hydra_Melee_Only.GetItem().IsReady())
                ItemData.Titanic_Hydra_Melee_Only.GetItem().Cast();

            else if (ItemData.Tiamat_Melee_Only.GetItem().IsReady())
                ItemData.Tiamat_Melee_Only.GetItem().Cast();

            PortAIO.OrbwalkerManager.ResetAutoAttackTimer();
        }
        public static void CastYoumoo()
        {
            if (ItemData.Youmuus_Ghostblade.GetItem().IsReady()) ItemData.Youmuus_Ghostblade.GetItem().Cast();
        }
    }
}
