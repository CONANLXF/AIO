using System;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy;
using EloBuddy.SDK;

 namespace ExorAIO.Champions.Tristana
{
    /// <summary>
    ///     The logics class.
    /// </summary>
    internal partial class Logics
    {
        /// <summary>
        ///     Called when the game updates itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void Combo(EventArgs args)
        {
            if (Bools.HasSheenBuff() ||
                !(Orbwalker.LastTarget as AIHeroClient).IsValidTarget())
            {
                return;
            }

            /// <summary>
            ///     The E Combo Logic.
            /// </summary>
            if (Vars.E.IsReady() &&
                !Invulnerable.Check((Orbwalker.LastTarget as AIHeroClient)) &&
                (Orbwalker.LastTarget as AIHeroClient).IsValidTarget(Vars.E.Range) &&
                Vars.getCheckBoxItem(Vars.EMenu, "combo") &&
                Vars.getCheckBoxItem(Vars.WhiteListMenu, Targets.Target.ChampionName.ToLower()))
            {
                Vars.E.CastOnUnit(Orbwalker.LastTarget as AIHeroClient);
            }
        }
    }
}