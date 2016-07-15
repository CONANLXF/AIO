using LeagueSharp.SDK.Core.Utils;
using SharpDX;
using System;
using System.Linq;
using PrideStalker_Rengar.Main;
using LeagueSharp.SDK;
using EloBuddy;
using EloBuddy.SDK;

using TargetSelector = PortAIO.TSManager; namespace PrideStalker_Rengar.Handlers
{
    class BeforeAA : Core
    {
        
        public static void Orbwalker_OnPreAttack(LeagueSharp.Common.BeforeAttackArgs args)
        {
            if (PortAIO.OrbwalkerManager.isComboActive)
            {
                var hero = args.Target as AIHeroClient;

                if(Mode.getBoxItem(MenuConfig.comboMenu, "ComboMode") == 1)
                {
                    if(MenuConfig.TripleQAAReset)
                    {
                        if(Player.Mana == 5)
                        {
                            Spells.Q.Cast(hero);
                        }
                        if(Player.Mana < 5)
                        {
                            Spells.Q.Cast(hero);
                        }
                    }
                }
            }
        }
    }
}
