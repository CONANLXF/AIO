using System;
using System.Linq;
using DZAwarenessAIO.Utility;
using DZAwarenessAIO.Utility.HudUtility;
using LeagueSharp;
using EloBuddy;

 namespace DZAwarenessAIO
{
    /// <summary>
    /// The DZAwareness Class
    /// </summary>
    internal class DZAwareness
    {
        /// <summary>
        /// Called when the assembly gets loaded.
        /// </summary>
        public static void OnLoad()
        {
            Game.OnUpdate += OnUpdate;
        }

        /// <summary>
        /// Raises the <see cref="Game.OnUpdate" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs"/> instance containing the event data.</param>
        private static void OnUpdate(EventArgs args)
        {
            foreach (var module in Variables.Modules.Where(mod => mod.ShouldRun() && mod.GetModuleType() == ModuleTypes.OnUpdate))
            {
                module.OnTick();
            }

            ImageLoader.OnUpdate();
        }
    }
}
