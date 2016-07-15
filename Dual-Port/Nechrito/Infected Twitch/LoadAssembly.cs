#region

using Infected_Twitch.Core;
using Infected_Twitch.Event;
using Infected_Twitch.Menus;
using EloBuddy;

#endregion

 namespace Infected_Twitch
{
    internal class LoadAssembly
    {
        public static void OnGameLoad()
        {

            Spells.Load();
            MenuConfig.Load();

            Recall.Load();
            Game.OnUpdate += Exploit.Update;
            Game.OnUpdate += Modes.Update;
            Game.OnUpdate += EOnDeath.Update;
            Game.OnUpdate += Trinkets.Update;
            Game.OnUpdate += Killsteal.Update;
            Drawing.OnEndScene += DrawDmg.Draw;
            Drawing.OnDraw += DrawSpells.OnDraw;

            Chat.Print("<b><font color=\"#FFFFFF\">[</font></b><b><font color=\"#00e5e5\">Infected Twitch</font></b><b><font color=\"#FFFFFF\">]</font></b><b><font color=\"#FFFFFF\"> Version: 2</font></b>");
            Chat.Print("<b><font color=\"#FFFFFF\">[</font></b><b><font color=\"#00e5e5\">Update</font></b><b><font color=\"#FFFFFF\">]</font></b><b><font color=\"#FFFFFF\"> E Damage Calculations</font></b>");
        }
    }
}
