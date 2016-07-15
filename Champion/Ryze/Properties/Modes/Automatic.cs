using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using EloBuddy;
using EloBuddy.SDK;

 namespace ExorAIO.Champions.Ryze
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
            ///     The Stacking Logics.
            /// </summary>
            if (Vars.Q.IsReady())
            {
                /// <summary>
                ///     The Tear Stacking Logic.
                /// </summary>
                if (!Targets.Minions.Any() &&
                    Bools.HasTear(GameObjects.Player) &&
                    GameObjects.Player.CountEnemyHeroesInRange(1500) == 0 &&
                    Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None) &&
                    GameObjects.Player.ManaPercent >
                        ManaManager.GetNeededMana(Vars.Q.Slot, Vars.getSliderItem(Vars.MiscMenu, "tear")) &&
                    Vars.getSliderItem(Vars.MiscMenu, "tear") != 101)
                {
                    Vars.Q.Cast(Game.CursorPos);
                }

                /// <summary>
                ///     The Passive Stacking Logic.
                /// </summary>
                if (!GameObjects.Player.HasBuff("RyzePassiveCharged") &&
                    Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None) &&
                    Vars.getSliderItem(Vars.MiscMenu, "stacks") != 0 &&
                    Vars.getSliderItem(Vars.MiscMenu, "stacks") >
                        GameObjects.Player.GetBuffCount("RyzePassiveStack") &&
                    GameObjects.Player.ManaPercent >
                        Vars.getSliderItem(Vars.MiscMenu, "stacksmana"))
                {
                    Vars.Q.Cast(Game.CursorPos);
                }
            }
        }
    }
}