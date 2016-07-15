using System.Linq;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;

using static Firestorm_AIO.Helpers.Helpers;
using EloBuddy;
using EloBuddy.SDK;

using TargetSelector = PortAIO.TSManager; namespace Firestorm_AIO.Helpers
{
    public static class SmartCaster
    {
        
        public static void SmartCast(this LeagueSharp.SDK.Spell spell, Obj_AI_Base target = null, HitChance hitchance = HitChance.Medium,
            int minimunHits = 0)
        {
            if (target == null) return;
            if (!spell.CanCast(target)) return;

            if (PortAIO.OrbwalkerManager.isComboActive || PortAIO.OrbwalkerManager.isHarassActive)
            {
                if (spell.IsSkillshot)
                {
                    if (spell.Collision)
                    {
                        if (Me.CountEnemyHeroesInRange(spell.Range) <= (minimunHits == 0 ? 2 : minimunHits))
                        {
                            spell.CastOnBestTarget(0f, true);
                            return;
                        }

                        if (Me.CountEnemyHeroesInRange(spell.Range) >= (minimunHits == 0 ? 2 : minimunHits))
                        {
                            spell.CastOnBestTarget(0f, true, 1);
                            return;
                        }
                    }
                    else
                    {
                        spell.CastIfHitchanceMinimum(target, hitchance);
                        return;
                    }
                }
                else
                {
                    spell.CastOnUnit(target);
                    return;
                }
            }

            if (PortAIO.OrbwalkerManager.isLaneClearActive)
            {
                if (spell.Collision)
                {
                    spell.CastIfHitchanceMinimum(target, HitChance.Low);
                    return;
                }
                else
                {
                    var minions =
                        GameObjects.EnemyMinions.Where(m => m.LSIsValidTarget())
                            .OrderBy(m => m.Distance(target))
                            .ThenBy(m => m.Health)
                            .ToList();

                    if (minions != null)
                    {
                        switch (spell.Type)
                        {
                            case SkillshotType.SkillshotLine:
                                var posLine = spell.GetLineFarmLocation(minions);
                                if (posLine.MinionsHit >= (minimunHits == 0 ? 1 : minimunHits))
                                {
                                    spell.Cast(posLine.Position);
                                    return;
                                }
                                break;
                            case SkillshotType.SkillshotCircle:
                                var posCircle = spell.GetCircularFarmLocation(minions);
                                if (posCircle.MinionsHit >= (minimunHits == 0 ? 1 : minimunHits))
                                {
                                    spell.Cast(posCircle.Position);
                                    return;
                                }
                                break;
                            case SkillshotType.SkillshotCone:
                                var posCone = spell.GetLineFarmLocation(minions, spell.Width);
                                if (posCone.MinionsHit >= (minimunHits == 0 ? 1 : minimunHits))
                                {
                                    spell.Cast(posCone.Position);
                                    return;
                                }
                                break;
                        }
                    }
                    spell.CastIfHitchanceMinimum(target, HitChance.Medium);
                }
            }
            else
            {
                spell.CastOnUnit(target);
            }

            if (PortAIO.OrbwalkerManager.isLastHitActive)
            {
                if (spell.IsSkillshot)
                {
                    spell.CastIfHitchanceMinimum(target, HitChance.Low);
                    return;
                }
                spell.CastOnUnit(target);
            }
        }
    }
}