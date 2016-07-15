using EloBuddy;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;
using SharpDX;
using System;
using System.Linq;
using PrideStalker_Rengar.Main;
using PrideStalker_Rengar.Handlers;
using PrideStalker_Rengar.Draw;
using EloBuddy.SDK;

using TargetSelector = PortAIO.TSManager; namespace PrideStalker_Rengar
{
    class Program : Core
    {

        public static void Load()
        {
            if (GameObjects.Player.ChampionName != "Rengar")
            {
                return;
            }

            Chat.Print("<b><font color=\"#FFFFFF\">[</font></b><b><font color=\"#00e5e5\">Nechrito Rengar</font></b><b><font color=\"#FFFFFF\">]</font></b><b><font color=\"#FFFFFF\"> Update 11</font></b>");
            Chat.Print("<b><font color=\"#FFFFFF\">[</font></b><b><font color=\"#00e5e5\">Update</font></b><b><font color=\"#FFFFFF\">]</font></b><b><font color=\"#FFFFFF\"> Range E</font></b>");

            Spells.Load();
            MenuConfig.Load();

            LeagueSharp.Common.LSEvents.AfterAttack += AfterAA.Orbwalker_OnPostAttack;
            LeagueSharp.Common.LSEvents.BeforeAttack += BeforeAA.Orbwalker_OnPreAttack;

            Spellbook.OnCastSpell += OnSpell;

            Drawing.OnDraw += DRAW.OnDraw;
            Drawing.OnEndScene += Drawing_OnEndScene;
            Game.OnUpdate += OnUpdate;
        }

        private static void OnSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.Q)
            {
                PortAIO.OrbwalkerManager.ResetAutoAttackTimer();
            }
        }
        

        private static void OnUpdate(EventArgs args)
        {
            if(Player.IsDead || Player.IsRecalling())
            {
                return;
            }
            DelayAction.Add(600, Mode.ChangeComboMode);
            KillSteal.Killsteal();

            if (PortAIO.OrbwalkerManager.isComboActive)
            {
                switch (Mode.getBoxItem(MenuConfig.comboMenu, "ComboMode"))
                {
                    case 0:
                        Mode.Combo();
                        break;
                    case 1:
                        Mode.TripleQ();
                        break;
                    case 2:
                        Mode.ApCombo();
                        break;
                    case 3:
                        Mode.OneShot();
                        break;
                }
            }
            if (PortAIO.OrbwalkerManager.isLaneClearActive)
            {
                Mode.Lane();
            }
            if (PortAIO.OrbwalkerManager.isLaneClearActive)
            {
                Mode.Jungle();
            }       
            if (PortAIO.OrbwalkerManager.isLastHitActive)
            {
                Mode.LastHit();
            }
        }
        private static void Drawing_OnEndScene(EventArgs args)
        {
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(ene => ene.LSIsValidTarget() && !ene.IsZombie))
            {
                if (MenuConfig.dind)
                {
                    var EasyKill = Spells.Q.IsReady() && enemy.LSIsValidTarget(Player.AttackRange + 75) && Dmg.IsLethal(enemy)
                       ? new ColorBGRA(0, 255, 0, 120)
                       : new ColorBGRA(255, 255, 0, 120);

                    DRAW.DrawHpBar.unit = enemy;
                    DRAW.DrawHpBar.drawDmg(Dmg.ComboDmg(enemy), EasyKill);
                }
            }
        }
    }
}
