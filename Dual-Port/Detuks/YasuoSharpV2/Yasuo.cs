using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Geometry = LeagueSharp.Common.Geometry;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;
using static EloBuddy.SDK.Spell;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Spells;

 namespace YasuoSharpV2
{
    class Yasuo
    {

        public struct IsSafeResult
        {
            public bool IsSafe;
            public List<Skillshot> SkillshotList;
            public List<Obj_AI_Base> casters;
        }

        internal class YasDash
        {
            public Vector3 from = new Vector3(-1, -1, -1);
            public Vector3 to = new Vector3(-1, -1, -1);

            public YasDash()
            {
                from = new Vector3(-1, -1, -1);
                to = new Vector3(-1, -1, -1);
            }

            public YasDash(Vector3 fromV, Vector3 toV)
            {
                from = fromV;
                to = toV;
            }

            public YasDash(YasDash dash)
            {
                from = dash.from;
                to = dash.to;
            }

        }

        internal class YasWall
        {
#pragma warning disable 0618
            public Obj_SpellLineMissile pointL;
            public Obj_SpellLineMissile pointR;
            public float endtime = 0;
            public YasWall()
            {

            }

            public YasWall(Obj_SpellLineMissile L, Obj_SpellLineMissile R)
            {
                pointL = L;
                pointR = R;
                endtime = Game.Time + 4;
            }

            public void setR(Obj_SpellLineMissile R)
            {
                pointR = R;
                endtime = Game.Time + 4;
            }

            public void setL(Obj_SpellLineMissile L)
            {
                pointL = L;
                endtime = Game.Time + 4;
            }

            public bool isValid(int time = 0)
            {
                return pointL != null && pointR != null && endtime - (time / 1000) > Game.Time;
            }
        }



        public static List<YasDash> dashes = new List<YasDash>();

        public static YasDash lastDash = new YasDash();

        public static AIHeroClient Player = ObjectManager.Player;

        public static Vector3 test = new Vector3();

        public static Spellbook sBook = Player.Spellbook;

        public static SpellDataInst Qdata = sBook.GetSpell(SpellSlot.Q);
        public static SpellDataInst Wdata = sBook.GetSpell(SpellSlot.W);
        public static SpellDataInst Edata = sBook.GetSpell(SpellSlot.E);
        public static SpellDataInst Rdata = sBook.GetSpell(SpellSlot.R);
        public static LeagueSharp.Common.Spell Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 475);
        public static LeagueSharp.Common.Spell QEmp = new LeagueSharp.Common.Spell(SpellSlot.Q, 1000);
        public static LeagueSharp.Common.Spell QCir = new LeagueSharp.Common.Spell(SpellSlot.Q, 315);
        public static LeagueSharp.Common.Spell W = new LeagueSharp.Common.Spell(SpellSlot.W, 400);
        public static LeagueSharp.Common.Spell E = new LeagueSharp.Common.Spell(SpellSlot.E, 475);
        public static LeagueSharp.Common.Spell R = new LeagueSharp.Common.Spell(SpellSlot.R, 1200);
        //Much Skillshot                    1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8
        public static LeagueSharp.Common.Spell[] levelUpSeq = { Q, E, W, Q, Q, R, Q, E, Q, E, R, E, W, E, W, R, W, W };

        //Much NotSoMuch                    1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8
        public static LeagueSharp.Common.Spell[] levelUpSeq2 = { Q, E, Q, W, Q, R, Q, E, Q, E, R, E, W, E, W, R, W, W };

        //Ignore these spells with W
        //public static List<string> WIgnore;

        public static Vector3 point1 = new Vector3();
        public static Vector3 point2 = new Vector3();

        public static Vector3 castFrom;
        public static bool isDashigPro = false;
        public static float startDash = 0;
        public static float time = 0;

        public static YasWall wall = new YasWall();


        public static SummonerItems sumItems;

        #region WallDashing

        public static void setDashes()
        {
            #region WallDashingValues
            //botoomside
            dashes.Add(new YasDash(new Vector3(5997.00f, 5065.00f, 51.67f), new Vector3(6447.35f, 5216.45f, 56.11f)));
            dashes.Add(new YasDash(new Vector3(3582.00f, 7936.00f, 53.67f), new Vector3(3845.85f, 7376.56f, 51.56f)));
            dashes.Add(new YasDash(new Vector3(3880.00f, 7978.00f, 51.81f), new Vector3(3824.00f, 7356.00f, 51.50f)));
            dashes.Add(new YasDash(new Vector3(3724.00f, 7408.00f, 51.87f), new Vector3(3631.26f, 7824.56f, 53.78f)));
            dashes.Add(new YasDash(new Vector3(3850.00f, 7968.00f, 51.91f), new Vector3(4042.00f, 7376.00f, 51.00f)));
            dashes.Add(new YasDash(new Vector3(3894.00f, 6446.00f, 52.46f), new Vector3(4480.45f, 6432.95f, 50.77f)));
            dashes.Add(new YasDash(new Vector3(3732.00f, 6528.00f, 52.46f), new Vector3(3732.00f, 7154.00f, 50.53f)));
            dashes.Add(new YasDash(new Vector3(4374.00f, 6258.00f, 51.36f), new Vector3(3946.00f, 6462.00f, 52.46f)));
            dashes.Add(new YasDash(new Vector3(3674.00f, 7058.00f, 50.33f), new Vector3(3734.00f, 6588.00f, 52.46f)));
            dashes.Add(new YasDash(new Vector3(3786.00f, 6534.00f, 52.46f), new Vector3(3470.00f, 6888.00f, 51.15f)));
            dashes.Add(new YasDash(new Vector3(3890.00f, 6520.00f, 52.46f), new Vector3(4258.00f, 6218.00f, 51.94f)));
            dashes.Add(new YasDash(new Vector3(2124.00f, 8506.00f, 51.78f), new Vector3(1880.00f, 7930.00f, 51.43f)));
            dashes.Add(new YasDash(new Vector3(2148.00f, 8370.00f, 51.78f), new Vector3(1690.00f, 8688.00f, 52.66f)));
            dashes.Add(new YasDash(new Vector3(1724.00f, 8156.00f, 52.84f), new Vector3(2108.00f, 8436.00f, 51.78f)));
            dashes.Add(new YasDash(new Vector3(8370.00f, 2698.00f, 51.04f), new Vector3(7977.40f, 3171.12f, 51.58f)));
            dashes.Add(new YasDash(new Vector3(8314.00f, 2678.00f, 51.12f), new Vector3(8376.00f, 3300.00f, 52.56f)));
            dashes.Add(new YasDash(new Vector3(8272.00f, 3208.00f, 51.89f), new Vector3(8324.00f, 2736.00f, 51.13f)));
            dashes.Add(new YasDash(new Vector3(7858.00f, 3912.00f, 53.76f), new Vector3(8362.70f, 3652.84f, 54.42f)));
            dashes.Add(new YasDash(new Vector3(7564.00f, 4112.00f, 54.46f), new Vector3(7686.00f, 4726.00f, 49.53f)));
            dashes.Add(new YasDash(new Vector3(7030.00f, 5460.00f, 54.20f), new Vector3(7410.00f, 5954.00f, 52.48f)));
            dashes.Add(new YasDash(new Vector3(6972.00f, 5508.00f, 55.43f), new Vector3(6395.04f, 5313.03f, 48.53f)));
            dashes.Add(new YasDash(new Vector3(6924.00f, 5492.00f, 54.36f), new Vector3(6334.00f, 5292.00f, 48.53f)));
            dashes.Add(new YasDash(new Vector3(7372.00f, 5858.00f, 52.57f), new Vector3(7062.00f, 5500.00f, 55.03f)));

            #endregion
            sumItems = new SummonerItems(Player);
        }

        public static YasDash getClosestDash(float dist = 350)
        {
            YasDash closestWall = dashes[0];
            for (int i = 1; i < dashes.Count; i++)
            {
                closestWall = closestDashToMouse(closestWall, dashes[i]);
            }
            if (closestWall.to.LSDistance(Game.CursorPos) < dist)
                return closestWall;
            return null;
        }

        public static YasDash closestDashToMouse(YasDash w1, YasDash w2)
        {
            return Vector3.DistanceSquared(w1.to, Game.CursorPos) + Vector3.DistanceSquared(w1.from, Player.Position) > Vector3.DistanceSquared(w2.to, Game.CursorPos) + Vector3.DistanceSquared(w2.from, Player.Position) ? w2 : w1;
        }

        public static void saveLastDash()
        {
            if (lastDash.from.X != -1 && lastDash.from.Y != -1)
                dashes.Add(new YasDash(lastDash));
            lastDash = new YasDash();
        }

        public static void fleeToMouse()
        {
            try
            {
                YasDash closeDash = getClosestDash();
                if (closeDash != null)
                {
                    List<Obj_AI_Base> jumps = canGoThrough(closeDash);
                    if (jumps.Count > 0 || ((W.IsReady() || (Yasuo.wall != null && (Yasuo.wall.endtime - Game.Time) > 3f))))
                    {
                        var distToDash = Player.LSDistance(closeDash.from);

                        if (W.IsReady() && distToDash < 136f && jumps.Count == 0 && NavMesh.LineOfSightTest(closeDash.to, closeDash.to)
                           && MinionManager.GetMinions(Game.CursorPos, 350).Where(min => min.IsVisible).Count() < 2)
                        {
                            W.Cast(closeDash.to);
                        }

                        if (distToDash > 2f)
                        {
                            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, closeDash.from);
                            return;
                        }

                        if (distToDash < 3f && jumps.Count > 0 && jumps.First().LSDistance(Player) <= 473)
                        {
                            E.Cast(jumps.First());
                        }
                        return;
                    }
                }
                if (getClosestDash(400) == null)
                    Yasuo.gapCloseE(Game.CursorPos.LSTo2D());
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static List<Obj_AI_Base> canGoThrough(YasDash dash)
        {
            List<Obj_AI_Base> jumps = ObjectManager.Get<Obj_AI_Base>().Where(enemy => enemyIsJumpable(enemy) && enemy.LSIsValidTarget(550, true, dash.to)).ToList();
            List<Obj_AI_Base> canBejump = new List<Obj_AI_Base>();
            foreach (var jumpe in jumps)
            {
                if (YasMath.interCir(dash.from.LSTo2D(), dash.to.LSTo2D(), jumpe.Position.LSTo2D(), 35) /*&& jumpe.LSDistance(dash.to) < Player.LSDistance(dash.to)*/)
                {
                    canBejump.Add(jumpe);
                }
            }
            return canBejump.OrderBy(jum => Player.LSDistance(jum)).ToList();
        }


        public static float getLengthTillPos(Vector3 pos)
        {
            return 0;
        }

        #endregion

        public static void setSkillShots()
        {
            Q.SetSkillshot(getNewQSpeed(), 50f, float.MaxValue, false, SkillshotType.SkillshotLine);
            QEmp.SetSkillshot(0.1f, 50f, 1200f, false, SkillshotType.SkillshotLine);
            QCir.SetSkillshot(0f, 375f, float.MaxValue, false, SkillshotType.SkillshotCircle);
        }

        public static float getNewQSpeed()
        {
            float ds = 0.5f;//s
            float a = 1 / ds * Yasuo.Player.AttackSpeedMod;
            return 1 / a;
        }

        public static void doCombo(AIHeroClient target)
        {

            if (target == null) return;
            useHydra(target);
            if (target.LSDistance(Player) < 500)
            {
                sumItems.cast(SummonerItems.ItemIds.Ghostblade);
            }
            if (target.LSDistance(Player) < 500 && (Player.Health / Player.MaxHealth) * 100 < 85)
            {
                sumItems.cast(SummonerItems.ItemIds.BotRK, target);

            }
            if (YasuoSharp.smartW["smartW"].Cast<CheckBox>().CurrentValue)
                putWallBehind(target);
            if (YasuoSharp.comboMenu["useEWall"].Cast<CheckBox>().CurrentValue)
                eBehindWall(target);

            Obj_AI_Base goodTarg = canDoEQEasly(target);
            var outPut = LeagueSharp.Common.Prediction.GetPrediction(goodTarg, 700 + Player.MoveSpeed);
            if (goodTarg != null && outPut.UnitPosition.LSDistance(Player.Position) <= 470)
            {

                E.Cast(goodTarg);

                Q.Cast(target);
            }
            if (!useESmart(target))
            {
                List<AIHeroClient> ignore = new List<AIHeroClient>();
                ignore.Add(target);
                gapCloseE(target.Position.LSTo2D());
            }

            useQSmart(target);
        }


        public static void stackQ()
        {
            if (!Q.IsReady() || isQEmpovered() || !YasuoSharp.flee["fleeStack"].Cast<CheckBox>().CurrentValue)//fleeStack
                return;
            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range + 50);

            if (!isDashigPro)
            {

                List<Vector2> minionPs = YasMath.GetCastMinionsPredictedPositions(minions, getNewQSpeed() * 0.3f, 30f,
                    float.MaxValue, Player.ServerPosition, 465, false, SkillshotType.SkillshotLine);
                Vector2 clos = LeagueSharp.Common.Geometry.Closest(Player.ServerPosition.LSTo2D(), minionPs);
                if (Player.LSDistance(clos) < 475)
                {
                    Q.Cast(clos, false);
                    return;
                }
            }
            else
            {
                if (minions.Count(min => !min.IsDead && min.IsValid && min.LSDistance(getDashEndPos()) < QCir.Range) >
                    1)
                {
                    QCir.Cast(Player.Position, false);
                    return;
                }
            }
        }

        public static Obj_AI_Base canDoEQEasly(AIHeroClient target)
        {
            if (!E.IsReady() || Q.IsReady(150) || !isQEmpovered())
                return null;
            List<Obj_AI_Base> jumps = ObjectManager.Get<Obj_AI_Base>().Where(enemy => enemy.NetworkId != target.NetworkId && enemyIsJumpable(enemy) && enemy.LSIsValidTarget(470, true)).OrderBy(jp => V2E(Player.Position, jp.Position, 475).LSDistance(target.Position, true)).ToList();

            if (jumps.Any() && V2E(Player.Position, jumps.First().Position, 475).LSDistance(target.Position, true) < 250 * 250)
            {
                return jumps.First();
            }
            return null;
        }


        public static Vector2 getNextPos(AIHeroClient target)
        {
            Vector2 dashPos = target.Position.LSTo2D();
            if (target.IsMoving && target.Path.Count() != 0)
            {
                Vector2 tpos = target.Position.LSTo2D();
                Vector2 path = target.Path[0].LSTo2D() - tpos;
                path.Normalize();
                dashPos = tpos + (path * 100);
            }
            return dashPos;
        }

        public static void putWallBehind(AIHeroClient target)
        {
            if (!W.IsReady() || !E.IsReady() || target.IsMelee())
                return;
            Vector2 dashPos = getNextPos(target);
            PredictionOutput po = LeagueSharp.Common.Prediction.GetPrediction(target, 0.5f);

            float dist = Player.LSDistance(po.UnitPosition);
            if (!target.IsMoving || Player.LSDistance(dashPos) <= dist + 40)
                if (dist < 330 && dist > 100 && W.IsReady())
                {
                    W.Cast(po.UnitPosition);
                }
        }

        public static void eBehindWall(AIHeroClient target)
        {
            if (!E.IsReady() || !enemyIsJumpable(target) || target.IsMelee())
                return;
            float dist = Player.LSDistance(target);
            var pPos = Player.Position.LSTo2D();
            Vector2 dashPos = target.Position.LSTo2D();
            if (!target.IsMoving || Player.LSDistance(dashPos) <= dist)
            {
                foreach (Obj_AI_Base enemy in ObjectManager.Get<Obj_AI_Base>().Where(enemy => enemyIsJumpable(enemy)))
                {
                    Vector2 posAfterE = pPos + (Vector2.Normalize(enemy.Position.LSTo2D() - pPos) * E.Range);
                    if ((target.LSDistance(posAfterE) < dist
                        || target.LSDistance(posAfterE) < Orbwalking.GetRealAutoAttackRange(target) + 100)
                        && goesThroughWall(target.Position, posAfterE.To3D()))
                    {
                        if (useENormal(target))
                            return;
                    }
                }
            }
        }



        public static bool goesThroughWall(Vector3 vec1, Vector3 vec2)
        {
            if (wall.endtime < Game.Time || wall.pointL == null || wall.pointL == null)
                return false;
            Vector2 inter = YasMath.LineIntersectionPoint(vec1.LSTo2D(), vec2.LSTo2D(), wall.pointL.Position.LSTo2D(), wall.pointR.Position.LSTo2D());
            float wallW = (300 + 50 * W.Level);
            if (wall.pointL.Position.LSTo2D().LSDistance(inter) > wallW ||
                wall.pointR.Position.LSTo2D().LSDistance(inter) > wallW)
                return false;
            var dist = vec1.LSDistance(vec2);
            if (vec1.LSTo2D().LSDistance(inter) + vec2.LSTo2D().LSDistance(inter) - 30 > dist)
                return false;

            return true;
        }

        public static void doLastHit(AIHeroClient target)
        {
            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range + 50);
            foreach (var minion in minions.Where(minion => minion.LSIsValidTarget(Q.Range)))
            {
                if (Player.LSDistance(minion) < Orbwalking.GetRealAutoAttackRange(minion) && minion.Health < Player.LSGetAutoAttackDamage(minion))
                    return;
                if (YasuoSharp.lasthit["useElh"].Cast<CheckBox>().CurrentValue && minion.Health < Player.LSGetSpellDamage(minion, E.Slot))
                    useENormal(minion);

                if (YasuoSharp.lasthit["useQlh"].Cast<CheckBox>().CurrentValue && !isQEmpovered() && minion.Health < Player.LSGetSpellDamage(minion, Q.Slot))
                    if (!(target != null && isQEmpovered() && Player.LSDistance(target) < 1050))
                    {
                        if (canCastFarQ())
                        {
                            Q.Cast(minion);
                        }
                    }
            }
        }

        public static void doLaneClear(AIHeroClient target)
        {
            List<Obj_AI_Base> minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, 1000, MinionTypes.All, MinionTeam.NotAlly);
            if (YasuoSharp.laneclear["useElc"].Cast<CheckBox>().CurrentValue && E.IsReady())
                foreach (var minion in minions.Where(minion => minion.LSIsValidTarget(E.Range) && enemyIsJumpable(minion)))
                {
                    if (minion.Health < Player.LSGetSpellDamage(minion, E.Slot)
                        || Q.IsReady() && minion.Health < (Player.LSGetSpellDamage(minion, E.Slot) + Player.LSGetSpellDamage(minion, Q.Slot)))
                    {
                        if (useENormal(minion))
                            return;
                    }
                }

            if (Q.IsReady() && YasuoSharp.laneclear["useQlc"].Cast<CheckBox>().CurrentValue)
            {
                if (isQEmpovered() && !(target != null && Player.LSDistance(target) < 1050))
                {
                    if (canCastFarQ())
                    {
                        List<Vector2> minionPs = YasMath.GetCastMinionsPredictedPositions(minions, getNewQSpeed(), 50f, 1200f, Player.ServerPosition, 900f, false, SkillshotType.SkillshotLine);
                        MinionManager.FarmLocation farm = QEmp.GetLineFarmLocation(minionPs); //MinionManager.GetBestLineFarmLocation(minionPs, 50f, 900f);
                        if (farm.MinionsHit >= YasuoSharp.laneclear["useEmpQHit"].Cast<Slider>().CurrentValue)
                        {
                            //Console.WriteLine("Cast q simp Emp");
                            QEmp.Cast(farm.Position, false);
                            return;
                        }
                    }
                    else
                    {
                        if (minions.Count(min => min.IsValid && !min.IsDead && min.LSDistance(getDashEndPos()) < QCir.Range) >= YasuoSharp.laneclear["useEmpQHit"].Cast<Slider>().CurrentValue)
                        {
                            QCir.Cast(Player.Position, false);
                            Console.WriteLine("Cast q circ simp");
                        }
                    }
                }
                else
                {
                    if (!isDashigPro)
                    {
                        List<Vector2> minionPs = YasMath.GetCastMinionsPredictedPositions(minions, getNewQSpeed() * 0.3f, 30f, float.MaxValue, Player.ServerPosition, 465, false, SkillshotType.SkillshotLine);
                        Vector2 clos = LeagueSharp.Common.Geometry.Closest(Player.ServerPosition.LSTo2D(), minionPs);
                        if (Player.LSDistance(clos) < 475)
                        {
                            Console.WriteLine("Cast q simp");
                            Q.Cast(clos, false);
                            return;
                        }
                    }
                    else
                    {
                        if (minions.Count(min => !min.IsDead && min.IsValid && min.LSDistance(getDashEndPos()) < QCir.Range) > 1)
                        {
                            QCir.Cast(Player.Position, false);
                            Console.WriteLine("Cast q circ simp");
                            return;
                        }
                    }
                }
            }

        }

        public static void doHarass(AIHeroClient target)
        {
            if (!Player.ServerPosition.UnderTurret(true) || YasuoSharp.harass["harassTower"].Cast<CheckBox>().CurrentValue)
                useQSmart(target);
        }



        public static void useHydra(Obj_AI_Base target)
        {

            if ((Items.CanUseItem(3074) || Items.CanUseItem(3074)) && target.LSDistance(Player.ServerPosition) < (400 + target.BoundingRadius - 20))
            {
                Items.UseItem(3074, target);
                Items.UseItem(3077, target);
            }
        }


        public static Vector3 getDashEndPos()
        {
            Vector2 dashPos2 = Player.LSGetDashInfo().EndPos;
            return new Vector3(dashPos2, Player.ServerPosition.Z);
        }

        public static bool isQEmpovered()
        {
            return Player.HasBuff("yasuoq3w");
        }

        public static bool isDashing()
        {
            return isDashigPro;
        }

        public static bool canCastFarQ()
        {
            return !isDashigPro;
        }

        public static bool canCastCircQ()
        {
            return isDashigPro;
        }


        public static void setUpWall()
        {
            if (wall == null)
                return;

        }

        public static void useQSmart(AIHeroClient target, bool onlyEmp = false)
        {
            if (!Q.IsReady() || target == null)
                return;
            if (isQEmpovered())
            {
                if (canCastFarQ())
                {
                    PredictionOutput po = QEmp.GetPrediction(target); //QEmp.GetPrediction(target, true);
                    if (po.Hitchance >= LeagueSharp.Common.HitChance.Medium)
                    {
                        QEmp.Cast(po.CastPosition);
                        return;
                    }
                }
                else//dashing
                {
                    Vector3 endPos = getDashEndPos();
                    if (Player.LSDistance(endPos) < 40 && target.LSDistance(endPos) < QCir.Range)
                    {
                        QCir.Cast(target.Position);
                        return;
                    }
                }
            }
            else if (!onlyEmp)
            {
                if (canCastFarQ())
                {
                    PredictionOutput po = Q.GetPrediction(target);
                    if (po.Hitchance >= LeagueSharp.Common.HitChance.Medium)
                    {
                        Q.Cast(po.CastPosition);
                    }
                    return;

                }
                else//dashing
                {
                    float trueRange = QCir.Range - 10;
                    Vector3 endPos = getDashEndPos();
                    if (Player.LSDistance(endPos) < 40 && target.LSDistance(endPos) < QCir.Range)
                    {
                        QCir.Cast(target.Position);
                        return;
                    }
                }
            }
        }


        public static IsSafeResult isSafePoint(Vector2 point, bool igonre = false)
        {
            var result = new IsSafeResult();
            result.SkillshotList = new List<Skillshot>();
            result.casters = new List<Obj_AI_Base>();
            result.IsSafe = (result.SkillshotList.Count == 0);
            return result;
        }

        private static Vector2 V2E(Vector3 from, Vector3 direction, float distance)
        {
            return (from + distance * Vector3.Normalize(direction - from)).LSTo2D();
        }

        public static bool missleWillHit(MissileClient missle)
        {
            if (missle.Target.IsMe || YasMath.interCir(missle.StartPosition.LSTo2D(), missle.EndPosition.LSTo2D(), Player.Position.LSTo2D(), missle.SData.LineWidth + Player.BoundingRadius))
            {
                if (missle.StartPosition.LSDistance(Player.Position) < (missle.StartPosition.LSDistance(missle.EndPosition)))
                    return true;
            }
            return false;
        }


        public static bool useENormal(Obj_AI_Base target)
        {
            if (!E.IsReady() || target.LSDistance(Player) > 470)
                return false;
            Vector2 posAfter = V2E(Player.Position, target.Position, 475);
            if (!YasuoSharp.extra["djTur"].Cast<CheckBox>().CurrentValue)
            {
                if (isSafePoint(posAfter).IsSafe)
                {
                    E.Cast(target, false);
                }
                return true;
            }
            else
            {
                Vector2 pPos = Player.ServerPosition.LSTo2D();
                Vector2 posAfterE = pPos + (Vector2.Normalize(target.Position.LSTo2D() - pPos) * E.Range);
                if (!(posAfterE.To3D().UnderTurret(true)))
                {
                    Console.WriteLine("use gap?");
                    if (isSafePoint(posAfter, true).IsSafe)
                    {
                        E.Cast(target, false);
                    }
                    return true;
                }
            }
            return false;

        }

        public static bool useESmart(AIHeroClient target, List<AIHeroClient> ignore = null)
        {
            if (!E.IsReady())
                return false;
            float trueAARange = Player.AttackRange + target.BoundingRadius;
            float trueERange = target.BoundingRadius + E.Range;

            float dist = Player.LSDistance(target);
            Vector2 dashPos = new Vector2();
            if (target.IsMoving && target.Path.Count() != 0)
            {
                Vector2 tpos = target.Position.LSTo2D();
                Vector2 path = target.Path[0].LSTo2D() - tpos;
                path.Normalize();
                dashPos = tpos + (path * 100);
            }
            float targ_ms = (target.IsMoving && Player.LSDistance(dashPos) > dist) ? target.MoveSpeed : 0;
            float msDif = (Player.MoveSpeed - targ_ms) == 0 ? 0.0001f : (Player.MoveSpeed - targ_ms);
            float timeToReach = (dist - trueAARange) / msDif;
            if (dist > trueAARange && dist < E.Range)
            {
                if (timeToReach > 1.7f || timeToReach < 0.0f)
                {
                    if (useENormal(target))
                        return true;
                }
            }
            return false;
        }

        public static void gapCloseE(Vector2 pos, List<AIHeroClient> ignore = null)
        {
            if (!E.IsReady())
                return;

            Vector2 pPos = Player.ServerPosition.LSTo2D();
            Obj_AI_Base bestEnem = null;


            float distToPos = Player.LSDistance(pos);
            if (((distToPos < Q.Range)) &&
                goesThroughWall(pos.To3D(), Player.Position))
                return;
            Vector2 bestLoc = pPos + (Vector2.Normalize(pos - pPos) * (Player.MoveSpeed * 0.35f));
            float bestDist = pos.LSDistance(pPos) - 50;
            try
            {
                foreach (Obj_AI_Base enemy in ObjectManager.Get<Obj_AI_Base>().Where(ob => enemyIsJumpable(ob, ignore)))
                {

                    float trueRange = E.Range + enemy.BoundingRadius;
                    float distToEnem = Player.LSDistance(enemy);
                    if (distToEnem < trueRange && distToEnem > 15)
                    {
                        Vector2 posAfterE = pPos + (Vector2.Normalize(enemy.Position.LSTo2D() - pPos) * E.Range);
                        float distE = pos.LSDistance(posAfterE);
                        if (distE < bestDist)
                        {
                            bestLoc = posAfterE;
                            bestDist = distE;
                            bestEnem = enemy;
                            // Console.WriteLine("Gap to best enem");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            if (bestEnem != null)
            {
                Console.WriteLine("should use gap");
                useENormal(bestEnem);
            }

        }

        private const int RWidth = 400;

        public static void useRSmart()
        {
            var obj = (from enemy in HeroManager.Enemies.Where(i => R.IsInRange(i) && CanCastR(i))
                       let sub = enemy.GetEnemiesInRange(RWidth).Where(CanCastR).ToList()
                       where
                           (sub.Count > 1 && R.IsKillable(enemy))
                           || sub.Any(i => i.HealthPercent < YasuoSharp.smartR["useRHP"].Cast<Slider>().CurrentValue)
                           || sub.Count >= YasuoSharp.smartR["useRHit"].Cast<Slider>().CurrentValue
                       orderby sub.Count descending
                       select enemy).ToList();
            if (obj.Any())
            {
                var target = !YasuoSharp.smartR["useRHitTime"].Cast<CheckBox>().CurrentValue ? obj.FirstOrDefault() : obj.Where(i => TimeLeftR(i) * 1000 < 150 + Game.Ping * 2).MinOrDefault(TimeLeftR);
                if (target != null && R.CastOnUnit(target))
                {
                    return;
                }
            }
            /*
            float timeToLand = float.MaxValue;
            List<AIHeroClient> enemInAir = getKockUpEnemies(ref timeToLand);
            foreach (AIHeroClient enem in enemInAir)
            {
                int aroundAir = 0;
                foreach (AIHeroClient enem2 in enemInAir)
                {
                    if (Vector3.DistanceSquared(enem.ServerPosition, enem2.ServerPosition) < 400 * 400)
                        aroundAir++;
                }
                if (aroundAir >= YasuoSharp.smartR["useRHit"].Cast<Slider>().CurrentValue && timeToLand < 0.4f)
                    R.Cast(enem);
            }
            */
        }

        private static float TimeLeftR(AIHeroClient target)
        {
            var buff = target.Buffs.FirstOrDefault(i => i.Type == BuffType.Knockback || i.Type == BuffType.Knockup);
            return buff != null ? buff.EndTime - Game.Time : -1;
        }

        public static List<AIHeroClient> getKockUpEnemies(ref float lessKnockTime)
        {
            List<AIHeroClient> enemKonck = new List<AIHeroClient>();
            foreach (AIHeroClient enem in ObjectManager.Get<AIHeroClient>().Where(enem => enem.IsEnemy))
            {
                foreach (BuffInstance buff in enem.Buffs)
                {
                    if (buff.Type == BuffType.Knockback || buff.Type == BuffType.Knockup)
                    {
                        if (buff.Type == BuffType.Knockup)
                            lessKnockTime = (buff.EndTime - Game.Time) < lessKnockTime ? (buff.EndTime - Game.Time) : lessKnockTime;
                        enemKonck.Add(enem);
                        break;
                    }
                }
            }
            if (!YasuoSharp.smartR["useRHitTime"].Cast<CheckBox>().CurrentValue)
                lessKnockTime = 0;
            return enemKonck;
        }

        private static bool CanCastR(AIHeroClient target)
        {
            return target.HasBuffOfType(BuffType.Knockup) || target.HasBuffOfType(BuffType.Knockback);
        }

        public static bool isMissileCommingAtMe(MissileClient missle)
        {
            Vector3 step = missle.StartPosition + Vector3.Normalize(missle.StartPosition - missle.EndPosition) * 10;
            return (!(Player.LSDistance(step) < Player.LSDistance(missle.StartPosition)));
        }

        public static bool enemyIsJumpable(Obj_AI_Base enemy, List<AIHeroClient> ignore = null)
        {
            if (enemy.IsValid && enemy.IsEnemy && !enemy.IsInvulnerable && !enemy.MagicImmune && !enemy.IsDead && !(enemy is FollowerObject))
            {
                if (ignore != null)
                    foreach (AIHeroClient ign in ignore)
                    {
                        if (ign.NetworkId == enemy.NetworkId)
                            return false;
                    }
                foreach (BuffInstance buff in enemy.Buffs)
                {
                    if (buff.Name == "YasuoDashWrapper")
                        return false;
                }
                return true;
            }
            return false;
        }

        public static float getSpellCastTime(LeagueSharp.Common.Spell spell)
        {
            return sBook.GetSpell(spell.Slot).SData.SpellCastTime;
        }

        public static float getSpellCastTime(SpellSlot slot)
        {
            return sBook.GetSpell(slot).SData.SpellCastTime;
        }
    }
}