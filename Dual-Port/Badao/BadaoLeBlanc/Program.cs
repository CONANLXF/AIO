using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;
using Spell = LeagueSharp.Common.Spell;

 namespace BLeblanc
{
    class Program
    {
        private static AIHeroClient Player { get { return ObjectManager.Player; } }

        public static Spell Q, W, E, R;

        public static Menu Menu;

        public static Menu spellMenu, twochainMenu;

        public static int Rstate, Wstate, Ecol;

        public static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "Leblanc")
                return;

            Q = new Spell(SpellSlot.Q, 710);
            W = new Spell(SpellSlot.W, 750);
            E = new Spell(SpellSlot.E, 950);
            R = new Spell(SpellSlot.R);
            if (Rstate == 1)
                R = new Spell(SpellSlot.R, Q.Range);
            if (Rstate == 2)
            {
                R = new Spell(SpellSlot.R, W.Range);
                R.SetSkillshot(0, 70, 1500, false, SkillshotType.SkillshotLine);
            }

            //Q.SetSkillshot(300, 50, 2000, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 70, 1600, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(0, 70, 1500, false, SkillshotType.SkillshotLine);

            Menu = MainMenu.AddMenu(Player.ChampionName, Player.ChampionName);

            spellMenu = Menu.AddSubMenu("Spells", "Spells");
            spellMenu.Add("Qharass", new CheckBox("Use Q Harass"));
            spellMenu.Add("Wharass", new CheckBox("Use W Harass"));
            spellMenu.Add("Wbackharass", new CheckBox("Use W Back Harass"));
            spellMenu.Add("Wcombo", new CheckBox("Use W Combo"));
            spellMenu.Add("Wgap", new CheckBox("Use W Combo Gap"));
            spellMenu.Add("force", new CheckBox("force focus selected", false));
            spellMenu.Add("selected", new Slider("if selected in :", 1000, 1000, 1500));
            spellMenu.Add("qe", new KeyBind("QE Selected Target", false, KeyBind.BindTypes.HoldActive, 'G'));


            twochainMenu = Menu.AddSubMenu("Two Chains", "Twochains");
            twochainMenu.Add("2chainz", new KeyBind("Two Chains Active", false, KeyBind.BindTypes.HoldActive, 'T'));
            twochainMenu.Add("force1", new CheckBox("Only On Selected Target", true));

            //Drawing.OnDraw += Drawing_OnDraw;

            Game.OnUpdate += Game_OnGameUpdate;


            Chat.Print("Leblanc by badao updated 06/17/16!");

            Chat.Print("Visist forum for more information");

            Chat.Print("Leave an upvote in database if you like it <3!");
        }

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
        
        public static bool WgapCombo { get { return getCheckBoxItem(spellMenu, "Wgap"); } }
        public static void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead)
                return;
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }
            if (getKeyBindItem(twochainMenu, "2chainz"))
            {
                AIHeroClient target = null;
                if (getCheckBoxItem(twochainMenu, "force1"))
                {
                    target = TargetSelector.SelectedTarget;
                }
                else
                    target = TargetSelector.GetTarget(-1, DamageType.Physical);
                if (target.LSIsValidTarget() && Player.IsInAutoAttackRange(target))
                {
                    TwoChains.TwoChainsActive(target);
                }
                else
                {
                    TwoChains.TwoChainsActive(null);
                }
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                if (getCheckBoxItem(spellMenu, "Qharass"))
                {
                    useQ();
                }
                if (getCheckBoxItem(spellMenu, "Wharass"))
                {                   
                       useWH();
                }
                if (getCheckBoxItem(spellMenu, "Wbackharass"))
                {
                    useWBH();
                }

            }
            CheckR();
            CheckW();
            if (getKeyBindItem(spellMenu, "qe"))
            {
                useQE();
            }
        }

        public static bool Selected()
        {
            if (!getCheckBoxItem(spellMenu, "force"))
            {
                return false;
            }
            else
            {
                var target = TargetSelector.SelectedTarget;
                float a = getSliderItem(spellMenu, "selected");
                if (target == null || target.IsDead || target.IsZombie)
                {
                    return false;
                }
                else
                {
                    if (Player.LSDistance(target.Position) > a)
                    {
                        return false;
                    }
                    return true;
                }
            }
        }

        public static void useQ()
        {
            if (Selected())
            {
                var target = TargetSelector.SelectedTarget;
                if (target != null && target.LSIsValidTarget(Q.Range))
                {
                    Q.Cast(target);
                }

            }
            else
            {
                var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
                if (target != null && target.LSIsValidTarget(Q.Range))
                {
                    Q.Cast(target);
                }
            }
        }

        public static void useE()
        {
            if (Selected())
            {
                var target = TargetSelector.SelectedTarget;
                if (target != null && target.LSIsValidTarget(E.Range))
                {
                    CastE(target);
                }

            }
            else
            {
                var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical) ??
                             TargetSelector.GetTarget(E.Range, DamageType.Magical);
                if (target != null && target.LSIsValidTarget(E.Range))
                {
                    CastE(target);
                }
            }
        }
        public static void useW()
        {
            if (getCheckBoxItem(spellMenu, "Wcombo"))
            {
                if (Selected())
                {
                    var target = TargetSelector.SelectedTarget;
                    if (target != null && target.LSIsValidTarget(W.Range))
                    {
                        CastW(target);
                    }

                }
                else
                {
                    var target = TargetSelector.GetTarget(W.Range, DamageType.Magical);
                    if (target != null && target.LSIsValidTarget(W.Range))
                    {
                        CastW(target);
                    }
                }
            }
        }

        public static void useWH()
        {
            if (Selected())
            {
                var target = TargetSelector.SelectedTarget;
                if (target != null && target.LSIsValidTarget(W.Range))
                {
                    CastW(target);
                }

            }
            else
            {
                var target = TargetSelector.GetTarget(W.Range, DamageType.Magical);
                if (target != null && target.LSIsValidTarget(W.Range))
                {
                    CastW(target);
                }
            }
        }

        public static void useWBH()
        {
            if (Wstate == 2)
                W.Cast();
        }

        public static void useR()
        {
            if (Selected())
            {
                var target = TargetSelector.SelectedTarget;
                if (target != null && target.LSIsValidTarget(Q.Range))
                {
                    CastR(target);
                }

            }
            else
            {
                var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
                if (target != null && target.LSIsValidTarget(E.Range))
                {
                    CastR(target);
                }
            }
        }

        public static void CastR(Obj_AI_Base target)
        {
            if (R.IsReady())
            {
                if (Rstate == 1)
                {
                    if (target.LSIsValidTarget(R.Range))
                    {
                        R.Cast(target);
                    }
                }
                if (Rstate == 2)
                {
                    var t = LeagueSharp.Common.Prediction.GetPrediction(target, 400).CastPosition;
                    float x = target.MoveSpeed;
                    float y = x * 400 / 1000;
                    var pos = target.Position;
                    if (target.LSDistance(t) <= y)
                    {
                        pos = t;
                    }
                    if (target.LSDistance(t) > y)
                    {
                        pos = target.Position.LSExtend(t, y - 50);
                    }
                    if (Player.LSDistance(pos) <= 600)
                    {
                        R.Cast(pos);
                    }
                    if (Player.LSDistance(pos) > 600)
                    {
                        if (target.LSDistance(t) > y)
                        {
                            var pos2 = target.Position.LSExtend(t, y);
                            if (Player.LSDistance(pos2) <= 600)
                            {
                                R.Cast(pos2);
                            }
                            else
                            {
                                var prediction = R.GetPrediction(target);
                                if (prediction.Hitchance >= HitChance.High)
                                {
                                    var pos3 = prediction.CastPosition;
                                    var pos4 = Player.Position.LSExtend(pos3, 600);
                                    R.Cast(pos4);
                                }
                            }
                        }

                    }
                }
            }
        }

        public static void CastW(Obj_AI_Base target)
        {
            if (!W.IsReady() || Wstate != 1)
                return;
            var t = LeagueSharp.Common.Prediction.GetPrediction(target, 400).CastPosition;
            float x = target.MoveSpeed;
            float y = x * 400 / 1000;
            var pos = target.Position;
            if (target.LSDistance(t) <= y)
            {
                pos = t;
            }
            if (target.LSDistance(t) > y)
            {
                pos = target.Position.LSExtend(t, y - 50);
            }
            if (Player.LSDistance(pos) <= 600)
            {
                W.Cast(pos);
            }
            if (Player.LSDistance(pos) > 600)
            {
                if (target.LSDistance(t) > y)
                {
                    var pos2 = target.Position.LSExtend(t, y);
                    if (Player.LSDistance(pos2) <= 600)
                    {
                        W.Cast(pos2);
                    }
                    else
                    {
                        var prediction = W.GetPrediction(target);
                        if (prediction.Hitchance >= HitChance.High)
                        {
                            var pos3 = prediction.CastPosition;
                            var pos4 = Player.Position.LSExtend(pos3, 600);
                            W.Cast(pos4);
                        }
                    }

                }
            }
        }

        public static void CastE(Obj_AI_Base target)
        {
            if (E.IsReady() && !Player.LSIsDashing())
            {
                if (!R.IsReady())
                { E.Cast(target); }
                if (R.IsReady() && Rstate == 4)
                { E.Cast(target); }
            }
        }

        public static void CheckE(Obj_AI_Base target)
        {
            if (E.IsReady())
            {
                var prediction = E.GetPrediction(target);
                if (prediction.Hitchance == HitChance.Collision)
                {
                    Ecol = 1;
                }
                else
                {
                    Ecol = 0;
                }
            }
            if (!E.IsReady())
            {
                Ecol = 0;
            }
        }
        public static void CheckR()
        {
            string x = Player.Spellbook.GetSpell(SpellSlot.R).Name;          
            if (x == "LeblancChaosOrbM")
                Rstate = 1;
            if (x == "LeblancSlideM")
                Rstate = 2;
            if (x == "LeblancSoulShackleM")
                Rstate = 3;
            if (x == "LeblancSlideReturnM")
            {
                Rstate = 4;
            }
            if (Rstate == 1)
                R = new Spell(SpellSlot.R, Q.Range);
            if (Rstate == 2)
            {
                R = new Spell(SpellSlot.R, W.Range);
                R.SetSkillshot(0, 70, 1500, false, SkillshotType.SkillshotLine);
            }
        }
        public static void CheckW()
        {
            string x = Player.Spellbook.GetSpell(SpellSlot.W).Name;
            if (x == "LeblancSlideReturn")
            {
                Wstate = 2;
            }
            else
                Wstate = 1;
        }

        public static void Combo()
        {
            if (Selected())
            {
                var target = TargetSelector.SelectedTarget;
                CheckE(target);
                float a = Player.LSDistance(target.Position);
                if (a > Q.Range && a <= 1200)
                {
                    if (WgapCombo && R.IsReady() && Rstate != 4 && W.IsReady() 
                        && Wstate != 2 && getCheckBoxItem(spellMenu, "Wcombo"))
                    {
                        W.Cast(Player.Position.LSExtend(target.Position, 600));
                    }
                }
                else if (a <= Q.Range)
                {
                    if (Ecol == 1)
                    {
                        if (W.IsReady() && Wstate != 2)
                        {
                            useW();
                            useQ();
                            useR();
                            useE();
                        }
                        if (!W.IsReady() && Wstate == 1 && R.IsReady() && Rstate == 2)
                        {
                            useR();
                            useQ();
                            useE();
                            useW();
                        }
                        else
                        {
                            useQ();
                            useR();
                            useE();
                            useW();
                        }
                    }
                    if (Ecol == 0)
                    {
                        useQ();
                        useR();
                        useE();
                        if (!(R.IsReady() && Rstate == 1))
                            useW();
                    }
                }
            }
            else
            {
                var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
                if (target.LSIsValidTarget())
                {
                    CheckE(target);
                    if (Ecol == 1)
                    {
                        if (W.IsReady() && Wstate != 2)
                        {
                            useW();
                            useQ();
                            useR();
                            useE();
                        }
                        if (!W.IsReady() && Wstate == 1 && R.IsReady() && Rstate == 2)
                        {
                            useR();
                            useQ();
                            useE();
                            useW();
                        }
                        else
                        {
                            useQ();
                            useR();
                            useE();
                            useW();
                        }
                    }
                    if (Ecol == 0)
                    {
                        useQ();
                        useR();
                        useE();
                        if (!(R.IsReady() && Rstate == 1))
                            useW();
                    }
                }
                else
                {
                    var target1 = TargetSelector.GetTarget(1200, DamageType.Magical);
                    if (target1 != null)
                    {
                        if (WgapCombo && R.IsReady() && Rstate != 4 && W.IsReady() 
                            && Wstate != 2 && getCheckBoxItem(spellMenu, "Wcombo"))
                        {
                            W.Cast(Player.Position.LSExtend(target1.Position, 600));
                        }
                    }
                }
            }
        }

        public static void useQE()
        {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                var target = TargetSelector.SelectedTarget;
                if (target != null && target.LSIsValidTarget() && !target.IsZombie)
                {
                    if( Player.LSDistance(target.Position) <= Q.Range)
                    {
                        Q.Cast(target);
                    }
                    if (Player.LSDistance(target.Position) <= E.Range)
                    {
                        E.Cast(target);
                    }
                } 
        }
    }
}
