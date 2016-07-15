#region Copyright © 2015 Kurisu Solutions
// All rights are reserved. Transmission or reproduction in part or whole,
// any form or by any means, mechanical, electronical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
// 
// Document:	Data/Gametroydata.cs
// Date:		22/09/2015
// Author:		Robin Kurisu
#endregion

using LeagueSharp;
using Activators.Base;
using System.Collections.Generic;
using EloBuddy;

using TargetSelector = PortAIO.TSManager; namespace Activators.Data
{
    public class Gametroydata
    {
        public string Name { get; set; }
        public string ChampionName { get; set; }
        public SpellSlot Slot { get; set; }
        public float Radius { get; set; }
        public double Interval { get; set; }
        public bool PredictDmg { get; set; }
        public HitType[] HitType { get; set; }
        public int DelayFromStart { get; set; }

        public static List<Gametroydata> Troys = new List<Gametroydata>(); 

        static Gametroydata()
        {
            //Troys.Add(new Gametroydata
            //{
            //Name = "Q_Hit",
            //ChampionName = "Sion",
            //Radius = 600f,
            //Slot = SpellSlot.Q,
            //HitType = new[] { Base.HitType.Danger },
            //PredictDmg = true,
            //Interval = 0.75
            //});

            Troys.Add(new Gametroydata
            {
                Name = "R_Cas",
                ChampionName = "Nunu",
                Radius = 650f,
                Slot = SpellSlot.R,
                HitType = new[] { Base.HitType.CrowdControl },
                PredictDmg = true,
                Interval = 0.75
            });

            Troys.Add(new Gametroydata
            {
                Name = "E_mis_bounce",
                ChampionName = "Ryze",
                Radius = 200f,
                Slot = SpellSlot.E,
                HitType = new[] { Base.HitType.None },
                PredictDmg = true,
                Interval = .75
            });

            Troys.Add(new Gametroydata
            {
                Name = "R_E_mis_bounce",
                ChampionName = "Ryze",
                Radius = 250f,
                Slot = SpellSlot.E,
                HitType = new[] { Base.HitType.None },
                PredictDmg = true,
                Interval = .75
            });

            Troys.Add(new Gametroydata
            {
                Name = "Hecarim_Defile",
                ChampionName = "Hecarim",
                Radius = 425f,
                Slot = SpellSlot.W,
                HitType = new[] { Base.HitType.None },
                PredictDmg = true,
                Interval = .75
            });

            Troys.Add(new Gametroydata
            {
                Name = "W_AoE",
                ChampionName = "Hecarim",
                Radius = 425f,
                Slot = SpellSlot.W,
                HitType = new[] { Base.HitType.None },
                PredictDmg = true,
                Interval = .75
            });

            Troys.Add(new Gametroydata
            {
                Name = "R_AoE",
                ChampionName = "Gangplank",
                Radius = 450f,
                Slot = SpellSlot.R,
                HitType = new[] { Base.HitType.None },
                PredictDmg = true,
                Interval = 1.5
            });

            Troys.Add(new Gametroydata
            {
                Name = "W_Shield",
                ChampionName = "Diana",
                Radius = 225f,
                Slot = SpellSlot.W,
                HitType = new[] { Base.HitType.None },
                PredictDmg = true,
                Interval = 1.0
            });

            Troys.Add(new Gametroydata
            {
                Name = "W_Shield",
                ChampionName = "Sion",
                Radius = 225f,
                Slot = SpellSlot.W,
                DelayFromStart = 2800,
                HitType = new[] { Base.HitType.None },
                PredictDmg = true,
                Interval = 1.0
            });

            Troys.Add(new Gametroydata
            {
                Name = "W_aoe_red",
                ChampionName = "Malzahar",
                Radius = 325f,
                Slot = SpellSlot.W,
                HitType = new[] { Base.HitType.None },
                PredictDmg = true,
                Interval = 1.0
            });

            Troys.Add(new Gametroydata
            {
                Name = "E_Defile",
                ChampionName = "Karthus",
                Radius = 425f,
                Slot = SpellSlot.E,
                HitType = new[] { Base.HitType.None },
                PredictDmg = true,
                Interval = 1.0
            });

            Troys.Add(new Gametroydata
            {
                Name = "W_volatile",
                ChampionName = "Elise",
                Radius =  250f,
                Slot = SpellSlot.W,
                HitType = new[] { Base.HitType.None },
                PredictDmg = true,
                Interval = 0.3
            });

            Troys.Add(new Gametroydata
            {
                Name = "DarkWind_tar",
                ChampionName = "FiddleSticks",
                Radius = 250f,
                Slot = SpellSlot.E,
                HitType = new[] { Base.HitType.None },
                PredictDmg = true,
                Interval = 0.8
            });

            Troys.Add(new Gametroydata
            {
                Name = "lr_buf",
                ChampionName = "Kennen",
                Radius = 250f,
                Slot = SpellSlot.E,
                HitType = new[] { Base.HitType.None },
                PredictDmg = true,
                Interval = 0.8
            });

            Troys.Add(new Gametroydata
            {
                Name = "ss_aoe",
                ChampionName = "Kennen",
                Radius = 475f,
                Slot = SpellSlot.R,
                HitType = new[] { Base.HitType.Danger, Base.HitType.Ultimate },
                PredictDmg = true,
                Interval = 0.5
            });

            Troys.Add(new Gametroydata
            {
                Name = "Ahri_Base_FoxFire",
                ChampionName = "Ahri",
                Radius = 550f,
                Slot = SpellSlot.W,
                HitType = new[] { Base.HitType.None },
                PredictDmg = true,
                Interval = 1.0
            });

            Troys.Add(new Gametroydata
            {
                Name = "AurelionSol_Base_P",
                ChampionName = "AurelionSol",
                Radius = 165f,
                Slot = SpellSlot.W,
                HitType = new[] { Base.HitType.None },
                PredictDmg = true,
                Interval = 1.0
            });

            Troys.Add(new Gametroydata
            {
                Name = "Fizz_Ring_Red",
                ChampionName = "Fizz",
                Radius = 300f,
                Slot = SpellSlot.R,
                DelayFromStart = 800,
                HitType = new[] { Base.HitType.Danger, Base.HitType.Ultimate },
                PredictDmg = true,
                Interval = 1.0
             });

            Troys.Add(new Gametroydata
            {
                Name = "katarina_deathLotus_tar",
                ChampionName = "Katarina",
                Radius = 550f,
                Slot = SpellSlot.R,
                HitType = new[] { Base.HitType.ForceExhaust, Base.HitType.Danger },
                PredictDmg = true,
                Interval = 0.5
            });

            Troys.Add(new Gametroydata
            {
                Name = "Nautilus_R_sequence_impact",
                ChampionName = "Nautilus",
                Radius = 250f,
                Slot = SpellSlot.R,
                HitType = new[] { Base.HitType.CrowdControl, Base.HitType.Danger, Base.HitType.Ultimate },
                PredictDmg = false,
                Interval = 1.0
            });

            Troys.Add(new Gametroydata
            {
                Name = "Acidtrail_buf",
                ChampionName = "Singed",
                Radius = 200f,
                Slot = SpellSlot.Q,
                HitType = new []{ Base.HitType.None },
                PredictDmg = true,
                Interval = 0.5
            });

            Troys.Add(new Gametroydata
            {
                Name = "Tremors_cas",
                ChampionName = "Rammus",
                Radius = 450f,
                Slot = SpellSlot.R,
                HitType = new[] { Base.HitType.None },
                PredictDmg = true,
                Interval = 1.0
            });

            Troys.Add(new Gametroydata
            {
                Name = "Crowstorm",
                ChampionName = "FiddleSticks",
                Radius = 425f,
                Slot = SpellSlot.R,
                HitType =  new[] { Base.HitType.Danger, Base.HitType.Ultimate, Base.HitType.ForceExhaust },
                PredictDmg = true,
                Interval = 1.0
            });

            Troys.Add(new Gametroydata
            {
                Name = "yordleTrap_idle",
                ChampionName = "Caitlyn",
                Radius = 265f,
                Slot = SpellSlot.W,
                HitType = new[] { Base.HitType.CrowdControl },
                PredictDmg = false,
                Interval = 1.0
            });

            Troys.Add(new Gametroydata
            {
                Name = "tar_aoe_red",
                ChampionName = "Lux",
                Radius = 400f,
                Slot = SpellSlot.E,
                HitType = new[] { Base.HitType.None },
                PredictDmg = true,
                Interval = 2.0
            });

            Troys.Add(new Gametroydata
            {
                Name = "Viktor_ChaosStorm",
                ChampionName = "Viktor",
                Radius = 425f,
                Slot = SpellSlot.R,
                HitType = new[] { Base.HitType.Danger, Base.HitType.CrowdControl },
                PredictDmg = true,
                Interval = 1.0
            });

            Troys.Add(new Gametroydata
            {
                Name = "Viktor_Catalyst",
                ChampionName = "Viktor",
                Radius = 375f,
                Slot = SpellSlot.W,
                HitType = new[] { Base.HitType.CrowdControl },
                PredictDmg = false,
                Interval = 1.0
            });

            Troys.Add(new Gametroydata
            {
                Name = "W_AUG",
                ChampionName = "Viktor",
                Radius = 375f,
                Slot = SpellSlot.W,
                HitType = new[] { Base.HitType.CrowdControl },
                PredictDmg = false,
                Interval = 1.0
            });

            Troys.Add(new Gametroydata
            {
                Name = "cryo_storm",
                ChampionName = "Anivia",
                Radius = 400f,
                Slot = SpellSlot.R,
                HitType = new[] { Base.HitType.CrowdControl },
                PredictDmg = true,
                Interval = 1.0
            });

            Troys.Add(new Gametroydata
            {
                Name = "ZiggsE",
                ChampionName = "Ziggs",
                Radius = 325f,
                Slot = SpellSlot.E,
                HitType = new []{ Base.HitType.CrowdControl },
                PredictDmg = true,
                Interval = 1.0
            });

            Troys.Add(new Gametroydata
            {
                Name = "ZiggsWRing",
                ChampionName = "Ziggs",
                Radius = 325f,
                Slot = SpellSlot.W,
                HitType = new []{ Base.HitType.CrowdControl },
                PredictDmg = false,
                Interval = 1.0
            });

            Troys.Add(new Gametroydata
            {
                Name = "W_Miasma_tar",
                ChampionName = "Cassiopeia",
                Radius = 365f,
                Slot = SpellSlot.W,
                HitType = new[] { Base.HitType.None },
                PredictDmg = true,
                Interval = 1.0
            });

            Troys.Add(new Gametroydata
            {
                Name = "E_rune",
                ChampionName = "Soraka",
                Radius = 375f,
                Slot = SpellSlot.E,
                HitType = new[] { Base.HitType.None },
                PredictDmg = true,
                Interval = 1.0
            });

            Troys.Add(new Gametroydata
            {
                Name = "W_Tar",
                ChampionName = "Morgana",
                Radius = 275f,
                Slot = SpellSlot.W,
                HitType = new []{ Base.HitType.None },
                PredictDmg = true,
                Interval = .75
            });
        }
    }
}
