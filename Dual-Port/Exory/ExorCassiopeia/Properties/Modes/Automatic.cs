using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using EloBuddy;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy.SDK;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.Cassiopeia
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
            ///     The No AA while in Combo option.
            /// </summary>
            if (Vars.getCheckBoxItem(Vars.MiscMenu, "noaa"))
            {
                PortAIO.OrbwalkerManager.SetAttack(Bools.HasSheenBuff() || GameObjects.Player.ManaPercent < 10 || !PortAIO.OrbwalkerManager.isComboActive || (!Vars.Q.IsReady() && !Vars.W.IsReady() && !Vars.E.IsReady()));
            }

            /// <summary>
            ///     The Tear Stacking Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                Bools.HasTear(GameObjects.Player) &&
                !GameObjects.Player.LSIsRecalling() &&
                PortAIO.OrbwalkerManager.isNoneActive &&
                GameObjects.Player.CountEnemyHeroesInRange(1500) == 0 &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.Q.Slot, Vars.getSliderItem(Vars.MiscMenu, "tear")) &&
                Vars.getSliderItem(Vars.MiscMenu, "tear") != 101)
            {
                Vars.Q.Cast(GameObjects.Player.ServerPosition.LSExtend(Game.CursorPos, Vars.Q.Range-5f));
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
                    Vars.Q.Cast(target.ServerPosition);
                    return;
                }
            }

            /// <summary>
            ///     The Automatic W Logic.
            /// </summary>
            DelayAction.Add(1000, () =>
            {
                if (Vars.W.IsReady() &&
                    !Vars.Q.IsReady() &&
                    Vars.getCheckBoxItem(Vars.WMenu, "logical"))
                {
                    foreach (var target in GameObjects.EnemyHeroes.Where(
                        t =>
                            Bools.IsImmobile(t) &&
                            !Invulnerable.Check(t) &&
                            t.LSIsValidTarget(Vars.W.Range)))
                    {
                        Vars.W.Cast(target.ServerPosition);
                    }
                }
            });
        }
    }
}