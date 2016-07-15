#region Copyright © 2015 Kurisu Solutions
// All rights are reserved. Transmission or reproduction in part or whole,
// any form or by any means, mechanical, electronical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
// 
// Document:	Base/Gametroy.cs
// Date:		22/09/2015
// Author:		Robin Kurisu
#endregion

using EloBuddy;
using LeagueSharp;
using System.Collections.Generic;

using TargetSelector = PortAIO.TSManager; namespace Activators.Base
{
    public class Gametroy
    {
        public int Damage;
        public bool Included;
        public string Name;
        public GameObject Obj;
        public string Owner;
        public SpellSlot Slot;
        public int Start;
        public int Limiter;

        public Gametroy(
            string owner, 
            SpellSlot slot, 
            string name, 
            int start, 
            bool inculded, 
            int incdmg = 0,
            GameObject obj = null)
        {
            Owner = owner;
            Slot = slot;
            Start = start;
            Name = name;
            Obj = obj;
            Included = inculded;
            Damage = incdmg;
        }

        public static List<Gametroy> Objects = new List<Gametroy>(); 

        static Gametroy()
        {
            
        }
    }
}
