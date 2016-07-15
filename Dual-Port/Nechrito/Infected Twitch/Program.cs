#region

using System;
using LeagueSharp.SDK;

#endregion

using TargetSelector = PortAIO.TSManager; namespace Infected_Twitch
{
    internal class Program
    {

        public static void Load()
        {
            if (GameObjects.Player.ChampionName != "Twitch") return;

            LoadAssembly.OnGameLoad();
        }
    }
}
