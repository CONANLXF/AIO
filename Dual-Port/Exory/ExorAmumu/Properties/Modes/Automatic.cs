using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using EloBuddy.SDK;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.Amumu
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
            ///     The Automatic W Logic.
            /// </summary>
            if (Vars.W.IsReady() &&
                Vars.getSliderItem(Vars.WMenu, "logical") != 101)
            {
                /// <summary>
                ///     If the player doesn't have the W Buff.
                /// </summary>
                if (!GameObjects.Player.HasBuff("AuraOfDespair"))
                {
                    if (PortAIO.OrbwalkerManager.isComboActive)
                    {
                        Vars.W.Cast();
                    }

                    if (PortAIO.OrbwalkerManager.isLaneClearActive)
                    {
                        if (GameObjects.Player.ManaPercent >= ManaManager.GetNeededMana(Vars.W.Slot, Vars.getSliderItem(Vars.WMenu, "logical")) && (Targets.Minions.Count() >= 2 || Targets.JungleMinions.Any()))
                        {
                            Vars.W.Cast();
                        }
                    }
                }

                /// <summary>
                ///     If the player has the W Buff.
                /// </summary>
                else
                {

                    if (PortAIO.OrbwalkerManager.isLaneClearActive)
                    {
                        if (GameObjects.Player.ManaPercent <
                                ManaManager.GetNeededMana(Vars.W.Slot, Vars.getSliderItem(Vars.WMenu, "logical")) ||
                            (Targets.Minions.Count() < 2 && !Targets.JungleMinions.Any()))
                        {
                            Vars.W.Cast();
                        }
                        return;
                    }

                    if (!GameObjects.EnemyHeroes.Any(t => t.LSIsValidTarget(Vars.W.Range)))
                    {
                        Vars.W.Cast();
                    }
                }
            }
        }
    }
}