using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using LeagueSharp.SDK.Core.Utils;
using SharpDX;
using EloBuddy;
using EloBuddy.SDK;

 namespace ExorAIO.Champions.Vayne
{
    /// <summary>
    ///     The champion class.
    /// </summary>
    internal class Vayne
    {
        /// <summary>
        ///     Loads Tryndamere.
        /// </summary>
        public void OnLoad()
        {
            /// <summary>
            ///     Initializes the menus.
            /// </summary>
            Menus.Initialize();

            /// <summary>
            ///     Initializes the methods.
            /// </summary>
            Methods.Initialize();

            /// <summary>
            ///     Initializes the drawings.
            /// </summary>
            Drawings.Initialize();

            /// <summary>
            ///     Initializes the prediction drawings.
            /// </summary>
            PredictionDrawings.Initialize();
        }
        
        /// <summary>
        ///     Fired when the game is updated.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void OnUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }

            /// <summary>
            ///     Updates the spells.
            /// </summary>
            Spells.Initialize();

            /// <summary>
            ///     Initializes the Automatic actions.
            /// </summary>
            Logics.Automatic(args);
            /// <summary>
            ///     Initializes the Killsteal events.
            /// </summary>
            Logics.Killsteal(args);

            if (ObjectManager.Player.Spellbook.IsAutoAttacking)
            {
                return;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Logics.Harass(args);
            }
        }

        /// <summary>
        ///     Called on do-cast.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        public static void OnDoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe &&
                AutoAttack.IsAutoAttack(args.SData.Name))
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    Logics.Weaving(sender, args);
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
                {
                    Logics.Clear(sender, args);
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                {
                    Logics.Clear(sender, args);
                    Logics.JungleClear(sender, args);
                    Logics.BuildingClear(sender, args);
                }
            }
        }

        /// <summary>
        ///     Fired on an incoming gapcloser.
        /// </summary>
        /// <param name="sender">The object.</param>
        /// <param name="args">The <see cref="Events.GapCloserEventArgs" /> instance containing the event data.</param>
        public static void OnGapCloser(object sender, Events.GapCloserEventArgs args)
        {
            if (Vars.E.IsReady() &&
                args.Sender.LSIsValidTarget(Vars.E.Range) &&
                !Invulnerable.Check(args.Sender, DamageType.Magical, false))
            {
                /// <summary>
                ///     The Anti-GapCloser E Logic.
                /// </summary>
                if (args.Sender.IsMelee)
                {
                    if (args.IsDirectedToPlayer &&
                        Vars.getCheckBoxItem(Vars.EMenu, "gapcloser"))
                    {
                        Vars.E.CastOnUnit(args.Sender);
                    }
                }

                /// <summary>
                ///     The Dash-Condemn Prediction Logic.
                /// </summary>
                else
                {
                    if (!GameObjects.Player.IsDashing() &&
                        GameObjects.Player.Distance(args.End) >
                            GameObjects.Player.BoundingRadius &&
                        Vars.getCheckBoxItem(Vars.EMenu, "dashpred") &&
                        Vars.getCheckBoxItem(Vars.WhiteListMenu, args.Sender.ChampionName.ToLower()))
                    {
                        for (var i = 1; i < 10; i++)
                        {
                            if ((args.End + Vector3.Normalize(args.End - GameObjects.Player.ServerPosition) * (float)(i * 42.5)).LSIsWall() &&
                                (args.End + Vector3.Normalize(args.End - GameObjects.Player.ServerPosition) * i * 44).LSIsWall())
                            {
                                Console.WriteLine("DASHPREDICTION CONDEMN!!1!11");
                                Vars.E.CastOnUnit(args.Sender);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Called on interruptable spell.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Events.InterruptableTargetEventArgs" /> instance containing the event data.</param>
        public static void OnInterruptableTarget(object sender, Events.InterruptableTargetEventArgs args)
        {
            if (Vars.E.IsReady() &&
                args.Sender.LSIsValidTarget(Vars.E.Range) &&
                !Invulnerable.Check(args.Sender, DamageType.Magical, false) &&
                Vars.getCheckBoxItem(Vars.EMenu, "interrupter"))
            {
                Vars.E.CastOnUnit(args.Sender);
            }
        }

        public static void Orbwalker_OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {

            /// <summary>
            ///     The Automatic Stealth Logics.
            /// </summary>
            if (!GameObjects.Player.IsUnderEnemyTurret() &&
                GameObjects.Player.HasBuff("vaynetumblefade"))
            {
                /// <summary>
                ///     The Automatic Stealth Logic.
                /// </summary>
                if (GameObjects.Player.GetBuff("vaynetumblefade").EndTime - Game.Time >
                    GameObjects.Player.GetBuff("vaynetumblefade").EndTime - GameObjects.Player.GetBuff("vaynetumblefade").StartTime -
                    Vars.getSliderItem(Vars.MiscMenu, "stealthtime") / 1000)
                {
                    args.Process = false;
                }
                /// <summary>
                ///     The Automatic Stealth Logic.
                /// </summary>
                else if (GameObjects.Player.HasBuff("summonerexhaust") ||
                    GameObjects.Player.HasBuffOfType(BuffType.Blind))
                {
                    args.Process = false;
                }
            }


            /// <summary>
            ///     The Target Forcing Logic (W Stacks).
            /// </summary>
            if (args.Target is AIHeroClient)
            {
                if (!GameObjects.EnemyHeroes.Any(
                    t =>
                        t.LSIsValidTarget(Vars.AARange) &&
                        t.GetBuffCount("vaynesilvereddebuff") == 2))
                {
                    Orbwalker.ForcedTarget =(null);
                    return;
                }

                Orbwalker.ForcedTarget =(GameObjects.EnemyHeroes.FirstOrDefault(
                    t =>
                        t.LSIsValidTarget(Vars.AARange) &&
                        t.GetBuffCount("vaynesilvereddebuff") == 2));
            }
        }
    }
}