using System;
using System.Linq;
using LeagueSharp.Common;
using SharpDX;
using ItemData = LeagueSharp.Common.Data.ItemData;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;


using EloBuddy.SDK;

namespace HoolaRiven
{
    public class Program
    {
        public static Menu Menu;
        
        private static readonly EloBuddy.AIHeroClient Player = EloBuddy.ObjectManager.Player;
        private static readonly HpBarIndicator Indicator = new HpBarIndicator();
        private const string IsFirstR = "RivenFengShuiEngine";
        private const string IsSecondR = "RivenIzunaBlade";
        private static readonly EloBuddy.SpellSlot Flash = Player.GetSpellSlot("summonerFlash");
        private static LeagueSharp.Common.Spell Q, W, E, R;
        private static int QStack = 1;
        public static Render.Text Timer, Timer2;
        private static bool forceQ;
        private static bool forceW;
        private static bool forceR;
        private static bool forceR2;
        private static bool forceItem;
        private static float LastQ;
        private static float LastR;
        private static EloBuddy.AttackableUnit QTarget;

        public static void OnGameLoad()
        {

            if (Player.ChampionName != "Riven") return;
            EloBuddy.Chat.Print("Hoola Riven - Loaded Successfully, Good Luck! :):)");
            EloBuddy.Chat.Print("Hoola Riven - Change Keys Bind in Orbwalk Menu");
            Q = new LeagueSharp.Common.Spell(EloBuddy.SpellSlot.Q);
            W = new LeagueSharp.Common.Spell(EloBuddy.SpellSlot.W);
            E = new LeagueSharp.Common.Spell(EloBuddy.SpellSlot.E, 300);
            R = new LeagueSharp.Common.Spell(EloBuddy.SpellSlot.R, 900);
            R.SetSkillshot(0.25f, 45, 1600, false, SkillshotType.SkillshotCone);

            OnMenuLoad();


            Timer = new Render.Text("Q Expiry =>  " + ((double)(LastQ - Utils.GameTimeTickCount + 3800) / 1000).ToString("0.0"), (int)EloBuddy.Drawing.WorldToScreen(Player.Position).X - 140, (int)EloBuddy.Drawing.WorldToScreen(Player.Position).Y + 10, 30, Color.MidnightBlue, "calibri");
            Timer2 = new Render.Text("R Expiry =>  " + (((double)LastR - Utils.GameTimeTickCount + 15000) / 1000).ToString("0.0"), (int)EloBuddy.Drawing.WorldToScreen(Player.Position).X - 60, (int)EloBuddy.Drawing.WorldToScreen(Player.Position).Y + 10, 30, Color.IndianRed, "calibri");

            EloBuddy.Game.OnUpdate += OnTick;
            EloBuddy.Drawing.OnDraw += Drawing_OnDraw;
            EloBuddy.Drawing.OnEndScene += Drawing_OnEndScene;
            EloBuddy.Obj_AI_Base.OnProcessSpellCast += OnCast;
            EloBuddy.Obj_AI_Base.OnSpellCast += OnDoCast;
            EloBuddy.Obj_AI_Base.OnSpellCast += OnDoCastLC;
            EloBuddy.Obj_AI_Base.OnPlayAnimation += OnPlay;
            EloBuddy.Obj_AI_Base.OnProcessSpellCast += OnCasting;
            Interrupter2.OnInterruptableTarget += Interrupt;
        }

        private static bool HasTitan() => (Items.HasItem(3748) && Items.CanUseItem(3748));

        private static void CastTitan()
        {
            if (Items.HasItem(3748) && Items.CanUseItem(3748))
            {
                Items.UseItem(3748);
                Orbwalker.ResetAutoAttack();
            }
        }
        private static void Drawing_OnEndScene(EventArgs args)
        {
            foreach (
                var enemy in
                    EloBuddy.ObjectManager.Get<EloBuddy.AIHeroClient>()
                        .Where(ene => ene.LSIsValidTarget() && !ene.IsZombie && ene.IsHPBarRendered))
            {
                if (Dind)
                {
                    Indicator.unit = enemy;
                    Indicator.drawDmg(getComboDamage(enemy), new ColorBGRA(255, 204, 0, 170));
                }

            }
        }

        private static void OnDoCastLC(EloBuddy.Obj_AI_Base Sender, EloBuddy.GameObjectProcessSpellCastEventArgs args)
        {
            if (!Sender.IsMe || !Orbwalking.IsAutoAttack((args.SData.Name))) return;
            QTarget = (EloBuddy.Obj_AI_Base)args.Target;
            if (args.Target is EloBuddy.Obj_AI_Minion)
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                {
                    var Minions = MinionManager.GetMinions(70 + 120 + Player.BoundingRadius);
                    if (HasTitan())
                    {
                        CastTitan();
                        return;
                    }
                    if (Q.IsReady() && LaneQ)
                    {
                        ForceItem();
                        Utility.DelayAction.Add(1, () => ForceCastQ(Minions[0]));
                    }
                    if ((!Q.IsReady() || (Q.IsReady() && !LaneQ)) && W.IsReady() && LaneW != 0 &&
                        Minions.Count >= LaneW)
                    {
                        ForceItem();
                        Utility.DelayAction.Add(1, ForceW);
                    }
                    if ((!Q.IsReady() || (Q.IsReady() && !LaneQ)) && (!W.IsReady() || (W.IsReady() && LaneW == 0) || Minions.Count < LaneW) &&
                        E.IsReady() && LaneE)
                    {
                        E.Cast(Minions[0].Position);
                        Utility.DelayAction.Add(1, ForceItem);
                    }
                }
            }
        }
        private static int Item => Items.CanUseItem(3077) && Items.HasItem(3077) ? 3077 : Items.CanUseItem(3074) && Items.HasItem(3074) ? 3074 : 0;
        private static void OnDoCast(EloBuddy.Obj_AI_Base sender, EloBuddy.GameObjectProcessSpellCastEventArgs args)
        {
            var spellName = args.SData.Name;
            if (!sender.IsMe || !Orbwalking.IsAutoAttack(spellName)) return;
            QTarget = (EloBuddy.Obj_AI_Base)args.Target;

            if (args.Target is EloBuddy.Obj_AI_Minion)
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                {
                    var Mobs = MinionManager.GetMinions(120 + 70 + Player.BoundingRadius, MinionTypes.All,
                        MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
                    if (Mobs.Count != 0)
                    {
                        if (HasTitan())
                        {
                            CastTitan();
                            return;
                        }
                        if (Q.IsReady())
                        {
                            ForceItem();
                            Utility.DelayAction.Add(1, () => ForceCastQ(Mobs[0]));
                        }
                        else if (W.IsReady())
                        {
                            ForceItem();
                            Utility.DelayAction.Add(1, ForceW);
                        }
                        else if (E.IsReady())
                        {
                            E.Cast(Mobs[0].Position);
                        }
                    }
                }
            }
            if (args.Target is EloBuddy.Obj_AI_Turret || args.Target is EloBuddy.Obj_Barracks || args.Target is EloBuddy.Obj_BarracksDampener || args.Target is EloBuddy.Obj_Building) if (args.Target.IsValid && args.Target != null && Q.IsReady() && LaneQ && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear)) ForceCastQ((EloBuddy.Obj_AI_Base)args.Target);
            if (args.Target is EloBuddy.AIHeroClient)
            {
                var target = (EloBuddy.AIHeroClient)args.Target;
                if (KillstealR && R.IsReady() && R.Instance.Name == IsSecondR) if (target.Health < (Rdame(target, target.Health) + Player.LSGetAutoAttackDamage(target)) && target.Health > Player.LSGetAutoAttackDamage(target)) R.Cast(target.Position);
                if (KillstealW && W.IsReady()) if (target.Health < (W.GetDamage(target) + Player.LSGetAutoAttackDamage(target)) && target.Health > Player.LSGetAutoAttackDamage(target)) W.Cast();
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && target != null)
                {
                    if (HasTitan())
                    {
                        CastTitan();
                        return;
                    }
                    if (Q.IsReady())
                    {
                        ForceItem();
                        Utility.DelayAction.Add(1, () => ForceCastQ(target));
                    }
                    else if (W.IsReady() && InWRange(target))
                    {
                        ForceItem();
                        Utility.DelayAction.Add(1, ForceW);
                    }
                    else if (E.IsReady() && !Orbwalking.InAutoAttackRange(target)) E.Cast(target.Position);
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                {
                    if (HasTitan())
                    {
                        CastTitan();
                        return;
                    }
                    if (QStack == 2 && Q.IsReady())
                    {
                        ForceItem();
                        Utility.DelayAction.Add(1, () => ForceCastQ(target));
                    }
                }
            }
        }
        static Menu Combo1, Lane, Misc, Draw;
        private static bool Dind => getCheckBoxItem(Draw, "Dind");
        private static bool DrawCB => getCheckBoxItem(Draw, "DrawCB");
        private static bool KillstealW => getCheckBoxItem(Misc, "killstealw");
        private static bool KillstealR => getCheckBoxItem(Misc, "killstealr");
        private static bool DrawAlwaysR => getCheckBoxItem(Draw, "DrawAlwaysR");
        private static bool DrawUseHoola => getCheckBoxItem(Draw, "DrawUseHoola");
        private static bool DrawFH => getCheckBoxItem(Draw, "DrawFH");
        private static bool DrawTimer1 => getCheckBoxItem(Draw, "DrawTimer1");
        private static bool DrawTimer2 => getCheckBoxItem(Draw, "DrawTimer2");
        private static bool DrawHS => getCheckBoxItem(Draw, "DrawHS");
        private static bool DrawBT => getCheckBoxItem(Draw, "DrawBT");
        private static bool UseHoola => getCheckBoxItem(Combo1, "UseHoola");
        private static bool AlwaysR => getKeyBindItem(Combo1, "AlwaysR");
        private static bool AutoShield => getCheckBoxItem(Misc, "AutoShield");
        private static bool Shield => getCheckBoxItem(Misc, "Shield");
        private static bool KeepQ => getCheckBoxItem(Misc, "KeepQ");
        private static int QD => getSliderItem(Misc, "QD");
        private static int QLD => getSliderItem(Misc, "QLD");
        private static int AutoW => getSliderItem(Misc, "AutoW");
        private static bool ComboW => getCheckBoxItem(Combo1, "ComboW");
        private static bool RMaxDam => getCheckBoxItem(Misc, "RMaxDam");
        private static bool RKillable => getCheckBoxItem(Combo1, "RKillable");
        private static int LaneW => getSliderItem(Lane, "LaneW");
        private static bool LaneE => getCheckBoxItem(Lane, "LaneE");
        private static bool WInterrupt => getCheckBoxItem(Misc, "WInterrupt");
        private static bool Qstrange => getCheckBoxItem(Misc, "Qstrange");
        private static bool FirstHydra => getCheckBoxItem(Misc, "FirstHydra");
        private static bool LaneQ => getCheckBoxItem(Lane, "LaneQ");
        private static bool Youmu => getCheckBoxItem(Misc, "youmu");
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
        private static void OnMenuLoad()
        {
            Menu = MainMenu.AddMenu("Hoola Riven", "hoolariven");
            Combo1 = Menu.AddSubMenu("Combo", "Combo");
            Combo1.Add("AlwaysR", new KeyBind("Always Use R (Toggle)", false, KeyBind.BindTypes.PressToggle, "N".ToCharArray()[0]));
            Combo1.Add("UseHoola", new CheckBox("Use Hoola Combo Logic"));
            Combo1.Add("ComboW", new CheckBox("Always use W"));
            Combo1.Add("RKillable", new CheckBox("Use R When Target Can Killable"));

            Lane = Menu.AddSubMenu("Lane", "Lane");
            Lane.Add("LaneQ", new CheckBox("Use Q While LaneClear"));
            Lane.Add("LaneW", new Slider("Use W X Minion (0 = Don't)", 5, 0, 5));
            Lane.Add("LaneE", new CheckBox("Use E While Laneclear"));

            Misc = Menu.AddSubMenu("Misc", "Misc");
            Misc.Add("youmu", new CheckBox("Use Youmus When E", false));
            Misc.Add("FirstHydra", new CheckBox("Flash Burst Hydra Cast before W", false));
            Misc.Add("Qstrange", new CheckBox("Strange Q For Speed", false));
            Misc.Add("Winterrupt", new CheckBox("W interrupt"));
            Misc.Add("AutoW", new Slider("Auto W When x Enemy", 5, 0, 5));
            Misc.Add("RMaxDam", new CheckBox("Use Second R Max Damagae"));
            Misc.Add("killstealw", new CheckBox("Killsteal W"));
            Misc.Add("killstealr", new CheckBox("Killsteal Second R"));
            Misc.Add("AutoShield", new CheckBox("Auto Cast E"));
            Misc.Add("Shield", new CheckBox("Auto Cast E While LastHit"));
            Misc.Add("KeepQ", new CheckBox("Keep Q Alive"));
            Misc.Add("QD", new Slider("First,Second Q Delay", 29, 23, 43));
            Misc.Add("QLD", new Slider("Third Q Delay", 39, 36, 53)); ;

            Draw = Menu.AddSubMenu("Draw", "Draw");
            Draw.Add("DrawAlwaysR", new CheckBox("Draw Always R Status"));
            Draw.Add("DrawTimer1", new CheckBox("Draw Q Expiry Time"));
            Draw.Add("DrawTimer2", new CheckBox("Draw R Expiry Time"));
            Draw.Add("DrawUseHoola", new CheckBox("Draw Hoola Logic Status"));
            Draw.Add("Dind", new CheckBox("Draw Damage Indicator"));
            Draw.Add("DrawCB", new CheckBox("Draw Combo Engage Range", false));
            Draw.Add("DrawBT", new CheckBox("Draw Burst Engage Range", false));
            Draw.Add("DrawFH", new CheckBox("Draw FastHarass Engage Range", false));
            Draw.Add("DrawHS", new CheckBox("Draw Harass Engage Range", false));
        }

        private static void Interrupt(EloBuddy.AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender.IsEnemy && W.IsReady() && sender.LSIsValidTarget() && !sender.IsZombie && WInterrupt)
            {
                if (sender.LSIsValidTarget(125 + Player.BoundingRadius + sender.BoundingRadius)) W.Cast();
            }
        }

        private static int GetWRange => Player.HasBuff("RivenFengShuiEngine") ? 330 : 265;

        private static void AutoUseW()
        {
            if (AutoW > 0)
            {
                if (Player.LSCountEnemiesInRange(GetWRange) >= AutoW)
                {
                    ForceW();
                }
            }
        }

        private static void OnTick(EventArgs args)
        {
            Timer.X = (int)EloBuddy.Drawing.WorldToScreen(Player.Position).X - 60;
            Timer.Y = (int)EloBuddy.Drawing.WorldToScreen(Player.Position).Y + 43;
            Timer2.X = (int)EloBuddy.Drawing.WorldToScreen(Player.Position).X - 60;
            Timer2.Y = (int)EloBuddy.Drawing.WorldToScreen(Player.Position).Y + 65;
            ForceSkill();
            UseRMaxDam();
            AutoUseW();
            Killsteal();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)) Combo();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear)) Jungleclear();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)) Harass();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee)) Flee();
            if (Utils.GameTimeTickCount - LastQ >= 3650 && QStack != 1 && !Player.LSIsRecalling() && KeepQ && Q.IsReady()) Q.Cast(EloBuddy.Game.CursorPos);
        }

        private static void Killsteal()
        {
            if (KillstealW && W.IsReady())
            {
                var targets = HeroManager.Enemies.Where(x => x.LSIsValidTarget(R.Range) && !x.IsZombie);
                foreach (var target in targets)
                {
                    if (target != null)
                    {
                        if (target.Health < W.GetDamage(target) && InWRange(target))
                            W.Cast();
                    }
                }
            }
            if (KillstealR && R.IsReady() && R.Instance.Name == IsSecondR)
            {
                var targets = HeroManager.Enemies.Where(x => x.LSIsValidTarget(R.Range) && !x.IsZombie);
                foreach (var target in targets)
                {
                    if (target.Health < Rdame(target, target.Health) && (!target.HasBuff("kindrednodeathbuff") && !target.HasBuff("Undying Rage") && !target.HasBuff("JudicatorIntervention")))
                        R.Cast(target.Position);
                }
            }
        }
        private static void UseRMaxDam()
        {
            if (RMaxDam && R.IsReady() && R.Instance.Name == IsSecondR)
            {
                var targets = HeroManager.Enemies.Where(x => x.LSIsValidTarget(R.Range) && !x.IsZombie);
                foreach (var target in targets)
                {
                    if (target.Health / target.MaxHealth <= 0.25 && (!target.HasBuff("kindrednodeathbuff") || !target.HasBuff("Undying Rage") || !target.HasBuff("JudicatorIntervention")))
                        R.Cast(target.Position);
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead)
                return;
            var heropos = EloBuddy.Drawing.WorldToScreen(EloBuddy.ObjectManager.Player.Position);


            if (QStack != 1 && DrawTimer1)
            {
                Timer.text = ("Q Expiry =>  " + ((double)(LastQ - Utils.GameTimeTickCount + 3800) / 1000).ToString("0.0") + "S");
                Timer.OnEndScene();
            }

            if (Player.HasBuff("RivenFengShuiEngine") && DrawTimer2)
            {
                Timer2.text = ("R Expiry =>  " + (((double)LastR - Utils.GameTimeTickCount + 15000) / 1000).ToString("0.0") + "S");
                Timer2.OnEndScene();
            }

            if (DrawCB) Render.Circle.DrawCircle(Player.Position, 250 + Player.AttackRange + 70, E.IsReady() ? System.Drawing.Color.FromArgb(120, 0, 170, 255) : System.Drawing.Color.IndianRed);
            if (DrawBT && Flash != EloBuddy.SpellSlot.Unknown) Render.Circle.DrawCircle(Player.Position, 800, R.IsReady() && Flash.IsReady() ? System.Drawing.Color.FromArgb(120, 0, 170, 255) : System.Drawing.Color.IndianRed);
            if (DrawFH) Render.Circle.DrawCircle(Player.Position, 450 + Player.AttackRange + 70, E.IsReady() && Q.IsReady() ? System.Drawing.Color.FromArgb(120, 0, 170, 255) : System.Drawing.Color.IndianRed);
            if (DrawHS) Render.Circle.DrawCircle(Player.Position, 400, Q.IsReady() && W.IsReady() ? System.Drawing.Color.FromArgb(120, 0, 170, 255) : System.Drawing.Color.IndianRed);
            if (DrawAlwaysR)
            {
                EloBuddy.Drawing.DrawText(heropos.X - 40, heropos.Y + 20, System.Drawing.Color.DodgerBlue, "Always R  (     )");
                EloBuddy.Drawing.DrawText(heropos.X + 40, heropos.Y + 20, AlwaysR ? System.Drawing.Color.LimeGreen : System.Drawing.Color.Red, AlwaysR ? "On" : "Off");
            }
            if (DrawUseHoola)
            {
                EloBuddy.Drawing.DrawText(heropos.X - 40, heropos.Y + 33, System.Drawing.Color.DodgerBlue, "Hoola Logic  (     )");
                EloBuddy.Drawing.DrawText(heropos.X + 60, heropos.Y + 33, UseHoola ? System.Drawing.Color.LimeGreen : System.Drawing.Color.Red, UseHoola ? "On" : "Off");
            }
        }

        private static void Jungleclear()
        {

            var Mobs = MinionManager.GetMinions(250 + Player.AttackRange + 70, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (Mobs.Count <= 0)
                return;

            if (W.IsReady() && E.IsReady() && !Orbwalking.InAutoAttackRange(Mobs[0]))
            {
                E.Cast(Mobs[0].Position);
                Utility.DelayAction.Add(1, ForceItem);
                Utility.DelayAction.Add(200, ForceW);
            }
        }

        private static void Combo()
        {
            var targetR = LSTargetSelector.GetTarget(250 + Player.AttackRange + 70, EloBuddy.DamageType.Physical);
            if (targetR != null)
            {
                if (R.IsReady() && R.Instance.Name == IsFirstR && Orbwalking.InAutoAttackRange(targetR) && AlwaysR && targetR != null) ForceR();
                if (R.IsReady() && R.Instance.Name == IsFirstR && W.IsReady() && InWRange(targetR) && ComboW && AlwaysR && targetR != null)
                {
                    ForceR();
                    Utility.DelayAction.Add(1, ForceW);
                }
                if (W.IsReady() && InWRange(targetR) && ComboW && targetR != null) W.Cast();
                if (UseHoola && R.IsReady() && R.Instance.Name == IsFirstR && W.IsReady() && targetR != null && E.IsReady() && targetR.LSIsValidTarget() && !targetR.IsZombie && (IsKillableR(targetR) || AlwaysR))
                {
                    if (!InWRange(targetR))
                    {
                        E.Cast(targetR.Position);
                        ForceR();
                        Utility.DelayAction.Add(200, ForceW);
                        Utility.DelayAction.Add(305, () => ForceCastQ(targetR));
                    }
                }
                else if (!UseHoola && R.IsReady() && R.Instance.Name == IsFirstR && W.IsReady() && targetR != null && E.IsReady() && targetR.LSIsValidTarget() && !targetR.IsZombie && (IsKillableR(targetR) || AlwaysR))
                {
                    if (!InWRange(targetR))
                    {
                        E.Cast(targetR.Position);
                        ForceR();
                        Utility.DelayAction.Add(200, ForceW);
                    }
                }
                else if (UseHoola && W.IsReady() && E.IsReady())
                {
                    if (targetR.LSIsValidTarget() && targetR != null && !targetR.IsZombie && !InWRange(targetR))
                    {
                        E.Cast(targetR.Position);
                        Utility.DelayAction.Add(10, ForceItem);
                        Utility.DelayAction.Add(200, ForceW);
                        Utility.DelayAction.Add(305, () => ForceCastQ(targetR));
                    }
                }
                else if (!UseHoola && W.IsReady() && targetR != null && E.IsReady())
                {
                    if (targetR.LSIsValidTarget() && targetR != null && !targetR.IsZombie && !InWRange(targetR))
                    {
                        E.Cast(targetR.Position);
                        Utility.DelayAction.Add(10, ForceItem);
                        Utility.DelayAction.Add(240, ForceW);
                    }
                }
                else if (E.IsReady())
                {
                    if (targetR.LSIsValidTarget() && !targetR.IsZombie && !InWRange(targetR))
                    {
                        E.Cast(targetR.Position);
                    }
                }
            }
        }


        private static void Burst()
        {
            var target = LSTargetSelector.GetSelectedTarget();
            if (target != null && target.LSIsValidTarget() && !target.IsZombie)
            {
                if (R.IsReady() && R.Instance.Name == IsFirstR && W.IsReady() && E.IsReady() && Player.LSDistance(target.Position) <= 250 + 70 + Player.AttackRange)
                {
                    E.Cast(target.Position);
                    CastYoumoo();
                    ForceR();
                    Utility.DelayAction.Add(100, ForceW);
                }
                else if (R.IsReady() && R.Instance.Name == IsFirstR && E.IsReady() && W.IsReady() && Q.IsReady() &&
                         Player.LSDistance(target.Position) <= 400 + 70 + Player.AttackRange)
                {
                    E.Cast(target.Position);
                    CastYoumoo();
                    ForceR();
                    Utility.DelayAction.Add(150, () => ForceCastQ(target));
                    Utility.DelayAction.Add(160, ForceW);
                }
                else if (Flash.IsReady()
                    && R.IsReady() && R.Instance.Name == IsFirstR && (Player.LSDistance(target.Position) <= 800) && (!FirstHydra || (FirstHydra && !HasItem())))
                {
                    E.Cast(target.Position);
                    CastYoumoo();
                    ForceR();
                    Utility.DelayAction.Add(180, FlashW);
                }
                else if (Flash.IsReady()
                    && R.IsReady() && E.IsReady() && W.IsReady() && R.Instance.Name == IsFirstR && (Player.LSDistance(target.Position) <= 800) && FirstHydra && HasItem())
                {
                    E.Cast(target.Position);
                    ForceR();
                    Utility.DelayAction.Add(100, ForceItem);
                    Utility.DelayAction.Add(210, FlashW);
                }
            }
        }

        private static void FastHarass()
        {
            if (Q.IsReady() && E.IsReady())
            {
                var target = LSTargetSelector.GetTarget(450 + Player.AttackRange + 70, EloBuddy.DamageType.Physical);
                if (target.LSIsValidTarget() && !target.IsZombie && target != null)
                {
                    if (!Orbwalking.InAutoAttackRange(target) && !InWRange(target)) E.Cast(target.Position);
                    Utility.DelayAction.Add(10, ForceItem);
                    Utility.DelayAction.Add(170, () => ForceCastQ(target));
                }
            }
        }

        private static void Harass()
        {
            var target = LSTargetSelector.GetTarget(400, EloBuddy.DamageType.Physical);
            if (Q.IsReady() && W.IsReady() && E.IsReady() && QStack == 1)
            {
                if (target.LSIsValidTarget() && !target.IsZombie)
                {
                    ForceCastQ(target);
                    Utility.DelayAction.Add(1, ForceW);
                }
            }
            if (Q.IsReady() && E.IsReady() && QStack == 3 && !Orbwalker.CanAutoAttack && Orbwalker.CanMove)
            {
                var epos = Player.ServerPosition +
                          (Player.ServerPosition - target.ServerPosition).LSNormalized() * 300;
                E.Cast(epos);
                Utility.DelayAction.Add(190, () => Q.Cast(epos));
            }
        }

        private static void Flee()
        {
            var enemy =
                HeroManager.Enemies.Where(
                    hero =>
                        hero.LSIsValidTarget(Player.HasBuff("RivenFengShuiEngine")
                            ? 70 + 195 + Player.BoundingRadius
                            : 70 + 120 + Player.BoundingRadius) && W.IsReady());
            var x = Player.Position.LSExtend(EloBuddy.Game.CursorPos, 300);
            if (W.IsReady() && enemy.Any()) foreach (var target in enemy)if(target != null) if (InWRange(target)) W.Cast();
            if (Q.IsReady() && !Player.LSIsDashing()) Q.Cast(EloBuddy.Game.CursorPos);
            if (E.IsReady() && !Player.LSIsDashing()) E.Cast(x);
        }

        private static void OnPlay(EloBuddy.Obj_AI_Base sender, EloBuddy.GameObjectPlayAnimationEventArgs args)
        {
            if (!sender.IsMe) return;

            switch (args.Animation)
            {
                case "Spell1a":
                    LastQ = Utils.GameTimeTickCount;
                    if (Qstrange && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None)) EloBuddy.Chat.Say("/d");
                    QStack = 2;
                    if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None) && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit) && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee)) Utility.DelayAction.Add((QD * 10) + 1, Reset);
                    break;
                case "Spell1b":
                    LastQ = Utils.GameTimeTickCount;
                    if (Qstrange && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None)) EloBuddy.Chat.Say("/d");
                    QStack = 3;
                    if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None) && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit) && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee)) Utility.DelayAction.Add((QD * 10) + 1, Reset);
                    break;
                case "Spell1c":
                    LastQ = Utils.GameTimeTickCount;
                    if (Qstrange && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None)) EloBuddy.Chat.Say("/d");
                    QStack = 1;
                    if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None) && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit) && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee)) Utility.DelayAction.Add((QLD * 10) + 3, Reset);
                    break;
                case "Spell3":
                    if (( Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) ||
                        Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee)) && Youmu) CastYoumoo();
                    break;
                case "Spell4a":
                    LastR = Utils.GameTimeTickCount;
                    break;
                case "Spell4b":
                    var target = LSTargetSelector.GetSelectedTarget();
                    if (Q.IsReady() && target.LSIsValidTarget()) ForceCastQ(target);
                    break;
            }
        }

        private static void OnCast(EloBuddy.Obj_AI_Base sender, EloBuddy.GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) return;

            if (args.SData.Name.Contains("ItemTiamatCleave")) forceItem = false;
            if (args.SData.Name.Contains("RivenTriCleave")) forceQ = false;
            if (args.SData.Name.Contains("RivenMartyr")) forceW = false;
            if (args.SData.Name == IsFirstR) forceR = false;
            if (args.SData.Name == IsSecondR) forceR2 = false;
        }

        private static void Reset()
        {
            Orbwalker.ResetAutoAttack();
            EloBuddy.Chat.Say("/d");
            EloBuddy.Player.IssueOrder(EloBuddy.GameObjectOrder.MoveTo, Player.Position.LSExtend(EloBuddy.Game.CursorPos, Player.LSDistance(EloBuddy.Game.CursorPos) + 10));
        }
        private static bool InWRange(EloBuddy.GameObject target) =>(Player.HasBuff("RivenFengShuiEngine") && target != null) ?
                      330 >= Player.LSDistance(target.Position) : 265 >= Player.LSDistance(target.Position);
        private static void ForceSkill()
        {
            if (forceQ && QTarget != null && QTarget.LSIsValidTarget(E.Range + Player.BoundingRadius + 70) && Q.IsReady()) Q.Cast(QTarget.Position);
            if (forceW) W.Cast();
            if (forceR && R.Instance.Name == IsFirstR) R.Cast();
            if (forceItem && Items.CanUseItem(Item) && Items.HasItem(Item) && Item != 0) Items.UseItem(Item);
            if (forceR2 && R.Instance.Name == IsSecondR)
            {
                var target = LSTargetSelector.GetSelectedTarget();
                if (target != null) R.Cast(target.Position);
            }
        }

        private static void ForceItem()
        {
            if (Items.CanUseItem(Item) && Items.HasItem(Item) && Item != 0) forceItem = true;
            Utility.DelayAction.Add(500, () => forceItem = false);
        }
        private static void ForceR()
        {
            forceR = (R.IsReady() && R.Instance.Name == IsFirstR);
            Utility.DelayAction.Add(500, () => forceR = false);
        }
        private static void ForceR2()
        {
            forceR2 = R.IsReady() && R.Instance.Name == IsSecondR;
            Utility.DelayAction.Add(500, () => forceR2 = false);
        }
        private static void ForceW()
        {
            forceW = W.IsReady();
            Utility.DelayAction.Add(500, () => forceW = false);
        }

        private static void ForceCastQ(EloBuddy.AttackableUnit target)
        {
            forceQ = true;
            QTarget = target;
        }


        private static void FlashW()
        {
            var target = LSTargetSelector.GetSelectedTarget();
            if (target != null && target.LSIsValidTarget() && !target.IsZombie)
            {
                W.Cast();
                Utility.DelayAction.Add(10, () => Player.Spellbook.CastSpell(Player.GetSpellSlot("summonerFlash"), target.Position));
            }
        }

        private static bool HasItem() => ItemData.Tiamat_Melee_Only.GetItem().IsReady() || ItemData.Ravenous_Hydra_Melee_Only.GetItem().IsReady();

        private static void CastYoumoo() { if (ItemData.Youmuus_Ghostblade.GetItem().IsReady()) ItemData.Youmuus_Ghostblade.GetItem().Cast(); }
        private static void OnCasting(EloBuddy.Obj_AI_Base sender, EloBuddy.GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsEnemy && sender.Type == Player.Type && (AutoShield || (Shield && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))))
            {
                var epos = Player.ServerPosition +
                          (Player.ServerPosition - sender.ServerPosition).LSNormalized() * 300;

                if (Player.LSDistance(sender.ServerPosition) <= args.SData.CastRange)
                {
                    switch (args.SData.TargettingType)
                    {
                        case EloBuddy.SpellDataTargetType.Unit:

                            if (args.Target.NetworkId == Player.NetworkId)
                            {
                                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit) && !args.SData.Name.Contains("NasusW"))
                                {
                                    if (E.IsReady()) E.Cast(epos);
                                }
                            }

                            break;
                        case EloBuddy.SpellDataTargetType.SelfAoe:

                            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
                            {
                                if (E.IsReady()) E.Cast(epos);
                            }

                            break;
                    }
                    if (args.SData.Name.Contains("IreliaEquilibriumStrike"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId && sender != null)
                        {
                            if (W.IsReady() && InWRange(sender)) W.Cast();
                            else if (E.IsReady()) E.Cast(epos);
                        }
                    }
                    if (args.SData.Name.Contains("TalonCutthroat"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (W.IsReady()) W.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("RenektonPreExecute"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (W.IsReady()) W.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("GarenRPreCast"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (E.IsReady()) E.Cast(epos);
                        }
                    }
                    if (args.SData.Name.Contains("GarenQAttack"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (E.IsReady()) E.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("XenZhaoThrust3"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (W.IsReady()) W.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("RengarQ"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (E.IsReady()) E.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("RengarPassiveBuffDash"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (E.IsReady()) E.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("RengarPassiveBuffDashAADummy"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (E.IsReady()) E.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("TwitchEParticle"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (E.IsReady()) E.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("FizzPiercingStrike"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (E.IsReady()) E.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("HungeringStrike"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (E.IsReady()) E.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("YasuoDash"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (E.IsReady()) E.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("KatarinaRTrigger"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (W.IsReady() && InWRange(sender)) W.Cast();
                            else if (E.IsReady()) E.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("YasuoDash"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (E.IsReady()) E.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("KatarinaE"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (W.IsReady()) W.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("MonkeyKingQAttack"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (E.IsReady()) E.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("MonkeyKingSpinToWin"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (E.IsReady()) E.Cast();
                            else if (W.IsReady()) W.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("MonkeyKingQAttack"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (E.IsReady()) E.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("MonkeyKingQAttack"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (E.IsReady()) E.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("MonkeyKingQAttack"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (E.IsReady()) E.Cast();
                        }
                    }
                }
            }
        }

        private static double basicdmg(EloBuddy.Obj_AI_Base target)
        {
            if (target != null)
            {
                double dmg = 0;
                double passivenhan = 0;
                if (Player.Level >= 18) { passivenhan = 0.5; }
                else if (Player.Level >= 15) { passivenhan = 0.45; }
                else if (Player.Level >= 12) { passivenhan = 0.4; }
                else if (Player.Level >= 9) { passivenhan = 0.35; }
                else if (Player.Level >= 6) { passivenhan = 0.3; }
                else if (Player.Level >= 3) { passivenhan = 0.25; }
                else { passivenhan = 0.2; }
                if (HasItem()) dmg = dmg + Player.LSGetAutoAttackDamage(target) * 0.7;
                if (W.IsReady()) dmg = dmg + W.GetDamage(target);
                if (Q.IsReady())
                {
                    var qnhan = 4 - QStack;
                    dmg = dmg + Q.GetDamage(target) * qnhan + Player.LSGetAutoAttackDamage(target) * qnhan * (1 + passivenhan);
                }
                dmg = dmg + Player.LSGetAutoAttackDamage(target) * (1 + passivenhan);
                return dmg;
            }
            return 0;
        }


        private static float getComboDamage(EloBuddy.Obj_AI_Base enemy)
        {
            if (enemy != null)
            {
                float damage = 0;
                float passivenhan = 0;
                if (Player.Level >= 18) { passivenhan = 0.5f; }
                else if (Player.Level >= 15) { passivenhan = 0.45f; }
                else if (Player.Level >= 12) { passivenhan = 0.4f; }
                else if (Player.Level >= 9) { passivenhan = 0.35f; }
                else if (Player.Level >= 6) { passivenhan = 0.3f; }
                else if (Player.Level >= 3) { passivenhan = 0.25f; }
                else { passivenhan = 0.2f; }
                if (HasItem()) damage = damage + (float)Player.LSGetAutoAttackDamage(enemy) * 0.7f;
                if (W.IsReady()) damage = damage + W.GetDamage(enemy);
                if (Q.IsReady())
                {
                    var qnhan = 4 - QStack;
                    damage = damage + Q.GetDamage(enemy) * qnhan + (float)Player.LSGetAutoAttackDamage(enemy) * qnhan * (1 + passivenhan);
                }
                damage = damage + (float)Player.LSGetAutoAttackDamage(enemy) * (1 + passivenhan);
                if (R.IsReady())
                {
                    return damage * 1.2f + R.GetDamage(enemy);
                }

                return damage;
            }
            return 0;
        }

        public static bool IsKillableR(EloBuddy.AIHeroClient target)
        {
            if (RKillable && target.LSIsValidTarget() && (totaldame(target) >= target.Health
                 && basicdmg(target) <= target.Health) || Player.LSCountEnemiesInRange(900) >= 2 && (!target.HasBuff("kindrednodeathbuff") && !target.HasBuff("Undying Rage") && !target.HasBuff("JudicatorIntervention")))
            {
                return true;
            }
            return false;
        }

        private static double totaldame(EloBuddy.Obj_AI_Base target)
        {
            if (target != null)
            {
                double dmg = 0;
                double passivenhan = 0;
                if (Player.Level >= 18) { passivenhan = 0.5; }
                else if (Player.Level >= 15) { passivenhan = 0.45; }
                else if (Player.Level >= 12) { passivenhan = 0.4; }
                else if (Player.Level >= 9) { passivenhan = 0.35; }
                else if (Player.Level >= 6) { passivenhan = 0.3; }
                else if (Player.Level >= 3) { passivenhan = 0.25; }
                else { passivenhan = 0.2; }
                if (HasItem()) dmg = dmg + Player.LSGetAutoAttackDamage(target) * 0.7;
                if (W.IsReady()) dmg = dmg + W.GetDamage(target);
                if (Q.IsReady())
                {
                    var qnhan = 4 - QStack;
                    dmg = dmg + Q.GetDamage(target) * qnhan + Player.LSGetAutoAttackDamage(target) * qnhan * (1 + passivenhan);
                }
                dmg = dmg + Player.LSGetAutoAttackDamage(target) * (1 + passivenhan);
                if (R.IsReady())
                {
                    var rdmg = Rdame(target, target.Health - dmg * 1.2);
                    return dmg * 1.2 + rdmg;
                }
                return dmg;
            }
            return 0;
        }

        private static double Rdame(EloBuddy.Obj_AI_Base target, double health)
        {
            if (target != null)
            {
                var missinghealth = (target.MaxHealth - health) / target.MaxHealth > 0.75 ? 0.75 : (target.MaxHealth - health) / target.MaxHealth;
                var pluspercent = missinghealth * (8 / 3);
                var rawdmg = new double[] { 80, 120, 160 }[R.Level - 1] + 0.6 * Player.FlatPhysicalDamageMod;
                return Player.CalcDamage(target, EloBuddy.DamageType.Physical, rawdmg * (1 + pluspercent));
            }
            return 0;
        }
    }
}
