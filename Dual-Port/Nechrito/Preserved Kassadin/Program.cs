using LeagueSharp.SDK;
using System;

using TargetSelector = PortAIO.TSManager; namespace Preserved_Kassadin
{
    class Program
    {
        public static void Load()
        {
            if (GameObjects.Player.ChampionName != "Kassadin") return;
            LoadAssembly.Load();
        }
    }
}
