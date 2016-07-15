using System;
using ExorAIO.Utilities;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;

using TargetSelector = PortAIO.TSManager; namespace ExorAIO.Champions.Ryze
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
            if (!Targets.Target.LSIsValidTarget() ||
                Invulnerable.Check(Targets.Target))
            {
                return;
            }
            
            if (Bools.HasSheenBuff())
            {
                if (Targets.Target.LSIsValidTarget(Vars.AARange))
                {
                    return;
                }
            }

            /// <summary>
            ///     The R Combo Logic.
            /// </summary>
            if (Vars.R.IsReady()  &&
                Vars.getCheckBoxItem(Vars.RMenu, "combo") && Targets.Target.LSIsValidTarget(Vars.Q.Range))
            {
                if (!GameObjects.Player.HasBuff("RyzePassiveCharged") &&
                    GameObjects.Player.GetBuffCount("RyzePassiveStack") == 0)
                {
                    return;
                }

                if (Vars.W.IsReady() || Vars.Q.IsReady() &&
                    GameObjects.Player.GetBuffCount("RyzePassiveStack") == 3)
                {
                    Vars.R.Cast();
                }
                if (GameObjects.Player.GetBuffCount("RyzePassiveStack") == 2 && !Vars.E.IsReady() )
                {
                    Vars.R.Cast();
                }
                if (GameObjects.Player.GetBuffCount("RyzePassiveStack") == 1 && !Vars.Q.IsReady() || !Vars.E.IsReady())
                {
                    Vars.R.Cast();
                }
            }

            /// <summary>
            ///     The W Combo Logic.
            /// </summary>
            if (Vars.W.IsReady() &&
                Targets.Target.LSIsValidTarget(Vars.W.Range) &&
                Vars.getCheckBoxItem(Vars.WMenu, "combo"))
            {
                if (!Vars.Q.IsReady() &&
                    GameObjects.Player.GetBuffCount("RyzePassiveStack") == 1)
                {
                    return;
                }

                if (GameObjects.Player.HasBuff("RyzePassiveCharged") ||
                    GameObjects.Player.GetBuffCount("RyzePassiveStack") != 0)
                {
                    Vars.W.CastOnUnit(Targets.Target);
                }
            }

            /// <summary>
            ///     The Q Combo Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                Targets.Target.LSIsValidTarget(Vars.Q.Range) &&
                Vars.getCheckBoxItem(Vars.QMenu, "combo") && GameObjects.Player.GetBuffCount("RyzePassiveStack") != 4)
            {
                Vars.Q.Cast(Vars.Q.GetPrediction(Targets.Target).UnitPosition);
            }
            if (Vars.Q.IsReady() &&
            Targets.Target.LSIsValidTarget(Vars.Q.Range) &&
            Vars.getCheckBoxItem(Vars.QMenu, "combo") && GameObjects.Player.GetBuffCount("RyzePassiveStack") == 4 && !Vars.W.IsReady())
            {
                Vars.Q.Cast(Vars.Q.GetPrediction(Targets.Target).UnitPosition);
            }
            /// <summary>
            ///     The E Combo Logic.
            /// </summary>
            if (Vars.E.IsReady() &&
                Targets.Target.LSIsValidTarget(Vars.E.Range) &&
                Vars.getCheckBoxItem(Vars.EMenu, "combo"))
            {
                if (!Vars.Q.IsReady() &&
                    GameObjects.Player.GetBuffCount("RyzePassiveStack") == 1)
                {
                    return;
                }

                if (GameObjects.Player.HasBuff("RyzePassiveCharged") ||
                    GameObjects.Player.GetBuffCount("RyzePassiveStack") != 0 && GameObjects.Player.GetBuffCount("RyzePassiveStack") != 4)
                {
                    Vars.E.CastOnUnit(Targets.Target);
                }
                if (GameObjects.Player.GetBuffCount("RyzePassiveStack") == 4 && !Vars.W.IsReady())
                {
                    Vars.E.CastOnUnit(Targets.Target);
                }
            }
        }
    }
}
