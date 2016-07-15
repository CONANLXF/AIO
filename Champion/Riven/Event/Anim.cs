using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp;
using LeagueSharp.Common;
using NechritoRiven.Core;
using NechritoRiven.Menus;
using System;

 namespace NechritoRiven.Event
{
    class Anim : Core.Core
    {
        public static void OnPlay(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (!sender.IsMe) return;
            var t = 0;

            var a = 0;
            var b = "";

            switch (args.Animation) // Logic from Fluxy
            {
                case "Spell1a":
                    lastQ = Utils.GameTimeTickCount;
                    t = 291;
                    Qstack = 2;
                    b = "Q2";
                    a = t - MenuConfig.Qld - (Game.Ping - MenuConfig.Qd);
                    break;
                case "Spell1b":
                    lastQ = Utils.GameTimeTickCount;
                    t = 290;
                    Qstack = 3;
                    b = "Q3";
                    a = t - MenuConfig.Qld - (Game.Ping - MenuConfig.Qd);
                    break;
                case "Spell1c": // q3?
                    lastQ = Utils.GameTimeTickCount;
                    t = 343;
                    Qstack = 1;
                    b = "Q1";
                    a = t - MenuConfig.Qld - (Game.Ping - MenuConfig.Qd);
                    break;
                case "Spell2":
                    t = 170;
                    break;
                case "Spell3":
                    if (MenuConfig.Burst || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) ||
                        MenuConfig.FastHarass || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
                        Usables.CastYoumoo();
                    break;
                case "Spell4a":
                    t = 0;
                    lastR = Utils.GameTimeTickCount;
                    break;
                case "Spell4b":
                    t = 150;
                    var target = TargetSelector.SelectedTarget;
                    if (Spells.Q.IsReady() && target.LSIsValidTarget()) ForceCastQ(target);
                    break;
            }

            if (a != 0 && (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None)))
            {
                LeagueSharp.Common.Utility.DelayAction.Add(a, () =>
                {
                    Console.WriteLine(b);
                    Orbwalker.ResetAutoAttack();
                    EloBuddy.Player.DoEmote(Emote.Dance);
                });
            }
        }
        
        private static void CancelAnimation()
        {
            Orbwalker.ResetAutoAttack();
            if (MenuConfig.QReset)
            {
                EloBuddy.Player.DoEmote(Emote.Dance);
            }
            else if (MenuConfig.Qstrange && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None))
            {
                if (MenuConfig.AnimDance) EloBuddy.Player.DoEmote(Emote.Dance);
                if (MenuConfig.AnimLaugh) EloBuddy.Player.DoEmote(Emote.Laugh);
                if (MenuConfig.AnimTaunt) EloBuddy.Player.DoEmote(Emote.Taunt);
                if (MenuConfig.AnimTalk) EloBuddy.Player.DoEmote(Emote.Joke);
            }
        }
    }
}
