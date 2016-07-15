using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using VayneHunter_Reborn.Modules.ModuleHelpers;
using VayneHunter_Reborn.Skills.Condemn;
using VayneHunter_Reborn.Utility;
using VayneHunter_Reborn.Utility.MenuUtility;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

using TargetSelector = PortAIO.TSManager; namespace VayneHunter_Reborn.Modules.ModuleList.Condemn
{
    class CondemnJungleMobs : IModule
    {
        private static readonly string[] MobNames =
            {
                "SRU_Red", "SRU_Blue", "SRU_Gromp", "SRU_Murkwolf",
                "SRU_Razorbeak", "SRU_Krug", "Sru_Crab",
                "TT_Spiderboss", "TTNGolem", "TTNWolf",
                "TTNWraith"
            };

        public void OnLoad()
        {

        }

        public bool ShouldGetExecuted()
        {
            return MenuGenerator.farmMenu["dz191.vhr.farm.condemnjungle"].Cast<CheckBox>().CurrentValue &&
                   Variables.spells[SpellSlot.E].IsReady() && (PortAIO.OrbwalkerManager.isLaneClearActive);
        }
        

        public ModuleType GetModuleType()
        {
            return ModuleType.OnAfterAA;
        }

        public void OnExecute()
        {
            var owTarget = PortAIO.OrbwalkerManager.LastTarget();
            if (owTarget is Obj_AI_Base && MobNames.Contains((owTarget as Obj_AI_Base).CharData.BaseSkinName))
            {
                var owTargetBase = (owTarget as Obj_AI_Base);
                if (owTargetBase.IsCondemnable(ObjectManager.Player.ServerPosition))
                {
                    Variables.spells[SpellSlot.E].CastOnUnit(owTargetBase);
                }
            }
        }
    }
}
