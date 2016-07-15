using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using LeagueSharp.SDK.Core.Utils;
using LeagueSharp.Data.Enumerations;
using EloBuddy;
using EloBuddy.SDK;

 namespace ExorAIO.Champions.Kalista
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
            /// <summary>
            ///     The Soulbound declaration.
            /// </summary>
            if (Vars.SoulBound == null)
            {
                Vars.SoulBound = GameObjects.AllyHeroes.Find(
                    a =>
                        a.Buffs.Any(
                            b =>
                                b.Caster.IsMe &&
                                b.Name.Contains("kalistacoopstrikeally")));
            }
            else
            {
                /// <summary>
                ///     The Automatic R Logic.
                /// </summary>
                if (Vars.R.IsReady() &&
                    Vars.SoulBound.HealthPercent < 10 &&
                    Vars.SoulBound.CountEnemyHeroesInRange(800f) > 0 &&
                    Vars.SoulBound.LSIsValidTarget(Vars.R.Range, false) &&
                    Vars.getCheckBoxItem(Vars.RMenu, "lifesaver"))
                {
                    Vars.R.Cast();
                }
            }

            /// <summary>
            ///     The Automatic W Logic.
            /// </summary>
            if (Vars.W.IsReady() &&
                !GameObjects.Player.LSIsRecalling() &&
                !GameObjects.Player.IsUnderEnemyTurret() &&
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None) &&
                GameObjects.Player.CountEnemyHeroesInRange(1500f) == 0 &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.W.Slot, Vars.getSliderItem(Vars.WMenu, "logical")) &&
                Vars.getSliderItem(Vars.WMenu, "logical") != 101)
            {
                foreach (var loc in Vars.Locations.Where(
                    l =>
                        GameObjects.Player.Distance(l) < Vars.W.Range &&
                        !ObjectManager.Get<Obj_AI_Minion>().Any(
                            m =>
                                m.Distance(l) < 1000f &&
                                m.CharData.BaseSkinName.Equals("kalistaspawn"))))
                {
                    Vars.W.Cast(loc);
                }
            }

            /// <summary>
            ///     The Automatic E Logics.
            /// </summary>
            if (Vars.E.IsReady())
            {
                /// <summary>
                ///     The E Before death Logic.
                /// </summary>
                if (Health.GetPrediction(GameObjects.Player, (int)(1000 + Game.Ping/2f)) <= 0 &&
                    Vars.getCheckBoxItem(Vars.EMenu, "ondeath"))
                {
                    Vars.E.Cast();
                }

                /// <summary>
                ///     The E Minion Harass Logic.
                /// </summary>
                if (GameObjects.EnemyHeroes.Any(t => Bools.IsPerfectRendTarget(t)) &&
                    Vars.getSliderItem(Vars.EMenu, "harass") != 101 &&
                    Targets.Minions.Any(
						m =>
							Bools.IsPerfectRendTarget(m) &&
							Vars.GetRealHealth(m) <= EDamage(m)))
                {
                    /// <summary>
                    ///     Check for Mana Manager if not in combo mode and the killable minion is only one, else do not use it.
                    /// </summary>
                    if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) &&
                        Targets.Minions.Count(
                            m =>
                                Bools.IsPerfectRendTarget(m) &&
                                Vars.GetRealHealth(m) <
                                    EDamage(m)) == 1)
                    {
                        if (GameObjects.Player.ManaPercent <
                                ManaManager.GetNeededMana(Vars.E.Slot, Vars.getSliderItem(Vars.EMenu, "harass")))
                        {
                            return;
                        }
                    }

                    /// <summary>
                    ///     Check for E Whitelist if the harassable target is only one and there is only one killable minion, else do not use the whitelist.
                    /// </summary>
                    if (GameObjects.EnemyHeroes.Count(t => Bools.IsPerfectRendTarget(t)) == 1 && Targets.Minions.Count(m => Bools.IsPerfectRendTarget(m) && Vars.GetRealHealth(m) < EDamage(m)) == 1)
                    {
                        if (!Vars.getCheckBoxItem(Vars.WhiteListMenu, GameObjects.EnemyHeroes.FirstOrDefault( t => Bools.IsPerfectRendTarget(t)).ChampionName.ToLower()))
                        {
                            return;
                        }
                    }

                    /// <summary>
                    ///     Check for invulnerability through all the harassable targets.
                    /// </summary>
                    foreach (var target in GameObjects.EnemyHeroes.Where(t => Bools.IsPerfectRendTarget(t)))
                    {
                        if (Invulnerable.Check(target, DamageType.True, false))
                        {
                            return;
                        }
                    }

                    Vars.E.Cast();
                }

                /// <summary>
                ///     The E JungleClear Logic.
                /// </summary>
                if (Vars.getCheckBoxItem(Vars.EMenu, "junglesteal"))
                {
                    foreach (var minion in Targets.JungleMinions.Where(
                        m =>
                            Bools.IsPerfectRendTarget(m) &&
                            m.Health < EDamage(m)))
                    {
                        Vars.E.Cast();
                    }
                }
            }
        }
    }
}