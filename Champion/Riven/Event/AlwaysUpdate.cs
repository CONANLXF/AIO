#region

using System;
using LeagueSharp;
using LeagueSharp.Common;
using NechritoRiven.Core;
using NechritoRiven.Menus;
using EloBuddy;
using EloBuddy.SDK;

#endregion

 namespace NechritoRiven.Event
{
    internal class AlwaysUpdate : Core.Core
    {
        
        public static void Update(EventArgs args)
        {
            if (Player.IsDead || Player.LSIsRecalling())
            {
                return;
            }

            if (Environment.TickCount - lastQ >= 3650 && Qstack != 1 && !Player.InFountain() && MenuConfig.KeepQ && Player.HasBuff("RivenTriCleave") &&
              !Player.Spellbook.IsChanneling && Spells.Q.IsReady()) Spells.Q.Cast(Game.CursorPos);

            Modes.QMove();
            ForceSkill();

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Modes.Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                Modes.Flee();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Modes.Harass();
            }

            if (MenuConfig.Burst)
            {
                Modes.Burst();
            }

            if (MenuConfig.FastHarass)
            {
                Modes.FastHarass();
            }
        }
    }
}
