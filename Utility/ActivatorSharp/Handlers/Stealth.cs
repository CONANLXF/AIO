#region Copyright © 2015 Kurisu Solutions
// All rights are reserved. Transmission or reproduction in part or whole,
// any form or by any means, mechanical, electronical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
// 
// Document:	Handlers/Events.cs
// Date:		22/09/2015
// Author:		Robin Kurisu
#endregion

using System;
using System.Linq;
using Activators.Base;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;

 namespace Activators.Handlers
{
    public class Stealth
    {
        private static Random _random;
        private static bool _loaded;

        public static void Init()
        {
            if (!_loaded)
            {
                _random = new Random();
                Obj_AI_Base.OnBuffGain += Obj_AI_Base_OnBuffAdd;
                Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnStealth;
                _loaded = true;
            }
        }

        static void Obj_AI_Base_OnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            foreach (var ally in Activator.Allies())
            {
                if (sender.LSIsValidTarget(1000) && !sender.IsZombie && sender.NetworkId == ally.Player.NetworkId)
                {
                    if (args.Buff.Name == "rengarralertsound")
                    {
                        ally.HitTypes.Add(HitType.Stealth);
                        LeagueSharp.Common.Utility.DelayAction.Add(100 + _random.Next(200, 450), () => ally.HitTypes.Remove(HitType.Stealth));
                    }
                }
            }
        }

        static void Obj_AI_Base_OnStealth(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            #region Stealth

            var attacker = sender as AIHeroClient;
            if (attacker == null || attacker.IsAlly || !attacker.IsValid<AIHeroClient>())
            {
                return;
            }

            foreach (var hero in Activator.Heroes.Where(h => h.Player.LSDistance(attacker) <= 1000))
            {
                foreach (var x in Data.Somedata.Spells)
                {
                    if (args.SData.Name.Equals(x.SDataName, StringComparison.InvariantCultureIgnoreCase) && x.HitType.Contains(HitType.Stealth))
                    {
                        hero.HitTypes.Add(HitType.Stealth);
                        LeagueSharp.Common.Utility.DelayAction.Add(100 + _random.Next(200, 450), () => hero.HitTypes.Remove(HitType.Stealth));
                        break;
                    }
                }
            }

            #endregion
        }
    }
}
