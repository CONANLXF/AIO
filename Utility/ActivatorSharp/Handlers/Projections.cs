using System;
using System.Linq;
using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;
using Activators.Base;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;

using TargetSelector = PortAIO.TSManager; namespace Activators.Handlers
{
    public class Projections
    {
        internal static int LastCastedTimeStamp;
        internal static AIHeroClient Player => ObjectManager.Player;
        internal static List<HitType> MenuTypes = new List<HitType>
        {
            HitType.Danger,
            HitType.CrowdControl,
            HitType.Ultimate,
            HitType.ForceExhaust
        };
        public static void Init()
        {
            MissileClient.OnCreate += MissileClient_OnSpellMissileCreate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnUnitSpellCast;
            AIHeroClient.OnPlayAnimation += AIHeroClient_OnPlayAnimation;
        }

        private static void MissileClient_OnSpellMissileCreate(GameObject sender, EventArgs args)
        {
            #region FoW / Missile

            if (!sender.IsValid<MissileClient>())
            {
                return;
            }

            var missile = (MissileClient)sender;
            if (missile.SpellCaster is AIHeroClient && missile.SpellCaster?.Team != Player.Team)
            {
                var startPos = missile.StartPosition.LSTo2D();
                var endPos = missile.EndPosition.LSTo2D();

                var data = Data.Somedata.GetByMissileName(missile.SData.Name.ToLower());
                if (data == null)
                    return;

                var direction = (endPos - startPos).LSNormalized();

                if (startPos.LSDistance(endPos) > data.Range)
                    endPos = startPos + direction * data.Range;

                if (startPos.LSDistance(endPos) < data.Range && data.FixedRange)
                    endPos = startPos + direction * data.Range;

                foreach (var hero in Activator.Allies())
                {
                    // reset if needed
                    Essentials.ResetIncomeDamage(hero.Player);

                    var distance = (1000 * (startPos.LSDistance(hero.Player.ServerPosition) / data.Speed));
                    var endtime = -100 + Game.Ping / 2 + distance;

                    // setup projection
                    var proj = hero.Player.ServerPosition.LSTo2D().LSProjectOn(startPos, endPos);
                    var projdist = hero.Player.ServerPosition.LSTo2D().LSDistance(proj.SegmentPoint);

                    // get the evade time 
                    var evadetime = (int)(1000 *
                       (data.Width - projdist + hero.Player.BoundingRadius) / hero.Player.MoveSpeed);

                    // check if hero on segment
                    if (data.Width + hero.Player.BoundingRadius + 35 <= projdist)
                    {
                        continue;
                    }

                    if (data.Range > 10000)
                    {
                        // ignore if can evade
                        if (hero.Player.NetworkId == Player.NetworkId)
                        {
                            if (hero.Player.CanMove && evadetime < endtime)
                            {
                                // check next player
                                continue;
                            }
                        }
                    }

                    if (Activator.zmenu[data.SDataName + "predict"].Cast<CheckBox>().CurrentValue)
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(100, () =>
                        {
                            hero.Attacker = missile.SpellCaster;
                            hero.IncomeDamage += 1;
                            hero.HitTypes.Add(HitType.Spell);
                            hero.HitTypes.AddRange(
                                MenuTypes.Where(
                                    x =>
                                        Activator.zmenu[data.SDataName + x.ToString().ToLower()].Cast<CheckBox>().CurrentValue));

                            LeagueSharp.Common.Utility.DelayAction.Add((int)endtime * 2 + (200 - Game.Ping), () =>
                           {
                               hero.Attacker = null;
                               hero.IncomeDamage -= 1;
                               hero.HitTypes.RemoveAll(
                                   x =>
                                       !x.Equals(HitType.Spell) &&
                                       Activator.zmenu[data.SDataName + x.ToString().ToLower()].Cast<CheckBox>().CurrentValue);
                               hero.HitTypes.Remove(HitType.Spell);
                           });
                        });
                    }
                }
            }

            #endregion
        }

        private static void Obj_AI_Base_OnUnitSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (Essentials.IsEpicMinion(sender) || sender.Name.StartsWith("Sru_Crab"))
                return;

            #region Hero

            if (sender.IsEnemy && sender is AIHeroClient)
            {
                var attacker = (AIHeroClient)sender;
                if (attacker.IsValid<AIHeroClient>())
                {
                    LastCastedTimeStamp = Utils.GameTimeTickCount;

                    foreach (var hero in Activator.Allies())
                    {
                        Essentials.ResetIncomeDamage(hero.Player);

                        #region auto attack

                        if (args.SData.Name.ToLower().Contains("attack") && args.Target != null)
                        {
                            if (args.Target.NetworkId == hero.Player.NetworkId)
                            {
                                float dmg = 0;

                                var adelay = Math.Max(100, sender.AttackCastDelay * 1000);
                                var dist = 1000 * sender.LSDistance(args.Target.Position) / args.SData.MissileSpeed;
                                var end = adelay + dist + Game.Ping;

                                dmg += (int)Math.Max(attacker.LSGetAutoAttackDamage(hero.Player, true), 0);

                                if (attacker.HasBuff("sheen"))
                                    dmg += (int)Math.Max(attacker.LSGetAutoAttackDamage(hero.Player, true) +
                                                          attacker.GetCustomDamage("sheen", hero.Player), 0);

                                if (attacker.HasBuff("lichbane"))
                                    dmg += (int)Math.Max(attacker.LSGetAutoAttackDamage(hero.Player, true) +
                                                          attacker.GetCustomDamage("lichbane", hero.Player), 0);

                                if (attacker.HasBuff("itemstatikshankcharge") &&
                                    attacker.GetBuff("itemstatikshankcharge").Count == 100)
                                    dmg += new[] { 62, 120, 200 }[attacker.Level / 6];

                                if (args.SData.Name.ToLower().Contains("crit"))
                                    dmg += (int)Math.Max(attacker.LSGetAutoAttackDamage(hero.Player, true), 0);

                                dmg = dmg / 100 * Activator.zmenu["weightdmg"].Cast<Slider>().CurrentValue;

                                LeagueSharp.Common.Utility.DelayAction.Add((int)end / 2, () =>
                               {
                                   hero.Attacker = attacker;
                                   hero.HitTypes.Add(HitType.AutoAttack);
                                   hero.IncomeDamage += dmg;

                                   LeagueSharp.Common.Utility.DelayAction.Add(Math.Max((int)end + (150 - Game.Ping), 250), () =>
                                   {
                                       hero.Attacker = null;
                                       hero.IncomeDamage -= dmg;
                                       hero.HitTypes.Remove(HitType.AutoAttack);
                                   });
                               });
                            }
                        }

                        #endregion

                        var data = Data.Somedata.SomeSpells.Find(x => x.SDataName == args.SData.Name.ToLower());
                        if (data == null)
                            continue;

                        #region self/selfaoe

                        if (args.SData.TargettingType == SpellDataTargetType.Self ||
                            args.SData.TargettingType == SpellDataTargetType.SelfAoe)
                        {
                            GameObject fromobj = null;
                            if (data.FromObject != null)
                            {
                                fromobj =
                                    ObjectManager.Get<GameObject>()
                                        .FirstOrDefault(
                                            x =>
                                                data.FromObject != null && !x.IsAlly &&
                                                data.FromObject.Any(y => x.Name.Contains(y)));
                            }

                            var correctpos = fromobj?.Position ?? attacker.ServerPosition;
                            if (hero.Player.LSDistance(correctpos) > data.Range + 125)
                                continue;

                            if (data.SDataName == "kalistaexpungewrapper" && !hero.Player.HasBuff("kalistaexpungemarker"))
                                continue;

                            //var evadetime = 1000 * (data.Range - hero.Player.LSDistance(correctpos) +
                            //hero.Player.BoundingRadius) / hero.Player.MoveSpeed;

                            if (!Activator.zmenu[data.SDataName + "predict"].Cast<CheckBox>().CurrentValue)
                                continue;

                            var dmg = (int)Math.Max(attacker.LSGetSpellDamage(hero.Player, data.SDataName), 0);
                            if (dmg == 0)
                            {
                                dmg = (int)(hero.Player.Health / hero.Player.MaxHealth * 5);
                                // Console.WriteLine("Activator# - There is no Damage Lib for: " + data.SDataName);
                            }

                            dmg = dmg / 100 * Activator.zmenu["weightdmg"].Cast<Slider>().CurrentValue;

                            // delay the spell a bit before missile endtime
                            LeagueSharp.Common.Utility.DelayAction.Add((int)(data.Delay / 2), () =>
                           {
                               hero.Attacker = attacker;
                               hero.IncomeDamage += dmg;
                               hero.HitTypes.Add(HitType.Spell);
                               hero.HitTypes.AddRange(
                                   MenuTypes.Where(
                                       x =>
                                           Activator.zmenu[data.SDataName + x.ToString().ToLower()].Cast<CheckBox>().CurrentValue));

                                // lazy safe reset
                                LeagueSharp.Common.Utility.DelayAction.Add((int)data.Delay * 2 + (200 - Game.Ping), () =>
                               {
                                   hero.Attacker = null;
                                   hero.IncomeDamage -= dmg;
                                   hero.HitTypes.Remove(HitType.Spell);
                                   hero.HitTypes.RemoveAll(
                                       x =>
                                           !x.Equals(HitType.Spell) &&
                                           Activator.zmenu[data.SDataName + x.ToString().ToLower()].Cast<CheckBox>().CurrentValue);
                                   hero.HitTypes.Remove(HitType.Spell);
                               });
                           });
                        }

                        #endregion

                        #region skillshot

                        if (args.SData.TargettingType == SpellDataTargetType.Cone ||
                            args.SData.TargettingType.ToString().Contains("Location"))
                        {
                            GameObject fromobj = null;
                            if (data.FromObject != null)
                            {
                                fromobj =
                                    ObjectManager.Get<GameObject>()
                                        .FirstOrDefault(
                                            x =>
                                                data.FromObject != null && !x.IsAlly &&
                                                data.FromObject.Any(y => x.Name.Contains(y)));
                            }

                            var isline = data.SpellType.ToString().Contains("Cone") ||
                                         data.SpellType.ToString().Contains("Line");

                            var startpos = fromobj?.Position ?? attacker.ServerPosition;

                            if (hero.Player.LSDistance(startpos) > data.Range + 35)
                                continue;

                            if ((data.SDataName == "azirq" || data.SDataName == "azire") && fromobj == null)
                                continue;

                            var distance = (int)(1000 * (startpos.LSDistance(hero.Player.ServerPosition) / data.Speed));
                            var endtime = data.Delay - 100 + Game.Ping / 2f + distance - (Utils.GameTimeTickCount - LastCastedTimeStamp);

                            var iscone = data.SpellType.ToString().Contains("Cone");
                            var direction = (args.End.LSTo2D() - startpos.LSTo2D()).LSNormalized();
                            var endpos = startpos.LSTo2D() + direction * startpos.LSTo2D().LSDistance(args.End.LSTo2D());

                            if (startpos.LSTo2D().LSDistance(endpos) > data.Range)
                                endpos = startpos.LSTo2D() + direction * data.Range;

                            if (startpos.LSTo2D().LSDistance(endpos) < data.Range && data.FixedRange)
                                endpos = startpos.LSTo2D() + direction * data.Range;

                            var proj = hero.Player.ServerPosition.LSTo2D().LSProjectOn(startpos.LSTo2D(), endpos);
                            var projdist = hero.Player.ServerPosition.LSTo2D().LSDistance(proj.SegmentPoint);

                            int evadetime = 0;

                            if (isline)
                                evadetime =
                                    (int)(1000 * (data.Width - projdist + hero.Player.BoundingRadius) / hero.Player.MoveSpeed);

                            if (!isline || iscone)
                                evadetime =
                                    (int)(1000 * (data.Width - hero.Player.LSDistance(startpos) + hero.Player.BoundingRadius) / hero.Player.MoveSpeed);

                            if ((!isline || iscone) && hero.Player.LSDistance(endpos) <= data.Width + hero.Player.BoundingRadius + 35 ||
                                isline && data.Width + hero.Player.BoundingRadius + 35 > projdist)
                            {
                                if (data.Range > 10000)
                                {
                                    if (hero.Player.NetworkId == Player.NetworkId)
                                    {
                                        if (hero.Player.CanMove && evadetime < endtime)
                                        {
                                            continue;
                                        }
                                    }
                                }
                                if (!Activator.zmenu[data.SDataName + "predict"].Cast<CheckBox>().CurrentValue)
                                    continue;

                                var dmg = (int)Math.Max(attacker.LSGetSpellDamage(hero.Player, data.SDataName), 0);
                                if (dmg == 0)
                                {
                                    dmg = (int)(hero.Player.Health / hero.Player.MaxHealth * 5);
                                    Console.WriteLine("Activator# - There is no Damage Lib for: " + data.SDataName);
                                }

                                dmg = dmg / 100 * Activator.zmenu["weightdmg"].Cast<Slider>().CurrentValue;

                                LeagueSharp.Common.Utility.DelayAction.Add((int)(endtime / 2), () =>
                               {
                                   hero.Attacker = attacker;
                                   hero.IncomeDamage += dmg;
                                   hero.HitTypes.Add(HitType.Spell);
                                   hero.HitTypes.AddRange(
                                       MenuTypes.Where(
                                           x =>
                                               Activator.zmenu[data.SDataName + x.ToString().ToLower()].Cast<CheckBox>().CurrentValue));

                                   LeagueSharp.Common.Utility.DelayAction.Add((int)endtime * 3 + (200 - Game.Ping), () =>
                                   {
                                       hero.Attacker = null;
                                       hero.IncomeDamage -= dmg;
                                       hero.HitTypes.RemoveAll(
                                           x =>
                                               !x.Equals(HitType.Spell) &&
                                               Activator.zmenu[data.SDataName + x.ToString().ToLower()].Cast<CheckBox>().CurrentValue);
                                       hero.HitTypes.Remove(HitType.Spell);
                                   });
                               });
                            }
                        }

                        #endregion

                        #region unit type

                        if (args.SData.TargettingType == SpellDataTargetType.Unit ||
                            args.SData.TargettingType == SpellDataTargetType.SelfAndUnit)
                        {
                            if (args.Target == null || args.Target.Type != GameObjectType.AIHeroClient)
                                continue;

                            // check if is targeteting the hero on our table
                            if (hero.Player.NetworkId != args.Target.NetworkId)
                                continue;

                            // target spell dectection
                            if (hero.Player.LSDistance(attacker.ServerPosition) > data.Range + 100)
                                continue;

                            var distance =
                                (int)(1000 * (attacker.LSDistance(hero.Player.ServerPosition) / data.Speed));

                            var endtime = data.Delay - 100 + Game.Ping / 2 + distance -
                                          (Utils.GameTimeTickCount - LastCastedTimeStamp);

                            if (!Activator.zmenu[data.SDataName + "predict"].Cast<CheckBox>().CurrentValue)
                                continue;

                            var dmg = (int)Math.Max(attacker.LSGetSpellDamage(hero.Player, args.SData.Name), 0);
                            if (dmg == 0)
                            {
                                dmg = (int)(hero.Player.Health / hero.Player.MaxHealth * 5);
                                // Console.WriteLine("Activator# - There is no Damage Lib for: " + data.SDataName);
                            }

                            dmg = dmg / 100 * Activator.zmenu["weightdmg"].Cast<Slider>().CurrentValue;

                            LeagueSharp.Common.Utility.DelayAction.Add((int)(endtime / 2), () =>
                           {
                               hero.Attacker = attacker;
                               hero.IncomeDamage += dmg;
                               hero.HitTypes.Add(HitType.Spell);
                               hero.HitTypes.AddRange(
                                   MenuTypes.Where(
                                       x =>
                                           Activator.zmenu[data.SDataName + x.ToString().ToLower()].Cast<CheckBox>().CurrentValue));

                                // lazy reset
                                LeagueSharp.Common.Utility.DelayAction.Add((int)endtime * 2 + (200 - Game.Ping), () =>
                               {
                                   hero.Attacker = null;
                                   hero.IncomeDamage -= dmg;
                                   hero.HitTypes.RemoveAll(
                                       x =>
                                           !x.Equals(HitType.Spell) &&
                                           Activator.zmenu[data.SDataName + x.ToString().ToLower()].Cast<CheckBox>().CurrentValue);
                                   hero.HitTypes.Remove(HitType.Spell);
                               });
                           });
                        }

                        #endregion
                    }
                }
            }

            #endregion

            #region Turret

            if (sender.IsEnemy && sender.Type == GameObjectType.obj_AI_Turret && args.Target.Type == Player.Type)
            {
                var turret = sender as Obj_AI_Turret;
                if (turret != null && turret.IsValid<Obj_AI_Turret>())
                {
                    foreach (var hero in Activator.Allies())
                    {
                        if (args.Target.NetworkId == hero.Player.NetworkId && !hero.Immunity)
                        {
                            var dmg = (int)Math.Max(turret.CalcDamage(hero.Player, DamageType.Physical,
                                turret.BaseAttackDamage + turret.FlatPhysicalDamageMod), 0);

                            if (turret.LSDistance(hero.Player.ServerPosition) <= 900)
                            {
                                if (Player.LSDistance(hero.Player.ServerPosition) <= 1000)
                                {
                                    LeagueSharp.Common.Utility.DelayAction.Add(450, () =>
                                    {
                                        hero.HitTypes.Add(HitType.TurretAttack);
                                        hero.TowerDamage += dmg;

                                        LeagueSharp.Common.Utility.DelayAction.Add(150, () =>
                                        {
                                            hero.Attacker = null;
                                            hero.TowerDamage -= dmg;
                                            hero.HitTypes.Remove(HitType.TurretAttack);
                                        });
                                    });
                                }
                            }
                        }
                    }
                }
            }

            #endregion

            #region Minion

            if (sender.IsEnemy && sender.Type == GameObjectType.obj_AI_Minion && args.Target.Type == Player.Type)
            {
                var minion = sender as Obj_AI_Minion;
                if (minion != null && minion.IsValid<Obj_AI_Minion>())
                {
                    foreach (var hero in Activator.Allies())
                    {
                        if (hero.Player.NetworkId == args.Target.NetworkId && !hero.Immunity)
                        {
                            if (hero.Player.LSDistance(minion.ServerPosition) <= 750)
                            {
                                if (Player.LSDistance(hero.Player.ServerPosition) <= 1000)
                                {
                                    hero.HitTypes.Add(HitType.MinionAttack);
                                    hero.MinionDamage =
                                        (int)Math.Max(minion.CalcDamage(hero.Player, DamageType.Physical,
                                            minion.BaseAttackDamage + minion.FlatPhysicalDamageMod), 0);

                                    LeagueSharp.Common.Utility.DelayAction.Add(250, () =>
                                    {
                                        hero.HitTypes.Remove(HitType.MinionAttack);
                                        hero.MinionDamage = 0;
                                    });
                                }
                            }
                        }
                    }
                }
            }

            #endregion

            #region Gangplank Barrel

            if (sender.IsEnemy && sender.Type == GameObjectType.AIHeroClient)
            {
                var attacker = sender as AIHeroClient;
                if (attacker.ChampionName == "Gangplank" && attacker.IsValid<AIHeroClient>())
                {
                    foreach (var hero in Activator.Allies())
                    {
                        Essentials.ResetIncomeDamage(hero.Player);
                        List<Obj_AI_Minion> gplist = new List<Obj_AI_Minion>();

                        gplist.AddRange(ObjectManager.Get<Obj_AI_Minion>()
                            .Where(
                                x =>
                                    x.CharData.BaseSkinName == "gangplankbarrel" &&
                                    x.Position.LSDistance(x.Position) <= 375 && x.IsHPBarRendered)
                            .OrderBy(y => y.Position.LSDistance(hero.Player.ServerPosition)));

                        for (var i = 0; i < gplist.Count; i++)
                        {
                            var obj = gplist[i];
                            if (hero.Player.LSDistance(obj.Position) > 375 || args.Target.Name != "Barrel")
                            {
                                continue;
                            }

                            var dmg = (int)Math.Abs(attacker.LSGetAutoAttackDamage(hero.Player, true) * 1.2 + 150);
                            if (args.SData.Name.ToLower().Contains("crit"))
                            {
                                dmg = dmg * 2;
                            }

                            LeagueSharp.Common.Utility.DelayAction.Add(100 + (100 * i), () =>
                            {
                                hero.Attacker = attacker;
                                hero.HitTypes.Add(HitType.Spell);
                                hero.IncomeDamage += dmg;

                                LeagueSharp.Common.Utility.DelayAction.Add(400 + (100 * i), delegate
                                {
                                    hero.Attacker = null;
                                    hero.HitTypes.Remove(HitType.Spell);
                                    hero.IncomeDamage -= dmg;
                                });
                            });
                        }
                    }
                }
            }

            #endregion      

            #region Items

            if (sender.IsEnemy && sender.Type == GameObjectType.AIHeroClient)
            {
                var attacker = sender as AIHeroClient;
                if (attacker != null && attacker.IsValid<AIHeroClient>())
                {
                    if (args.SData.TargettingType == SpellDataTargetType.Unit)
                    {
                        foreach (var hero in Activator.Allies())
                        {
                            Essentials.ResetIncomeDamage(hero.Player);

                            if (args.Target.NetworkId != hero.Player.NetworkId)
                                continue;

                            if (args.SData.Name.ToLower() == "bilgewatercutlass")
                            {
                                var dmg = (float)attacker.GetItemDamage(hero.Player, Damage.DamageItems.Bilgewater);

                                hero.Attacker = attacker;
                                hero.HitTypes.Add(HitType.AutoAttack);
                                hero.IncomeDamage += dmg;

                                LeagueSharp.Common.Utility.DelayAction.Add(250, () =>
                                {
                                    hero.Attacker = null;
                                    hero.HitTypes.Remove(HitType.AutoAttack);
                                    hero.IncomeDamage -= dmg;
                                });
                            }

                            if (args.SData.Name.ToLower() == "itemswordoffeastandfamine")
                            {
                                var dmg = (float)attacker.GetItemDamage(hero.Player, Damage.DamageItems.Botrk);

                                hero.Attacker = attacker;
                                hero.HitTypes.Add(HitType.AutoAttack);
                                hero.IncomeDamage += dmg;

                                LeagueSharp.Common.Utility.DelayAction.Add(250, () =>
                                {
                                    hero.Attacker = null;
                                    hero.HitTypes.Remove(HitType.AutoAttack);
                                    hero.IncomeDamage -= dmg;
                                });
                            }

                            if (args.SData.Name.ToLower() == "hextechgunblade")
                            {
                                var dmg = (float)attacker.GetItemDamage(hero.Player, Damage.DamageItems.Hexgun);

                                hero.Attacker = attacker;
                                hero.HitTypes.Add(HitType.AutoAttack);
                                hero.IncomeDamage += dmg;

                                LeagueSharp.Common.Utility.DelayAction.Add(250, () =>
                                {
                                    hero.Attacker = null;
                                    hero.HitTypes.Remove(HitType.AutoAttack);
                                    hero.IncomeDamage -= dmg;
                                });
                            }
                        }
                    }
                }
            }

            #endregion

            #region LucianQ

            if (sender.IsEnemy && args.SData.Name == "LucianQ")
            {
                foreach (var a in Activator.Allies())
                {
                    var delay = ((350 - Game.Ping) / 1000f);

                    var herodir = (a.Player.ServerPosition - a.Player.Position).LSNormalized();
                    var expectedpos = args.Target.Position + herodir * a.Player.MoveSpeed * (delay);

                    if (args.Start.LSDistance(expectedpos) < 1100)
                        expectedpos = args.Target.Position +
                                     (args.Target.Position - sender.ServerPosition).LSNormalized() * 800;

                    var proj = a.Player.ServerPosition.LSTo2D().LSProjectOn(args.Start.LSTo2D(), expectedpos.LSTo2D());
                    var projdist = a.Player.ServerPosition.LSTo2D().LSDistance(proj.SegmentPoint);

                    if (Activator.zmenu["lucianqpredict"].Cast<CheckBox>().CurrentValue)
                    {
                        if (100 + a.Player.BoundingRadius > projdist)
                        {
                            a.Attacker = sender;
                            a.HitTypes.Add(HitType.Spell);
                            a.IncomeDamage += 1;

                            if (Activator.zmenu["lucianqdanger"].Cast<CheckBox>().CurrentValue)
                                a.HitTypes.Add(HitType.Danger);
                            if (Activator.zmenu["lucianqcrowdcontrol"].Cast<CheckBox>().CurrentValue)
                                a.HitTypes.Add(HitType.CrowdControl);
                            if (Activator.zmenu["lucianqultimate"].Cast<CheckBox>().CurrentValue)
                                a.HitTypes.Add(HitType.Ultimate);
                            if (Activator.zmenu["lucianqforceexhaust"].Cast<CheckBox>().CurrentValue)
                                a.HitTypes.Add(HitType.ForceExhaust);

                            LeagueSharp.Common.Utility.DelayAction.Add(350 - Game.Ping, () =>
                            {
                                if (a.IncomeDamage > 0)
                                    a.IncomeDamage -= 1;

                                a.Attacker = null;
                                a.HitTypes.Remove(HitType.Spell);

                                if (Activator.zmenu["lucianqdanger"].Cast<CheckBox>().CurrentValue)
                                    a.HitTypes.Remove(HitType.Danger);
                                if (Activator.zmenu["lucianqcrowdcontrol"].Cast<CheckBox>().CurrentValue)
                                    a.HitTypes.Remove(HitType.CrowdControl);
                                if (Activator.zmenu["lucianqultimate"].Cast<CheckBox>().CurrentValue)
                                    a.HitTypes.Remove(HitType.Ultimate);
                                if (Activator.zmenu["lucianqforceexhaust"].Cast<CheckBox>().CurrentValue)
                                    a.HitTypes.Remove(HitType.ForceExhaust);
                            });
                        }
                    }
                }
            }
        }
        #endregion

        private static void AIHeroClient_OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (!(sender is AIHeroClient))
                return;

            var aiHero = (AIHeroClient)sender;

            #region Jax

            if (aiHero.ChampionName == "Jax" && aiHero.IsEnemy)
            {
                if (args.Animation == "Spell3")
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(Game.Ping + 100, () =>
                    {
                        if (aiHero.HasBuff("JaxCounterStrike"))
                        {
                            var buff = aiHero.GetBuff("JaxCounterStrike");
                            var time = (int)((buff.EndTime - buff.StartTime) * 1000);

                            LeagueSharp.Common.Utility.DelayAction.Add(time / 2, () =>
                            {
                                foreach (var hero in Activator.Allies())
                                {
                                    var dmg = (float)Math.Max(aiHero.LSGetSpellDamage(hero.Player, SpellSlot.E), 0);

                                    if (aiHero.LSDistance(hero.Player) <= 250)
                                    {
                                        LeagueSharp.Common.Utility.DelayAction.Add(150, () =>
                                        {
                                            hero.Attacker = null;
                                            hero.HitTypes.Remove(HitType.Spell);
                                            hero.HitTypes.RemoveAll(
                                                x =>
                                                    !x.Equals(HitType.Spell) &&
                                                    true);
                                            hero.HitTypes.Remove(HitType.Spell);
                                            if (hero.IncomeDamage > 0)
                                                hero.IncomeDamage -= dmg;
                                        });

                                        hero.Attacker = aiHero;
                                        hero.IncomeDamage += dmg;
                                        hero.HitTypes.Add(HitType.Spell);
                                        hero.HitTypes.AddRange(
                                            MenuTypes.Where(
                                                x =>
                                                    true));
                                    }
                                }
                            });
                        }
                    });
                }
            }

            #endregion

        }
    }
}