using System;
using System.Collections.Generic;
using System.Diagnostics.PerformanceData;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using UnderratedAIO.Helpers;
using EloBuddy;

namespace UnderratedAIO
{
    internal class Program
    {
        public static IncomingDamage IncDamages;

        public static void Init()
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }

        private static void OnGameLoad(EventArgs args)
        {
            try
            {
                switch (ObjectManager.Player.ChampionName.ToLower())
                {
                    case "chogath":
                        Champions.Chogath.Load();
                        break;
                    case "olaf":
                        new Champions.Olaf();
                        break;
                    case "renekton":
                        Champions.Renekton.OnLoad();
                        break;
                    case "galio":
                        Champions.Galio.OnLoad();
                        break;
                    case "gangplank":
                        new Champions.Gangplank();
                        break;
                    case "garen":
                        Champions.Garen.OnLoad();
                        break;
                    case "kennen":
                        new Champions.Kennen();
                        break;
                    case "maokai":
                        new Champions.Maokai();
                        break;
                    case "nocturne":
                        new Champions.Nocturne();
                        break;
                    case "poppy":
                        new Champions.Poppy();
                        break;
                    case "rumble":
                        new Champions.Rumble();
                        break;
                    case "shaco":
                        new Champions.Shaco();
                        break;
                    case "shen":
                        new Champions.Shen();
                        break;
                    case "skarner":
                        new Champions.Skarner();
                        break;
                    case "yorick":
                        new Champions.Yorick();
                        break;
                    case "tahmkench":
                        new Champions.TahmKench();
                        break;
                    case "sion":
                        new Champions.Sion();
                        break;
                    case "volibear":
                        new Champions.Volibear();
                        break;
                    case "zac":
                        new Champions.Zac();
                        break;
                    case "trundle":
                        new Champions.Trundle();
                        break;
                    case "hecarim":
                        new Champions.Hecarim();
                        break;
                }
                IncDamages = new IncomingDamage();
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed To load: " + e);
            }
        }
    }
}