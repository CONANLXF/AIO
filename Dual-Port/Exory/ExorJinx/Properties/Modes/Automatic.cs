using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy;
using EloBuddy.SDK;

 namespace ExorAIO.Champions.Jinx
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
            ///     The Q Switching Logics.
            /// </summary>
            if (Vars.Q.IsReady())
            {
                /// <summary>
                ///     PowPow.Range -> FishBones Logics.
                /// </summary>
                if (!GameObjects.Player.HasBuff("JinxQ"))
                {
                    /// <summary>
                    ///     The Q Combo Enable Logics,
                    ///     The Q Harass Enable Logics.
                    /// </summary>
                    if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                    {
                        /// <summary>
                        ///     Enable if:
                        ///     If you are in combo mode, the combo option is enabled. (Option check).
                        /// </summary>
                        if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                        {
                            if (!Vars.getCheckBoxItem(Vars.QMenu, "combo"))
                            {
                                Console.WriteLine("ExorAIO: Jinx - Combo - Option Block.");
                                return;
                            }
                        }

                        /// <summary>
                        ///     Enable if:
                        ///     If you are in mixed mode, the mixed option is enabled.. (Option check).
                        ///     and respects the ManaManager check. (Mana check).
                        /// </summary>
                        else if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                        {
                            if (GameObjects.Player.ManaPercent < ManaManager.GetNeededMana(Vars.W.Slot, Vars.getSliderItem(Vars.QMenu, "harass")))
                            {
                                Console.WriteLine("ExorAIO: Jinx - Hybrid - ManaManager or Option Block.");
                                return;
                            }
                        }

                        /// <summary>
                        ///     Enable if:
                        ///     No hero in PowPow Range but 1 or more heroes in FishBones range. (Range Logic).
                        /// </summary>
                        if (!GameObjects.EnemyHeroes.Any(t => t.LSIsValidTarget(Vars.PowPow.Range)) &&
                            GameObjects.EnemyHeroes.Any(t2 => t2.LSIsValidTarget(Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)
                                ? Vars.Q.Range
                                : Vars.W.Range)))
                        {
                            Console.WriteLine("ExorAIO: Jinx - Combo/Hybrid - Enabled for Range Check.");
                            Vars.Q.Cast();
                            return;
                        }
                    }

                    if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                    {

                        /// <summary>
                        ///     Start if:
                        ///     It respects the ManaManager Check, (Mana check),
                        ///     The Clear Option is enabled. (Option check).
                        /// </summary>
                        if (GameObjects.Player.ManaPercent <
                                ManaManager.GetNeededMana(Vars.W.Slot, Vars.getSliderItem(Vars.QMenu, "clear")))
                        {
                            Console.WriteLine("ExorAIO: Jinx - Clear - ManaManager or Option Block.");
                            return;
                        }

                        /// <summary>
                        ///     The LaneClear Logics.
                        /// </summary>
                        if (Targets.Minions.Any(m => Vars.GetRealHealth(m) < GameObjects.Player.GetAutoAttackDamage(m) * 1.1))
                        {
                            /*
                            /// <summary>
                            ///     Enable if:
                            ///     No minion in PowPow Range and at least 1 minion in Fishbones Range. (Lane Range Logic).
                            /// </summary>
                            if (!Targets.Minions.Any(m => m.LSIsValidTarget(Vars.PowPow.Range)))
                            {
                                Vars.Q.Cast();
                                Console.WriteLine("ExorAIO: Jinx - LaneClear - Enabled for Range Check.");
                                return;
                            }
                            */

                            /// <summary>
                            ///     Disable if:
                            ///     The player has Runaan's Hurricane and there are more than 1 hittable Minions..
                            ///     And there more than 2 killable minions in Q explosion range (Lane AoE Logic).
                            /// </summary>
                            if ((Items.HasItem(3085) && Targets.Minions.Count() > 1) ||
                                Targets.Minions.Where(
                                m =>
                                    Vars.GetRealHealth(m) <
                                        GameObjects.Player.GetAutoAttackDamage(m) * 1.1).Count(
                                            m2 =>
                                                m2.Distance(Targets.Minions.First(m => Vars.GetRealHealth(m) < GameObjects.Player.GetAutoAttackDamage(m) * 1.1)) < 250f) >= 3)
                            {
                                Vars.Q.Cast();
                                Console.WriteLine("ExorAIO: Jinx - LaneClear - Enabled for AoE Check.");
                                return;
                            }
                        }

                        /// <summary>
                        ///     The JungleClear Logics.
                        /// </summary>
                        else if (Targets.JungleMinions.Any())
                        {
                            /// <summary>
                            ///     Enable if:
                            ///     No monster in PowPow Range and at least 1 monster in Fishbones Range.. (Jungle Range Logic).
                            /// </summary>
                            if (!Targets.JungleMinions.Any(m => m.IsValidTarget(Vars.PowPow.Range)))
                            {
                                Vars.Q.Cast();
                                Console.WriteLine("ExorAIO: Jinx - JungleClear - Enabled for Range Check.");
                                return;
                            }

                            /// <summary>
                            ///     Enable if:
                            ///     More or equal than 1 monster in explosion range from the target monster. (Lane AoE Logic).
                            /// </summary>
                            else if (Targets.JungleMinions.Count(m2 => m2.Distance(Targets.JungleMinions[0]) < 250f) >= 2)
                            {
                                Vars.Q.Cast();
                                Console.WriteLine("ExorAIO: Jinx - JungleClear - Enabled for AoE Check.");
                                return;
                            }
                        }
                    }

                    if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
                    {

                        /// <summary>
                        ///     Start if:
                        ///     It respects the ManaManager Check, (Mana check).
                        ///     The LastHit Option is enabled. (Option check).
                        /// </summary>
                        if (GameObjects.Player.ManaPercent <
                                ManaManager.GetNeededMana(Vars.W.Slot, Vars.getSliderItem(Vars.QMenu, "lasthit")))
                        {
                            Console.WriteLine("ExorAIO: Jinx - LastHit - ManaManager or Option Block.");
                            return;
                        }

                        /// <summary>
                        ///     Enable if:
                        ///     Any killable minion in FishBones Range and no killable minions in PowPow Range. (LastHit Range Logic).
                        /// </summary>
                        if (Targets.Minions.Any(
                            m =>
                                !m.LSIsValidTarget(Vars.PowPow.Range) &&
                                m.Health < GameObjects.Player.GetAutoAttackDamage(m) * 1.1))
                        {
                            if (!Targets.Minions.Any(
                                m =>
                                    m.LSIsValidTarget(Vars.PowPow.Range) &&
                                    m.Health < GameObjects.Player.GetAutoAttackDamage(m)))
                            {
                                Vars.Q.Cast();
                                Console.WriteLine("ExorAIO: Jinx - LastHit - Enabled.");
                                return;
                            }
                        }
                    }
                }

                /// <summary>
                ///     FishBones -> PowPow.Range Logics.
                /// </summary>
                else
                {
                    if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                    {

                        /// <summary>
                        ///     Disable if:
                        ///     If you are in combo mode, the combo option is disabled. (Option check).
                        /// </summary>
                        if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                        {
                            if (!Vars.getCheckBoxItem(Vars.QMenu, "combo"))
                            {
                                Vars.Q.Cast();
                                Console.WriteLine("ExorAIO: Jinx - Combo - Option Disable.");
                                return;
                            }
                        }

                        /// <summary>
                        ///     Disable if:
                        ///     If you are in mixed mode, the mixed option is disabled.. (Option check).
                        ///     or it doesn't respect the ManaManager check. (Mana check).
                        /// </summary>
                        else if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                        {
                            if (GameObjects.Player.ManaPercent <
                                    ManaManager.GetNeededMana(Vars.W.Slot, Vars.getSliderItem(Vars.QMenu, "harass")))
                            {
                                Vars.Q.Cast();
                                Console.WriteLine("ExorAIO: Jinx - Mixed - ManaManager or Option Disable.");
                                return;
                            }
                        }

                        /// <summary>
                        ///     Disable if:
                        ///     The target is not a hero. (Target check),
                        /// </summary>
                        if (Orbwalker.LastTarget as AIHeroClient != null &&
                            (Orbwalker.LastTarget as AIHeroClient).LSIsValidTarget())
                        {
                            /// <summary>
                            ///     Disable if:
                            ///     No enemies in explosion range from the target. (AOE Logic),
                            ///     Any hero in PowPow Range. (Range Logic).
                            /// </summary>
                            if (GameObjects.EnemyHeroes.Any(t => t.LSIsValidTarget(Vars.PowPow.Range)) &&
                                (Orbwalker.LastTarget as AIHeroClient).CountEnemyHeroesInRange(200f) < 2)
                            {
                                Vars.Q.Cast();
                                Console.WriteLine("ExorAIO: Jinx - Combo/Hybrid - Disabled.");
                                return;
                            }
                        }
                    }

                    else if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                    {

                        /// <summary>
                        ///     Disable if:
                        ///     Doesn't respect the ManaManager Check, (Mana check).
                        ///     The Clear Option is disabled. (Option check).
                        /// </summary>
                        if (GameObjects.Player.ManaPercent <
                                ManaManager.GetNeededMana(Vars.W.Slot, Vars.getSliderItem(Vars.QMenu, "clear")))
                        {
                            Vars.Q.Cast();
                            Console.WriteLine("ExorAIO: Jinx - Clear - ManaManager or Option Disable.");
                            return;
                        }

                        /// <summary>
                        ///     Disable if:
                        ///     There is at least 1 minion in PowPow Range.. (Lane Range Logic).
                        ///     .. And less than 2 minions in explosion range from the minion target (Lane AoE Logic).
                        /// </summary>
                        if ((!Items.HasItem(3085) || Targets.Minions.Count() < 2) &&
                            (!Targets.Minions.Any(m => Vars.GetRealHealth(m) < GameObjects.Player.GetAutoAttackDamage(m) * 1.1) ||
                            Targets.Minions.Count(m2 => m2.Distance(Targets.Minions.First(m => Vars.GetRealHealth(m) < GameObjects.Player.GetAutoAttackDamage(m) * 1.1)) < 250f) < 3))
                        {
                            Vars.Q.Cast();
                            Console.WriteLine("ExorAIO: Jinx - LaneClear - Disabled.");
                            return;
                        }

                        /// <summary>
                        ///     Disable if:
                        ///     There is at least 1 monster in PowPow Range.. (Jungle Range Logic).
                        ///     .. And less than 1 monster in explosion range from the monster target (Jungle AoE Logic).
                        /// </summary>
                        else if (Targets.JungleMinions.Any(m => m.IsValidTarget(Vars.PowPow.Range)) &&
                            Targets.JungleMinions.Count(m2 => m2.Distance(Targets.JungleMinions[0]) < 250f) < 2)
                        {
                            Vars.Q.Cast();
                            Console.WriteLine("ExorAIO: Jinx - JungleClear - Disabled.");
                            return;
                        }
                    }

                    else if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
                    {
                        /// <summary>
                        ///     Disable if:
                        ///     Doesn't respect the ManaManager Check, (Mana check).
                        ///     The LastHit Option is disabled. (Option check).
                        /// </summary>
                        if (GameObjects.Player.ManaPercent <
                                ManaManager.GetNeededMana(Vars.W.Slot, Vars.getSliderItem(Vars.QMenu, "lasthit")))
                        {
                            Vars.Q.Cast();
                            Console.WriteLine("ExorAIO: Jinx - LastHit - ManaManager or Option Disable.");
                            return;
                        }

                        /// <summary>
                        ///     Disable if:
                        ///     No killable minion in FishBones Range. (LastHit Range Logic).
                        /// </summary>
                        if (!Targets.Minions.Any(
                            m =>
                                !m.LSIsValidTarget(Vars.PowPow.Range) &&
                                m.Health < GameObjects.Player.GetAutoAttackDamage(m) * 1.1))
                        {
                            Vars.Q.Cast();
                            Console.WriteLine("ExorAIO: Jinx - LastHit - Range Killable Disable.");
                            return;
                        }
                        else if (Targets.Minions.Any(
                            m =>
                                m.LSIsValidTarget(Vars.PowPow.Range) &&
                                m.Health < GameObjects.Player.GetAutoAttackDamage(m)))
                        {
                            Vars.Q.Cast();
                            Console.WriteLine("ExorAIO: Jinx - LastHit - Normally Killable Disable.");
                            return;
                        }
                    }
                    else
                    {
                        Vars.Q.Cast();
                        Console.WriteLine("ExorAIO: Jinx - General - Disabled.");
                    }
                }
            }

            /// <summary>
            ///     The Automatic W Logic.
            /// </summary>
            if (Vars.W.IsReady() &&
                !GameObjects.Player.IsUnderEnemyTurret() &&
                GameObjects.Player.CountEnemyHeroesInRange(Vars.Q.Range) < 3 &&
                Vars.getCheckBoxItem(Vars.WMenu, "logical"))
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(
                    t =>
                        Bools.IsImmobile(t) &&
                        !Invulnerable.Check(t) &&
                        t.LSIsValidTarget(Vars.W.Range)))
                {
                    if (!Vars.W.GetPrediction(Targets.Target).CollisionObjects.Any(c => Targets.Minions.Contains(c)))
                    {
                        Vars.W.Cast(target.ServerPosition);
                        return;
                    }
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
                        t.LSIsValidTarget(Vars.E.Range) &&
                        !Invulnerable.Check(t, DamageType.Magical, false)))
                {
                    Vars.E.Cast(target.ServerPosition);
                }
            }
        }

        /// <summary>
        ///     Called upon calling a spelaneclearlearast.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="SpellbookCastSpellEventArgs" /> instance containing the event data.</param>
        public static void Automatic(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            /// <summary>
            ///     The Q Switching Logics.
            /// </summary>
            if (Vars.Q.IsReady() &&
                Vars.getCheckBoxItem(Vars.MiscMenu, "blockq"))
            {
                if (GameObjects.Player.HasBuff("JinxQ"))
                {
                    return;
                }

                /// <summary>
                ///     Block if:
                ///     It doesn't respect the ManaManager Check, (Mana check),
                /// </summary>
                if (GameObjects.Player.ManaPercent <
                        ManaManager.GetNeededMana(Vars.W.Slot, Vars.getSliderItem(Vars.QMenu, "clear")))
                {
                    args.Process = false;
                }
            }
        }
    }
}