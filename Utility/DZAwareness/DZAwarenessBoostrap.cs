using System;
using DZAwarenessAIO.Utility;
using DZAwarenessAIO.Utility.HudUtility;
using DZAwarenessAIO.Utility.Logs;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu;
using EloBuddy;

 namespace DZAwarenessAIO
{
    /// <summary>
    /// The Bootstrap for the assembly.
    /// </summary>
    internal class DZAwarenessBoostrap
    {
        /// <summary>
        /// Called when the assembly is loaded.
        /// </summary>
        public static void OnLoad()
        {
            Variables.Menu = MainMenu.AddMenu("DZAwareness", "dz191.dza");
            LogHelper.OnLoad();
            HudDisplay.OnLoad();
          
            foreach (var module in Variables.Modules)
            {
                module.OnLoad();
            }

            ImageLoader.InitSprites();

            foreach (var element in HudVariables.HudElements)
            {
                element.OnLoad();
                element.InitDrawings();
            }

            DZAwareness.OnLoad();

            Chat.Print("<font color='#2675A9'>DZ</font>Awareness Loaded: Remember to enable the HUD in the Options!");
        }
    }
}
