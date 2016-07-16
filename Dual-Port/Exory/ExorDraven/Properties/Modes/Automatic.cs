using System;
using ExorAIO.Utilities;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using EloBuddy.SDK;
using System.Linq;
using LeagueSharp.SDK.Core.Utils;

namespace ExorAIO.Champions.Draven
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
            ///     The Automatic Q Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None) &&
                GameObjects.Player.GetBuffCount("dravenspinningattack") < 1 &&
                Vars.getCheckBoxItem(Vars.QMenu, "logical"))
            {
                Vars.Q.Cast();
            }

            /// <summary>
            ///     The Semi-Automatic R Management.
            /// </summary>
            if (Vars.R.IsReady() &&
                Vars.getCheckBoxItem(Vars.RMenu, "bool") &&
                Vars.getKeyBindItem(Vars.RMenu, "key"))
            {
                if (!GameObjects.EnemyHeroes.Any(
                    t =>
                        !Invulnerable.Check(t) &&
                        t.IsValidTarget(Vars.R.Range) &&
                        Vars.getCheckBoxItem(Vars.WhiteListMenu, Targets.Target.ChampionName.ToLower())))
                {
                    return;
                }

                Vars.R.CastOnUnit(
                    GameObjects.EnemyHeroes.Where(
                        t =>
                            !Invulnerable.Check(t) &&
                            t.IsValidTarget(Vars.R.Range) &&
                            Vars.getCheckBoxItem(Vars.WhiteListMenu, Targets.Target.ChampionName.ToLower())).OrderBy(o => o.Health).First());
            }
        }
    }
}