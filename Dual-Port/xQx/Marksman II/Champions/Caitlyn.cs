#region

using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Marksman.Common;
using Marksman.Utils;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
#endregion


 namespace Marksman.Champions
{
    internal class Caitlyn : Champion
    {
        public static LeagueSharp.Common.Spell R;

        public static LeagueSharp.Common.Spell E;

        public static LeagueSharp.Common.Spell W;

        private static int LastCastWTick;

        private bool canCastR = true;

        // private static bool headshotReady = ObjectManager.Player.Buffs.Any(buff => buff.DisplayName == "CaitlynHeadshotReady");

        private string[] dangerousEnemies =
        {
            "Alistar", "Garen", "Zed", "Fizz", "Rengar", "JarvanIV", "Irelia", "Amumu", "DrMundo", "Ryze", "Fiora",
            "KhaZix", "LeeSin", "Riven",
            "Lissandra", "Vayne", "Lucian", "Zyra"
        };

        public Caitlyn()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 1240);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 820);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 900);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 2000);

            Q.SetSkillshot(0.50f, 50f, 2000f, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 60f, 1600f, true, SkillshotType.SkillshotLine);

            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;

            Obj_AI_Base.OnBuffGain += (sender, args) =>
            {
                if (W.IsReady())
                {
                    var aBuff =
                        (from fBuffs in
                            sender.Buffs.Where(
                                s =>
                                    sender.Team != ObjectManager.Player.Team
                                    && sender.LSDistance(ObjectManager.Player.Position) < W.Range)
                         from b in new[]
                         {
                                "teleport", /* Teleport */
                                "pantheon_grandskyfall_jump", /* Pantheon */ 
                                "crowstorm", /* FiddleScitck */
                                "zhonya", "katarinar", /* Katarita */
                                "MissFortuneBulletTime", /* MissFortune */
                                "gate", /* Twisted Fate */
                                "chronorevive" /* Zilean */
                            }
                         where args.Buff.Name.ToLower().Contains(b)
                         select fBuffs).FirstOrDefault();

                    if (aBuff != null)
                    {
                        CastW(sender.Position);
                        //W.Cast(sender.Position);
                    }
                }
            };

            Utils.Utils.PrintMessage("Caitlyn loaded.");
        }

        public static LeagueSharp.Common.Spell Q { get; set; }

        public void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Program.misc["Misc.AntiGapCloser"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }

            if (E.IsReady() && gapcloser.Sender.LSIsValidTarget(E.Range))
            {
                E.Cast(gapcloser.Sender.Position);
            }
        }

        public override void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            //if (args.Slot == SpellSlot.W && LastCastWTick + 2000 > Utils.TickCount)
            //{
            //    args.Process = false;
            //}
            //else
            //{
            //    args.Process = true;
            //}

            //if (args.Slot == SpellSlot.Q)
            //{
            //    if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && GetValue<bool>("UseQC"))
            //    {
            //        var t = TargetSelector.GetTarget(Q.Range - 20, DamageType.Physical);
            //        if (!t.LSIsValidTarget())
            //        {
            //            args.Process = false;
            //        }
            //        else
            //        {
            //            args.Process = true;
            //            //CastQ(t);
            //        }
            //    }
            //}
        }

        public override void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsEnemy && sender is Obj_AI_Turret && args.Target.IsMe)
            {
                canCastR = false;
            }
            else
            {
                canCastR = true;
            }
        }

        public override void Drawing_OnDraw(EventArgs args)
        {
            var t = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (t.LSIsValidTarget())
            {
                Render.Circle.DrawCircle(t.Position, 105f, Color.GreenYellow);

                var wcCenter = ObjectManager.Player.Position.LSExtend(t.Position,
                    ObjectManager.Player.LSDistance(t.Position) / 2);

                var wcLeft = ObjectManager.Player.Position.LSTo2D() +
                             Vector2.Normalize(t.Position.LSTo2D() - ObjectManager.Player.Position.LSTo2D())
                                 .Rotated(ObjectManager.Player.LSDistance(t.Position) < 300
                                     ? 45
                                     : 37 * (float)Math.PI / 180) * ObjectManager.Player.LSDistance(t.Position) / 2;

                var wcRight = ObjectManager.Player.Position.LSTo2D() +
                              Vector2.Normalize(t.Position.LSTo2D() - ObjectManager.Player.Position.LSTo2D())
                                  .Rotated(ObjectManager.Player.LSDistance(t.Position) < 300
                                      ? -45
                                      : -37 * (float)Math.PI / 180) * ObjectManager.Player.LSDistance(t.Position) / 2;

                Render.Circle.DrawCircle(wcCenter, 50f, Color.Red);
                Render.Circle.DrawCircle(wcLeft.To3D(), 50f, Color.Green);
                Render.Circle.DrawCircle(wcRight.To3D(), 50f, Color.Yellow);
            }
            //var bx = HeroManager.Enemies.Where(e => e.LSIsValidTarget(E.Range * 3));
            //foreach (var n in bx)
            //{
            //    if (n.LSIsValidTarget(800) && ObjectManager.Player.Distance(n) < 450)
            //    {
            //        Vector3[] x = new[] { ObjectManager.Player.Position, n.Position };
            //        Vector2 aX =
            //            Drawing.WorldToScreen(new Vector3(CommonGeometry.CenterOfVectors(x).X,
            //                CommonGeometry.CenterOfVectors(x).Y, CommonGeometry.CenterOfVectors(x).Z));

            //        Render.Circle.DrawCircle(CommonGeometry.CenterOfVectors(x), 85f, Color.White );
            //        Drawing.DrawText(aX.X - 15, aX.Y - 15, Color.GreenYellow, n.ChampionName);

            //    }
            //}

            //var enemies = HeroManager.Enemies.Where(e => e.LSIsValidTarget(1500));
            //var objAiHeroes = enemies as Obj_AI_Hero[] ?? enemies.ToArray();
            //IEnumerable<Obj_AI_Hero> nResult =
            //    (from e in objAiHeroes join d in dangerousEnemies on e.ChampionName equals d select e)
            //        .Distinct();

            //foreach (var n in nResult)
            //{
            //    var x = E.GetPrediction(n).CollisionObjects.Count;
            //    Render.Circle.DrawCircle(n.Position, (Orbwalking.GetRealAutoAttackRange(null) + 65) - 300, Color.GreenYellow);
            //}

            var nResult = HeroManager.Enemies.Where(e => e.LSIsValidTarget(E.Range * 2));
            foreach (var n in nResult.Where(n => n.LSIsFacing(ObjectManager.Player)))
            {
                Render.Circle.DrawCircle(n.Position, E.Range - 200, Color.Red, 1);
            }

            LeagueSharp.Common.Spell[] spellList = { Q, W, E, R };
            foreach (var spell in spellList)
            {
                var menuItem = Program.marksmanDrawings["Draw" + spell.Slot].Cast<CheckBox>().CurrentValue;
                if (menuItem)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spell.Range, Color.FromArgb(100, 255, 255, 255));
                }
            }
        }

        private static void CastQ(Obj_AI_Base t)
        {
            if (Q.CanCast(t))
            {
                var qPrediction = Q.GetPrediction(t);
                var hithere = qPrediction.CastPosition.LSExtend(ObjectManager.Player.Position, -100);

                if (qPrediction.Hitchance >= Q.GetHitchance())
                {
                    Q.Cast(hithere);
                }
            }
        }

        private static void CastE(Obj_AI_Base t)
        {
            if (E.CanCast(t))
            {
                var pred = E.GetPrediction(t);
                var hithere = pred.CastPosition.LSExtend(ObjectManager.Player.Position, -100);

                if (pred.Hitchance >= E.GetHitchance())
                {
                    E.Cast(hithere);
                }
            }
        }


        private static void CastW(Vector3 pos, bool delayControl = true)
        {
            var enemy =
                HeroManager.Enemies.Find(
                    e =>
                        e.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65) &&
                        e.Health < ObjectManager.Player.TotalAttackDamage * 2);
            if (enemy != null)
            {
                return;
            }

            if (!W.IsReady())
            {
                return;
            }

            //if (headshotReady)
            //{
            //    return;
            //}

            if (delayControl && LastCastWTick + 2000 > LeagueSharp.Common.Utils.TickCount)
            {
                return;
            }

            W.Cast(pos);
            LastCastWTick = LeagueSharp.Common.Utils.TickCount;
        }

        private static void CastW2(Obj_AI_Base t)
        {
            if (t.LSIsValidTarget(W.Range))
            {
                BuffType[] buffList =
                {
                    BuffType.Fear,
                    BuffType.Taunt,
                    BuffType.Stun,
                    BuffType.Slow,
                    BuffType.Snare
                };

                foreach (var b in buffList.Where(t.HasBuffOfType))
                {
                    CastW(t.Position, false);
                }
            }
        }

        private static bool EnemyHasBuff(AIHeroClient t)
        {
            BuffType[] buffList =
            {
                BuffType.Blind,
                BuffType.Fear,
                BuffType.Knockup,
                BuffType.Taunt,
                BuffType.Slow,
                BuffType.Snare
            };

            return buffList.Where(t.HasBuffOfType).Any();
        }

        private static void DrawingOnOnEndScene(EventArgs args)
        {
            //if (GetValue<bool>("UseEC") && E.IsReady())
            //{
            //    var nResult = HeroManager.Enemies.Where(e => e.LSIsValidTarget(E.Range) && E.GetPrediction(e).CollisionObjects.Count == 0).OrderBy(e => e.Distance(ObjectManager.Player)).FirstOrDefault();
            //    if (nResult != null)
            //    {
            //        var isMelee = nResult.AttackRange < 400;
            //        if (isMelee)
            //        {
            //            Render.Circle.DrawCircle(nResult.Position, nResult.BoundingRadius, Color.DarkCyan, 3);
            //            if (ObjectManager.Player.Distance(nResult) < nResult.AttackRange * 2)
            //            {
            //                CastE(nResult);
            //            }
            //        }
            //        else
            //        {
            //            Render.Circle.DrawCircle(nResult.Position, nResult.BoundingRadius, Color.Gold, 3);
            //            if (nResult.LSIsValidTarget(nResult.IsFacing(ObjectManager.Player) ? E.Range - 200 : E.Range - 400))
            //            {
            //                CastE(nResult);
            //            }
            //        }
            //    }
            //}


            //var enemies =
            //    HeroManager.Enemies.Count(
            //        e => e.Health <= R.GetDamage(e) && !e.IsDead && !e.IsZombie && e.LSIsValidTarget(R.Range));
            //if (enemies > 0)
            //{
            //    for (var i = 0; i < enemies; i++)

            //    {
            //        var a1 = (i + 1) * 0.025f;

            //        CommonGeometry.DrawBox(new Vector2(Drawing.Width * 0.43f, Drawing.Height * (0.700f + a1)), 150, 18,
            //            Color.FromArgb(170, 255, 0, 0), 1, Color.Black);

            //        CommonGeometry.Text.DrawTextCentered(HeroManager.Enemies[i].ChampionName + " Killable",
            //            (int)(Drawing.Width * 0.475f), (int)(Drawing.Height * (0.803f + a1 - 0.093f)), SharpDX.Color.Wheat);
            //    }
            //}
            foreach (var e in HeroManager.Enemies.Where(e => e.LSIsValidTarget(W.Range)))
            {
                if (EnemyHasBuff(e))
                {
                    var pos =
                        e.Position.LSTo2D()
                            .LSExtend(ObjectManager.Player.Position.LSTo2D(),
                                ObjectManager.Player.HealthPercent < e.HealthPercent ? 100 : -100)
                            .To3D();
                    Render.Circle.DrawCircle(pos, 50f, Color.Chartreuse);

                    //if (pos.Distance(ObjectManager.Player.Position) <= W.Range)
                    //{
                    //    CastW(pos);
                    //}
                }
            }

            if (Drawing.Direct3DDevice == null || Drawing.Direct3DDevice.IsDisposed)
            {
                return;
            }

            var x = 0;
            foreach (var b in ObjectManager.Player.Buffs.Where(buff => buff.DisplayName == "CaitlynHeadshotCount"))
            {
                x = b.Count;
            }

            for (var i = 1; i < 7; i++)
            {
                CommonGeometry.DrawBox(
                    new Vector2(ObjectManager.Player.HPBarPosition.X + 23 + i * 17,
                        ObjectManager.Player.HPBarPosition.Y + 25), 15, 4, Color.Transparent, 1, Color.Black);
            }
            var headshotReady = ObjectManager.Player.Buffs.Any(buff => buff.DisplayName == "CaitlynHeadshotReady");
            for (var i = 1; i < (headshotReady ? 7 : x + 1); i++)
            {
                CommonGeometry.DrawBox(
                    new Vector2(ObjectManager.Player.HPBarPosition.X + 24 + i * 17,
                        ObjectManager.Player.HPBarPosition.Y + 26), 13, 3, headshotReady ? Color.Red : Color.LightGreen,
                    0, Color.Black);
            }

            var rCircle2 = Program.marksmanDrawings["Draw.UltiMiniMap"].Cast<CheckBox>().CurrentValue;
            if (rCircle2)
            {
#pragma warning disable 618
                LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, R.Range, Color.FromArgb(255, 255, 255, 255), 1, 23, true);
#pragma warning restore 618
            }
        }

        public override void Game_OnGameUpdate(EventArgs args)
        {
            R.Range = 500 * (R.Level == 0 ? 1 : R.Level) + 1500;

            AIHeroClient t;

            if (Program.misc["AutoWI"].Cast<ComboBox>().CurrentValue != 0 && W.IsReady())
            {
                foreach (
                    var hero in
                        HeroManager.Enemies.Where(h => h.LSIsValidTarget(W.Range) && h.HasBuffOfType(BuffType.Stun)))
                {
                    CastW(hero.Position, false);
                }
            }

            if (W.IsReady() &&
                (Program.misc["AutoWI"].Cast<ComboBox>().CurrentValue == 1 ||
                 (Program.misc["AutoWI"].Cast<ComboBox>().CurrentValue == 2 && ComboActive)))
            {
                t = TargetSelector.GetTarget(W.Range, DamageType.Physical);
                if (t.LSIsValidTarget(W.Range))
                {
                    if (t.HasBuffOfType(BuffType.Stun) || t.HasBuffOfType(BuffType.Snare) ||
                        t.HasBuffOfType(BuffType.Taunt) || t.HasBuffOfType(BuffType.Knockup) ||
                        t.HasBuff("zhonyasringshield") || t.HasBuff("Recall"))
                    {
                        CastW(t.Position);
                    }

                    if (t.HasBuffOfType(BuffType.Slow) && t.LSIsValidTarget(E.Range - 200))
                    {
                        //W.Cast(t.Position.Extend(ObjectManager.Player.Position, +200));
                        //W.Cast(t.Position.Extend(ObjectManager.Player.Position, -200));

                        var hit = t.LSIsFacing(ObjectManager.Player)
                            ? t.Position.LSExtend(ObjectManager.Player.Position, +200)
                            : t.Position.LSExtend(ObjectManager.Player.Position, -200);
                        CastW(hit);
                    }
                }
            }

            if (Q.IsReady() &&
                (Program.misc["AutoQI"].Cast<ComboBox>().CurrentValue == 1 || (Program.misc["AutoQI"].Cast<ComboBox>().CurrentValue == 2 && ComboActive)))
            {
                t = TargetSelector.GetTarget(Q.Range - 30, DamageType.Physical);
                if (t.LSIsValidTarget(Q.Range)
                    &&
                    (t.HasBuffOfType(BuffType.Stun) ||
                     t.HasBuffOfType(BuffType.Snare) ||
                     t.HasBuffOfType(BuffType.Taunt) ||
                     (t.Health <= ObjectManager.Player.LSGetSpellDamage(t, SpellSlot.Q)
                      && !t.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65))))
                {
                    CastQ(t);
                }
            }

            if (Program.combo["UseQMC"].Cast<KeyBind>().CurrentValue)
            {
                t = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                CastQ(t);
            }

            //if (GetValue<KeyBind>("UseEMC").Active)
            //{
            //    t = TargetSelector.GetTarget(E.Range - 50, DamageType.Physical);
            //    E.Cast(t);
            //}

            if (Program.combo["UseRMC"].Cast<KeyBind>().CurrentValue && R.IsReady())
            {
                foreach (
                    var e in
                        HeroManager.Enemies.Where(
                            e =>
                                e.LSIsValidTarget(R.Range) &&
                                ObjectManager.Player.LSCountEnemiesInRange(Orbwalking.GetRealAutoAttackRange(null) + 65) <=
                                1).OrderBy(e => e.Health))
                {
                    //Utils.MPing.Ping(enemy.Position.To2D());
                    R.CastOnUnit(e);
                }
            }

            //for (int i = 1; i < HeroManager.Enemies.Count(e => e.Health < R.GetDamage(e)); i++)
            //{

            //    Common.CommonGeometry.DrawBox(new Vector2(Drawing.Width * 0.45f, Drawing.Height * 0.80f), 125, 18, Color.Transparent, 1, System.Drawing.Color.Black);
            //    Common.CommonGeometry.DrawText(CommonGeometry.Text, HeroManager.Enemies[i].ChampionName + " Killable", Drawing.Width * 0.455f, Drawing.Height * (0.803f + i * 50), SharpDX.Color.Wheat);
            //}

            if (Program.misc["UseEQC"].Cast<KeyBind>().CurrentValue && E.IsReady() && Q.IsReady())
            {
                t = TargetSelector.GetTarget(E.Range, DamageType.Physical);
                if (t.LSIsValidTarget(E.Range)
                    && t.Health
                    < ObjectManager.Player.LSGetSpellDamage(t, SpellSlot.Q)
                    + ObjectManager.Player.LSGetSpellDamage(t, SpellSlot.E) + 20 && E.CanCast(t))
                {
                    E.Cast(t);
                    CastQ(t);
                }
            }

            if ((!ComboActive && !HarassActive) || !Orbwalker.CanMove)
            {
                return;
            }

            //var useQ = GetValue<bool>("UseQ" + (ComboActive ? "C" : "H"));
            var useW = Program.combo["UseWC"].Cast<CheckBox>().CurrentValue;
            var useE = Program.combo["UseEC"].Cast<CheckBox>().CurrentValue;
            var useR = Program.combo["UseRC"].Cast<CheckBox>().CurrentValue;

            //if (Q.IsReady() && useQ)
            //{
            //    t = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            //    if (t != null)
            //    {
            //        CastQ(t);
            //    }
            //}

            if (useE && E.IsReady())
            {
                //var enemies = HeroManager.Enemies.Where(e => e.LSIsValidTarget(E.Range));
                //var objAiHeroes = enemies as Obj_AI_Hero[] ?? enemies.ToArray();
                //IEnumerable<Obj_AI_Hero> nResult =
                //    (from e in objAiHeroes join d in dangerousEnemies on e.ChampionName equals d select e)
                //        .Distinct();

                //foreach (var n in nResult.Where(n => n.IsFacing(ObjectManager.Player)))
                //{
                //    if (n.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65 - 300) && E.GetPrediction(n).CollisionObjects.Count == 0)
                //    {
                //        E.Cast(n.Position);
                //        if (W.IsReady())
                //            W.Cast(n.Position);
                //    }
                //}
                //if (GetValue<bool>("E.ProtectDistance"))
                //{
                //    foreach (var n in HeroManager.Enemies)
                //    {
                //        if (GetValue<bool>("E." + n.ChampionName + ".ProtectDistance") &&
                //            n.Distance(ObjectManager.Player) < E.Range - 100)
                //        {
                //            E.Cast(n.Position);
                //        }

                //    }
                //}
                var nResult =
                    HeroManager.Enemies.Where(
                        e =>
                            e.LSIsValidTarget(E.Range) && e.Health >= ObjectManager.Player.TotalAttackDamage * 2 &&
                            e.LSIsFacing(ObjectManager.Player) && e.LSIsValidTarget(E.Range - 300) &&
                            E.GetPrediction(e).CollisionObjects.Count == 0);
                foreach (var n in nResult)
                {
                    //                    if (n.LSIsValidTarget(n.IsFacing(ObjectManager.Player) ? E.Range - 200 : E.Range - 300) && E.GetPrediction(n).CollisionObjects.Count == 0)
                    //                    {
                    E.Cast(n.Position);
                    if (W.IsReady())
                    {
                        W.Cast(n.Position);
                    }
                    if (Q.IsReady())
                    {
                        Q.Cast(n.Position);
                    }

                    //                    }
                }
            }

            if (useW && W.IsReady())
            {
                var nResult = HeroManager.Enemies.Where(e => e.LSIsValidTarget(W.Range));
                foreach (var n in nResult)
                {
                    if (ObjectManager.Player.LSDistance(n) < 450 && n.LSIsFacing(ObjectManager.Player))
                    {
                        CastW(CommonGeometry.CenterOfVectors(new[] { ObjectManager.Player.Position, n.Position }));
                    }
                }
            }

            if (R.IsReady() && useR)
            {
                foreach (
                    var e in
                        HeroManager.Enemies.Where(
                            e =>
                                e.LSIsValidTarget(R.Range) && e.Health <= R.GetDamage(e) && ObjectManager.Player.LSCountEnemiesInRange(Orbwalking.GetRealAutoAttackRange(null) + 350) == 0 &&
                                !Orbwalking.InAutoAttackRange(e) && canCastR))
                {
                    R.CastOnUnit(e);
                }
            }
        }

        public override void Orbwalking_AfterAttack(AttackableUnit target, EventArgs args)
        {
            var t = target as AIHeroClient;
            if (t == null || (!ComboActive && !HarassActive)) return;

            //var useQ = GetValue<bool>("UseQ" + (ComboActive ? "C" : "H"));
            //if (useQ) Q.Cast(t, false, true);

            base.Orbwalking_AfterAttack(target,args);
        }

        public override bool ComboMenu(Menu config)
        {
            config.Add("UseQC", new CheckBox("Q:"));
            config.Add("UseQMC", new KeyBind("Q: Semi-Manual", false, KeyBind.BindTypes.HoldActive, 'G'));
            config.Add("UseWC", new CheckBox("W:"));
            config.Add("UseEC", new CheckBox("E:"));
            config.Add("UseRC", new CheckBox("R:"));
            config.Add("UseRMC", new KeyBind("R: Semi-Manual", false, KeyBind.BindTypes.HoldActive, 'R'));


            return true;
        }

        public override bool HarassMenu(Menu config)
        {
            config.Add("UseQH", new CheckBox("Use Q"));
            return true;
        }

        public override bool DrawingMenu(Menu config)
        {
            config.Add("DrawQ", new CheckBox(Marksman.Utils.Utils.Tab + "Q:"));//.SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
            config.Add("DrawW", new CheckBox(Marksman.Utils.Utils.Tab + "W:"));//.SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
            config.Add("DrawE", new CheckBox(Marksman.Utils.Utils.Tab + "E:"));//.SetValue(new Circle(false, Color.FromArgb(100, 255, 255, 255))));
            config.Add("DrawR", new CheckBox(Marksman.Utils.Utils.Tab + "R:"));//.SetValue(new Circle(false, Color.FromArgb(100, 255, 255, 255))));
            config.Add("Draw.UltiMiniMap", new CheckBox(Marksman.Utils.Utils.Tab + "Draw Ulti Minimap"));//.SetValue(new Circle(true, Color.FromArgb(255, 255, 255, 255))));
            return true;
        }

        public override bool MiscMenu(Menu config)
        {
            config.Add("Misc.AntiGapCloser", new CheckBox("E Anti Gap Closer"));
            config.Add("UseEQC", new KeyBind("Use E-Q Combo", false, KeyBind.BindTypes.HoldActive, 'T'));
            config.Add("Dash", new KeyBind("Dash to Mouse", false, KeyBind.BindTypes.HoldActive, 'Z'));
            config.Add("AutoQI", new ComboBox("Auto Q (Stun/Snare/Taunt/Slow)", 2, "Off", "On: Everytime", "On: Combo Mode"));
            config.Add("AutoWI", new ComboBox("Auto W (Stun/Snare/Taunt)", 2, "Off", "On: Everytime", "On: Combo Mode"));

            return true;
        }

        public override bool LaneClearMenu(Menu config)
        {
            return true;
        }

        public override bool JungleClearMenu(Menu config)
        {
            return true;
        }

        public override void ExecuteFlee()
        {
            if (E.IsReady())
            {
                var pos = Vector3.Zero;
                var enemy =
                    HeroManager.Enemies.FirstOrDefault(
                        e =>
                            e.LSIsValidTarget(E.Range +
                                            (ObjectManager.Player.MoveSpeed > e.MoveSpeed
                                                ? ObjectManager.Player.MoveSpeed - e.MoveSpeed
                                                : e.MoveSpeed - ObjectManager.Player.MoveSpeed)) && E.CanCast(e));

                pos = enemy?.Position ??
                      ObjectManager.Player.ServerPosition.LSTo2D().LSExtend(Game.CursorPos.LSTo2D(), -300).To3D();
                //E.Cast(pos);
            }

            base.PermaActive();
        }
    }
}