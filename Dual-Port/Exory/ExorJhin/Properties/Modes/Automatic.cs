using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using EloBuddy;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy.SDK;
using SharpDX;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.Jhin
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
                        t.IsValidTarget(Vars.R.Range) &&
                        !Vars.Cone.IsOutside((Vector2)t.ServerPosition)))
                {
                    foreach (var target in GameObjects.EnemyHeroes.Where(
                        t =>
                            t.IsValidTarget(Vars.R.Range) &&
                            !Vars.Cone.IsOutside((Vector2)t.ServerPosition)))
                    {
                        if (Vars.getCheckBoxItem(Vars.RMenu, "nearmouse"))
                        {
                            Vars.R.Cast(Vars.R.GetPrediction(GameObjects.EnemyHeroes.Where(
                                t =>
                                    t.IsValidTarget(Vars.R.Range) &&
                                    !Vars.Cone.IsOutside((Vector2)t.ServerPosition)).OrderBy(
                                        o =>
                                            o.Distance(Game.CursorPos)).First()).UnitPosition);
                            return;
                        }

                        Vars.R.Cast(Vars.R.GetPrediction(target).UnitPosition);
                        return;
                    }
                }

                if (PortAIO.OrbwalkerManager.isComboActive)
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
                !PortAIO.OrbwalkerManager.isComboActive &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.Q.Slot, Vars.getSliderItem(Vars.QMenu, "lasthit")) &&
                Vars.getSliderItem(Vars.QMenu, "lasthit") != 101 && !Vars.R.Instance.Name.Equals("JhinRShot"))
            {
                foreach (var minion in Targets.Minions.Where(
                    m =>
                        m.LSIsValidTarget(Vars.Q.Range) &&
                        Vars.GetRealHealth(m) <
                            (float)GameObjects.Player.LSGetSpellDamage(m, SpellSlot.Q)))
                {
                    Vars.Q.CastOnUnit(minion);
                }
            }

            /// <summary>
            ///     The Automatic W Logic.
            /// </summary>
            if (Vars.W.IsReady() &&
                !GameObjects.Player.IsUnderEnemyTurret() &&
                Vars.getCheckBoxItem(Vars.WMenu, "logical") && !Vars.R.Instance.Name.Equals("JhinRShot"))
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(
                    t =>
                        !Invulnerable.Check(t) &&
                        t.HasBuff("jhinespotteddebuff") &&
                        t.IsValidTarget(Vars.W.Range - 150f) &&
                        !t.IsValidTarget(Vars.AARange + 50f) &&
                        Vars.getCheckBoxItem(Vars.WhiteListMenu, t.ChampionName.ToLower())))
                {
                    if (!Vars.W.GetPrediction(target).CollisionObjects.Any(c => !c.HasBuff("jhinespotteddebuff")))
                    {
                        if (Bools.IsImmobile(target))
                        {
                            if (Vars.E.IsReady() &&
                                target.LSIsValidTarget(Vars.E.Range))
                            {
                                Vars.E.Cast(target.ServerPosition);
                            }
                            Vars.W.Cast(target.ServerPosition);
                            return;
                        }
                        if (Vars.E.IsReady() &&
                                target.LSIsValidTarget(Vars.E.Range))
                        {
                            Vars.E.Cast(Vars.E.GetPrediction(target).UnitPosition);
                        }
                        Vars.W.Cast(Vars.W.GetPrediction(target).UnitPosition);
                    }
                }
            }

            /// <summary>
            ///     The Automatic E Logic.
            /// </summary>
            if (Vars.E.IsReady() &&
                Vars.getCheckBoxItem(Vars.EMenu, "logical") && !Vars.R.Instance.Name.Equals("JhinRShot"))
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(
                    t =>
                        Bools.IsImmobile(t) &&
                        !Invulnerable.Check(t) &&
                        t.LSIsValidTarget(Vars.E.Range)))
                {
                    Vars.E.Cast(GameObjects.Player.ServerPosition.LSExtend(
                        target.ServerPosition,
                        GameObjects.Player.Distance(target) + target.BoundingRadius*2));
                }
            }
        }
    }
}