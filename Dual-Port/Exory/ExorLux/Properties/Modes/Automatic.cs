using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;
using LeagueSharp.SDK.Enumerations;
using EloBuddy;
using EloBuddy.SDK;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.Lux
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
        public static void Automatic(EventArgs args)
        {
            if (GameObjects.Player.LSIsRecalling())
            {
                return;
            }

            /// <summary>
            ///     The E Missile Manager.
            /// </summary>
            if (Vars.E.IsReady() &&
                Lux.EMissile != null &&
                GameObjects.Player.Spellbook.GetSpell(SpellSlot.E).ToggleState != 1)
            {
                if (PortAIO.OrbwalkerManager.isComboActive)
                {
                    foreach (var target in GameObjects.EnemyHeroes.Where(
                        t =>
                            !Bools.IsImmobile(t) &&
                            !t.HasBuff("luxilluminatingfraulein") &&
                            t.Distance(Lux.EMissile.Position) < Vars.E.Width - 10f))
                    {
                        Vars.E.Cast();
                        break;
                    }
                }

                if (PortAIO.OrbwalkerManager.isLaneClearActive)
                {
                    if (Targets.EMinions.Any() &&
                        Targets.EMinions.Count() >= 3)
                    {
                        Vars.E.Cast();
                    }
                }
            }

            /// <summary>
            ///     The Automatic Q Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                Vars.getCheckBoxItem(Vars.QMenu, "logical"))
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(
                    t =>
                        Bools.IsImmobile(t) &&
                        !Invulnerable.Check(t) &&
                        t.LSIsValidTarget(Vars.Q.Range)))
                {
                    if (!Vars.Q.GetPrediction(target).CollisionObjects.Any())
                    {
                        Vars.Q.Cast(target.ServerPosition);
                    }
                }
            }
        }
    }
}