using System;
using ExorAIO.Utilities;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;

 namespace ExorAIO.Champions.Twitch
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
            ///     The W Combo Logic.
            /// </summary>
            if (Vars.W.IsReady() &&
                Targets.Target.LSIsValidTarget(Vars.W.Range) &&
                Vars.getCheckBoxItem(Vars.WMenu, "combo"))
            {
                if (!GameObjects.Player.HasBuff("TwitchFullAutomatic") &&
                    Targets.Target.GetBuffCount("twitchdeadlyvenom") <= 4)
                {
                    Vars.W.Cast(Vars.W.GetPrediction(Targets.Target).CastPosition);
                }
            }
        }
    }
}