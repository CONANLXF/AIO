using EloBuddy.SDK;
using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

 namespace Azir_Creator_of_Elo
{
    class Modes
    {
        
        public virtual void Update(AzirMain azir)
        {

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo(azir);
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harash(azir);
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
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
