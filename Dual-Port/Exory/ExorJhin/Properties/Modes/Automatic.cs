using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;
using LeagueSharp.SDK.Enumerations;
using SharpDX;
using EloBuddy;
using EloBuddy.SDK;

namespace ExorAIO.Champions.Jhin
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
            ///     The R Automatic Logic.
            /// </summary>
            if (Vars.R.IsReady() &&
                Vars.R.Instance.Name.Equals("JhinRShot") &&
                Vars.getCheckBoxItem(Vars.RMenu, "logical"))
            {
                if (GameObjects.EnemyHeroes.Any(
                    t =>
                        t.LSIsValidTarget(Vars.R.Range) &&
                        !Vars.Cone.IsOutside((Vector2)t.ServerPosition)))
                {
                    foreach (var target in GameObjects.EnemyHeroes.Where(
                        t =>
                            t.LSIsValidTarget(Vars.R.Range) &&
                            !Vars.Cone.IsOutside((Vector2)t.ServerPosition)))
                    {
                        if (Vars.getCheckBoxItem(Vars.RMenu, "nearmouse"))
                        {
                            Vars.R.Cast(Vars.R.GetPrediction(GameObjects.EnemyHeroes.Where(
                                t =>
                                    t.LSIsValidTarget(Vars.R.Range) &&
                                    !Vars.Cone.IsOutside((Vector2)t.ServerPosition)).OrderBy(
                                        o =>
                                            o.Distance(Game.CursorPos)).First()).UnitPosition);
                            return;
                        }

                        Vars.R.Cast(Vars.R.GetPrediction(target).UnitPosition);
                        return;
                    }
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    Vars.R.Cast(Game.CursorPos);
                }
            }

            if (Vars.R.Instance.Name.Equals("JhinRShot"))
            {
                return;
            }

            /// <summary>
            ///     The Automatic Q LastHit Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                GameObjects.Player.HasBuff("JhinPassiveReload") &&
                !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.Q.Slot, Vars.getSliderItem(Vars.QMenu, "lasthit")) &&
                Vars.getSliderItem(Vars.QMenu, "lasthit") != 101)
            {
                foreach (var minion in Targets.Minions.Where(
                    m =>
                        m.LSIsValidTarget(Vars.Q.Range) &&
                        Vars.GetRealHealth(m) <
                            (float)GameObjects.Player.GetSpellDamage(m, SpellSlot.Q)))
                {
                    Vars.Q.CastOnUnit(minion);
                }
            }

            /// <summary>
            ///     The Automatic E Logic.
            /// </summary>
            if (Vars.E.IsReady() &&
                Vars.getCheckBoxItem(Vars.EMenu, "logical"))
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(
                    t =>
                        Bools.IsImmobile(t) &&
                        !Invulnerable.Check(t) &&
                        t.LSIsValidTarget(Vars.E.Range)))
                {
                    Vars.E.Cast(GameObjects.Player.ServerPosition.Extend(
                        target.ServerPosition,
                        GameObjects.Player.Distance(target) + target.BoundingRadius * 2));
                }
            }

            /// <summary>
            ///     The Automatic W Logic.
            /// </summary>
            if (Vars.W.IsReady() &&
                !GameObjects.Player.IsUnderEnemyTurret() &&
                Vars.getCheckBoxItem(Vars.WMenu, "logical"))
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(
                    t =>
                        !Invulnerable.Check(t) &&
                        Bools.IsImmobile(t) &&
                        t.HasBuff("jhinespotteddebuff") &&
                        t.LSIsValidTarget(Vars.W.Range - 150f) &&
                        Vars.getCheckBoxItem(Vars.WhiteListMenu, t.ChampionName.ToLower())))
                {
                    Vars.W.Cast(target.ServerPosition);
                }
            }
        }
    }
}