#region

using System;
using System.Linq;
using Arcane_Ryze.Draw;
using Arcane_Ryze.Handler;
using Arcane_Ryze.Main;
using Arcane_Ryze.Modes;
using EloBuddy;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core;
using SharpDX;
using EloBuddy.SDK;

#endregion

using TargetSelector = PortAIO.TSManager; namespace Arcane_Ryze
{
    internal class Program : Core
    {
        Random Random = new Random();


        public static void Load()
        {

            if (GameObjects.Player.ChampionName != "Ryze")
            {
                return;
            }
            Chat.Print("<b><font color=\"#FFFFFF\">[</font></b><b><font color=\"#00e5e5\">Arcane Ryze</font></b><b><font color=\"#FFFFFF\">]</font></b><b><font color=\"#FFFFFF\"> Version: 1</font></b>");
            Chat.Print("<b><font color=\"#FFFFFF\">[</font></b><b><font color=\"#00e5e5\">Update</font></b><b><font color=\"#FFFFFF\">]</font></b><b><font color=\"#FFFFFF\"> Release</font></b>");

            Spells.Load();
            MenuConfig.Load();
            LeagueSharp.Common.LSEvents.AfterAttack += BeforeAA.OnAction;

            Game.OnUpdate += OnUpdate;
            Drawing.OnEndScene += Drawing_OnEndScene;
        }
        
        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead || Player.LSIsRecalling())
            {
                return;
            }
            //  Console.WriteLine("Buffs: {0}", string.Join(" | ", Player.Buffs.Where(b => b.Caster.NetworkId == Player.NetworkId).Select(b => b.DisplayName)));


            // Useless Code.

            if (PortAIO.OrbwalkerManager.isComboActive)
            {
                Combo.ComboLogic();
            }
            if (PortAIO.OrbwalkerManager.isHarassActive)
            {
                Harass.HarassLogic();
            }
            if (PortAIO.OrbwalkerManager.isLaneClearActive)
            {
                Jungle.JungleLogic();
                Lane.LaneLogic();
            }
            if (PortAIO.OrbwalkerManager.isLastHitActive)
            {
                LastHit.LastHitLogic();
            }

            KillSteal.Killsteal();

        }
        public static HpBarDraw HpBarDraw = new HpBarDraw();
        private static void Drawing_OnEndScene(EventArgs args)
        {
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(ene => ene.LSIsValidTarget() && !ene.IsZombie))
            {
                if (MenuConfig.dind)
                {
                    var EasyFuckingKill = Spells.Q.IsReady() && Dmg.EasyFuckingKillKappa(enemy)
                        ? new ColorBGRA(0, 255, 0, 120)
                        : new ColorBGRA(255, 255, 0, 120);
                    HpBarDraw.unit = enemy;
                    HpBarDraw.drawDmg(Dmg.EDmg(enemy) + Dmg.QDmg(enemy) + Dmg.WDmg(enemy), EasyFuckingKill);
                }
            }
        }
    }
}
