using System;
using ExorAIO.Utilities;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;

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
        public static void Harass(EventArgs args)
        {
            if (!Targets.Target.LSIsValidTarget() ||
                Invulnerable.Check(Targets.Target))
            {
                return;
            }

            /// <summary>
            ///     The E Harass Logic.
            /// </summary>
            if (Vars.E.IsReady() &&
                Targets.Target.LSIsValidTarget(Vars.E.Range) &&
                GameObjects.Player.ManaPercent > 
                    Vars.getSliderItem(Vars.EMenu, "harass") +
                    (int)(GameObjects.Player.Spellbook.GetSpell(Vars.E.Slot).SData.Mana / GameObjects.Player.MaxMana * 100) &&
                Vars.getSliderItem(Vars.EMenu, "harass") != 101 &&
                Vars.getCheckBoxItem(Vars.WhiteListMenu, Targets.Target.ChampionName.ToLower()))
            {
                Vars.E.CastOnUnit(Targets.Target);
            }
        }
    }
}