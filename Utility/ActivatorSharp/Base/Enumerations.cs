#region Copyright © 2015 Kurisu Solutions
// All rights are reserved. Transmission or reproduction in part or whole,
// any form or by any means, mechanical, electronical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
// 
// Document:	Base/Enumerations.cs
// Date:		22/09/2015
// Author:		Robin Kurisu
#endregion

using Activators.Items;
using Activators.Spells;
using Activators.Summoners;
using System.Collections.Generic;

using TargetSelector = PortAIO.TSManager; namespace Activators.Base
{
    public class Lists
    {
        public static List<CoreItem> Items = new List<CoreItem>();
        public static List<CoreItem> BoughtItems = new List<CoreItem>();
        public static List<CoreSpell> Spells = new List<CoreSpell>();
        public static List<CoreSum> Summoners = new List<CoreSum>();
    }

    public enum HitType
    {
        None,
        AutoAttack,
        MinionAttack,
        TurretAttack,
        Spell,
        Danger,
        Ultimate,
        CrowdControl,
        Stealth,
        ForceExhaust
    }

    public enum MapType
    {        
        Common = 0,
        SummonersRift = 1,
        CrystalScar = 2,
        TwistedTreeline = 3,
        HowlingAbyss = 4
    }

    public enum MenuType
    {
        Zhonyas,
        Stealth,
        Cleanse,
        SlowRemoval,
        SpellShield,
        ActiveCheck,
        SelfCount,
        SelfMuchHP,
        SelfLowHP,
        SelfLowMP,
        SelfMinMP,
        SelfMinHP,
        EnemyLowHP
    }

}
