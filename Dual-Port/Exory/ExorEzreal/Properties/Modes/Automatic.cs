using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.SDK.Core.Utils;

 namespace ExorAIO.Champions.Ezreal
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
            ///     The Q LastHit Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.Q.Slot, Vars.getSliderItem(Vars.QMenu, "farmhelper")) &&
                Vars.getSliderItem(Vars.QMenu, "farmhelper") != 101)
            {
                foreach (var minion in Targets.Minions.Where(
                    m =>
                        !m.LSIsValidTarget(Vars.AARange) &&
                        Vars.GetRealHealth(m) >
                            GameObjects.Player.GetAutoAttackDamage(m) &&
                        Vars.GetRealHealth(m) <
                            (float)GameObjects.Player.LSGetSpellDamage(m, SpellSlot.Q)).OrderBy(
                                o =>
                                    o.MaxHealth))
                {
                    if (!Vars.Q.GetPrediction(minion).CollisionObjects.Any())
                    {
                        Vars.Q.Cast(Vars.Q.GetPrediction(minion).UnitPosition);
                    }
                }
            }

            /// <summary>
            ///     The Tear Stacking Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                !Targets.Minions.Any() &&
                Bools.HasTear(GameObjects.Player) &&
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None) &&
                GameObjects.Player.CountEnemyHeroesInRange(1500) == 0 &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.Q.Slot, Vars.getSliderItem(Vars.MiscMenu, "tear")) &&
                Vars.getSliderItem(Vars.MiscMenu, "tear") != 101)
            {
                Vars.Q.Cast(Game.CursorPos);
            }

            if (GameObjects.Player.TotalAttackDamage < GameObjects.Player.TotalMagicalDamage)
            {
                return;
            }

            /// <summary>
            ///     Initializes the orbwalkingmodes.
            /// </summary>
            /// 

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                if (!(Orbwalker.LastTarget is AIHeroClient))
                {
                    return;
                }
            }

            else if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                if (!(Orbwalker.LastTarget is Obj_HQ) &&
                    !(Orbwalker.LastTarget is Obj_AI_Turret) &&
                    !(Orbwalker.LastTarget is Obj_BarracksDampener))
                {
                    return;
                }
            }

            else
            {
                if (!GameObjects.Jungle.Contains(Orbwalker.LastTarget) &&
                    !(Orbwalker.LastTarget is Obj_HQ) &&
                    !(Orbwalker.LastTarget is AIHeroClient) &&
                    !(Orbwalker.LastTarget is Obj_AI_Turret) &&
                    !(Orbwalker.LastTarget is Obj_BarracksDampener))
                {
                    return;
                }
            }

            /// <summary>
            ///     The Automatic W Logic.
            /// </summary>
            if (Vars.W.IsReady() &&
                ObjectManager.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.W.Slot, Vars.getSliderItem(Vars.WMenu, "logical")) &&
                Vars.getSliderItem(Vars.WMenu, "logical") != 101)
            {
                foreach (var target in GameObjects.AllyHeroes.Where(
                    t =>
                        !t.IsMe &&
                        t.Spellbook.IsAutoAttacking &&
                        t.LSIsValidTarget(Vars.W.Range, false)))
                {
                    Vars.W.Cast(Vars.W.GetPrediction(target).UnitPosition);
                }
            }
        }

        /// <summary>
        ///     Called while processing Spellcasting operations.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="GameObjectProcessSpellCastEventArgs" /> instance containing the event data.</param>
        public static void AutoE(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe ||
                Invulnerable.Check(GameObjects.Player, DamageType.True, false))
            {
                return;
            }

            if (args.Target == null ||
                !sender.IsValidTarget())
            {
                return;
            }

            if (sender.IsEnemy &&
                sender is AIHeroClient)
            {
                /// <summary>
                ///     Block Gangplank's Barrels.
                /// </summary>
                if ((sender as AIHeroClient).ChampionName.Equals("Gangplank"))
                {
                    if (AutoAttack.IsAutoAttack(args.SData.Name) ||
                        args.SData.Name.Equals("GangplankQProceed"))
                    {
                        if ((args.Target as Obj_AI_Minion).Health == 1 &&
                            (args.Target as Obj_AI_Minion).CharData.BaseSkinName.Equals("gangplankbarrel"))
                        {
                            if (GameObjects.Player.Distance(args.Target) < 450)
                            {
                                Vars.E.Cast();
                            }
                        }
                    }
                    else if (args.SData.Name.Equals("GangplankEBarrelFuseMissile"))
                    {
                        if (GameObjects.Player.Distance(args.End) < 450)
                        {
                            Vars.E.Cast();
                        }
                    }
                }

                if (!args.Target.IsMe)
                {
                    return;
                }

                if (args.SData.Name.Contains("Summoner") ||
                    args.SData.Name.Equals("HextechGunblade") ||
                    args.SData.Name.Equals("BilgewaterCutlass") ||
                    args.SData.Name.Equals("ItemSwordOfFeastAndFamine"))
                {
                    return;
                }

                switch (args.SData.TargettingType)
                {
                    /// <summary>
                    ///     Special check for the AutoAttacks.
                    /// </summary>
                    case SpellDataTargetType.Unit:
                    case SpellDataTargetType.Self:
                    case SpellDataTargetType.LocationAoe:

                        if (args.SData.Name.Equals("NasusE") ||
                            args.SData.Name.Equals("GangplankE") ||
                            args.SData.Name.Equals("TrundleCircle") ||
                            args.SData.Name.Equals("TormentedSoil") ||
                            args.SData.Name.Equals("SwainDecrepify") ||
                            args.SData.Name.Equals("MissFortuneScattershot") ||
                            args.SData.Name.Equals("OrianaDissonanceCommand"))
                        {
                            return;
                        }

                        if (AutoAttack.IsAutoAttack(args.SData.Name))
                        {
                            if ((!sender.IsMelee && args.SData.Name.Contains("Card")) ||
                                sender.Buffs.Any(b => AutoAttack.IsAutoAttackReset(args.SData.Name)))
                            {
                                Vars.E.Cast();
                            }

                            return;
                        }

                        switch (sender.CharData.BaseSkinName)
                        {
                            case "Zed":
                                DelayAction.Add(200, () => { Vars.E.Cast(Game.CursorPos); });
                                break;

                            default:
                                Vars.E.Cast(Game.CursorPos);
                                break;
                        }
                        break;

                    default:
                        break;
                }
            }
        }
    }
}