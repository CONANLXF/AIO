 namespace ElLux
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Color = System.Drawing.Color;
    using EloBuddy.SDK.Menu;
    using EloBuddy;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK;
    internal enum Spells
    {
        Q,

        W,

        E,

        R
    }

    internal static class Lux
    {
        #region Static Fields

        public static AIHeroClient AggroTarget;

        public static Dictionary<Spells, LeagueSharp.Common.Spell> spells = new Dictionary<Spells, LeagueSharp.Common.Spell>
                                                             {
                                                                 { Spells.Q, new LeagueSharp.Common.Spell(SpellSlot.Q, 1175f) },
                                                                 { Spells.W, new LeagueSharp.Common.Spell(SpellSlot.W, 1075f) },
                                                                 { Spells.E, new LeagueSharp.Common.Spell(SpellSlot.E, 1075f - 100f) },
                                                                 { Spells.R, new LeagueSharp.Common.Spell(SpellSlot.R, 3340f) }
                                                             };

        private static float incomingDamage;

        public static Menu Menu, cMenu, hMenu, wMenu, lMenu, jMenu, miscMenu, ksMenu, drawMenu;

        #endregion

        #region Public Properties

        public static bool EState => spells[Spells.E].Instance.Name == "LuxLightStrikeKugel";

        public static string ScriptVersion => typeof(Lux).Assembly.GetName().Version.ToString();

        #endregion

        #region Properties

        private static AIHeroClient Player => ObjectManager.Player;

        private static GameObject Troy { get; set; }

        #endregion

        #region Public Methods and Operators

        public static void OnLoad(EventArgs args)
        {
            if (ObjectManager.Player.CharData.BaseSkinName != "Lux")
            {
                return;
            }

            spells[Spells.Q].SetSkillshot(0.25f, 70f, 1200f, false, SkillshotType.SkillshotLine);
            spells[Spells.E].SetSkillshot(0.25f, 275f, 1300f, false, SkillshotType.SkillshotCircle);
            spells[Spells.R].SetSkillshot(1000f, 190f, int.MaxValue, false, SkillshotType.SkillshotCircle);

            ElLuxMenu.Initialize();
            Game.OnUpdate += OnUpdate;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            GameObject.OnCreate += Obj_AI_Base_OnCreate;
            GameObject.OnDelete += Obj_AI_Base_OnDelete;
            Drawing.OnDraw += OnDraw;

            Menu = ElLuxMenu.Menu;
            cMenu = ElLuxMenu.cMenu;
            hMenu = ElLuxMenu.hMenu;
            wMenu = ElLuxMenu.wMenu;
            lMenu = ElLuxMenu.lMenu;
            jMenu = ElLuxMenu.jMenu;
            miscMenu = ElLuxMenu.miscMenu;
            ksMenu = ElLuxMenu.ksMenu;
            drawMenu = ElLuxMenu.drawMenu;
        }

        #endregion

        #region Methods

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }

        private static AIHeroClient Allies()
        {
            var target = Player;
            foreach (var unit in
                ObjectManager.Get<AIHeroClient>()
                    .Where(x => x.IsAlly && x.LSIsValidTarget(900, false))
                    .OrderByDescending(xe => xe.Health / xe.MaxHealth * 100))
            {
                target = unit;
            }

            return target;
        }

        private static void AutoE()
        {
            var target =
                ObjectManager.Get<AIHeroClient>()
                    .FirstOrDefault(
                        h => h.LSIsValidTarget(spells[Spells.E].Range) && h.GetStunDuration() >= spells[Spells.E].Delay);

            if (target.LSIsValidTarget() && target != null)
            {
                spells[Spells.E].Cast(target.Position);
            }
        }

        private static void AutoQ()
        {
            var target =
                ObjectManager.Get<AIHeroClient>()
                    .FirstOrDefault(
                        h => h.LSIsValidTarget(spells[Spells.Q].Range) && h.GetStunDuration() >= spells[Spells.Q].Delay);

            if (target.LSIsValidTarget() && target != null)
            {
                spells[Spells.Q].Cast(target.Position);
            }
        }

        private static bool CastE(AIHeroClient target)
        {
            if (!spells[Spells.E].IsReady() || !target.LSIsValidTarget(spells[Spells.E].Range))
            {
                return false;
            }

            var prediction = LeagueSharp.Common.Prediction.GetPrediction(target, 0.2f);

            var cast = spells[Spells.E].GetPrediction(target);
            var castPos = spells[Spells.E].IsInRange(cast.CastPosition) ? cast.CastPosition : target.ServerPosition;

            return cast.Hitchance >= HitChance.High && spells[Spells.E].Cast(castPos);
        }


        private static void CastQ(AIHeroClient target)
        {
            if (!spells[Spells.Q].IsReady() || !target.IsValidTarget(spells[Spells.Q].Range))
            {
                return;
            }

            var prediction = spells[Spells.Q].GetPrediction(target, false, -1, new[] { CollisionableObjects.YasuoWall });
            if (prediction.Hitchance < HitChance.VeryHigh)
            {
                return;
            }

            var collision = spells[Spells.Q].GetCollision(
                Player.ServerPosition.LSTo2D(),
                new List<Vector2>
                    {
                        prediction.CastPosition.LSTo2D()
                    });

            if (collision.Count == 1 || (collision.Count == 1 && collision.ElementAt(0).IsChampion())
                || collision.Count <= 1
                || (collision.Count == 2 && (collision.ElementAt(0).IsChampion() || collision.ElementAt(1).IsChampion())))
            {
                spells[Spells.Q].Cast(prediction.CastPosition);
            }
            else if (prediction.Hitchance >= HitChance.VeryHigh)
            {
                spells[Spells.Q].Cast(prediction.CastPosition);
            }
        }

        private static void CastR(Obj_AI_Base target)
        {
            if (!spells[Spells.R].IsReady() || !target.LSIsValidTarget(spells[Spells.R].Range)
                || !getCheckBoxItem(cMenu, "ElLux.Combo.R") || target.IsZombie || target.IsDead)
            {
                return;
            }


            if (getCheckBoxItem(cMenu, "ElLux.Combo.R.AOE"))
            {
                const float LuxRDistance = 3340;
                const float LuxRWidth = 70;
                var minREnemies = getSliderItem(cMenu, "ElLux.Combo.R.Count")
;
                foreach (var enemy in HeroManager.Enemies)
                {
                    var startPos = enemy.ServerPosition;
                    var endPos = Player.ServerPosition.LSExtend(
                        startPos,
                        Player.Distance(enemy) + LuxRDistance);

                    var rectangle = new LeagueSharp.Common.Geometry.Polygon.Rectangle(startPos, endPos, LuxRWidth);
                    if (HeroManager.Enemies.Count(x => rectangle.IsInside(x)) >= minREnemies)
                    {
                        spells[Spells.R].Cast(enemy);
                    }
                }
            }

            if (getCheckBoxItem(cMenu, "ElLux.Combo.R.Rooted"))
            {
                if (target.HasBuff("LuxLightBindingMis"))
                {
                    var prediction = spells[Spells.Q].GetPrediction(target);
                    if (prediction.Hitchance >= HitChance.High)
                    {
                        spells[Spells.R].Cast(prediction.CastPosition);
                    }
                }
            }

            if (spells[Spells.R].GetDamage(target) > GetHealth(target) && getCheckBoxItem(cMenu, "ElLux.Combo.R.Kill"))
            {
                var prediction = spells[Spells.R].GetPrediction(target);
                if (prediction.Hitchance >= HitChance.High)
                {
                    spells[Spells.R].Cast(prediction.CastPosition);
                }
            }
        }

        private static void CheckWDamage(float incdmg = 0)
        {
            if (!getCheckBoxItem(wMenu, "W.Activated"))
            {
                return;
            }

            var target = Allies();
            var iDamagePercent = (int)((incdmg / Player.MaxHealth) * 100);

            if (target.LSDistance(Player.ServerPosition) <= spells[Spells.W].Range
                && Player.LSCountEnemiesInRange(spells[Spells.W].Range) >= 1)
            {
                var aHealthPercent = (int)((target.Health / target.MaxHealth) * 100);
                if (aHealthPercent <= getSliderItem(wMenu, "W.Damage")
                    && getCheckBoxItem(wMenu, "wOn" + target.ChampionName) && !Player.LSIsRecalling()
                    && !Player.InFountain())
                {
                    if ((iDamagePercent >= 1 || incdmg >= target.Health))
                    {
                        if (AggroTarget.NetworkId == target.NetworkId)
                        {
                            spells[Spells.W].Cast(target.Position);
                        }
                    }
                }
            }
        }

        private static float GetHealth(Obj_AI_Base target)
        {
            return target.Health;
        }

        private static float GetStunDuration(this Obj_AI_Base target)
        {
            return
                target.Buffs.Where(
                    b =>
                    b.IsActive && Game.Time < b.EndTime
                    && (b.Type == BuffType.Charm || b.Type == BuffType.Knockback || b.Type == BuffType.Stun
                        || b.Type == BuffType.Suppression || b.Type == BuffType.Snare))
                    .Aggregate(0f, (current, buff) => Math.Max(current, buff.EndTime)) - Game.Time;
        }

        private static void KillstealHandler()
        {
            try
            {
                if (getCheckBoxItem(ksMenu, "ElLux.KS.R"))
                {
                    foreach (var target in HeroManager.Enemies.Where(t => t.LSIsValidTarget(spells[Spells.R].Range)))
                    {
                        if (spells[Spells.R].GetDamage(target) > target.Health)
                        {
                            var prediction = spells[Spells.R].GetPrediction(target);
                            if (prediction.Hitchance >= HitChance.High)
                            {
                                spells[Spells.R].Cast(prediction.CastPosition);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        private static void Obj_AI_Base_OnCreate(GameObject obj, EventArgs args)
        {
            if (obj.Name == "Lux_Base_E_mis.troy")
            {
                Troy = obj;
            }
        }

        private static void Obj_AI_Base_OnDelete(GameObject obj, EventArgs args)
        {
            if (obj.Name == "Lux_Base_E_tar_nova.troy")
            {
                Troy = null;
            }
        }

        private static void OnCombo()
        {
            var target = TargetSelector.GetTarget(spells[Spells.R].Range, DamageType.Magical);
            if (target == null)
            {
                return;
            }

            if (EState && getCheckBoxItem(cMenu, "ElLux.Combo.E") && target.LSIsValidTarget(spells[Spells.E].Range))
            {
                if (Troy == null)
                {
                    CastE(target);
                }
            }

            if (getCheckBoxItem(cMenu, "ElLux.Combo.Q"))
            {
                CastQ(target);
            }

            if (!EState && Troy != null && Troy.Position.CountEnemiesInRange(spells[Spells.E].Width) >= 1)
            {
                spells[Spells.E].Cast();
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
            }

            if (getCheckBoxItem(cMenu, "ElLux.Combo.R"))
            {
                CastR(target);
            }
        }

        private static void OnDraw(EventArgs args)
        {
            var drawOff = drawMenu["ElLux.Draw.off"].Cast<CheckBox>().CurrentValue;
            var drawQ = drawMenu["ElLux.Draw.Q"].Cast<CheckBox>().CurrentValue;
            var drawW = drawMenu["ElLux.Draw.W"].Cast<CheckBox>().CurrentValue;
            var drawE = drawMenu["ElLux.Draw.E"].Cast<CheckBox>().CurrentValue;
            var drawR = drawMenu["ElLux.Draw.R"].Cast<CheckBox>().CurrentValue;

            if (drawOff)
            {
                return;
            }

            if (drawQ)
            {
                if (spells[Spells.Q].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.Q].Range, Color.White);
                }
            }

            if (drawW)
            {
                if (spells[Spells.W].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.W].Range, Color.White);
                }
            }

            if (drawE)
            {
                if (spells[Spells.E].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.E].Range, Color.Purple);
                }
            }

            if (drawR)
            {
                if (spells[Spells.R].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.R].Range, Color.Gold);
                }
            }
        }

        private static void OnHarass()
        {
            var target = TargetSelector.GetTarget(spells[Spells.R].Range, DamageType.Magical);
            if (target == null || !target.LSIsValidTarget())
            {
                return;
            }

            if (EState && getCheckBoxItem(hMenu, "ElLux.Harass.E") && target.LSIsValidTarget(spells[Spells.E].Range))
            {
                if (Troy == null)
                {
                    CastE(target);
                }
            }


            if (!EState && Troy != null && Troy.Position.LSCountEnemiesInRange(spells[Spells.E].Width) >= 1)
            {
                spells[Spells.E].Cast();
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
            }

            if (getCheckBoxItem(hMenu, "ElLux.Harass.Q"))
            {
                CastQ(target);
            }
        }

        private static void OnJungleClear()
        {
            var minion = MinionManager.GetMinions(
                ObjectManager.Player.ServerPosition,
                spells[Spells.W].Range,
                MinionTypes.All,
                MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);

            if (minion.Count <= 0)
            {
                return;
            }

            if (getCheckBoxItem(jMenu, "ElLux.JungleClear.Q"))
            {
                var qFarm = spells[Spells.Q].GetLineFarmLocation(new List<Obj_AI_Base>(minion), spells[Spells.Q].Width);
                if (qFarm.MinionsHit >= 1)
                {
                    spells[Spells.Q].Cast(qFarm.Position);
                }
            }

            if (getCheckBoxItem(jMenu, "ElLux.JungleClear.W"))
            {
                spells[Spells.W].Cast(Player.Position);
            }

            if (getCheckBoxItem(jMenu, "ElLux.JungleClear.E"))
            {
                var eFarm = spells[Spells.E].GetCircularFarmLocation(
                    new List<Obj_AI_Base>(minion),
                    spells[Spells.E].Width);

                if (EState)
                {
                    if (Troy == null
                        && eFarm.MinionsHit >= getSliderItem(jMenu, "ElLux.JungleClear.E.Count"))
                    {
                        spells[Spells.E].Cast(eFarm.Position);
                    }

                    if (!EState && Troy != null)
                    {
                        spells[Spells.E].Cast();
                    }
                }
            }
        }

        private static void OnLaneClear()
        {
            var minion = MinionManager.GetMinions(
                ObjectManager.Player.ServerPosition,
                spells[Spells.E].Range,
                MinionTypes.All,
                MinionTeam.NotAlly,
                MinionOrderTypes.MaxHealth);

            if (minion.Count <= 0)
            {
                return;
            }

            if (getCheckBoxItem(lMenu, "ElLux.Laneclear.Q"))
            {
                var qFarm = spells[Spells.Q].GetLineFarmLocation(new List<Obj_AI_Base>(minion), spells[Spells.Q].Width);
                if (qFarm.MinionsHit >= 1)
                {
                    spells[Spells.Q].Cast(qFarm.Position);
                }
            }

            if (getCheckBoxItem(lMenu, "ElLux.Laneclear.W"))
            {
                spells[Spells.W].Cast(Player.Position);
            }

            if (getCheckBoxItem(lMenu, "ElLux.Laneclear.E"))
            {
                var eFarm = spells[Spells.E].GetCircularFarmLocation(
                    new List<Obj_AI_Base>(minion),
                    spells[Spells.E].Width);

                if (EState)
                {
                    if (Troy == null
                        && eFarm.MinionsHit >= getSliderItem(lMenu, "ElLux.Laneclear.E.Count"))
                    {
                        spells[Spells.E].Cast(eFarm.Position);
                    }
                }

                if (!EState && Troy != null)
                {
                    spells[Spells.E].Cast();
                }
            }
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsValid<AIHeroClient>() && sender.LSIsValidTarget() && sender.Type == GameObjectType.AIHeroClient && sender.IsEnemy && sender.IsVisible && sender.IsHPBarRendered)
            {
                var heroSender = ObjectManager.Get<AIHeroClient>().First(x => x.NetworkId == sender.NetworkId);
                if (heroSender.GetSpellSlot(args.SData.Name) == SpellSlot.Unknown && args.Target.Type == Player.Type)
                {
                    AggroTarget = ObjectManager.GetUnitByNetworkId<AIHeroClient>((uint)args.Target.NetworkId);
                    incomingDamage = (float)heroSender.LSGetAutoAttackDamage(AggroTarget);
                }

                if (heroSender.ChampionName == "Jinx" && args.SData.Name.Contains("JinxQAttack")
                    && args.Target.Type == Player.Type)
                {
                    AggroTarget = ObjectManager.GetUnitByNetworkId<AIHeroClient>((uint)args.Target.NetworkId);
                    incomingDamage = (float)heroSender.LSGetAutoAttackDamage(AggroTarget);
                }
            }

            if (sender.IsValid<Obj_AI_Turret>() && sender.IsStructure() && sender.Type == GameObjectType.obj_AI_Turret && sender.IsEnemy)
            {
                if (args.Target.Type == Player.Type)
                {
                    if (sender.LSDistance(Allies().ServerPosition, true) <= 900 * 900)
                    {
                        AggroTarget = ObjectManager.GetUnitByNetworkId<AIHeroClient>((uint)args.Target.NetworkId);

                        incomingDamage =
                            (float)
                            sender.CalcDamage(
                                AggroTarget,
                                DamageType.Physical,
                                sender.BaseAttackDamage + sender.FlatPhysicalDamageMod);
                    }
                }
            }
        }
        
        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                OnCombo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                OnHarass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                OnJungleClear();
                OnLaneClear();
            }

            if (getCheckBoxItem(miscMenu, "ElLux.Auto.Q"))
            {
                AutoQ();
            }

            if (getCheckBoxItem(miscMenu, "ElLux.Auto.E"))
            {
                AutoE();
            }

            CheckWDamage(incomingDamage);
            KillstealHandler();

            if (getKeyBindItem(Menu, "ElLux.Combo.Semi.R"))
            {
                SemiR();
            }

            if (getKeyBindItem(Menu, "ElLux.Combo.Semi.Q"))
            {
                SemiQ();
            }
        }

        private static void SemiQ()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, DamageType.Magical);
            if (target == null)
            {
                return;
            }

            CastQ(target);
        }

        private static void SemiR()
        {
            var target = TargetSelector.GetTarget(spells[Spells.R].Range, DamageType.Magical);
            if (target == null || !target.LSIsValidTarget())
            {
                return;
            }

            if (!spells[Spells.R].IsReady() || !target.LSIsValidTarget(spells[Spells.R].Range))
            {
                return;
            }

            var prediction = spells[Spells.R].GetPrediction(target);
            if (prediction.Hitchance >= HitChance.High)
            {
                spells[Spells.R].Cast(target.Position);
            }
        }

        #endregion
    }
}