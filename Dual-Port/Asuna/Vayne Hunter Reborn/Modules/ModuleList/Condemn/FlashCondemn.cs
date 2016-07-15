using System;
using System.Linq;
using EloBuddy;
using LeagueSharp.Common;
using VayneHunter_Reborn.Modules.ModuleHelpers;
using VayneHunter_Reborn.Skills.Tumble.VHRQ;
using VayneHunter_Reborn.Utility;
using VayneHunter_Reborn.Utility.MenuUtility;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;
using Spell = LeagueSharp.Common.Spell;


using TargetSelector = PortAIO.TSManager; namespace VayneHunter_Reborn.Modules.ModuleList.Condemn
{
    using Utility = LeagueSharp.Common.Utility;

    class FlashCondemn : IModule
    {
        private static Spell E => Variables.spells[SpellSlot.E];

        private static Spell Flash => new Spell(ObjectManager.Player.GetSpellSlot("SummonerFlash"), 425f);

        public void OnLoad()
        {
        }


        public bool ShouldGetExecuted()
        {
            return MenuGenerator.miscMenu["dz191.vhr.misc.condemn.flashcondemn"].Cast<KeyBind>().CurrentValue
                   && Variables.spells[SpellSlot.E].IsReady() && Flash.Slot != SpellSlot.Unknown && Flash.IsReady();
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public void OnExecute()
        {
            var pushDistance = 450;

            var target = TargetSelector.SelectedTarget != null
                             ? TargetSelector.SelectedTarget
                             : TargetSelector.GetTarget(E.Range, DamageType.Physical);

            var flashPosition = ObjectManager.Player.ServerPosition.LSExtend(Game.CursorPos, Flash.Range);

            var prediction = E.GetPrediction(target);

            if (target.LSIsDashing() || !E.IsReady()) return;

            if (prediction.Hitchance >= HitChance.VeryHigh)
            {
                var endPosition = prediction.UnitPosition.LSExtend(flashPosition, -pushDistance);
                if (endPosition.IsWall())
                {
                    Variables.LastCondemnFlashTime = Environment.TickCount;
                    E.CastOnUnit(target);
                    Utility.DelayAction.Add((int)(E.Delay + Game.Ping / 2f), () => Flash.Cast(flashPosition));
                }
                else
                {
                    // It's not a wall.
                    var step = pushDistance / 5f;
                    for (float i = 0; i < pushDistance; i += step)
                    {
                        var endPositionEx = prediction.UnitPosition.LSExtend(flashPosition, -i);
                        if (endPositionEx.IsWall())
                        {
                            Variables.LastCondemnFlashTime = Environment.TickCount;
                            E.CastOnUnit(target);
                            Utility.DelayAction.Add((int)(E.Delay + Game.Ping / 2f), () => Flash.Cast(flashPosition));

                            // Flash.Cast(flashPosition);
                            return;
                        }
                    }
                    
                }

            }
        }
    }
}
