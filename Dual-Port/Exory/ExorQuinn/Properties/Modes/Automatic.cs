using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.UI;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.Quinn
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
            ///     The Automatic W Logic.
            /// </summary>
            if (Vars.W.IsReady() &&
                Vars.getCheckBoxItem(Vars.WMenu, "logical"))
            {
                if (GameObjects.EnemyHeroes.Any(
                    x =>
                        !x.IsDead &&
                        !x.IsVisible &&
                        Vars.W.Range >
                            x.Distance(GameObjects.Player.ServerPosition)))
                {
                    foreach (var enemy in GameObjects.EnemyHeroes.Where(
                        x =>
                            !x.IsDead &&
                            !x.IsVisible &&
                            Vars.W.Range >
                                x.Distance(GameObjects.Player.ServerPosition)))
                    {
                        Vars.W.Cast();
                    }
                }
                else if (Vars.Locations.Any(h => Vars.W.Range > GameObjects.Player.Distance(h)))
                {
                    Vars.W.Cast();
                }
            }

            /// <summary>
            ///     The Automatic R Logic.
            /// </summary>
            if (Vars.R.IsReady() &&
                GameObjects.Player.InFountain() &&
                Vars.R.Instance.Name.Equals("QuinnR"))
            {
                Vars.R.Cast();
            }
        }
    }
}