using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SPrediction;
using EloBuddy;

using TargetSelector = PortAIO.TSManager; namespace SDKPredictioner
{
    class Program
    {
        public static void Init()
        {
            CustomEvents.Game.OnGameLoad += EventHandlers.Game_OnGameLoad;
        }
    }
}