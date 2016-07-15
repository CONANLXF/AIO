using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;
using SharpDX;
using EloBuddy;

using TargetSelector = PortAIO.TSManager; namespace AsunaCondemn
{
    /// <summary>
    ///     The main class.
    /// </summary>
    internal class Condem
    {
        /// <summary>
        ///     Called on game load.
        /// </summary>
        public static void OnLoad()
        {
            /// <summary>
            ///     Initialize the menus.
            /// </summary>
            Menus.Initialize();

            /// <summary>
            ///     Initialize the spells.
            /// </summary>
            Spells.Initialize();

            /// <summary>
            ///     Initializes the methods.
            /// </summary>
            Methods.Initialize();

            /// <summary>
            ///     Initializes the drawings.
            /// </summary>
            Drawings.Initialize();
        }

        /// <summary>
        ///     Called when the game updates itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void OnUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }

            if (!Vars.getCheckBoxItem(Vars.Menu, "enable") ||
                !Vars.getKeyBindItem(Vars.Menu, "keybind"))
            {
                return;
            }

            /// <summary>
            ///     The fixed Condem Logic Kappa.
            /// </summary>
            if (Vars.E.IsReady() &&
                Vars.Flash.IsReady() &&
				!GameObjects.Player.IsDashing())
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(
                    t =>
                        !t.IsDashing() &&
                        t.LSIsValidTarget(Vars.E.Range) &&
                        !Invulnerable.Check(t, DamageType.Magical, false) &&
                        !t.LSIsValidTarget(GameObjects.Player.BoundingRadius) &&
                        GameObjects.Player.Distance(GameObjects.Player.ServerPosition.LSExtend(t.ServerPosition, Vars.Flash.Range)) >
                            GameObjects.Player.Distance(t) + t.BoundingRadius &&
                        Vars.getCheckBoxItem(Vars.WhiteListMenu, t.ChampionName.ToLower())))
                {
                    for (var i = 1; i < 10; i++)
                    {
                        if ((target.ServerPosition - Vector3.Normalize(target.ServerPosition - GameObjects.Player.ServerPosition) * (float)(i * 42.5)).LSIsWall() &&
                            (Vars.E.GetPrediction(target).UnitPosition - Vector3.Normalize(target.ServerPosition - GameObjects.Player.ServerPosition) * (float)(i * 42.5)).LSIsWall() &&
                            (target.ServerPosition - Vector3.Normalize(target.ServerPosition - GameObjects.Player.ServerPosition) * i * 44).LSIsWall() &&
                            (Vars.E.GetPrediction(target).UnitPosition - Vector3.Normalize(target.ServerPosition - GameObjects.Player.ServerPosition) * i * 44).LSIsWall())
                        {
                            Vars.E.CastOnUnit(target);
                            Vars.Flash.Cast(GameObjects.Player.ServerPosition.LSExtend(target.ServerPosition, Vars.Flash.Range));
                        }
                    }
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
            if (!Vars.getCheckBoxItem(Vars.Menu, "enable") ||
                !Vars.getKeyBindItem(Vars.Menu, "keybind") ||
				!Vars.getCheckBoxItem(Vars.EMenu, "dashpred"))
            {
                return;
            }

            /// <summary>
            ///     The Dash-Condemn Prediction Logic.
            /// </summary>
            if (Vars.E.IsReady() &&
                Vars.Flash.IsReady() &&
				!GameObjects.Player.IsDashing() &&
                args.Sender.LSIsValidTarget(Vars.E.Range) &&
                !Invulnerable.Check(args.Sender, DamageType.Magical, false) &&
                GameObjects.Player.Distance(args.End) >
					GameObjects.Player.BoundingRadius &&
                Vars.getCheckBoxItem(Vars.WhiteListMenu, args.Sender.ChampionName.ToLower()))
            {
                for (var i = 1; i < 10; i++)
                {
                    if ((args.End - Vector3.Normalize(args.End - GameObjects.Player.ServerPosition) * (float)(i * 42.5)).LSIsWall() &&
                        (args.End - Vector3.Normalize(args.End - GameObjects.Player.ServerPosition) * i * 44).LSIsWall())
                    {
                        Vars.E.CastOnUnit(args.Sender);
                        Vars.Flash.Cast(GameObjects.Player.ServerPosition.LSExtend(args.Sender.ServerPosition, Vars.Flash.Range));
                    }
                }
            }
        }
    }
}