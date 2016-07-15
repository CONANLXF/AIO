#region

using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp;
using LeagueSharp.Common;
using NechritoRiven.Core;
using NechritoRiven.Draw;
using NechritoRiven.Event;
using NechritoRiven.Menus;

#endregion

 namespace NechritoRiven.Load
{
    internal class Load
    {
        
        public static void LoadAssembly()
        {
            MenuConfig.LoadMenu();
            Spells.Load();

            Obj_AI_Base.OnProcessSpellCast += OnCasted.OnCasting;
            Obj_AI_Base.OnSpellCast += Modes.OnDoCastLc;
            Obj_AI_Base.OnSpellCast += Modes.OnDoCast;
            Obj_AI_Base.OnProcessSpellCast += Core.Core.OnCast;
            Obj_AI_Base.OnPlayAnimation += Anim.OnPlay;

            Drawing.OnEndScene += DrawDmg.DmgDraw;
            Drawing.OnDraw += DrawRange.RangeDraw;
            Drawing.OnDraw += DrawWallSpot.WallDraw;

            Game.OnUpdate += Trinkets.Update;
            Game.OnUpdate += KillSteal.Update;
            Game.OnUpdate += AlwaysUpdate.Update;

            Spellbook.OnCastSpell += QSpell.OnSpell;

            Interrupter2.OnInterruptableTarget += Interrupt2.OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += Gapclose.gapcloser;

            Obj_AI_Base.OnSpellCast += (sender, args) =>
            {
                if (args.Slot == SpellSlot.W && sender.IsMe)
                {
                    Orbwalker.ResetAutoAttack();
                }
            };

            Chat.Print("<b><font color=\"#FFFFFF\">[</font></b><b><font color=\"#00e5e5\">Nechrito Riven</font></b><b><font color=\"#FFFFFF\">]</font></b><b><font color=\"#FFFFFF\"> Version: 69</font></b>");
            Chat.Print("<b><font color=\"#FFFFFF\">[</font></b><b><font color=\"#00e5e5\">Update</font></b><b><font color=\"#FFFFFF\">]</font></b><b><font color=\"#FFFFFF\"> Big Update</font></b>");
        }
    }
}
