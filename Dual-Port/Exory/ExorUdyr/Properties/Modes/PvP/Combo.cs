using System;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.Udyr
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
        public static void Combo(EventArgs args)
        {
            if (Bools.HasSheenBuff() ||
                !Targets.Target.LSIsValidTarget() ||
                Invulnerable.Check(Targets.Target))
            {
                return;
            }

            /// <summary>
            ///     The E Combo Logic.
            /// </summary>
            if (Vars.E.IsReady() &&
                !Targets.Target.HasBuff("udyrbearstuncheck") &&
                !Targets.Target.HasBuffOfType(BuffType.Stun) &&
                Vars.getCheckBoxItem(Vars.EMenu, "combo"))
            {
                Vars.E.Cast();
            }

            if (!Targets.Target.HasBuff("udyrbearstuncheck"))
            {
                return;
            }

            /// <summary>
            ///     The R Combo Logic.
            /// </summary>
            if (GameObjects.Player.HasBuff("itemmagicshankcharge") ||
                GameObjects.Player.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
            {
                if (Vars.R.IsReady() &&
                    GameObjects.Player.GetBuffCount("UdyrPhoenixStance") != 3 &&
                    Vars.getCheckBoxItem(Vars.RMenu, "combo"))
                {
                    Vars.R.Cast();
                }
            }

            /// <summary>
            ///     The Q Combo Logic.
            /// </summary>
            else
            {
                if (Vars.Q.IsReady() &&
                    Vars.getCheckBoxItem(Vars.QMenu, "combo"))
                {
                    Vars.Q.Cast();
                }
            }
        }
    }
}