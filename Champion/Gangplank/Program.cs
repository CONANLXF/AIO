using System;
using System.Collections.Generic;
using System.Linq;
using Color = System.Drawing.Color;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using UnderratedAIO.Helpers;
using Environment = UnderratedAIO.Helpers.Environment;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;

namespace UnderratedAIO.Champions
{
    internal class Gangplank
    {

        public static Menu config;
        public static LeagueSharp.Common.Spell Q, W, E, R;
        public static readonly AIHeroClient player = ObjectManager.Player;
        public static bool justQ, justE, chain;
        public Vector3 ePos;
        public const int BarrelExplosionRange = 325;
        public const int BarrelConnectionRange = 660;
        public List<Barrel> savedBarrels = new List<Barrel>();
        public List<CastedBarrel> castedBarrels = new List<CastedBarrel>();
        public double[] Rwave = new double[] { 50, 70, 90 };
        public double[] EDamage = new double[] { 60, 90, 120, 150, 180 };
        public Obj_AI_Minion NeedToBeDestroyed;

        public Gangplank()
        {
            InitGangPlank();
            InitMenu();
            //Game.PrintChat("<font color='#9933FF'>Soresu </font><font color='#FFFFFF'>- Gangplank</font>");
            Drawing.OnDraw += Game_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Obj_AI_Base.OnProcessSpellCast += Game_ProcessSpell;
            GameObject.OnCreate += GameObjectOnOnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
        }


        private void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            for (int i = 0; i < savedBarrels.Count; i++)
            {
                if (savedBarrels[i].barrel != null &&
                    (savedBarrels[i].barrel.NetworkId == sender.NetworkId || savedBarrels[i].barrel.IsDead))
                {
                    savedBarrels.RemoveAt(i);
                    return;
                }
            }
        }

        private void GameObjectOnOnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name == "Barrel")
            {
                savedBarrels.Add(new Barrel(sender as Obj_AI_Minion, System.Environment.TickCount));
            }
        }

        private IEnumerable<Obj_AI_Minion> GetBarrels()
        {
            return savedBarrels.Where(b => b.barrel != null).Select(b => b.barrel);
        }

        private bool KillableBarrel(Obj_AI_Base targetB,
            bool melee = false,
            float delay = 0,
            AIHeroClient sender = null,
            float missileTravelTime = -1)
        {
            if (targetB.Health < 2)
            {
                return true;
            }
            if (sender == null)
            {
                sender = player;
            }
            if (missileTravelTime == -1)
            {
                missileTravelTime = GetQTime(targetB);
            }
            var barrel = savedBarrels.FirstOrDefault(b => b.barrel.NetworkId == targetB.NetworkId);
            if (barrel != null)
            {
                var time = targetB.Health * getEActivationDelay() * 1000 + delay;
                if ((System.Environment.TickCount - barrel.time +
                     (melee ? (sender.AttackCastDelay) : missileTravelTime) * 1000) > time)
                {
                    return true;
                }
            }
            return false;
        }

        private float GetQTime(Obj_AI_Base targetB)
        {
            return player.LSDistance(targetB) / 2800f + Q.Delay;
        }

        private void InitGangPlank()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 590f); //2600f
            Q.SetTargetted(0.25f, 2200f);
            W = new LeagueSharp.Common.Spell(SpellSlot.W);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 950);
            E.SetSkillshot(0.8f, 50, float.MaxValue, false, SkillshotType.SkillshotCircle);
            R = new LeagueSharp.Common.Spell(SpellSlot.R);
            R.SetSkillshot(1f, 100, float.MaxValue, false, SkillshotType.SkillshotCircle);
        }

        private static void CleanserManager()
        {
            // List of disable buffs
            if (W.IsReady() && ((Player.HasBuffOfType(BuffType.Charm)) || (Player.HasBuffOfType(BuffType.Flee)) || (Player.HasBuffOfType(BuffType.Polymorph)) || (Player.HasBuffOfType(BuffType.Snare)) || (Player.HasBuffOfType(BuffType.Stun)) || (Player.HasBuffOfType(BuffType.Taunt)) || (Player.HasBuff("summonerexhaust")) || (Player.HasBuffOfType(BuffType.Suppression))))
            {
                LeagueSharp.Common.Utility.DelayAction.Add(100, () => { W.Cast(); });
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            var barrels =
                GetBarrels()
                    .Where(
                        o =>
                            o.IsValid && !o.IsDead && o.LSDistance(player) < 3000 && o.BaseSkinName == "GangplankBarrel" &&
                            o.GetBuff("gangplankebarrellife").Caster.IsMe)
                    .ToList();
            var QMana = Q.ManaCost < player.Mana;
            var shouldAAbarrel = (!Q.IsReady() ||
                                  menuC["comboPrior"].Cast<ComboBox>().CurrentValue == 1 ||
                                  (Q.IsReady() && !QMana) || !menuC["useq"].Cast<CheckBox>().CurrentValue);

            PortAIO.OrbwalkerManager.SetAttack(true);
            PortAIO.OrbwalkerManager.SetMovement(true);

            //Jungle.CastSmite(config.Item("useSmite").GetValue<KeyBind>().Active);

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo(barrels, shouldAAbarrel, QMana);
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass(barrels, shouldAAbarrel, QMana);
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                Clear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                if (menuM["useqLHH"].Cast<CheckBox>().CurrentValue && !justE)
                {
                    Lasthit();
                }
            }

            if (menuM["AutoW"].Cast<CheckBox>().CurrentValue)
            {
                CleanserManager();
            }

            if (menuM["AutoR"].Cast<CheckBox>().CurrentValue && R.IsReady())
            {
                foreach (var enemy in
                    HeroManager.Enemies.Where(
                        e =>
                            ((e.UnderTurret(true) &&
                              e.MaxHealth / 100 * menuM["Rhealt"].Cast<Slider>().CurrentValue * 0.75f >
                              e.Health - Program.IncDamages.GetEnemyData(e.NetworkId).DamageTaken) ||
                             (!e.UnderTurret(true) &&
                              e.MaxHealth / 100 * menuM["Rhealt"].Cast<Slider>().CurrentValue >
                              e.Health - Program.IncDamages.GetEnemyData(e.NetworkId).DamageTaken)) &&
                            e.HealthPercent > menuM["RhealtMin"].Cast<Slider>().CurrentValue &&
                            e.LSIsValidTarget() && e.LSDistance(player) > 1500))
                {
                    var pred = Program.IncDamages.GetEnemyData(enemy.NetworkId);
                    if (pred != null && pred.DamageTaken < enemy.Health)
                    {
                        var ally =
                            HeroManager.Allies.OrderBy(a => a.Health).FirstOrDefault(a => enemy.LSDistance(a) < 1000);
                        if (ally != null)
                        {
                            var pos = LeagueSharp.Common.Prediction.GetPrediction(enemy, 0.75f);
                            if (pos.CastPosition.LSDistance(enemy.Position) < 450 && pos.Hitchance >= HitChance.VeryHigh)
                            {
                                if (enemy.IsMoving)
                                {
                                    R.Cast(enemy.Position.LSExtend(pos.CastPosition, 450));
                                }
                                else
                                {
                                    R.Cast(enemy.ServerPosition);
                                }
                            }
                        }
                    }
                }
            }
            if (menuC["EQtoCursor"].Cast<KeyBind>().CurrentValue && E.IsReady() && Q.IsReady())
            {
                PortAIO.OrbwalkerManager.SetMovement(false);

                var barrel =
                    GetBarrels()
                        .Where(
                            o =>
                                o.IsValid && !o.IsDead && o.LSDistance(player) < Q.Range &&
                                o.BaseSkinName == "GangplankBarrel" && o.GetBuff("gangplankebarrellife").Caster.IsMe &&
                                KillableBarrel(o, false, -260))
                        .OrderBy(o => o.LSDistance(Game.CursorPos))
                        .FirstOrDefault();
                if (barrel != null)
                {
                    var cp = Game.CursorPos;
                    var cursorPos = barrel.LSDistance(cp) > BarrelConnectionRange
                        ? barrel.Position.LSExtend(cp, BarrelConnectionRange)
                        : cp;
                    var points =
                        CombatHelper.PointsAroundTheTarget(player.Position, E.Range - 200, 15, 6)
                            .Where(p => p.LSDistance(player.Position) < E.Range);
                    var cursorPos2 = cursorPos.LSDistance(cp) > BarrelConnectionRange
                        ? cursorPos.LSExtend(cp, BarrelConnectionRange)
                        : cp;
                    var middle = GetMiddleBarrel(barrel, points, cursorPos);
                    var threeBarrel = cursorPos.LSDistance(cp) > BarrelExplosionRange && E.Instance.Ammo >= 2 &&
                                      Game.CursorPos.LSDistance(player.Position) < E.Range && middle.IsValid();
                    var firsDelay = threeBarrel ? 500 : 265;
                    if (cursorPos.IsValid() && cursorPos.LSDistance(player.Position) < E.Range)
                    {
                        E.Cast(threeBarrel ? middle : cursorPos);
                        LeagueSharp.Common.Utility.DelayAction.Add(firsDelay, () => Q.CastOnUnit(barrel));
                        if (threeBarrel)
                        {
                            if (player.IsMoving)
                            {
                                EloBuddy.Player.IssueOrder(GameObjectOrder.Stop, player.Position);
                            }
                            LeagueSharp.Common.Utility.DelayAction.Add(801, () => E.Cast(middle.LSExtend(cp, BarrelConnectionRange)));
                        }
                        else
                        {
                            if (Orbwalker.CanMove)
                            {
                                PortAIO.OrbwalkerManager.SetMovement(true);
                                Orbwalker.MoveTo(Game.CursorPos);
                            }
                        }
                    }
                }
                else
                {
                    if (Orbwalker.CanMove)
                    {
                        PortAIO.OrbwalkerManager.SetMovement(true);
                        Orbwalker.MoveTo(Game.CursorPos);
                    }
                }
            }
            else if (menuC["EQtoCursor"].Cast<KeyBind>().CurrentValue)
            {
                if (Orbwalker.CanMove)
                {
                    PortAIO.OrbwalkerManager.SetMovement(true);
                    Orbwalker.MoveTo(Game.CursorPos);
                }
            }
            if (menuC["QbarrelCursor"].Cast<KeyBind>().CurrentValue && Q.IsReady())
            {
                var meleeRangeBarrel =
                    GetBarrels()
                        .OrderBy(o => o.LSDistance(Game.CursorPos))
                        .FirstOrDefault(
                            o =>
                                o.Health > 1 && o.LSDistance(player) < Orbwalking.GetRealAutoAttackRange(o) &&
                                !KillableBarrel(o, true));
                if (meleeRangeBarrel != null && Orbwalker.CanAutoAttack)
                {
                    PortAIO.OrbwalkerManager.SetMovement(false);
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, meleeRangeBarrel);
                    return;
                }
                var barrel =
                    GetBarrels()
                        .Where(
                            o =>
                                o.IsValid && !o.IsDead && o.LSDistance(player) < Q.Range &&
                                o.BaseSkinName == "GangplankBarrel" && o.GetBuff("gangplankebarrellife").Caster.IsMe &&
                                KillableBarrel(o))
                        .OrderBy(o => o.LSDistance(Game.CursorPos))
                        .FirstOrDefault();
                if (barrel != null)
                {
                    Q.CastOnUnit(barrel);
                }
                if (NeedToBeDestroyed != null && NeedToBeDestroyed.IsValidTarget() && NeedToBeDestroyed.IsValidTarget() && Orbwalker.CanAutoAttack && NeedToBeDestroyed.IsInAttackRange())
                {
                    Console.WriteLine("NeedToBeDestroyed");
                    PortAIO.OrbwalkerManager.SetAttack(false);
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, NeedToBeDestroyed);
                }
            }

            if (menuM["AutoQBarrel"].Cast<CheckBox>().CurrentValue)
            {
                if (BlowUpBarrel(barrels, shouldAAbarrel, false))
                {
                    if (!chain)
                    {
                        chain = true;
                        LeagueSharp.Common.Utility.DelayAction.Add(450, () => chain = false);
                    }
                }
            }
            for (int i = 0; i < castedBarrels.Count; i++)
            {
                if (castedBarrels[i].shouldDie())
                {
                    castedBarrels.RemoveAt(i);
                    break;
                }
            }
        }

        private Vector3 GetMiddleBarrel(Obj_AI_Minion barrel, IEnumerable<Vector3> points, Vector3 cursorPos)
        {
            var middle =
                points.Where(
                    p =>
                        !p.LSIsWall() && p.LSDistance(barrel.Position) < BarrelConnectionRange &&
                        p.LSDistance(barrel.Position) > BarrelExplosionRange &&
                        p.LSDistance(cursorPos) < BarrelConnectionRange && p.LSDistance(cursorPos) > BarrelExplosionRange &&
                        p.LSDistance(barrel.Position) + p.LSDistance(cursorPos) > BarrelExplosionRange * 2 - 100)
                    .OrderByDescending(p => p.LSCountEnemiesInRange(BarrelExplosionRange))
                    .ThenByDescending(p => p.LSDistance(barrel.Position))
                    .FirstOrDefault();
            return middle;
        }

        private void Lasthit()
        {
            if (Q.IsReady())
            {
                var mini =
                    MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.NotAlly)
                        .Where(m => m.Health < Q.GetDamage(m) && m.BaseSkinName != "GangplankBarrel")
                        .OrderByDescending(m => m.MaxHealth)
                        .ThenByDescending(m => m.LSDistance(player))
                        .FirstOrDefault();

                if (mini != null && !justE)
                {
                    Q.CastOnUnit(mini, config["packets"].Cast<CheckBox>().CurrentValue);
                }
            }
        }


        private void Harass(List<Obj_AI_Minion> barrels, bool shouldAAbarrel, bool qMana)
        {
            float perc = menuH["minmanaH"].Cast<Slider>().CurrentValue / 100f;
            if (player.Mana < player.MaxMana * perc)
            {
                return;
            }
            AIHeroClient target = TargetSelector.GetTarget(
                Q.Range + BarrelExplosionRange, DamageType.Physical);

            if (menuH["useqLHH"].Cast<CheckBox>().CurrentValue)
            {
                var mini =
                    MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.NotAlly)
                        .Where(m => m.Health < Q.GetDamage(m) && m.BaseSkinName != "GangplankBarrel")
                        .OrderByDescending(m => m.MaxHealth)
                        .ThenByDescending(m => m.LSDistance(player))
                        .FirstOrDefault();

                if (mini != null)
                {
                    Q.CastOnUnit(mini, config["packets"].Cast<CheckBox>().CurrentValue);
                    return;
                }
            }

            if (target == null || Environment.Minion.KillableMinion(player.AttackRange + 50))
            {
                return;
            }
            var dontQ = false;
            //Blow up barrels
            if (menuH["useqH"].Cast<CheckBox>().CurrentValue &&
                BlowUpBarrel(barrels, shouldAAbarrel, menuC["movetoBarrel"].Cast<CheckBox>().CurrentValue))
            {
                if (!chain)
                {
                    chain = true;
                    LeagueSharp.Common.Utility.DelayAction.Add(450, () => chain = false);
                }
                return;
            }

            //Cast E to chain
            if (E.IsReady() && E.Instance.Ammo > 0 && !justQ && !chain && menuH["useeH"].Cast<CheckBox>().CurrentValue &&
                menuH["eStacksH"].Cast<Slider>().CurrentValue < E.Instance.Ammo)
            {
                if (barrels.Any())
                {
                    var bestEMelee = GetEPos(barrels, target, true);
                    var bestEQ = GetEPos(barrels, target, false);
                    if (bestEMelee.IsValid() && shouldAAbarrel)
                    {
                        dontQ = true;
                        E.Cast(bestEMelee);
                    }
                    else if (bestEQ.IsValid() && menuH["useqH"].Cast<CheckBox>().CurrentValue && Q.IsReady())
                    {
                        dontQ = true;
                        E.Cast(bestEQ);
                    }
                }
            }
            if (menuH["useqH"].Cast<CheckBox>().CurrentValue && Q.IsReady() && !dontQ)
            {
                Q.CastOnUnit(target);
            }
        }

        private void Clear()
        {
            float perc = menuLC["minmana"].Cast<Slider>().CurrentValue / 100f;
            if (player.Mana < player.MaxMana * perc)
            {
                return;
            }
            if (menuLC["useqLC"].Cast<CheckBox>().CurrentValue)
            {
                var barrel =
                    GetBarrels()
                        .FirstOrDefault(
                            o =>
                                o.IsValid && !o.IsDead && o.LSDistance(player) < Q.Range &&
                                o.BaseSkinName == "GangplankBarrel" && o.GetBuff("gangplankebarrellife").Caster.IsMe &&
                                Environment.Minion.countMinionsInrange(o.Position, BarrelExplosionRange) >= 1);
                if (barrel != null)
                {
                    var minis = MinionManager.GetMinions(
                        barrel.Position, BarrelExplosionRange, MinionTypes.All, MinionTeam.NotAlly);
                    var Killable =
                        minis.Where(e => Q.GetDamage(e) >= e.Health && e.Health > 3);
                    if (Q.IsReady() && KillableBarrel(barrel) &&
                        Killable.Any(t => HealthPrediction.LaneClearHealthPrediction(t, 1000) <= 0))
                    {
                        Q.CastOnUnit(barrel, config["packets"].Cast<CheckBox>().CurrentValue);
                    }


                    if (menuLC["ePrep"].Cast<CheckBox>().CurrentValue)
                    {
                        if (Q.IsReady() && minis.Count == Killable.Count() && KillableBarrel(barrel))
                        {
                            Q.CastOnUnit(barrel, config["packets"].Cast<CheckBox>().CurrentValue);
                        }
                        else
                        {
                            foreach (var m in
                                minis.Where(
                                    e => Q.GetDamage(e) <= e.Health && e.Health > 3)
                                    .OrderBy(t => t.LSDistance(player))
                                    .ThenByDescending(t => t.Health))
                            {
                                Orbwalker.ForcedTarget =(m);
                                return;
                            }
                        }
                    }
                    else if (Q.IsReady() && KillableBarrel(barrel) &&
                             minis.Count >= menuLC["eMinHit"].Cast<Slider>().CurrentValue)
                    {
                        Q.CastOnUnit(barrel, config["packets"].Cast<CheckBox>().CurrentValue);
                    }

                    return;
                }
            }
            if (menuLC["useqLC"].Cast<CheckBox>().CurrentValue && !justE)
            {
                Lasthit();
            }
            if (menuLC["useeLC"].Cast<CheckBox>().CurrentValue && E.IsReady() &&
                menuLC["eStacksLC"].Cast<Slider>().CurrentValue < E.Instance.Ammo)
            {
                MinionManager.FarmLocation bestPositionE =
                    E.GetCircularFarmLocation(
                        MinionManager.GetMinions(
                            ObjectManager.Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.NotAlly),
                        BarrelExplosionRange);

                if (bestPositionE.MinionsHit >= menuLC["eMinHit"].Cast<Slider>().CurrentValue &&
                    bestPositionE.Position.LSDistance(ePos) > 400)
                {
                    E.Cast(bestPositionE.Position, config["packets"].Cast<CheckBox>().CurrentValue);
                }
            }
        }

        private void Combo(List<Obj_AI_Minion> barrels, bool shouldAAbarrel, bool Qmana)
        {
            var target = LSTargetSelector.GetTarget(1650, DamageType.Physical, true, HeroManager.Enemies.Where(h => h.IsInvulnerable));
            if (target == null)
            {
                return;
            }
            var ignitedmg = (float)player.GetSummonerSpellDamage(target, LeagueSharp.Common.Damage.SummonerSpell.Ignite);
            bool hasIgnite = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerDot")) == SpellState.Ready;
            if (menuC["useIgnite"].Cast<CheckBox>().CurrentValue && ignitedmg > target.Health && hasIgnite &&
                !CombatHelper.CheckCriticalBuffs(target) && !Q.IsReady() && !justQ)
            {
                player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
            }
            if (menuC["usew"].Cast<Slider>().CurrentValue > player.HealthPercent &&
                player.LSCountEnemiesInRange(500) > 0)
            {
                W.Cast();
            }
            if (R.IsReady() && menuC["user"].Cast<CheckBox>().CurrentValue)
            {
                var Rtarget =
                    HeroManager.Enemies.FirstOrDefault(e => e.HealthPercent < 50 && e.CountAlliesInRange(660) > 0);
                if (Rtarget != null)
                {
                    R.CastIfWillHit(Rtarget, menuC["Rmin"].Cast<Slider>().CurrentValue);
                }
            }
            var dontQ = false;

            //Blow up barrels
            if (BlowUpBarrel(barrels, shouldAAbarrel, menuC["movetoBarrel"].Cast<CheckBox>().CurrentValue))
            {
                if (!chain)
                {
                    chain = true;
                    LeagueSharp.Common.Utility.DelayAction.Add(450, () => chain = false);
                }
                return;
            }

            //Cast E to chain
            if (E.IsReady() && E.Instance.Ammo > 0 && !justQ && !chain && menuC["detoneateTarget"].Cast<CheckBox>().CurrentValue)
            {
                if (barrels.Any())
                {
                    var bestEMelee = GetEPos(barrels, target, true);
                    var bestEQ = GetEPos(barrels, target, false);
                    if (bestEMelee.IsValid() && shouldAAbarrel)
                    {
                        dontQ = true;
                        E.Cast(bestEMelee);
                    }
                    else if(bestEQ.IsValid() && menuC["useq"].Cast<CheckBox>().CurrentValue && Q.IsReady())
                    {
                        dontQ = true;
                        E.Cast(bestEQ);
                    }
                }
            }


            if (menuC["useeAlways"].Cast<CheckBox>().CurrentValue && E.IsReady() && player.LSDistance(target) < E.Range &&
                !justE && target.Health > Q.GetDamage(target) + player.GetAutoAttackDamage(target) &&
                Orbwalker.CanMove && menuC["eStacksC"].Cast<Slider>().CurrentValue < E.Instance.Ammo)
            {
                CastE(target, barrels);
            }
            var Qbarrelsb =
                GetBarrels()
                    .FirstOrDefault(
                        o =>
                            o.LSDistance(player) < Q.Range &&
                            o.LSDistance(target) < BarrelConnectionRange + BarrelExplosionRange);
            if (Qbarrelsb != null && E.Instance.Ammo > 0 && Q.IsReady() && target.Health > Q.GetDamage(target))
            {
                dontQ = true;
            }
            if (menuC["useq"].Cast<CheckBox>().CurrentValue && Q.CanCast(target) && Orbwalker.CanMove && !justE &&
                (!menuC["useqBlock"].Cast<CheckBox>().CurrentValue || !dontQ))
            {
                CastQonHero(target, barrels);
            }
        }

        private bool BlowUpBarrel(List<Obj_AI_Minion> barrels, bool shouldAAbarrel, bool movetoBarrel)
        {
            if (barrels.Any())
            {
                var moveDist = movetoBarrel ? 250 : 0;
                var bestBarrelMelee = GetBestBarrel(barrels, true, moveDist);
                var bestBarrelQ = GetBestBarrel(barrels, false);
                if (bestBarrelMelee != null && shouldAAbarrel)
                {
                    PortAIO.OrbwalkerManager.SetMovement(false);
                    if (Orbwalker.CanAutoAttack)
                    {
                        if (Orbwalking.GetRealAutoAttackRange(bestBarrelMelee) < player.LSDistance(bestBarrelMelee))
                        {
                            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, bestBarrelMelee.Position);
                        }
                        else
                        {
                            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, bestBarrelMelee);
                        }
                    }
                    return true;
                }
                if (bestBarrelQ != null && menuC["useq"].Cast<CheckBox>().CurrentValue)
                {
                    Q.CastOnUnit(bestBarrelQ);
                    return true;
                }
            }
            return false;
        }

        private bool EnemiesInBarrelRange(Vector3 barrel, float delay)
        {
            if (
                HeroManager.Enemies.Count(
                    enemy => enemy.LSIsValidTarget() && enemy.LSDistance(barrel) < BarrelExplosionRange) > 0)
            {
                return true;
            }
            return false;
        }

        private Obj_AI_Minion GetBestBarrel(List<Obj_AI_Minion> barrels, bool isMelee, float moveDist = 0f)
        {
            var meleeBarrels =
                barrels.Where(
                    b =>
                        player.LSDistance(b) <
                        (isMelee
                            ? Orbwalking.GetRealAutoAttackRange(b) +
                              (CombatHelper.IsFacing(player, b.Position) ? moveDist : 0f)
                            : Q.Range) && KillableBarrel(b, isMelee));
            var secondaryBarrels = barrels.Select(b => b.Position).Concat(castedBarrels.Select(c => c.pos));
            var meleeDelay = isMelee ? 0.25f : 0;
            if (moveDist > 0f)
            {
                meleeDelay -= (moveDist / player.MoveSpeed);
            }
            foreach (var melee in meleeBarrels)
            {
                var secondBarrels =
                    secondaryBarrels.Where(
                        b =>
                            !meleeBarrels.Any(n => b.LSDistance(n.Position) < 10) &&
                            melee.LSDistance(b) < BarrelConnectionRange);
                foreach (var second in secondBarrels)
                {
                    var thirdBarrels =
                        secondaryBarrels.Where(
                            b =>
                                !secondBarrels.Any(n => b.LSDistance(n) < 10) &&
                                !meleeBarrels.Any(n => b.LSDistance(n.Position) < 10) &&
                                second.LSDistance(b) < BarrelConnectionRange);
                    foreach (var third in thirdBarrels)
                    {
                        if (EnemiesInBarrelRange(third, 1f - meleeDelay))
                        {
                            return melee;
                        }
                    }
                    if (EnemiesInBarrelRange(second, 0.75f - meleeDelay))
                    {
                        return melee;
                    }
                }
                if (EnemiesInBarrelRange(melee.Position, 0.5f - meleeDelay))
                {
                    return melee;
                }
            }
            return null;
        }

        private Vector3 GetE(Vector3 barrel, AIHeroClient target, float delay, List<Vector3> barrels)
        {
            var enemies =
                HeroManager.Enemies.Where(
                    e =>
                        e.IsValidTarget(1650) && e.Distance(barrel) > BarrelExplosionRange &&
                        LeagueSharp.Common.Prediction.GetPrediction(e, delay).Hitchance >= HitChance.High &&
                        !barrels.Any(b => b.Distance(e.Position) < BarrelExplosionRange));
            var targetPred = LeagueSharp.Common.Prediction.GetPrediction(target, delay);
            var pos = Vector3.Zero;
            pos =
                GetBarrelPoints(barrel)
                    .Where(
                        p =>
                            !p.IsWall() && p.Distance(barrel) < BarrelConnectionRange &&
                            p.Distance(player.Position) < E.Range &&
                            barrels.Count(b => b.Distance(p) < BarrelExplosionRange) == 0 &&
                            HeroManager.Enemies.Count(e => e.Distance(p) < BarrelExplosionRange) > 0 &&
                            targetPred.CastPosition.Distance(p) < BarrelExplosionRange &&
                            target.Distance(p) < BarrelExplosionRange)
                    .OrderByDescending(p => enemies.Count(e => e.Distance(p) < BarrelExplosionRange))
                    .ThenBy(p => p.Distance(targetPred.CastPosition))
                    .FirstOrDefault();
            return pos;
        }

        private Vector3 GetMiddleE(Vector3 barrel, AIHeroClient target, float delay, List<Vector3> barrels)
        {
            if (E.Instance.Ammo < 2)
            {
                return Vector3.Zero;
            }
            var enemies =
                HeroManager.Enemies.Where(
                    e =>
                        e.IsValidTarget(1650) && e.Distance(barrel) > BarrelExplosionRange &&
                        !barrels.Any(b => b.Distance(e.Position) < BarrelExplosionRange));
            var targetPred = LeagueSharp.Common.Prediction.GetPrediction(target, delay);
            var pos = Vector3.Zero;
            pos =
                GetBarrelPoints(barrel)
                    .Where(
                        p =>
                            p.Distance(barrel) < BarrelConnectionRange && p.Distance(player.Position) < E.Range &&
                            barrels.Count(b => b.Distance(p) < BarrelExplosionRange) == 0 &&
                            targetPred.CastPosition.Distance(p) < (BarrelExplosionRange - 25) * 2)
                    .OrderByDescending(p => enemies.Count(e => e.Distance(p) < BarrelExplosionRange))
                    .ThenBy(p => p.Distance(targetPred.CastPosition))
                    .FirstOrDefault();
            return pos;
        }

        private Vector3 GetEPos(List<Obj_AI_Minion> barrels, AIHeroClient target, bool isMelee)
        {
            var barrelPositions = barrels.Select(b => b.Position).Concat(castedBarrels.Select(c => c.pos)).ToList();
            var barrelsInCloseRange =
                barrels.Where(
                    b =>
                        player.Distance(b) < (isMelee ? Orbwalking.GetRealAutoAttackRange(b) : Q.Range) &&
                        KillableBarrel(b, isMelee, -265))
                    .Select(b => b.Position)
                    .Concat(castedBarrels.Select(c => c.pos));
            var meleeDelay = isMelee ? 0.25f : 0;

            foreach (var melee in barrelsInCloseRange)
            {
                var secondPos = GetE(melee, target, 1.265f - meleeDelay, barrelPositions);
                var middle = GetMiddleE(melee, target, 1.465f - meleeDelay, barrelPositions);
                if (secondPos.IsValid())
                {
                    return secondPos;
                }
                var secondBarrels = barrelPositions.Where(b => melee.Distance(b) < BarrelConnectionRange).ToList();
                foreach (var secondBarrel in secondBarrels)
                {
                    var thirdE = GetE(secondBarrel, target, 1.265f - meleeDelay, barrelPositions);
                    if (thirdE.IsValid())
                    {
                        return thirdE;
                    }
                }
                if (middle.IsValid())
                {
                    return middle;
                }
            }
            return Vector3.Zero;
        }

        private void CastQonHero(AIHeroClient target, List<Obj_AI_Minion> barrels)
        {
            if (barrels.FirstOrDefault(b => target.LSDistance(b.Position) < BarrelExplosionRange) != null &&
                target.Health > Q.GetDamage(target))
            {
                return;
            }
            Q.CastOnUnit(target, config["packets"].Cast<CheckBox>().CurrentValue);
        }

        private void CastE(AIHeroClient target, List<Obj_AI_Minion> barrels)
        {
            if (barrels.Count(b => b.LSCountEnemiesInRange(BarrelConnectionRange) > 0) < 1)
            {
                if (menuC["useeAlways"].Cast<CheckBox>().CurrentValue)
                {
                    CastEtarget(target);
                }
                return;
            }
            var enemies =
                HeroManager.Enemies.Where(e => e.LSIsValidTarget() && e.LSDistance(player) < E.Range)
                    .Select(e => LeagueSharp.Common.Prediction.GetPrediction(e, 0.35f));
            List<Vector3> points = new List<Vector3>();
            foreach (var barrel in
                barrels.Where(b => b.LSDistance(player) < Q.Range && KillableBarrel(b)))
            {
                if (barrel != null)
                {
                    var newP = GetBarrelPoints(barrel.Position).Where(p => !p.LSIsWall());
                    if (newP.Any())
                    {
                        points.AddRange(newP.Where(p => p.LSDistance(player.Position) < E.Range));
                    }
                }
            }
            var bestPoint =
                points.Where(b => enemies.Count(e => e.UnitPosition.LSDistance(b) < BarrelExplosionRange) > 0)
                    .OrderByDescending(b => enemies.Count(e => e.UnitPosition.LSDistance(b) < BarrelExplosionRange))
                    .FirstOrDefault();
            if (bestPoint.IsValid() &&
                !savedBarrels.Any(b => b.barrel.Position.LSDistance(bestPoint) < BarrelConnectionRange))
            {
                E.Cast(bestPoint, config["packets"].Cast<CheckBox>().CurrentValue);
            }
        }

        private void CastEtarget(AIHeroClient target)
        {
            var ePred = LeagueSharp.Common.Prediction.GetPrediction(target, 1);
            var pos = target.Position.LSExtend(ePred.CastPosition, BarrelExplosionRange);
            if (pos.LSDistance(ePos) > 400 && !justE)
            {
                E.Cast(pos, config["packets"].Cast<CheckBox>().CurrentValue);
            }
        }

        private void Game_OnDraw(EventArgs args)
        {
            DrawHelper.DrawCircle(menuD["drawqq"].Cast<CheckBox>().CurrentValue, Q.Range, Color.FromArgb(180, 100, 146, 166));
            DrawHelper.DrawCircle(menuD["drawee"].Cast<CheckBox>().CurrentValue, E.Range, Color.FromArgb(180, 100, 146, 166));
            var drawecr = menuD["draweecr"].Cast<CheckBox>().CurrentValue;
            if (drawecr)
            {
                foreach (var barrel in GetBarrels().Where(b => b.LSDistance(player) < E.Range + BarrelConnectionRange))
                {
                    Render.Circle.DrawCircle(barrel.Position, BarrelConnectionRange, Color.FromArgb(180, 167, 141, 56), 7);
                }
            }

            if (menuD["drawW"].Cast<CheckBox>().CurrentValue)
            {
                if (W.IsReady() && player.HealthPercent < 100)
                {
                    float Heal = new int[] { 50, 75, 100, 125, 150 }[W.Level - 1] +
                                 (player.MaxHealth - player.Health) * 0.15f + player.FlatMagicDamageMod * 0.9f;
                    float mod = Math.Max(100f, player.Health + Heal) / player.MaxHealth;
                    float xPos = (float)((double)player.HPBarPosition.X + 36 + 103.0 * mod);
                    Drawing.DrawLine(
                        xPos, player.HPBarPosition.Y + 8, xPos, (float)((double)player.HPBarPosition.Y + 17), 2f,
                        Color.Coral);
                }
            }
            var tokens = player.GetBuff("gangplankbilgewatertoken");
            if (player.InFountain() && menuD["drawQpass"].Cast<CheckBox>().CurrentValue && tokens != null &&
                tokens.Count > 500)
            {
                var second = DateTime.Now.Second.ToString();
                var time = int.Parse(second[second.Length - 1].ToString());
                var color = Color.DeepSkyBlue;
                if (time >= 3 && time < 6)
                {
                    color = Color.GreenYellow;
                }
                if (time >= 6 && time < 8)
                {
                    color = Color.Yellow;
                }
                if (time >= 8)
                {
                    color = Color.Orange;
                }
                Drawing.DrawText(
                    Drawing.WorldToScreen(Game.CursorPos).X - 150, Drawing.WorldToScreen(Game.CursorPos).Y - 50, color,
                    "Spend your Silver Serpents, landlubber!");
            }
            if (menuD["drawKillableSL"].Cast<ComboBox>().CurrentValue != 0 && R.IsReady())
            {
                var text = new List<string>();
                foreach (var enemy in HeroManager.Enemies.Where(e => e.LSIsValidTarget()))
                {
                    if (getRDamage(enemy) > enemy.Health)
                    {
                        text.Add(enemy.ChampionName + "(" + Math.Ceiling(enemy.Health / Rwave[R.Level - 1]) + " wave)");
                    }
                }
                if (text.Count > 0)
                {
                    var result = string.Join(", ", text);
                    switch (menuD["drawKillableSL"].Cast<ComboBox>().CurrentValue)
                    {
                        case 2:
                            drawText(2, result);
                            break;
                        case 1:
                            drawText(1, result);
                            break;
                        default:
                            return;
                    }
                }
            }

            try
            {
                if (Q.IsReady() && menuD["drawEQ"].Cast<CheckBox>().CurrentValue)
                {
                    var points =
                        CombatHelper.PointsAroundTheTarget(player.Position, E.Range - 200, 15, 6)
                            .Where(p => p.LSDistance(player.Position) < E.Range);


                    var barrel =
                        GetBarrels()
                            .Where(
                                o =>
                                    o.IsValid && !o.IsDead && o.LSDistance(player) < Q.Range &&
                                    o.BaseSkinName == "GangplankBarrel" && o.GetBuff("gangplankebarrellife").Caster.IsMe &&
                                    KillableBarrel(o))
                            .OrderBy(o => o.LSDistance(Game.CursorPos))
                            .FirstOrDefault();
                    if (barrel != null)
                    {
                        var cp = Game.CursorPos;
                        var cursorPos = barrel.LSDistance(cp) > BarrelConnectionRange
                            ? barrel.Position.LSExtend(cp, BarrelConnectionRange)
                            : cp;
                        var cursorPos2 = cursorPos.LSDistance(cp) > BarrelConnectionRange
                            ? cursorPos.LSExtend(cp, BarrelConnectionRange)
                            : cp;
                        var middle = GetMiddleBarrel(barrel, points, cursorPos);
                        var threeBarrel = cursorPos.LSDistance(cp) > BarrelExplosionRange && E.Instance.Ammo >= 2 &&
                                          cursorPos2.LSDistance(player.Position) < E.Range && middle.IsValid();
                        if (threeBarrel)
                        {
                            Render.Circle.DrawCircle(
                                middle.LSExtend(cp, BarrelConnectionRange), BarrelExplosionRange, Color.DarkOrange, 6);
                            Render.Circle.DrawCircle(middle, BarrelExplosionRange, Color.DarkOrange, 6);
                            Drawing.DrawLine(
                                Drawing.WorldToScreen(barrel.Position),
                                Drawing.WorldToScreen(middle.LSExtend(barrel.Position, BarrelExplosionRange)), 2,
                                Color.DarkOrange);
                        }
                        else if (E.Instance.Ammo >= 1)
                        {
                            Drawing.DrawLine(
                                Drawing.WorldToScreen(barrel.Position),
                                Drawing.WorldToScreen(cursorPos.LSExtend(barrel.Position, BarrelExplosionRange)), 2,
                                Color.DarkOrange);
                            Render.Circle.DrawCircle(cursorPos, BarrelExplosionRange, Color.DarkOrange, 6);
                        }
                    }
                }
            }
            catch (Exception) { }
            if (menuD["drawWcd"].Cast<CheckBox>().CurrentValue)
            {
                foreach (var barrelData in savedBarrels)
                {
                    float time =
                        Math.Min(
                            System.Environment.TickCount - barrelData.time -
                            barrelData.barrel.Health * getEActivationDelay() * 1000f, 0) / 1000f;
                    if (time < 0)
                    {
                        Drawing.DrawText(
                            barrelData.barrel.HPBarPosition.X - time.ToString().Length * 5 + 40,
                            barrelData.barrel.HPBarPosition.Y - 20, Color.DarkOrange,
                            string.Format("{0:0.00}", time).Replace("-", ""));
                    }
                }
            }

            if (menuD["drawEmini"].Cast<CheckBox>().CurrentValue)
            {
                try
                {
                    var barrels =
                        GetBarrels()
                            .Where(
                                o =>
                                    o.IsValid && !o.IsDead && o.LSDistance(player) < E.Range &&
                                    o.BaseSkinName == "GangplankBarrel" && o.GetBuff("gangplankebarrellife").Caster.IsMe);
                    foreach (var b in barrels)
                    {
                        var minis = MinionManager.GetMinions(
                            b.Position, BarrelExplosionRange, MinionTypes.All, MinionTeam.NotAlly);
                        foreach (var m in
                            minis.Where(e => Q.GetDamage(e) >= e.Health && e.Health > 3))
                        {
                            Render.Circle.DrawCircle(m.Position, 57, Color.Yellow, 7);
                        }
                    }
                }
                catch (Exception) { }
            }
            var point2s =
                CombatHelper.PointsAroundTheTarget(Game.CursorPos, BarrelConnectionRange, 20f, 12)
                    .Where(p => p.Distance(Game.CursorPos) > BarrelExplosionRange);
            foreach (var p in point2s)
            {
                //Render.Circle.DrawCircle(p, 57, Color.Yellow, 7);
            }
        }

        public void drawText(int mode, string result)
        {
            const string baseText = "Killable with R: ";
            if (mode == 1)
            {
                Drawing.DrawText(
                    Drawing.Width / 2 - (baseText + result).Length * 5, Drawing.Height * 0.75f, Color.Red,
                    baseText + result);
            }
            else
            {
                Drawing.DrawText(
                    player.HPBarPosition.X - (baseText + result).Length * 5 + 110, player.HPBarPosition.Y + 250,
                    Color.Red, baseText + result);
            }
        }

        private float getRDamage(AIHeroClient enemy)
        {
            return
                (float)
                    LeagueSharp.Common.Damage.CalcDamage(
                        player, enemy, DamageType.Magical,
                        (Rwave[R.Level - 1] + 0.1 * player.FlatMagicDamageMod) * waveLength());
        }

        public int waveLength()
        {
            if (player.HasBuff("GangplankRUpgrade1"))
            {
                return 18;
            }
            else
            {
                return 12;
            }
        }

        private static float ComboDamage(AIHeroClient hero)
        {
            double damage = 0;
            if (Q.IsReady())
            {
                damage += LeagueSharp.Common.Damage.LSGetSpellDamage(player, hero, SpellSlot.Q);
            }
            //damage += ItemHandler.GetItemsDamage(hero);
            var ignitedmg = player.GetSummonerSpellDamage(hero, LeagueSharp.Common.Damage.SummonerSpell.Ignite);
            if (player.Spellbook.CanUseSpell(player.GetSpellSlot("summonerdot")) == SpellState.Ready &&
                hero.Health < damage + ignitedmg)
            {
                damage += ignitedmg;
            }
            return (float)damage;
        }

        private void Game_ProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.SData.Name == "GangplankQWrapper")
                {
                    if (!justQ)
                    {
                        justQ = true;
                        LeagueSharp.Common.Utility.DelayAction.Add(200, () => justQ = false);
                    }
                }
                if (args.SData.Name == "GangplankE")
                {
                    ePos = args.End;
                    if (!justE)
                    {
                        justE = true;
                        LeagueSharp.Common.Utility.DelayAction.Add(500, () => justE = false);
                        castedBarrels.Add(new CastedBarrel(ePos, System.Environment.TickCount));
                    }
                }
            }
            if (sender.IsEnemy && args.Target != null && sender is AIHeroClient && sender.LSDistance(player) < E.Range)
            {
                var targetBarrels =
                    savedBarrels.Where(
                        b =>
                            b.barrel.NetworkId == args.Target.NetworkId &&
                            KillableBarrel(
                                b.barrel, sender.IsMelee, 0, (AIHeroClient)sender,
                                sender.LSDistance(b.barrel) / args.SData.MissileSpeed));
                foreach (var barrelData in targetBarrels)
                {
                    if (Orbwalker.CanAutoAttack && NeedToBeDestroyed.IsInAttackRange())
                    {
                        NeedToBeDestroyed = barrelData.barrel;
                        LeagueSharp.Common.Utility.DelayAction.Add(230, () => NeedToBeDestroyed = null);
                    }
                    savedBarrels.Remove(barrelData);
                    return;
                }
            }
        }

        private IEnumerable<Vector3> GetBarrelPoints(Vector3 point)
        {
            return
                CombatHelper.PointsAroundTheTarget(point, BarrelConnectionRange, 15f)
                    .Where(p => !p.IsWall() && p.Distance(point) > BarrelExplosionRange);
        }

        private float getEActivationDelay()
        {
            if (player.Level >= 13)
            {
                return 0.5f;
            }
            if (player.Level >= 7)
            {
                return 1f;
            }
            return 2f;
        }

        private void InitMenu()
        {
            config = MainMenu.AddMenu("Gangplank ", "Gangplank");

            // Draw settings
            menuD = config.AddSubMenu("Drawings ", "dsettings");
            menuD.Add("drawqq", new CheckBox("Draw Q range", false));//.SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));
            menuD.Add("drawW", new CheckBox("Draw W", true));
            menuD.Add("drawee", new CheckBox("Draw E range", false));//.SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));
            menuD.Add("draweecr", new CheckBox("Draw Connection ranges", false));//.SetValue(new Circle(false, Color.FromArgb(180, 167, 141, 56)));
            menuD.Add("drawWcd", new CheckBox("Draw E countdown", true));
            menuD.Add("drawEmini", new CheckBox("Draw killable minions around E", true));
            menuD.Add("drawcombo", new CheckBox("Draw combo damage", true));
            menuD.Add("drawEQ", new CheckBox("Draw EQ to cursor", true));
            menuD.Add("drawKillableSL", new ComboBox("Show killable targets with R", 1, "OFF", "Above HUD", "Under GP"));
            menuD.Add("drawQpass", new CheckBox("Draw notification about Silver serpents", true));

            // Combo Settings
            menuC = config.AddSubMenu("Combo ", "csettings");
            menuC.AddGroupLabel("Q Settings : ");
            menuC.Add("useq", new CheckBox("Use Q", true));
            menuC.Add("useqBlock", new CheckBox("Block Q to save for EQ", false));
            menuC.Add("detoneateTarget", new CheckBox("Blow up target with E", true));
            menuC.AddGroupLabel("W Settings : ");
            menuC.Add("usew", new Slider("Use W under health", 20, 0, 100));
            menuC.Add("AutoW", new CheckBox("Use W with QSS options", true));
            menuC.AddGroupLabel("E Settings : ");
            menuC.Add("useeAlways", new CheckBox("Use E always", true));
            menuC.Add("eStacksC", new Slider("Keep stacks", 0, 0, 5));
            menuC.Add("movetoBarrel", new CheckBox("Move to barrel to AA", true));
            menuC.AddGroupLabel("R Settings : ");
            menuC.Add("user", new CheckBox("Use R", true));
            menuC.Add("Rmin", new Slider("R min", 2, 1, 5));
            menuC.Add("useIgnite", new CheckBox("Use Ignite", true));
            menuC.AddGroupLabel("Keys/Misc : ");
            menuC.Add("comboPrior", new ComboBox("Combo priority", 0, "E-Q", "E-AA"));
            menuC.Add("EQtoCursor", new KeyBind("EQ to cursor", false, KeyBind.BindTypes.HoldActive, 'T'));
            menuC.Add("QbarrelCursor", new KeyBind("Q barrel at cursor", false, KeyBind.BindTypes.HoldActive, 'H'));

            // Harass Settings
            menuH = config.AddSubMenu("Harass ", "Hsettings");
            menuH.Add("useqH", new CheckBox("Use Q harass", true));
            menuH.Add("useqLHH", new CheckBox("Use Q lasthit", true));
            menuH.Add("useeH", new CheckBox("Use E", true));
            menuH.Add("eStacksH", new Slider("Keep E stacks", 0, 0, 5));
            menuH.Add("minmanaH", new Slider("Keep X% mana", 1, 1, 100));

            // LaneClear Settings
            menuLC = config.AddSubMenu("LaneClear ", "Lcsettings");
            menuLC.Add("useqLC", new CheckBox("Use Q", true));
            menuLC.Add("useeLC", new CheckBox("Use E", true));
            menuLC.Add("eMinHit", new Slider("E Min hit", 3, 1, 6));
            menuLC.Add("eStacksLC", new Slider("Keep E stacks", 0, 0, 5));
            menuLC.Add("ePrep", new CheckBox("Prepare minions with E", true));
            menuLC.Add("minmana", new Slider("Keep X% mana", 1, 1, 100));

            // Misc Settings
            menuM = config.AddSubMenu("Misc ", "Msettings");
            menuM.Add("AutoR", new CheckBox("Cast R to get assists", false));
            menuM.Add("Rhealt", new Slider("R : Enemy health %", 35, 0, 100));
            menuM.Add("RhealtMin", new Slider("R : Enemy min health %", 10, 0, 100));
            menuM.Add("AutoQBarrel", new CheckBox("AutoQ barrel near enemies", false));

            config.Add("packets", new CheckBox("Use Packets", false));
        }

        public static Menu menuM, menuLC, menuH, menuC, menuD;
    }

    internal class Barrel
    {
        public Obj_AI_Minion barrel;
        public float time;

        public Barrel(Obj_AI_Minion objAiBase, int tickCount)
        {
            barrel = objAiBase;
            time = tickCount;
        }
    }

    internal class CastedBarrel
    {
        public float time;
        public Vector3 pos;

        public CastedBarrel(Vector3 position, int tickCount)
        {
            pos = position;
            time = tickCount;
        }

        public bool shouldDie()
        {
            return System.Environment.TickCount - time > 260;
        }
    }
}