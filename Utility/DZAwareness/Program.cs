using System;
using LeagueSharp.Common;

 namespace DZAwarenessAIO
{
    class Program
    {
        /// <summary>
        /// Called when the game is loaded.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs"/> instance containing the event data.</param>
        public static void Game_OnGameLoad()
        {
            DZAwarenessBoostrap.OnLoad();
        }
    }
}
