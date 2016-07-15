using System.Linq;
using Firestorm_AIO.Helpers;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;

using static Firestorm_AIO.Helpers.Helpers;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;

 namespace Firestorm_AIO.Champions.Yasuo
{
    public class Yasuo : Bases.ChampionBase
    {
        private static LeagueSharp.SDK.Spell Q3;
        private static int QCircleRange = 375;

        public override void Init()
        {
            HasMana = false;

            Q = new LeagueSharp.SDK.Spell(SpellSlot.Q, 475);
            Q3 = new LeagueSharp.SDK.Spell(SpellSlot.Q, 1100);
            W = new LeagueSharp.SDK.Spell(SpellSlot.W, 400);
            E = new LeagueSharp.SDK.Spell(SpellSlot.E, 475);
            R = new LeagueSharp.SDK.Spell(SpellSlot.R, 1200);

            Q.SetSkillshot(0.325f, 55f, float.MaxValue, false, SkillshotType.SkillshotLine);
            Q3.SetSkillshot(0.3f, 90f, 1200f, false, SkillshotType.SkillshotLine);

            DashManager.Load();

            Events.OnGapCloser += Events_OnGapCloser;
        }

        private void Events_OnGapCloser(object sender, Events.GapCloserEventArgs e)
        {
            if (e.Sender.IsAlly) return;

            if (e.IsDirectedToPlayer && HasQ3() && MiscMenu["qGap"].Cast<CheckBox>().CurrentValue)
            {
                Q3.CastIfHitchanceMinimum(e.Sender, HitChance.Medium);
            }
        }

        #region Functions

        #region Q

        private static bool HasQ3()
        {
            return Me.HasBuff("yasuoq3w");
        }

        private static int GetQRange()
        {
            return HasQ3() ? 1100 : 475;
        }

        //Cast
        private static void CastQ(Obj_AI_Base target)
        {
            if (target == null) return;

            if (Me.IsDashing() && DashManager.GetPlayerPosition(350).Distance(target.Position) <= QCircleRange)
            {
                Q.Cast();
            }

            if (HasQ3())
            {
                Q3.SmartCast(target, HitChance.High);
            }
            else
            {
                Q.SmartCast(target);
            }
        }

        #endregion Q

        #region E

        private static int ECount()
        {
            // ReSharper disable once StringLiteralTypo
            var count = Me.GetBuffCount("yasuodashscalar");
            return count > 0 ? count : 0;
        }

        private static bool HasEBuff(Obj_AI_Base target)
        {
            // ReSharper disable once StringLiteralTypo
            return target.HasBuff("yasuodashwrapper");
        }

        private static void CastE(Obj_AI_Base target)
        {
            if (target == null || !target.IsSafeToE() || HasEBuff(target)) return;
            E.SmartCast(target);
        }

        private static Obj_AI_Base GetBestGapcloserE()
        {
            return
                GameObjects.Enemy.Where(
                    m =>
                    !HasEBuff(m) &&
                        m.LSIsValidTarget(E.Range)).OrderBy(m => m.Distance(Target))
                    .ThenByDescending(m => m.Distance(Me))
                    .ThenBy(m => m.Distance(Game.CursorPos))
                    .FirstOrDefault(m => m.GetPosAfterE().Distance(Target.Position) < Me.Distance(Target));
        }

        private static Obj_AI_Base GetBestEscapeE()
        {
            return
                GameObjects.Enemy.Where(
                    m =>
                        !HasEBuff(m) &&
                        m.LSIsValidTarget(E.Range)).OrderBy(m => m.Distance(Game.CursorPos))
                        .ThenByDescending(m => Target != null ? m.Distance(Target) : 0)
                    .ThenByDescending(m => m.Distance(Me))
                    .FirstOrDefault();
        }

        #endregion E

        #region R

        private static int GetLowestKnockupTime()
        {
            return
                (int) GameObjects.EnemyHeroes.Where(a => a.IsKnockedUp() && a.LSIsValidTarget(R.Range)).Select(a =>
                {
                    var buff = a.Buffs.FirstOrDefault(b => b.Type == BuffType.Knockup || b.Type == BuffType.Knockback);
                    return buff != null ? (buff.EndTime - Game.Time)*1000 : 0;
                }).OrderBy(a => a).FirstOrDefault();
        }

        #endregion R

        private static bool HasShield()
        {
            return Me.Mana >= 100;
        }

        #endregion Functions

        public override void Menu()
        {
            Q.CreateBool(ComboMenu);
            E.CreateBool(ComboMenu);
            R.CreateBool(ComboMenu);
            ComboMenu.Add("rCount", new Slider("Only R if there are X knockedup", 2, 0, 5));

            Q.CreateBool(MixedMenu);
            //E.CreateBool(MixedMenu);

            Q.CreateBool(LaneClearMenu);
            E.CreateBool(LaneClearMenu);

            Q.CreateBool(LastHitMenu);
            E.CreateBool(LastHitMenu);

            Q.CreateBool(JungleClearMenu);
            E.CreateBool(JungleClearMenu);

            MiscMenu.Add("qGap", new CheckBox("Use Q3 on gapclosers", true));
        }

        #region Modes
        
        public override void Active()
        {
            Target = TargetSelector.GetTarget(R.Range, DamageType.Physical);

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                CastE(GetBestEscapeE());
            }

            if (Target == null) return;
            if (GetBoolValue(R, ComboMenu))
            {
                if (Target.HealthPercent > Me.HealthPercent - 10 || Me.HealthPercent < 10 || !Target.IsKnockedUp()) return;

                var count = GameObjects.EnemyHeroes.Count(h => h.IsValidTarget(R.Range) && h.IsKnockedUp() && h.Health > R.GetDamage(h));

                var time = GetLowestKnockupTime();

                if (count >= ComboMenu["rCount"].Cast<Slider>().CurrentValue && time < 100 + Game.Ping && time > 0)
                {
                    R.Cast();
                }
            }
        }

        public override void Combo()
        {
            if (GetBoolValue(Q, ComboMenu))
            {
                CastQ(Target);
            }

            if (GetBoolValue(E, ComboMenu))
            {
                if (!Target.LSIsInAARange())
                {
                    CastE(GetBestGapcloserE());
                }

                if (!Target.IsInRange(Me, 430))
                {
                    CastE(Target);
                }
            }

            if (GetBoolValue(R, ComboMenu))
            {
                if (Target.HealthPercent > Me.HealthPercent - 10 || Me.HealthPercent < 10 || !Target.IsKnockedUp()) return;

                var count = GameObjects.EnemyHeroes.Count(h => h.IsValidTarget(R.Range) && h.IsKnockedUp() && h.Health > R.GetDamage(h));

                var time = GetLowestKnockupTime();

                if (count >= ComboMenu["rCount"].Cast<Slider>().CurrentValue && time < 100 + Game.Ping && time > 0)
                {
                    R.Cast();
                }
            }
        }

        public override void Mixed()
        {
            if (GetBoolValue(Q, MixedMenu))
            {
                CastQ(Target);
            }
        }

        public override void LaneClear()
        {
            if (Me.CountEnemyHeroesInRange(1200) >= 1)
            {
                if (!Me.IsDashing())
                {
                    var minionQ =
                        GameObjects.EnemyMinions.OrderBy(m => m.Health)
                            .FirstOrDefault(
                                m =>
                                    m.LSIsValidTarget(GetQRange()) &&
                                    Health.GetPrediction(m,
                                        (int)Q.Delay + (E.IsReady() && m.IsSafeToE() ? 150 : 0) + Game.Ping) <
                                    Q.GetDamage(m) + (E.IsReady() && !HasEBuff(m) ? E.GetDamage(m) : 0f));

                    if (!HasQ3() && minionQ != null && GetBoolValue(Q, LaneClearMenu)) CastQ(minionQ);

                    var minionE =
                        GameObjects.EnemyMinions.OrderBy(m => m.Health)
                            .FirstOrDefault(
                                m =>
                                    m.LSIsValidTarget(E.Range) && m.IsSafeToE() &&
                                    Health.GetPrediction(m, 30 + Game.Ping) < E.GetDamage(m));

                    if (minionE != null && GetBoolValue(E, LaneClearMenu)) CastE(minionE);
                }
            }
            //Faster Laneclear(faster, not safe)
            else
            {
                if (!Me.IsDashing())
                {
                    //LastHit
                    var minionQ =
                        GameObjects.EnemyMinions.OrderBy(m => m.Health)
                            .FirstOrDefault(
                                m =>
                                    m.LSIsValidTarget(GetQRange()) &&
                                    Health.GetPrediction(m,
                                        (int) Q.Delay + (E.IsReady() ? 30 : 0) + Game.Ping) <=
                                    Q.GetDamage(m) +
                                    (E.IsReady() && !HasEBuff(m) && m.IsSafeToE() ? E.GetDamage(m) : 0f));

                    if (minionQ != null && GetBoolValue(Q, LaneClearMenu)) CastQ(minionQ);

                    var minionE =
                        GameObjects.EnemyMinions.OrderBy(m => m.Health)
                            .FirstOrDefault(
                                m =>
                                    m.LSIsValidTarget(E.Range) && !HasEBuff(m) &&
                                    Health.GetPrediction(m, 30 + Game.Ping) <= E.GetDamage(m));

                    if (minionE != null && GetBoolValue(E, LaneClearMenu)) CastE(minionE);
                    //Lasthit
                }

                if (Me.CountAllyMinions(900) >= 5)
                {
                    //Fast
                    var minionEKinda =
                        GameObjects.EnemyMinions.OrderBy(m => m.Health)
                            .FirstOrDefault(
                                m =>
                                    m.LSIsValidTarget(E.Range) && !HasEBuff(m) &&
                                    m.GetPosAfterE().CountEnemyMinions(QCircleRange) >= 1 &&
                                    Health.GetPrediction(m, 30 + Game.Ping) <
                                    E.GetDamage(m));

                    if (minionEKinda != null && GetBoolValue(E, LaneClearMenu)) CastE(minionEKinda);
                    if (Me.IsDashing())
                    {
                        var minionQFast =
                            GameObjects.EnemyMinions.OrderBy(m => m.Health)
                                .FirstOrDefault(
                                    m =>
                                        m.LSIsValidTarget(750) &&
                                        DashManager.GetPlayerPosition(250).CountEnemyMinions(QCircleRange) >= 1);

                        if (minionQFast != null && GetBoolValue(Q, LaneClearMenu)) CastQ(minionQFast);
                    }

                    //Fast
                }

                if (Me.CountAllyMinions(900) <= 1)
                {
                    //Fast
                    var minionEFast =
                        GameObjects.EnemyMinions.OrderBy(m => m.Health)
                            .FirstOrDefault(
                                m =>
                                    m.LSIsValidTarget(E.Range) && !HasEBuff(m) &&
                                    m.GetPosAfterE().CountEnemyMinions(QCircleRange) >= 1);

                    if (minionEFast != null && GetBoolValue(E, LaneClearMenu)) CastE(minionEFast);

                    var minionQFast =
                        GameObjects.EnemyMinions.OrderBy(m => m.Health)
                            .FirstOrDefault(
                                m =>
                                    m.LSIsValidTarget(750) &&
                                    DashManager.GetPlayerPosition(250).CountEnemyMinions(QCircleRange) >= 1);

                    if (minionQFast != null && GetBoolValue(Q, LaneClearMenu)) CastQ(minionQFast);
                    //Fast
                }
            }

            //Jungleclear
            if (GetBoolValue(E, JungleClearMenu))
            {
                E.SmartCast(E.GetBestJungleClearMinion());
            }

            if (GetBoolValue(Q, JungleClearMenu))
            {
                Q.SmartCast(Q.GetBestJungleClearMinion());
            }
        }

        public override void LastHit()
        {
            if (!Me.IsDashing())
            {
                var minionQ =
                    GameObjects.EnemyMinions.OrderBy(m => m.Health)
                        .FirstOrDefault(
                            m =>
                                m.LSIsValidTarget(GetQRange()) &&
                                Health.GetPrediction(m,
                                    (int) Q.Delay + (E.IsReady() ? 30 : 0) + Game.Ping) <=
                                Q.GetDamage(m) + (E.IsReady() ? E.GetDamage(m) : 0f));

                if (!HasQ3() && minionQ != null && GetBoolValue(Q, LastHitMenu)) CastQ(minionQ);

                var minionE =
                    GameObjects.EnemyMinions.OrderBy(m => m.Health)
                        .FirstOrDefault(
                            m =>
                                m.LSIsValidTarget(E.Range) &&

                                Health.GetPrediction(m, 30 + Game.Ping) <= E.GetDamage(m));

                if (minionE != null && GetBoolValue(E, LastHitMenu)) CastE(minionE);
            }
        }

        public override void KillSteal()
        {
        }

        #endregion Modes

        public override void Draw()
        {
        }
    }
}
