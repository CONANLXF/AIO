using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;

 namespace YasuoPro
{
    class Helper
    {

        internal static AIHeroClient Yasuo;

        internal float QRadius = 170;

        internal static Obj_Shop shop = ObjectManager.Get<Obj_Shop>().FirstOrDefault(x => x.IsAlly);

        internal static bool DontDash = false;

        internal static int Q = 1, Q2 = 2, W = 3, E = 4, R = 5, Ignite = 6;

        internal const float LaneClearWaitTimeMod = 2f;

        internal static float WCLastE = 0f;

        internal static ItemManager.Item Hydra, Titanic, Tiamat, Blade, Bilgewater, Youmu;

        internal float LastTornadoClearTick;

        internal float LastDashTick;

        public static int TickCount
        {
            get { return (int)(Game.Time * 1000f); }
        }

        internal bool InDash
        {
            get { return TickCount - LastDashTick < 500; }
        }

        /* Credits to Brian for Q Skillshot values */
        internal static Dictionary<int, LeagueSharp.Common.Spell> Spells;


        internal void InitSpells()
        {
            Spells = new Dictionary<int, LeagueSharp.Common.Spell> {
            { 1, new LeagueSharp.Common.Spell(SpellSlot.Q, 450f) },
            { 2, new LeagueSharp.Common.Spell(SpellSlot.Q, 1100f) },
            { 3, new LeagueSharp.Common.Spell(SpellSlot.W, 450f) },
            { 4, new LeagueSharp.Common.Spell(SpellSlot.E, 475f) },
            { 5, new LeagueSharp.Common.Spell(SpellSlot.R, 1250f) },
            { 6, new LeagueSharp.Common.Spell(ObjectManager.Player.GetSpellSlot("summonerdot"), 600) }
            };

            Spells[Q].SetSkillshot(GetQ1Delay, 20f, float.MaxValue, false, SkillshotType.SkillshotLine);

            Spells[Q2].SetSkillshot(GetQ2Delay, 90, 1500, false, SkillshotType.SkillshotLine);
            Spells[E].SetTargetted(0.075f, 1025);
        }

        private static float GetQDelay { get { return 1 - Math.Min((Yasuo.AttackSpeedMod - 1) * 0.0058552631578947f, 0.6675f); } }

        private static float GetQ1Delay { get { return 0.4f * GetQDelay; } }

        private static float GetQ2Delay { get { return 0.5f * GetQDelay; } }


        internal float Qrange
        {
            get { return TornadoReady ? Spells[Q2].Range : Spells[Q].Range; }
        }

        internal float Qdelay
        {
            get
            {
                return 0.250f - (Math.Min(BonusAttackSpeed, 0.66f) * 0.250f);
            }
        }


        internal float BonusAttackSpeed
        {
            get
            {
                return (1 / Yasuo.AttackDelay) - 0.658f;
            }
        }

        internal float Erange
        {
            get { return Spells[E].Range; }
        }

        internal float Rrange
        {
            get { return Spells[R].Range; }
        }

        internal bool TornadoReady
        {
            get { return Yasuo.HasBuff("yasuoq3w"); }
        }

        internal static int DashCount
        {
            get
            {
                var bc = Yasuo.GetBuffCount("yasuodashscalar");
                return bc;
            }
        }

        internal float QLeftPCT
        {
            get
            {
                var buff = Yasuo.GetBuff("yasuoq3w");
                if (buff != null)
                {
                    var timeLeft = buff.EndTime - Game.Time;
                    var totalDuration = buff.EndTime - buff.StartTime;
                    var pctRemain = timeLeft / totalDuration;
                    return pctRemain;

                }
                return 0;
            }
        }



        internal bool ShouldNormalQ(AIHeroClient target)
        {
            var pos = GetDashPos(target);
            return QLeftPCT <= 30 || !TowerCheck(pos, true) || !target.IsDashable(Spells[E].Range * 3) || !targInKnockupRadius(pos.To3D());
        }


        internal bool UseQ(AIHeroClient target, HitChance minhc = HitChance.Medium, bool UseQ1 = true, bool UseQ2 = true)
        {
            if (target == null)
            {
                return false;
            }

            if (Yasuo.LSIsDashing() || InDash)
            {
                return false;
            }

            var tready = TornadoReady;

            if ((tready && !UseQ2) || !tready && !UseQ1)
            {
                return false;
            }

            if (GetMode() == Modes.Beta && !ShouldNormalQ(target))
            {
                if (tready && GetBool("Combo.UseEQ", YasuoMenu.ComboM))
                {
                    if (Spells[E].IsReady() && target.IsDashable(500))
                    {
                        var dashPos = GetDashPos(target);
                        if (dashPos.To3D().CountEnemiesInRange(QRadius) >= 1)
                        {
                            //Cast E to trigger EQ 
                            if (GetBool("Misc.saveQ4QE", YasuoMenu.MiscM) && isHealthy && GetBool("Combo.UseE", YasuoMenu.ComboM) &&
                                (GetBool("Combo.ETower", YasuoMenu.ComboM) || GetKeyBind("Misc.TowerDive", YasuoMenu.MiscM) ||
                                 !GetDashPos(target).PointUnderEnemyTurret()))
                            {
                                return Spells[E].CastOnUnit(target);
                            }
                        }
                    }
                }
            }

            else
            {

                LeagueSharp.Common.Spell sp = tready ? Spells[Q2] : Spells[Q];
                PredictionOutput pred = sp.GetPrediction(target);

                if (pred.Hitchance >= minhc)
                {
                    return sp.Cast(pred.CastPosition);
                }
            }

            return false;
        }

        internal IEnumerable<AIHeroClient> KnockedUp
        {
            get
            {
                List<AIHeroClient> KnockedUpEnemies = new List<AIHeroClient>();
                foreach (var hero in HeroManager.Enemies)
                {
                    if (hero.IsValidEnemy(Spells[R].Range))
                    {
                        var knockup = hero.Buffs.Find(x => (x.Type == BuffType.Knockup && (x.EndTime - Game.Time) <= (GetSliderFloat("Combo.knockupremainingpct", YasuoMenu.ComboM) / 100) * (x.EndTime - x.StartTime)) || x.Type == BuffType.Knockback);
                        if (knockup != null)
                        {
                            KnockedUpEnemies.Add(hero);
                        }
                    }
                }
                return KnockedUpEnemies;
            }
        }

        internal static bool isHealthy
        {
            get { return Yasuo.IsInvulnerable || Yasuo.HasBuffOfType(BuffType.Invulnerability) || Yasuo.HasBuffOfType(BuffType.SpellShield) || Yasuo.HasBuffOfType(BuffType.SpellImmunity) || Yasuo.HealthPercent > GetSliderFloat("Misc.Healthy", YasuoMenu.MiscM) || Yasuo.HasBuff("yasuopassivemovementshield") && Yasuo.HealthPercent > 30; }
        }

        internal static bool GetBool(string name, Menu m)
        {
            return m[name].Cast<CheckBox>().CurrentValue;
        }

        internal static bool GetKeyBind(string name, Menu m)
        {
            return m[name].Cast<KeyBind>().CurrentValue;
        }

        internal static int GetSliderInt(string name, Menu m)
        {
            return m[name].Cast<Slider>().CurrentValue;
        }

        internal static float GetSliderFloat(string name, Menu m)
        {
            return m[name].Cast<Slider>().CurrentValue;
        }


        internal static int GetSL(string name, Menu m)
        {
            return m[name].Cast<ComboBox>().CurrentValue;
        }

        internal static bool GetCircle(string name, Menu m)
        {
            return m[name].Cast<CheckBox>().CurrentValue;
        }

        internal static Vector2 DashPosition;

        internal static Vector2 GetDashPos(Obj_AI_Base @base)
        {
            var predictedposition = Yasuo.ServerPosition.LSExtend(@base.Position, Yasuo.LSDistance(@base) + 475 - Yasuo.LSDistance(@base)).LSTo2D();
            DashPosition = predictedposition;
            return predictedposition;
        }

        internal static Vector2 GetDashPosFrom(Vector2 startPos, Obj_AI_Base @base)
        {
            var startPos3D = startPos.To3D();
            var predictedposition = startPos3D.LSExtend(@base.Position, startPos.LSDistance(@base) + 475 - startPos.LSDistance(@base)).LSTo2D();
            DashPosition = predictedposition;
            return predictedposition;
        }

        internal static double GetProperEDamage(Obj_AI_Base target)
        {
            double dmg = Yasuo.LSGetSpellDamage(target, SpellSlot.E);
            float amplifier = 0;
            if (DashCount == 0)
            {
                amplifier = 0;
            }
            else if (DashCount == 1)
            {
                amplifier = 0.25f;
            }
            else if (DashCount == 2)
            {
                amplifier = 0.50f;
            }
            dmg += dmg * amplifier;
            return dmg;
        }

        internal static bool Debug
        {
            get { return GetBool("Misc.Debug", YasuoMenu.MiscM); }
        }

        internal static HitChance GetHitChance(String search)
        {
            switch (GetSL(search, YasuoMenu.MiscM))
            {
                case 0:
                    return HitChance.Low;
                case 1:
                    return HitChance.Medium;
                case 2:
                    return HitChance.High;
                case 3:
                    return HitChance.VeryHigh;
            }
            return HitChance.Medium;
        }


        internal FleeType FleeMode
        {
            get
            {
                var GetFM = GetSL("Flee.Mode", YasuoMenu.MiscM);
                if (GetFM == 0)
                {
                    return FleeType.ToNexus;
                }
                if (GetFM == 1)
                {
                    return FleeType.ToAllies;
                }
                return FleeType.ToCursor;
            }
        }

        internal enum FleeType
        {
            ToNexus,
            ToAllies,
            ToCursor,
        }

        internal enum UltMode
        {
            Health,
            Priority,
            EnemiesHit
        }

        internal UltMode GetUltMode()
        {
            switch (GetSL("Combo.UltMode", YasuoMenu.ComboM))
            {
                case 0:
                    return UltMode.Health;
                case 1:
                    return UltMode.Priority;
                case 2:
                    return UltMode.EnemiesHit;
            }
            return UltMode.Priority;
        }

        internal void InitItems()
        {
            Hydra = new ItemManager.Item(3074, 225f, ItemManager.ItemCastType.RangeCast, 1, 2);
            Tiamat = new ItemManager.Item(3077, 225f, ItemManager.ItemCastType.RangeCast, 1, 2);
            Titanic = new ItemManager.Item(3053, 225f, ItemManager.ItemCastType.RangeCast, 1, 2);
            Blade = new ItemManager.Item(3153, 450f, ItemManager.ItemCastType.TargettedCast, 1);
            Bilgewater = new ItemManager.Item(3144, 450f, ItemManager.ItemCastType.TargettedCast, 1);
            Youmu = new ItemManager.Item(3142, 185f, ItemManager.ItemCastType.SelfCast, 1, 3);
        }

        internal bool TowerCheck(Obj_AI_Base unit, bool isCombo = false)
        {
            return (isCombo && GetBool("Combo.ETower", YasuoMenu.ComboM) || GetKeyBind("Misc.TowerDive", YasuoMenu.MiscM) ||
                    !GetDashPos(unit).PointUnderEnemyTurret());
        }

        internal bool ShouldDive(Obj_AI_Base unit)
        {
            return GetKeyBind("Misc.TowerDive", YasuoMenu.MiscM) || !Helper.GetDashPos(unit).PointUnderEnemyTurret();
        }

        internal bool TowerCheck(Vector2 pos, bool isCombo = false)
        {
            return (isCombo && GetBool("Combo.ETower", YasuoMenu.ComboM) || GetKeyBind("Misc.TowerDive", YasuoMenu.MiscM) ||
                    pos.PointUnderEnemyTurret());
        }

        internal bool targInKnockupRadius(AIHeroClient targ)
        {
            var dpos = GetDashPos(targ).To3D();
            return dpos.CountEnemiesInRange(QRadius) > 0;
        }

        internal bool targInKnockupRadius(Vector3 dpos)
        {
            return dpos.CountEnemiesInRange(QRadius) > 0;
        }

        internal Modes GetMode()
        {
            var mode = YasuoMenu.ComboM["Combo.Mode"].Cast<ComboBox>().CurrentValue;
            if (mode == 0)
            {
                return Modes.Old;
            }
            else
            {
                return Modes.Beta;
            }
        }


        internal enum Modes
        {
            Old,
            Beta
        }

    }
}