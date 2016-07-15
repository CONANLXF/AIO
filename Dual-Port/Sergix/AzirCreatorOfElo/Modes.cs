using EloBuddy.SDK;
using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TargetSelector = PortAIO.TSManager; namespace Azir_Creator_of_Elo
{
    class Modes
    {
        
        public virtual void Update(AzirMain azir)
        {

            if (PortAIO.OrbwalkerManager.isComboActive)
            {
                Combo(azir);
            }

            if (PortAIO.OrbwalkerManager.isHarassActive)
            {
                Harash(azir);
            }

            if (PortAIO.OrbwalkerManager.isLaneClearActive)
            {
                Laneclear(azir);
                Jungleclear(azir);
            }


        }
        public virtual void Jungleclear(AzirMain azir)
        {

        }

        public virtual void Laneclear(AzirMain azir)
        {

        }

        public virtual void Harash(AzirMain azir)
        {

        }

        public virtual void Combo(AzirMain azir)
        {



        }
    }
}
