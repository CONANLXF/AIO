#region Copyright © 2015 Kurisu Solutions
// All rights are reserved. Transmission or reproduction in part or whole,
// any form or by any means, mechanical, electronical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
// 
// Document:	Data/Somedata.cs
// Date:		22/09/2015
// Author:		Robin Kurisu
#endregion

using System.Linq;
using LeagueSharp;
using Activators.Base;
using LeagueSharp.Common;
using System.Collections.Generic;
using LeagueSharp.Data.Enumerations;
using EloBuddy;

using TargetSelector = PortAIO.TSManager; namespace Activators.Data
{
    public class Somedata
    {
        public string SDataName { get; set; }
        public string ChampionName { get; set; }
        public string[] FromObject { get; set; }
        public HitType[] HitType { get; set; }

        public int Speed { get; set; }
        public float Range { get; set; }
        public float Width { get; set; }
        public float Delay { get; set; }
        public bool FixedRange { get; set; }
        public SpellType SpellType { get; set; }
        public string MissileName { get; set; }
        public string[] ExtraMissileNames { get; set; }
        public SpellTags[] SpellTags { get; set; }

        // Spell data populated by L# Data.
        static Somedata()
        {
            Spells.Add(new Somedata
            {
                SDataName = "aatroxq",
                ChampionName = "aatrox",
                HitType = new[] { Base.HitType.Danger, Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "aatroxw",
                ChampionName = "aatrox",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "aatroxw2",
                ChampionName = "aatrox",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "aatroxe",
                ChampionName = "aatrox",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "aatroxr",
                ChampionName = "aatrox",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "ahriorbofdeception",
                ChampionName = "ahri",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "ahrifoxfire",
                ChampionName = "ahri",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "ahriseduce",
                ChampionName = "ahri",
                HitType = new[] { Base.HitType.Danger, Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "ahritumble",
                ChampionName = "ahri",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "akalimota",
                ChampionName = "akali",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "akalismokebomb",
                ChampionName = "akali",
                HitType = new[] { Base.HitType.Stealth },
            });

            Spells.Add(new Somedata
            {
                SDataName = "akalishadowswipe",
                ChampionName = "akali",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "akalishadowdance",
                ChampionName = "akali",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "pulverize",
                ChampionName = "alistar",
                HitType = new[] { Base.HitType.Danger, Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "headbutt",
                ChampionName = "alistar",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "triumphantroar",
                ChampionName = "alistar",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "feroucioushowl",
                ChampionName = "alistar",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "bandagetoss",
                ChampionName = "amumu",
                HitType = new[] { Base.HitType.Danger, Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "auraofdespair",
                ChampionName = "amumu",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "tantrum",
                ChampionName = "amumu",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "curseofthesadmummy",
                ChampionName = "amumu",
                HitType =
                    new[]
                    {
                        Base.HitType.Danger, Base.HitType.Ultimate,
                        Base.HitType.CrowdControl
                    },
            });

            Spells.Add(new Somedata
            {
                SDataName = "flashfrost",
                ChampionName = "anivia",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "crystalize",
                ChampionName = "anivia",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "frostbite",
                ChampionName = "anivia",
                HitType = new[] { Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "glacialstorm",
                ChampionName = "anivia",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "disintegrate",
                ChampionName = "annie",
                HitType = new[] { Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "incinerate",
                ChampionName = "annie",
                HitType = new[] { Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "moltenshield",
                ChampionName = "annie",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "infernalguardian",
                ChampionName = "annie",
                HitType =
                    new[]
                    {
                        Base.HitType.Danger, Base.HitType.Ultimate,
                        Base.HitType.CrowdControl
                    },
            });

            Spells.Add(new Somedata
            {
                SDataName = "frostshot",
                ChampionName = "ashe",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "frostarrow",
                ChampionName = "ashe",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "volley",
                ChampionName = "ashe",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "ashespiritofthehawk",
                ChampionName = "ashe",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "enchantedcrystalarrow",
                ChampionName = "ashe",
                HitType =
                    new[]
                    {
                        Base.HitType.Danger, Base.HitType.Ultimate,
                        Base.HitType.CrowdControl
                    },
            });

            Spells.Add(new Somedata
            {
                SDataName = "aurelionsolq",
                ChampionName = "aurelionsol",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "aurelionsolw",
                ChampionName = "aurelionsol",
                HitType = new[] { Base.HitType.None },
            });

            Spells.Add(new Somedata
            {
                SDataName = "aurelionsole",
                ChampionName = "aurelionsol",
                HitType = new[] { Base.HitType.None },
            });

            Spells.Add(new Somedata
            {
                SDataName = "aurelionsolr",
                ChampionName = "aurelionsol",
                HitType = new[] { Base.HitType.CrowdControl, Base.HitType.Ultimate, Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "azirq",
                ChampionName = "azir",
                HitType = new[] { Base.HitType.CrowdControl },
                FromObject = new[] { "AzirSoldier" },
            });

            Spells.Add(new Somedata
            {
                SDataName = "azirr",
                ChampionName = "azir",
                HitType =
                    new[]
                    {
                        Base.HitType.Danger, Base.HitType.Ultimate,
                        Base.HitType.CrowdControl
                    },
            });

            Spells.Add(new Somedata
            {
                SDataName = "bardq",
                ChampionName = "bard",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "bardw",
                ChampionName = "bard",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "barde",
                ChampionName = "bard",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "bardr",
                ChampionName = "bard",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "rocketgrabmissile",
                ChampionName = "blitzcrank",
                HitType = new[] { Base.HitType.CrowdControl, Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "overdrive",
                ChampionName = "blitzcrank",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "powerfist",
                ChampionName = "blitzcrank",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "staticfield",
                ChampionName = "blitzcrank",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "brandq",
                ChampionName = "brand",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "brandw",
                ChampionName = "brand",
                HitType = new[] { Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "brande",
                ChampionName = "brand",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "brandr",
                ChampionName = "brand",
                HitType = new[] { Base.HitType.Danger, Base.HitType.Ultimate },
            });

            Spells.Add(new Somedata
            {
                SDataName = "braumq",
                ChampionName = "braum",
                HitType = new[] { Base.HitType.Danger, Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "braumqmissle",
                ChampionName = "braum",
                HitType = new[] { Base.HitType.Danger, Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "braumw",
                ChampionName = "braum",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "braume",
                ChampionName = "braum",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "braumrwrapper",
                ChampionName = "braum",
                HitType =
                    new[]
                    {
                        Base.HitType.Danger, Base.HitType.Ultimate,
                        Base.HitType.CrowdControl
                    },
            });

            Spells.Add(new Somedata
            {
                SDataName = "caitlynpiltoverpeacemaker",
                ChampionName = "caitlyn",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "caitlynyordletrap",
                ChampionName = "caitlyn",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "caitlynentrapment",
                ChampionName = "caitlyn",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "cassiopeiaq",
                ChampionName = "cassiopeia",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "cassiopeiaw",
                ChampionName = "cassiopeia",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "cassiopeiae",
                ChampionName = "cassiopeia",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "cassiopeiaper",
                ChampionName = "cassiopeia",
                HitType =
                    new[]
                    {
                        Base.HitType.Danger, Base.HitType.Ultimate,
                        Base.HitType.CrowdControl
                    },
            });

            Spells.Add(new Somedata
            {
                SDataName = "rupture",
                ChampionName = "chogath",
                HitType = new[] { Base.HitType.Danger, Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "feralscream",
                ChampionName = "chogath",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "vorpalspikes",
                ChampionName = "chogath",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "feast",
                ChampionName = "chogath",
                HitType = new[] { Base.HitType.Danger, Base.HitType.Ultimate },
            });

            Spells.Add(new Somedata
            {
                SDataName = "phosphorusbomb",
                ChampionName = "corki",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "carpetbomb",
                ChampionName = "corki",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "ggun",
                ChampionName = "corki",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "missilebarrage",
                ChampionName = "corki",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "dariuscleave",
                ChampionName = "darius",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "dariusnoxiantacticsonh",
                ChampionName = "darius",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "dariusaxegrabcone",
                ChampionName = "darius",
                HitType = new[] { Base.HitType.CrowdControl, Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "dariusexecute",
                ChampionName = "darius",
                HitType = new[] { Base.HitType.Danger, Base.HitType.Ultimate },
            });

            Spells.Add(new Somedata
            {
                SDataName = "dianaarc",
                ChampionName = "diana",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "dianaorbs",
                ChampionName = "diana",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "dianavortex",
                ChampionName = "diana",
                HitType = new[] { Base.HitType.CrowdControl, Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "dianateleport",
                ChampionName = "diana",
                HitType = new[] { Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "dravenspinning",
                ChampionName = "draven",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "dravenfury",
                ChampionName = "draven",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "dravendoubleshot",
                ChampionName = "draven",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "dravenrcast",
                ChampionName = "draven",
                HitType = new[] { Base.HitType.Danger, Base.HitType.Ultimate },
            });

            Spells.Add(new Somedata
            {
                SDataName = "infectedcleavermissilecast",
                ChampionName = "drmundo",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "burningagony",
                ChampionName = "drmundo",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "masochism",
                ChampionName = "drmundo",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "sadism",
                ChampionName = "drmundo",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "ekkoq",
                ChampionName = "ekko",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "ekkoeattack",
                ChampionName = "ekko",
                HitType = new[] { Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "ekkor",
                ChampionName = "ekko",
                HitType = new[] { Base.HitType.Danger, Base.HitType.Ultimate },
                FromObject = new[] { "Ekko_Base_R_TrailEnd" },
            });

            Spells.Add(new Somedata
            {
                SDataName = "elisehumanq",
                ChampionName = "elise",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "elisespiderqcast",
                ChampionName = "elise",
                HitType = new[] { Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "elisehumanw",
                ChampionName = "elise",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "elisespiderw",
                ChampionName = "elise",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "elisehumane",
                ChampionName = "elise",
                HitType = new[] { Base.HitType.CrowdControl, Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "elisespidereinitial",
                ChampionName = "elise",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "elisespideredescent",
                ChampionName = "elise",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "eliser",
                ChampionName = "elise",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "elisespiderr",
                ChampionName = "elise",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "evelynnq",
                ChampionName = "evelynn",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "evelynnw",
                ChampionName = "evelynn",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "evelynne",
                ChampionName = "evelynn",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "evelynnr",
                ChampionName = "evelynn",
                HitType =
                    new[]
                    {
                        Base.HitType.Danger, Base.HitType.Ultimate,
                        Base.HitType.CrowdControl
                    },
            });

            Spells.Add(new Somedata
            {
                SDataName = "ezrealmysticshot",
                ChampionName = "ezreal",
                HitType = new[] { Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "ezrealessenceflux",
                ChampionName = "ezreal",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "ezrealessencemissle",
                ChampionName = "ezreal",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "ezrealarcaneshift",
                ChampionName = "ezreal",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "ezrealtrueshotbarrage",
                ChampionName = "ezreal",
                HitType = new[] { Base.HitType.Danger, Base.HitType.Ultimate },
            });

            Spells.Add(new Somedata
            {
                SDataName = "terrify",
                ChampionName = "fiddlesticks",
                HitType = new[] { Base.HitType.Danger, Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "drain",
                ChampionName = "fiddlesticks",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "fiddlesticksdarkwind",
                ChampionName = "fiddlesticks",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "crowstorm",
                ChampionName = "fiddlesticks",
                HitType = new[] { Base.HitType.ForceExhaust },
            });

            Spells.Add(new Somedata
            {
                SDataName = "fioraq",
                ChampionName = "fiora",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "fioraw",
                ChampionName = "fiora",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "fiorae",
                ChampionName = "fiora",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "fiorar",
                ChampionName = "fiora",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "fizzpiercingstrike",
                ChampionName = "fizz",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "fizzseastonepassive",
                ChampionName = "fizz",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "fizzjump",
                ChampionName = "fizz",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "fizzjumpbuffer",
                ChampionName = "fizz",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "fizzjumptwo",
                ChampionName = "fizz",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "fizzmarinerdoom",
                ChampionName = "fizz",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "galioresolutesmite",
                ChampionName = "galio",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "galiobulwark",
                ChampionName = "galio",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "galiorighteousgust",
                ChampionName = "galio",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "galioidolofdurand",
                ChampionName = "galio",
                HitType =
                    new[]
                    {
                        Base.HitType.Danger, Base.HitType.Ultimate,
                        Base.HitType.CrowdControl
                    },
            });

            Spells.Add(new Somedata
            {
                SDataName = "gangplankqwrapper",
                ChampionName = "gangplank",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "gangplankqproceed",
                ChampionName = "gangplank",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "gangplankw",
                ChampionName = "gangplank",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "gangplanke",
                ChampionName = "gangplank",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "gangplankr",
                ChampionName = "gangplank",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "garenq",
                ChampionName = "garen",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "garenqattack",
                ChampionName = "garen",
                HitType = new[] { Base.HitType.CrowdControl, Base.HitType.Danger },
            });


            Spells.Add(new Somedata
            {
                SDataName = "gnarq",
                ChampionName = "gnar",
                HitType = new[] { Base.HitType.CrowdControl },
            });


            Spells.Add(new Somedata
            {
                SDataName = "gnarbigq",
                ChampionName = "gnar",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "gnarbigw",
                ChampionName = "gnar",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "gnarult",
                ChampionName = "gnar",
                HitType =
                    new[]
                    {
                        Base.HitType.Danger, Base.HitType.Ultimate,
                        Base.HitType.CrowdControl
                    },

            });

            Spells.Add(new Somedata
            {
                SDataName = "garenw",
                ChampionName = "garen",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "garene",
                ChampionName = "garen",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "garenr",
                ChampionName = "garen",
                HitType = new[] { Base.HitType.Danger, Base.HitType.Ultimate },
            });

            Spells.Add(new Somedata
            {
                SDataName = "gragasq",
                ChampionName = "gragas",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "gragasqtoggle",
                ChampionName = "gragas",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "gragasw",
                ChampionName = "gragas",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "gragase",
                ChampionName = "gragas",
                HitType = new[] { Base.HitType.Danger, Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "gragasr",
                ChampionName = "gragas",
                HitType = new[] { Base.HitType.Danger, Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "gravesq",
                ChampionName = "graves",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "gravesw",
                ChampionName = "graves",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "gravese",
                ChampionName = "graves",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "gravesr",
                ChampionName = "graves",
                HitType = new[] { Base.HitType.Danger, Base.HitType.Ultimate },
            });

            Spells.Add(new Somedata
            {
                SDataName = "hecarimrapidslash",
                ChampionName = "hecarim",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "hecarimw",
                ChampionName = "hecarim",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "hecarimramp",
                ChampionName = "hecarim",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "hecarimult",
                ChampionName = "hecarim",
                HitType =
                    new[]
                    {
                        Base.HitType.Danger, Base.HitType.Ultimate,
                        Base.HitType.CrowdControl
                    },
            });

            Spells.Add(new Somedata
            {
                SDataName = "heimerdingerturretenergyblast",
                ChampionName = "heimerdinger",
                HitType = new HitType[] { },
                FromObject = new[] { "heimerdinger_turret_idle" },
            });

            Spells.Add(new Somedata
            {
                SDataName = "heimerdingerturretbigenergyblast",
                ChampionName = "heimerdinger",
                HitType = new HitType[] { },
                FromObject = new[] { "heimerdinger_base_r" },
            });

            Spells.Add(new Somedata
            {
                SDataName = "heimerdingerw",
                ChampionName = "heimerdinger",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "heimerdingere",
                ChampionName = "heimerdinger",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "heimerdingerr",
                ChampionName = "heimerdinger",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "heimerdingereult",
                ChampionName = "heimerdinger",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "ireliagatotsu",
                ChampionName = "irelia",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "ireliahitenstyle",
                ChampionName = "irelia",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "ireliaequilibriumstrike",
                ChampionName = "irelia",
                HitType = new[] { Base.HitType.Danger, Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "ireliatranscendentblades",
                ChampionName = "irelia",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "illaoiq",
                ChampionName = "illaoi",
                HitType = new[] { Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "illaoiw",
                ChampionName = "illaoi",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "illaoie",
                ChampionName = "illaoi",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "illaoir",
                ChampionName = "illaoi",
                HitType = new[] { Base.HitType.Ultimate, Base.HitType.Danger, },
            });

            Spells.Add(new Somedata
            {
                SDataName = "howlinggalespell",
                ChampionName = "janna",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "sowthewind",
                ChampionName = "janna",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "eyeofthestorm",
                ChampionName = "janna",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "reapthewhirlwind",
                ChampionName = "janna",
                HitType = new[] { Base.HitType.Danger, Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "jarvanivdragonstrike",
                ChampionName = "jarvaniv",
                HitType = new[] { Base.HitType.Danger, Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "jarvanivgoldenaegis",
                ChampionName = "jarvaniv",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "jarvanivdemacianstandard",
                ChampionName = "jarvaniv",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "jarvanivcataclysm",
                ChampionName = "jarvaniv",
                HitType = new[] { Base.HitType.Danger, Base.HitType.Ultimate },
            });

            Spells.Add(new Somedata
            {
                SDataName = "jaxleapstrike",
                ChampionName = "jax",
                HitType = new[] { Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "jaxempowertwo",
                ChampionName = "jax",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "jaxrelentlessasssault",
                ChampionName = "jax",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "jaycetotheskies",
                ChampionName = "jayce",
                HitType = new[] { Base.HitType.CrowdControl, Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "jayceshockblast",
                ChampionName = "jayce",
                HitType = new[] { Base.HitType.CrowdControl, Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "jaycestaticfield",
                ChampionName = "jayce",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "jaycehypercharge",
                ChampionName = "jayce",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "jaycethunderingblow",
                ChampionName = "jayce",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "jayceaccelerationgate",
                ChampionName = "jayce",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "jaycestancehtg",
                ChampionName = "jayce",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "jaycestancegth",
                ChampionName = "jayce",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "jhinq",
                ChampionName = "jhin",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "jhinw",
                ChampionName = "jhin",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "jhine",
                ChampionName = "jhin",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "jhinrshot",
                ChampionName = "jhin",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "jinxq",
                ChampionName = "jinx",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "jinxw",
                ChampionName = "jinx",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "jinxe",
                ChampionName = "jinx",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "jinxr",
                ChampionName = "jinx",
                HitType = new[] { Base.HitType.Danger, Base.HitType.Ultimate },
            });

            Spells.Add(new Somedata
            {
                SDataName = "karmaq",
                ChampionName = "karma",
                HitType = new[] { Base.HitType.Danger, Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "karmaspiritbind",
                ChampionName = "karma",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "karmasolkimshield",
                ChampionName = "karma",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "karmamantra",
                ChampionName = "karma",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "laywaste",
                ChampionName = "karthus",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "wallofpain",
                ChampionName = "karthus",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "defile",
                ChampionName = "karthus",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "fallenone",
                ChampionName = "karthus",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "nulllance",
                ChampionName = "kassadin",
                HitType = new[] { Base.HitType.CrowdControl, Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "netherblade",
                ChampionName = "kassadin",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "forcepulse",
                ChampionName = "kassadin",
                HitType = new[] { Base.HitType.CrowdControl, Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "riftwalk",
                ChampionName = "kassadin",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "katarinaq",
                ChampionName = "katarina",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "katarinaw",
                ChampionName = "katarina",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "katarinae",
                ChampionName = "katarina",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "katarinar",
                ChampionName = "katarina",
                HitType = new[] { Base.HitType.ForceExhaust },
            });

            Spells.Add(new Somedata
            {
                SDataName = "judicatorreckoning",
                ChampionName = "kayle",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "judicatordevineblessing",
                ChampionName = "kayle",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "judicatorrighteousfury",
                ChampionName = "kayle",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "judicatorintervention",
                ChampionName = "kayle",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "kennenshurikenhurlmissile1",
                ChampionName = "kennen",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "kennenbringthelight",
                ChampionName = "kennen",
                HitType = new[] { Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "kennenlightningrush",
                ChampionName = "kennen",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "kennenshurikenstorm",
                ChampionName = "kennen",
                HitType = new[] { Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "khazixq",
                ChampionName = "khazix",
                HitType = new[] { Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "khazixqlong",
                ChampionName = "khazix",
                HitType = new[] { Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "khazixw",
                ChampionName = "khazix",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "khazixwlong",
                ChampionName = "khazix",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "khazixe",
                ChampionName = "khazix",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "khazixelong",
                ChampionName = "khazix",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "khazixr",
                ChampionName = "khazix",
                HitType = new[] { Base.HitType.Stealth },
            });

            Spells.Add(new Somedata
            {
                SDataName = "khazixrlong",
                ChampionName = "khazix",
                HitType = new[] { Base.HitType.Stealth },
            });

            Spells.Add(new Somedata
            {
                SDataName = "kindredq",
                ChampionName = "kindred",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "kindrede",
                ChampionName = "kindred",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "kogmawq",
                ChampionName = "kogmaw",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "kogmawbioarcanebarrage",
                ChampionName = "kogmaw",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "kogmawvoidooze",
                ChampionName = "kogmaw",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "kogmawlivingartillery",
                ChampionName = "kogmaw",
                HitType = new[] { Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "leblancchaosorb",
                ChampionName = "leblanc",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "leblancslide",
                ChampionName = "leblanc",
                HitType = new[] { Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "leblacslidereturn",
                ChampionName = "leblanc",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "leblancsoulshackle",
                ChampionName = "leblanc",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "leblancchaosorbm",
                ChampionName = "leblanc",
                HitType = new[] { Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "leblancslidem",
                ChampionName = "leblanc",
                HitType = new[] { Base.HitType.Danger, Base.HitType.Ultimate },
            });

            Spells.Add(new Somedata
            {
                SDataName = "leblancslidereturnm",
                ChampionName = "leblanc",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "leblancsoulshacklem",
                ChampionName = "leblanc",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "blindmonkqone",
                ChampionName = "leesin",
                HitType = new[] { Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "blindmonkqtwo",
                ChampionName = "leesin",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "blindmonkwone",
                ChampionName = "leesin",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "blindmonkwtwo",
                ChampionName = "leesin",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "blindmonkeone",
                ChampionName = "leesin",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "blindmonketwo",
                ChampionName = "leesin",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "blindmonkrkick",
                ChampionName = "leesin",
                HitType =
                    new[]
                    {
                        Base.HitType.Danger, Base.HitType.Ultimate,
                        Base.HitType.CrowdControl
                    },
            });

            Spells.Add(new Somedata
            {
                SDataName = "leonashieldofdaybreak",
                ChampionName = "leona",
                HitType = new[] { Base.HitType.Danger, Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "leonasolarbarrier",
                ChampionName = "leona",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "leonazenithblade",
                ChampionName = "leona",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "leonasolarflare",
                ChampionName = "leona",
                HitType = new[] { Base.HitType.Danger, Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "lissandraq",
                ChampionName = "lissandra",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "lissandraw",
                ChampionName = "lissandra",
                HitType = new[] { Base.HitType.Danger, Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "lissandrae",
                ChampionName = "lissandra",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "lissandrar",
                ChampionName = "lissandra",
                HitType = new[] { Base.HitType.CrowdControl, Base.HitType.Danger, Base.HitType.Ultimate },
            });

            Spells.Add(new Somedata
            {
                SDataName = "lucianq",
                ChampionName = "lucian",
                HitType = new[] { Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "lucianw",
                ChampionName = "lucian",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "luciane",
                ChampionName = "lucian",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "lucianr",
                ChampionName = "lucian",
                HitType = new[] { Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "luluq",
                ChampionName = "lulu",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "luluw",
                ChampionName = "lulu",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "lulue",
                ChampionName = "lulu",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "lulur",
                ChampionName = "lulu",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "luxlightbinding",
                ChampionName = "lux",
                HitType = new[] { Base.HitType.Danger, Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "luxprismaticwave",
                ChampionName = "lux",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "luxlightstrikekugel",
                ChampionName = "lux",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "luxlightstriketoggle",
                ChampionName = "lux",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "luxmalicecannon",
                ChampionName = "lux",
                HitType = new[] { Base.HitType.Danger, Base.HitType.Ultimate },
            });

            Spells.Add(new Somedata
            {
                SDataName = "kalistamysticshot",
                ChampionName = "kalista",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "kalistaw",
                ChampionName = "kalista",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "kalistaexpungewrapper",
                ChampionName = "kalista",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "seismicshard",
                ChampionName = "malphite",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "obduracy",
                ChampionName = "malphite",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "landslide",
                ChampionName = "malphite",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "ufslash",
                ChampionName = "malphite",
                HitType =
                    new[]
                    {
                        Base.HitType.Danger, Base.HitType.Ultimate,
                        Base.HitType.CrowdControl
                    },
            });

            Spells.Add(new Somedata
            {
                SDataName = "malzaharq",
                ChampionName = "malzahar",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "malzaharw",
                ChampionName = "malzahar",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "malzahare",
                ChampionName = "malzahar",
                HitType = new[] { Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "malzaharr",
                ChampionName = "malzahar",
                HitType =
                    new[]
                    {
                        Base.HitType.Danger, Base.HitType.Ultimate,
                        Base.HitType.CrowdControl
                    },
            });

            Spells.Add(new Somedata
            {
                SDataName = "maokaitrunkline",
                ChampionName = "maokai",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "maokaiunstablegrowth",
                ChampionName = "maokai",
                HitType = new[] { Base.HitType.Danger, Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "maokaisapling2",
                ChampionName = "maokai",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "maokaidrain3",
                ChampionName = "maokai",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "alphastrike",
                ChampionName = "masteryi",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "meditate",
                ChampionName = "masteryi",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "wujustyle",
                ChampionName = "masteryi",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "highlander",
                ChampionName = "masteryi",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "missfortunericochetshot",
                ChampionName = "missfortune",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "missfortuneviciousstrikes",
                ChampionName = "missfortune",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "missfortunescattershot",
                ChampionName = "missfortune",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "missfortunebullettime",
                ChampionName = "missfortune",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "monkeykingdoubleattack",
                ChampionName = "monkeyking",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "monkeykingdecoy",
                ChampionName = "monkeyking",
                HitType = new[] { Base.HitType.Stealth },
            });

            Spells.Add(new Somedata
            {
                SDataName = "monkeykingdecoyswipe",
                ChampionName = "monkeyking",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "monkeykingnimbus",
                ChampionName = "monkeyking",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "monkeykingspintowin",
                ChampionName = "monkeyking",
                HitType =
                    new[]
                    {
                        Base.HitType.Danger, Base.HitType.Ultimate,
                        Base.HitType.CrowdControl
                    },
            });

            Spells.Add(new Somedata
            {
                SDataName = "monkeykingspintowinleave",
                ChampionName = "monkeyking",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "mordekaisermaceofspades",
                ChampionName = "mordekaiser",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "mordekaisercreepindeathcast",
                ChampionName = "mordekaiser",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "mordekaisersyphoneofdestruction",
                ChampionName = "mordekaiser",
                HitType = new[] { Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "mordekaiserchildrenofthegrave",
                ChampionName = "mordekaiser",
                HitType = new[] { Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "darkbindingmissile",
                ChampionName = "morgana",
                HitType = new[] { Base.HitType.Danger, Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "tormentedsoil",
                ChampionName = "morgana",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "blackshield",
                ChampionName = "morgana",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "soulshackles",
                ChampionName = "morgana",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "namiq",
                ChampionName = "nami",
                HitType = new[] { Base.HitType.Danger, Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "namiw",
                ChampionName = "nami",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "namie",
                ChampionName = "nami",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "namir",
                ChampionName = "nami",
                HitType = new[] { Base.HitType.Danger, Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "nasusq",
                ChampionName = "nasus",
                HitType = new[] { Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "nasusw",
                ChampionName = "nasus",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "nasuse",
                ChampionName = "nasus",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "nasusr",
                ChampionName = "nasus",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "nautilusanchordrag",
                ChampionName = "nautilus",
                HitType = new[] { Base.HitType.CrowdControl, Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "nautiluspiercinggaze",
                ChampionName = "nautilus",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "nautilussplashzone",
                ChampionName = "nautilus",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "nautilusgandline",
                ChampionName = "nautilus",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "javelintoss",
                ChampionName = "nidalee",
                HitType = new[] { Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "takedown",
                ChampionName = "nidalee",
                HitType = new[] { Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "bushwhack",
                ChampionName = "nidalee",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "pounce",
                ChampionName = "nidalee",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "primalsurge",
                ChampionName = "nidalee",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "swipe",
                ChampionName = "nidalee",
                HitType = new[] { Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "aspectofthecougar",
                ChampionName = "nidalee",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "nocturneduskbringer",
                ChampionName = "nocturne",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "nocturneshroudofdarkness",
                ChampionName = "nocturne",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "nocturneunspeakablehorror",
                ChampionName = "nocturne",
                HitType = new[] { Base.HitType.Danger, Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "nocturneparanoia",
                ChampionName = "nocturne",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "consume",
                ChampionName = "nunu",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "bloodboil",
                ChampionName = "nunu",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "iceblast",
                ChampionName = "nunu",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "absolutezero",
                ChampionName = "nunu",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "olafaxethrowcast",
                ChampionName = "olaf",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "olaffrenziedstrikes",
                ChampionName = "olaf",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "olafrecklessstrike",
                ChampionName = "olaf",
                HitType = new[] { Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "olafragnarok",
                ChampionName = "olaf",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "orianaizunacommand",
                ChampionName = "orianna",
                HitType = new HitType[] { },
                FromObject = new[] { "yomu_ring" },
            });

            Spells.Add(new Somedata
            {
                SDataName = "orianadissonancecommand",
                ChampionName = "orianna",
                HitType = new[] { Base.HitType.CrowdControl },
                FromObject = new[] { "yomu_ring" },
            });

            Spells.Add(new Somedata
            {
                SDataName = "orianaredactcommand",
                ChampionName = "orianna",
                HitType = new HitType[] { },
                FromObject = new[] { "yomu_ring" },
            });

            Spells.Add(new Somedata
            {
                SDataName = "orianadetonatecommand",
                ChampionName = "orianna",
                HitType =
                    new[]
                    {
                        Base.HitType.Danger, Base.HitType.Ultimate,
                        Base.HitType.CrowdControl
                    },
                FromObject = new[] { "yomu_ring" },
            });

            Spells.Add(new Somedata
            {
                SDataName = "pantheonq",
                ChampionName = "pantheon",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "pantheonw",
                ChampionName = "pantheon",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "pantheone",
                ChampionName = "pantheon",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "pantheonrjump",
                ChampionName = "pantheon",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "pantheonrfall",
                ChampionName = "pantheon",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "poppyq",
                ChampionName = "poppy",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "poppyw",
                ChampionName = "poppy",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "poppye",
                ChampionName = "poppy",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "poppyrspell",
                ChampionName = "poppy",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "poppyrspellinstant",
                ChampionName = "poppy",
                HitType = new[] { Base.HitType.Danger, Base.HitType.Ultimate },
            });

            Spells.Add(new Somedata
            {
                SDataName = "quinnq",
                ChampionName = "quinn",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "quinnw",
                ChampionName = "quinn",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "quinne",
                ChampionName = "quinn",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "quinnr",
                ChampionName = "quinn",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "quinnrfinale",
                ChampionName = "quinn",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "powerball",
                ChampionName = "rammus",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "defensiveballcurl",
                ChampionName = "rammus",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "puncturingtaunt",
                ChampionName = "rammus",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "tremors2",
                ChampionName = "rammus",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "renektoncleave",
                ChampionName = "renekton",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "renektonpreexecute",
                ChampionName = "renekton",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "renektonsliceanddice",
                ChampionName = "renekton",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "renektonreignofthetyrant",
                ChampionName = "renekton",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "rengarq",
                ChampionName = "rengar",
                HitType = new[] { Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "rengarw",
                ChampionName = "rengar",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "rengare",
                ChampionName = "rengar",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "rengarr",
                ChampionName = "rengar",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "reksaiq",
                ChampionName = "reksai",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "reksaiqburrowed",
                ChampionName = "reksai",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "reksaiw",
                ChampionName = "reksai",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "reksaiwburrowed",
                ChampionName = "reksai",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "reksaie",
                ChampionName = "reksai",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "reksaieburrowed",
                ChampionName = "reksai",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "reksair",
                ChampionName = "reksai",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "riventricleave",
                ChampionName = "riven",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "rivenmartyr",
                ChampionName = "riven",
                HitType = new[] { Base.HitType.Danger, Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "rivenfeint",
                ChampionName = "riven",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "rivenfengshuiengine",
                ChampionName = "riven",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "rivenizunablade",
                ChampionName = "riven",
                HitType = new[] { Base.HitType.Danger, Base.HitType.Ultimate },
            });

            Spells.Add(new Somedata
            {
                SDataName = "rumbleflamethrower",
                ChampionName = "rumble",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "rumbleshield",
                ChampionName = "rumble",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "rumbegrenade",
                ChampionName = "rumble",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "rumblecarpetbomb",
                ChampionName = "rumble",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "ryzeq",
                ChampionName = "ryze",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "ryzew",
                ChampionName = "ryze",
                HitType = new[] { Base.HitType.CrowdControl, Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "ryzee",
                ChampionName = "ryze",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "ryzer",
                ChampionName = "ryze",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "sejuaniarcticassault",
                ChampionName = "sejuani",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "sejuaninorthernwinds",
                ChampionName = "sejuani",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "sejuaniwintersclaw",
                ChampionName = "sejuani",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "sejuaniglacialprisoncast",
                ChampionName = "sejuani",
                HitType =
                    new[]
                    {
                        Base.HitType.Danger, Base.HitType.Ultimate,
                        Base.HitType.CrowdControl
                    },
            });

            Spells.Add(new Somedata
            {
                SDataName = "deceive",
                ChampionName = "shaco",
                HitType = new[] { Base.HitType.Stealth },
            });

            Spells.Add(new Somedata
            {
                SDataName = "jackinthebox",
                ChampionName = "shaco",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "twoshivpoison",
                ChampionName = "shaco",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "hallucinatefull",
                ChampionName = "shaco",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "shenq",
                ChampionName = "shen",
                HitType = new HitType[] { },
                FromObject = new[] { "ShenArrowVfxHostMinion" },
            });

            Spells.Add(new Somedata
            {
                SDataName = "shenw",
                ChampionName = "shen",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "shene",
                ChampionName = "shen",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "shenr",
                ChampionName = "shen",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "shyvanadoubleattack",
                ChampionName = "shyvana",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "shyvanadoubleattackdragon",
                ChampionName = "shyvana",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "shyvanaimmolationauraqw",
                ChampionName = "shyvana",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "shyvanaimmolateddragon",
                ChampionName = "shyvana",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "shyvanafireball",
                ChampionName = "shyvana",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "shyvanafireballdragon2",
                ChampionName = "shyvana",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "shyvanatransformcast",
                ChampionName = "shyvana",
                HitType =
                    new[]
                    {
                        Base.HitType.Danger, Base.HitType.CrowdControl,
                        Base.HitType.Ultimate
                    },
            });

            Spells.Add(new Somedata
            {
                SDataName = "poisentrail",
                ChampionName = "singed",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "megaadhesive",
                ChampionName = "singed",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "fling",
                ChampionName = "singed",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "insanitypotion",
                ChampionName = "singed",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "sionq",
                ChampionName = "sion",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "sionwdetonate",
                ChampionName = "sion",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "sione",
                ChampionName = "sion",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "sionr",
                ChampionName = "sion",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "sivirq",
                ChampionName = "sivir",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "sivirw",
                ChampionName = "sivir",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "sivire",
                ChampionName = "sivir",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "sivirr",
                ChampionName = "sivir",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "skarnervirulentslash",
                ChampionName = "skarner",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "skarnerexoskeleton",
                ChampionName = "skarner",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "skarnerfracture",
                ChampionName = "skarner",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "skarnerimpale",
                ChampionName = "skarner",
                HitType = new[] { Base.HitType.Danger, Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "sonaq",
                ChampionName = "sona",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "sonaw",
                ChampionName = "sona",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "sonae",
                ChampionName = "sona",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "sonar",
                ChampionName = "sona",
                HitType =
                    new[]
                    {
                        Base.HitType.Danger, Base.HitType.Ultimate,
                        Base.HitType.CrowdControl
                    },
            });

            Spells.Add(new Somedata
            {
                SDataName = "sorakaq",
                ChampionName = "soraka",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "sorakaw",
                ChampionName = "soraka",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "sorakae",
                ChampionName = "soraka",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "sorakar",
                ChampionName = "soraka",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "swaindecrepify",
                ChampionName = "swain",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "swainshadowgrasp",
                ChampionName = "swain",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "swaintorment",
                ChampionName = "swain",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "swainmetamorphism",
                ChampionName = "swain",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "syndraq",
                ChampionName = "syndra",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "syndrawcast",
                ChampionName = "syndra",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "syndrae",
                ChampionName = "syndra",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "syndrar",
                ChampionName = "syndra",
                HitType = new[] { Base.HitType.Danger, Base.HitType.Ultimate },
            });

            Spells.Add(new Somedata
            {
                SDataName = "tahmkenchq",
                ChampionName = "tahmkench",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "talonnoxiandiplomacy",
                ChampionName = "talon",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "talonrake",
                ChampionName = "talon",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "taloncutthroat",
                ChampionName = "talon",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "taliyahq",
                ChampionName = "taliyah",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "taliyahw",
                ChampionName = "taliyah",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "taliyahe",
                ChampionName = "taliyah",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "talonshadowassault",
                ChampionName = "talon",
                HitType = new[] { Base.HitType.Stealth },
            });

            Spells.Add(new Somedata
            {
                SDataName = "taricq",
                ChampionName = "taric",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "taricw",
                ChampionName = "taric",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "tarice",
                ChampionName = "taric",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "taricr",
                ChampionName = "taric",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "blindingdart",
                ChampionName = "teemo",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "movequick",
                ChampionName = "teemo",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "toxicshot",
                ChampionName = "teemo",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "bantamtrap",
                ChampionName = "teemo",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "threshq",
                ChampionName = "thresh",
                HitType = new[] { Base.HitType.CrowdControl, Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "threshw",
                ChampionName = "thresh",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "threshe",
                ChampionName = "thresh",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "threshrpenta",
                ChampionName = "thresh",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "tristanaq",
                ChampionName = "tristana",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "tristanaw",
                ChampionName = "tristana",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "tristanae",
                ChampionName = "tristana",
                HitType = new[] { Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "tristanar",
                ChampionName = "tristana",
                HitType = new[] { Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "trundletrollsmash",
                ChampionName = "trundle",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "trundledesecrate",
                ChampionName = "trundle",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "trundlecircle",
                ChampionName = "trundle",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "trundlepain",
                ChampionName = "trundle",
                HitType = new[] { Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "bloodlust",
                ChampionName = "tryndamere",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "mockingshout",
                ChampionName = "tryndamere",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "slashcast",
                ChampionName = "tryndamere",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "undyingrage",
                ChampionName = "tryndamere",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "twitchhideinshadows",
                ChampionName = "twitch",
                HitType = new[] { Base.HitType.Stealth },
            });

            Spells.Add(new Somedata
            {
                SDataName = "twitchvenomcask",
                ChampionName = "twitch",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "twitchvenomcaskmissle",
                ChampionName = "twitch",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "twitchexpunge",
                ChampionName = "twitch",
                HitType = new[] { Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "twitchsprayandprayattack",
                ChampionName = "twitch",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "twitchfullautomatic",
                ChampionName = "twitch",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "wildcards",
                ChampionName = "twistedfate",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "pickacard",
                ChampionName = "twistedfate",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "goldcardpreattack",
                ChampionName = "twistedfate",
                HitType = new[] { Base.HitType.CrowdControl, Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "redcardpreattack",
                ChampionName = "twistedfate",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "bluecardpreattack",
                ChampionName = "twistedfate",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "cardmasterstack",
                ChampionName = "twistedfate",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "destiny",
                ChampionName = "twistedfate",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "udyrtigerstance",
                ChampionName = "udyr",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "udyrturtlestance",
                ChampionName = "udyr",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "udyrbearstanceattack",
                ChampionName = "udyr",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "udyrphoenixstance",
                ChampionName = "udyr",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "urgotheatseekinglineqqmissile",
                ChampionName = "urgot",
                HitType = new[] { Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "urgotheatseekingmissile",
                ChampionName = "urgot",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "urgotterrorcapacitoractive2",
                ChampionName = "urgot",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "urgotplasmagrenade",
                ChampionName = "urgot",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "urgotplasmagrenadeboom",
                ChampionName = "urgot",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "urgotswap2",
                ChampionName = "urgot",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "varusq",
                ChampionName = "varus",
                HitType = new[] { Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "varusw",
                ChampionName = "varus",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "varuse",
                ChampionName = "varus",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "varusr",
                ChampionName = "varus",
                HitType =
                    new[]
                    {
                        Base.HitType.Danger, Base.HitType.Ultimate,
                        Base.HitType.CrowdControl
                    },
            });

            Spells.Add(new Somedata
            {
                SDataName = "vaynetumble",
                ChampionName = "vayne",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "vaynesilverbolts",
                ChampionName = "vayne",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "vaynecondemnmissile",
                ChampionName = "vayne",
                HitType = new[] { Base.HitType.CrowdControl, Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "vayneinquisition",
                ChampionName = "vayne",
                HitType = new[] { Base.HitType.Stealth },
            });

            Spells.Add(new Somedata
            {
                SDataName = "veigarbalefulstrike",
                ChampionName = "veigar",
                HitType = new[] { Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "veigardarkmatter",
                ChampionName = "veigar",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "veigareventhorizon",
                ChampionName = "veigar",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "veigarprimordialburst",
                ChampionName = "veigar",
                HitType = new[] { Base.HitType.Danger, Base.HitType.Ultimate },
            });

            Spells.Add(new Somedata
            {
                SDataName = "velkozq",
                ChampionName = "velkoz",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "velkozqsplitactivate",
                ChampionName = "velkoz",
                HitType = new[] { Base.HitType.CrowdControl, Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "velkozw",
                ChampionName = "velkoz",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "velkoze",
                ChampionName = "velkoz",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "velkozr",
                ChampionName = "velkoz",
                HitType = new[] { Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "viq",
                ChampionName = "vi",
                HitType = new[] { Base.HitType.Danger, Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "viw",
                ChampionName = "vi",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "vie",
                ChampionName = "vi",
                HitType = new[] { Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "vir",
                ChampionName = "vi",
                HitType = new[] { Base.HitType.Danger, Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "viktorpowertransfer",
                ChampionName = "viktor",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "viktorgravitonfield",
                ChampionName = "viktor",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "viktordeathray",
                ChampionName = "viktor",
                HitType = new[] { Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "viktorchaosstorm",
                ChampionName = "viktor",
                HitType =
                    new[]
                    {
                        Base.HitType.CrowdControl, Base.HitType.Ultimate,
                        Base.HitType.Danger
                    },
            });

            Spells.Add(new Somedata
            {
                SDataName = "vladimirq",
                ChampionName = "vladimir",
                HitType = new[] { Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "vladimirw",
                ChampionName = "vladimir",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "vladimire",
                ChampionName = "vladimir",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "vladimirr",
                ChampionName = "vladimir",
                HitType = new[] { Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "volibearq",
                ChampionName = "volibear",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "volibearw",
                ChampionName = "volibear",
                HitType = new[] { Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "volibeare",
                ChampionName = "volibear",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "volibearr",
                ChampionName = "volibear",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "hungeringstrike",
                ChampionName = "warwick",
                HitType = new[] { Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "hunterscall",
                ChampionName = "warwick",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "bloodscent",
                ChampionName = "warwick",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "infiniteduress",
                ChampionName = "warwick",
                HitType =
                    new[]
                    {
                        Base.HitType.Danger, Base.HitType.Ultimate,
                        Base.HitType.CrowdControl
                    },
            });

            Spells.Add(new Somedata
            {
                SDataName = "xeratharcanopulsechargeup",
                ChampionName = "xerath",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "xeratharcanebarrage2",
                ChampionName = "xerath",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "xerathmagespear",
                ChampionName = "xerath",
                HitType = new[] { Base.HitType.CrowdControl, Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "xerathlocusofpower2",
                ChampionName = "xerath",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "xenzhaothrust3",
                ChampionName = "xinzhao",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "xenzhaobattlecry",
                ChampionName = "xinzhao",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "xenzhaosweep",
                ChampionName = "xinzhao",
                HitType = new[] { Base.HitType.CrowdControl, Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "xenzhaoparry",
                ChampionName = "xinzhao",
                HitType =
                    new[]
                    {
                        Base.HitType.Danger, Base.HitType.Ultimate,
                        Base.HitType.CrowdControl
                    },
            });

            Spells.Add(new Somedata
            {
                SDataName = "yasuoqw",
                ChampionName = "yasuo",
                HitType = new[] { Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "yasuoq2w",
                ChampionName = "yasuo",
                HitType = new[] { Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "yasuoq3",
                ChampionName = "yasuo",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "yasuowmovingwall",
                ChampionName = "yasuo",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "yasuodashwrapper",
                ChampionName = "yasuo",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "yasuorknockupcombow",
                ChampionName = "yasuo",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "yorickspectral",
                ChampionName = "yorick",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "yorickdecayed",
                ChampionName = "yorick",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "yorickravenous",
                ChampionName = "yorick",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "yorickreviveally",
                ChampionName = "yorick",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "zacq",
                ChampionName = "zac",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "zacw",
                ChampionName = "zac",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "zace",
                ChampionName = "zac",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "zacr",
                ChampionName = "zac",
                HitType = new[] { Base.HitType.Danger, Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "zedq",
                ChampionName = "zed",
                HitType = new HitType[] { },
                FromObject = new[] { "Zed_Base_W_tar.troy", "Zed_Base_W_cloneswap_buf.troy" },
            });

            Spells.Add(new Somedata
            {
                SDataName = "zedw",
                ChampionName = "zed",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "zede",
                ChampionName = "zed",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "zedr",
                ChampionName = "zed",
                HitType = new[] { Base.HitType.Danger },
            });

            Spells.Add(new Somedata
            {
                SDataName = "ziggsq",
                ChampionName = "ziggs",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "ziggsw",
                ChampionName = "ziggs",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "ziggswtoggle",
                ChampionName = "ziggs",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "ziggse",
                ChampionName = "ziggs",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "ziggse2",
                ChampionName = "ziggs",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "ziggsr",
                ChampionName = "ziggs",
                HitType = new[] { Base.HitType.Danger, Base.HitType.Ultimate },
            });

            Spells.Add(new Somedata
            {
                SDataName = "zileanq",
                ChampionName = "zilean",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "zileanw",
                ChampionName = "zilean",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "zileane",
                ChampionName = "zilean",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "zileanr",
                ChampionName = "zilean",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "zyraq",
                ChampionName = "zyra",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "zyraw",
                ChampionName = "zyra",
                HitType = new HitType[] { },
            });

            Spells.Add(new Somedata
            {
                SDataName = "zyrae",
                ChampionName = "zyra",
                HitType = new[] { Base.HitType.CrowdControl },
            });

            Spells.Add(new Somedata
            {
                SDataName = "zyrar",
                ChampionName = "zyra",
                HitType =
                    new[]
                    {
                        Base.HitType.Danger, Base.HitType.Ultimate,
                        Base.HitType.CrowdControl
                    },
            });
        }

        public static List<Somedata> Spells = new List<Somedata>();
        public static List<Somedata> SomeSpells = new List<Somedata>();

        public static Dictionary<SpellDamageDelegate, SpellSlot> DamageLib =
            new Dictionary<SpellDamageDelegate, SpellSlot>();

        public static Somedata GetByMissileName(string missilename)
        {
            foreach (var sdata in Spells)
            {
                if (sdata.MissileName != null && sdata.MissileName.ToLower() == missilename ||
                    sdata.ExtraMissileNames != null && sdata.ExtraMissileNames.Contains(missilename))
                {
                    return sdata;
                }
            }

            return null;
        }
    }
}
