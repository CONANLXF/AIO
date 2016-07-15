using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy;

 namespace ExorAIO.Champions.Warwick
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
                !Targets.Target.LSIsValidTarget() ||
                Invulnerable.Check(Targets.Target))
            {
                return;
            }

            /// <summary>
            ///     The W Combo Logic.
            /// </summary>
            if (Vars.W.IsReady() &&
                GameObjects.Player.Spellbook.IsAutoAttacking &&
                Vars.getCheckBoxItem(Vars.WMenu, "combo"))
            {
                Vars.W.Cast();
            }

            if (GameObjects.Player.Spellbook.IsAutoAttacking)
            {
                return;
            }

            /// <summary>
            ///     The Q Combo Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                Vars.getCheckBoxItem(Vars.QMenu, "combo"))
            {
                if (GameObjects.Player.MaxHealth >
                        GameObjects.Player.Health +
                        (float)GameObjects.Player.LSGetSpellDamage(Targets.Target, SpellSlot.Q) * 0.8)
                {
                    Vars.Q.CastOnUnit(Targets.Target);
                }
            }

            /// <summary>
            ///     The R Combo Logic.
            /// </summary>
            if (Vars.R.IsReady())
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(
                    t =>
                        !t.IsUnderEnemyTurret() &&
                        t.LSIsValidTarget(Vars.R.Range) &&
                        !t.LSIsValidTarget(Vars.AARange) &&
                        Vars.getCheckBoxItem(Vars.RMenu, "combo") &&
                        Vars.getCheckBoxItem(Vars.WhiteListMenu, t.ChampionName.ToLower())))
                {
                    Vars.R.CastOnUnit(target);
                }
            }
        }
    }
}