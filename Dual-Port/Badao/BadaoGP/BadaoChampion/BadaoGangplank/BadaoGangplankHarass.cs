using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy.SDK;
using EloBuddy;

using TargetSelector = PortAIO.TSManager; namespace BadaoKingdom.BadaoChampion.BadaoGangplank
{
    public static class BadaoGangplankHarass
    {
        public static void BadaoActivate ()
        {
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (!PortAIO.OrbwalkerManager.isHarassActive)
                return;
            if (BadaoMainVariables.Q.IsReady() && BadaoGangplankVariables.HarassQ)
            {
                var target = TargetSelector.GetTarget(BadaoMainVariables.Q.Range, DamageType.Physical);
                if (target.BadaoIsValidTarget())
                {
                    BadaoMainVariables.Q.Cast(target);
                }
            }
        }
    }
}
