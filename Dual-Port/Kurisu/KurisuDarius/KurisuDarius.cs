using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SharpDX;
using KL = KurisuDarius.KurisuLib;
using TargetSelector = PortAIO.TSManager;
using Utility = LeagueSharp.Common.Utility;

namespace KurisuDarius
{
    internal class KurisuDarius
    {
        internal static Menu Config, drmenu, rmenu, cmenu;
        internal static int LastGrabTimeStamp;
        internal static int LastDunkTimeStamp;
        internal static HpBarIndicator HPi = new HpBarIndicator();

        public KurisuDarius()
        {
            if (ObjectManager.Player.ChampionName == "Darius")
            {
                Menu_OnLoad();

                // On Update Event
                Game.OnUpdate += Game_OnUpdate;

                // On Draw Event
                Drawing.OnDraw += Drawing_OnDraw;
                Drawing.OnEndScene += Drawing_OnEndScene;

                // After Attack Event
                LSEvents.AfterAttack += Orbwalking_AfterAttack;

                // On Spell Cast Event
                Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
                CustomEvents.Unit.OnDash += Unit_OnDash;

                // Interrupter
                Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            }
        }

        internal static void Unit_OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            if (!getCheckBoxItem(cmenu, "useeflee"))
            {
                return;
            }

            var hero = sender as AIHeroClient;
            if (hero != null && hero.IsEnemy && hero.LSDistance(args.EndPos) < KL.Player.LSDistance(args.EndPos))
            {
                if (hero.LSIsValidTarget(KL.Spellbook["E"].Range))
                {
                    if (KL.Spellbook["E"].IsReady())
                    {
                        KL.Spellbook["E"].Cast(args.EndPos);
                    }
                }
            }
        }

        internal static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender.LSIsValidTarget(KL.Spellbook["E"].Range) && KL.Spellbook["E"].IsReady())
            {
                if (getCheckBoxItem(cmenu, "useeint"))
                {
                    KL.Spellbook["E"].Cast(sender.ServerPosition);
                }
            }
        }

        internal static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            switch (args.SData.Name.ToLower())
            {
                case "dariuscleave":
                    Utility.DelayAction.Add(Game.Ping + 800, PortAIO.OrbwalkerManager.ResetAutoAttackTimer);
                    break;

                case "dariusaxegrabcone":
                    LastGrabTimeStamp = Utils.GameTimeTickCount;
                    Utility.DelayAction.Add(Game.Ping + 100, PortAIO.OrbwalkerManager.ResetAutoAttackTimer);
                    break;

                case "dariusexecute":
                    LastDunkTimeStamp = Utils.GameTimeTickCount;
                    Utility.DelayAction.Add(Game.Ping + 350, PortAIO.OrbwalkerManager.ResetAutoAttackTimer);
                    break;
            }
        }

        internal static float RModifier => getSliderItem(rmenu, "rmodi");

        internal static float MordeShield(AIHeroClient unit)
        {
            if (unit.ChampionName != "Mordekaiser")
            {
                return 0f;
            }

            return unit.Mana;
        }

        internal static int PassiveCount(Obj_AI_Base unit)
        {
            return unit.GetBuffCount("dariushemo") > 0 ? unit.GetBuffCount("dariushemo") : 0;
        }

        internal static void Drawing_OnEndScene(EventArgs args)
        {
            if (!getCheckBoxItem(drmenu, "drawfill") || KL.Player.IsDead)
            {
                return;
            }

            foreach (var enemy in HeroManager.Enemies.Where(ene => ene.LSIsValidTarget() && ene.IsHPBarRendered))
            {
                HPi.unit = enemy;
                HPi.drawDmg(KL.RDmg(enemy, PassiveCount(enemy)), new ColorBGRA(255, 255, 0, 90));
            }
        }

        internal static void Game_OnUpdate(EventArgs args)
        {
            if (KL.Spellbook["R"].IsReady() && getCheckBoxItem(rmenu,"ksr"))
            {
                foreach (var unit in HeroManager.Enemies.Where(ene => ene.LSIsValidTarget(KL.Spellbook["R"].Range) && !ene.IsZombie))
                {
                    if (unit.CountEnemiesInRange(1200) <= 1 && getCheckBoxItem(rmenu, "ksr1"))
                    {
                        if (KL.RDmg(unit, PassiveCount(unit)) + RModifier + 
                            KL.Hemorrhage(unit, PassiveCount(unit) - 1) >= unit.Health + MordeShield(unit))
                        {
                            if (!TargetSelector.IsInvulnerable(unit, DamageType.True))
                            {
                                if (!unit.HasBuff("kindredrnodeathbuff"))
                                    KL.Spellbook["R"].CastOnUnit(unit);
                            }
                        }
                    }

                    if (KL.RDmg(unit, PassiveCount(unit)) + RModifier >= unit.Health +
                        KL.Hemorrhage(unit, 1) + MordeShield(unit))
                    {
                        if (!TargetSelector.IsInvulnerable(unit, DamageType.True))
                        {
                            if (!unit.HasBuff("kindredrnodeathbuff"))
                                KL.Spellbook["R"].CastOnUnit(unit);
                        }
                    }
                }
            }

            if (PortAIO.OrbwalkerManager.isComboActive)
            {
                Combo(getCheckBoxItem(cmenu, "useq"), getCheckBoxItem(cmenu, "usew"),
                    getCheckBoxItem(cmenu, "usee"), getCheckBoxItem(rmenu, "user"));
            }

            if (PortAIO.OrbwalkerManager.isHarassActive)
            {
                Harass();
            }

            if (getKeyBindItem(cmenu, "caste"))
            {
                PortAIO.OrbwalkerManager.MoveA(Game.CursorPos);
                Combo(false, false, true, false);
            }
        }

        internal static void Drawing_OnDraw(EventArgs args)
        {
            if (KL.Player.IsDead)
            {
                return;
            }

            var acircle = getCheckBoxItem(drmenu, "drawe");
            if (acircle)
                Render.Circle.DrawCircle(KL.Player.Position, KL.Spellbook["E"].Range, System.Drawing.Color.Cyan, 1);

            var rcircle = getCheckBoxItem(drmenu, "drawr");
            if (rcircle)
                Render.Circle.DrawCircle(KL.Player.Position, KL.Spellbook["R"].Range, System.Drawing.Color.Cyan, 1);

            var qcircle = getCheckBoxItem(drmenu, "drawq");
            if (qcircle)
                Render.Circle.DrawCircle(KL.Player.Position, KL.Spellbook["Q"].Range, System.Drawing.Color.Cyan, 1);

            if (!getCheckBoxItem(drmenu, "drawstack"))
            {
                return;
            }

            var plaz = Drawing.WorldToScreen(KL.Player.Position); // player z axis
            if (KL.Player.GetBuffCount("dariusexecutemulticast") > 0)
            {
                var executetime = KL.Player.GetBuff("dariusexecutemulticast").EndTime - Game.Time;
                Drawing.DrawText(plaz[0] - 15, plaz[1] + 55, System.Drawing.Color.OrangeRed, executetime.ToString("0.0"));
            }

            foreach (var enemy in HeroManager.Enemies.Where(ene => ene.LSIsValidTarget() && !ene.IsZombie))
            {
                var enez = Drawing.WorldToScreen(enemy.Position); // enemy z axis
                if (enemy.GetBuffCount("dariushemo") > 0)
                {
                    var endtime = enemy.GetBuff("dariushemo").EndTime - Game.Time;
                    Drawing.DrawText(enez[0] - 50, enez[1], System.Drawing.Color.OrangeRed,  "Stack Count: " + enemy.GetBuffCount("dariushemo"));
                    Drawing.DrawText(enez[0] - 25, enez[1] + 20, System.Drawing.Color.OrangeRed, endtime.ToString("0.0"));
                }
            }
        }


        internal static void Orbwalking_AfterAttack(AfterAttackArgs args)
        {
            var hero = PortAIO.OrbwalkerManager.LastTarget() as AIHeroClient;
            if (hero == null || !hero.IsValid<AIHeroClient>() || hero.Type != GameObjectType.AIHeroClient ||
                !PortAIO.OrbwalkerManager.isComboActive)
            {
                return;
            }

            if (KL.Spellbook["R"].IsReady() && KL.Player.Mana - KL.Spellbook["W"].ManaCost > 
                KL.Spellbook["R"].ManaCost || !KL.Spellbook["R"].IsReady())
            {
                if (!hero.HasBuffOfType(BuffType.Slow) || getCheckBoxItem(cmenu, "wwww"))
                    KL.Spellbook["W"].Cast();
            }

            if (!KL.Spellbook["W"].IsReady() && getCheckBoxItem(cmenu, "iii"))
            {
                KL.HandleItems();
            }
        }

        internal static bool CanQ(Obj_AI_Base unit)
        {
            if (!unit.LSIsValidTarget() || unit.IsZombie ||
                TargetSelector.IsInvulnerable(unit, DamageType.Physical))
            {
                return false;
            }

            if (KL.Player.LSDistance(unit.ServerPosition) < 175 ||
                Utils.GameTimeTickCount - LastGrabTimeStamp < 350)
            {
                return false;
            }

            if (KL.Spellbook["R"].IsReady() &&
                KL.Player.Mana - KL.Spellbook["Q"].ManaCost < KL.Spellbook["R"].ManaCost)
            {
                return false;
            }

            if (KL.Spellbook["W"].IsReady() && KL.WDmg(unit) >= unit.Health &&
                unit.LSDistance(KL.Player.ServerPosition) <= 200)
            {
                return false;
            }

            if (KL.Spellbook["W"].IsReady() && KL.Player.HasBuff("DariusNoxonTactictsONH") &&
                unit.LSDistance(KL.Player.ServerPosition) <= 225)
            {
                return false;
            }

            if (KL.Player.LSDistance(unit.ServerPosition) > KL.Spellbook["Q"].Range)
            {
                return false;
            }

            if (KL.Spellbook["R"].IsReady() && KL.Spellbook["R"].IsInRange(unit) &&
                KL.RDmg(unit, PassiveCount(unit)) - KL.Hemorrhage(unit, 1) >= unit.Health)
            {
                return false;
            }

            if (KL.Player.GetAutoAttackDamage(unit) * 2 + KL.Hemorrhage(unit, PassiveCount(unit)) >= unit.Health &&
                KL.Player.LSDistance(unit.ServerPosition) <= 180)
            {
                return false;
            }

            return true;
        }

        internal static void Harass()
        {
            if (getCheckBoxItem(cmenu, "harassq") && KL.Spellbook["Q"].IsReady())
            {
                if (KL.Player.Mana / KL.Player.MaxMana * 100 > 60)
                {
                    if (CanQ(TargetSelector.GetTarget(KL.Spellbook["E"].Range, DamageType.Physical)))
                    {
                        KL.Spellbook["Q"].Cast();
                    }
                }
            }   
        }

        internal static void Combo(bool useq, bool usew, bool usee, bool user)
        {
            if (useq && KL.Spellbook["Q"].IsReady())
            {
                if (CanQ(TargetSelector.GetTarget(KL.Spellbook["E"].Range, DamageType.Physical)))
                {
                    KL.Spellbook["Q"].Cast();
                }
            }

            if (usew && KL.Spellbook["W"].IsReady())
            {
                var wtarget = TargetSelector.GetTarget(KL.Spellbook["E"].Range, DamageType.Physical);
                if (wtarget.LSIsValidTarget(KL.Spellbook["W"].Range) && !wtarget.IsZombie)
                {
                    if (wtarget.LSDistance(KL.Player.ServerPosition) <= 200 && KL.WDmg(wtarget) >= wtarget.Health)
                    {
                        if (Utils.GameTimeTickCount - LastDunkTimeStamp >= 500)
                        {
                            KL.Spellbook["W"].Cast();
                        }
                    }
                }
            }

            if (usee && KL.Spellbook["E"].IsReady())
            {
                var etarget = TargetSelector.GetTarget(KL.Spellbook["E"].Range, DamageType.Physical);
                if (etarget.LSIsValidTarget())
                {
                    if (etarget.LSDistance(KL.Player.ServerPosition) > 250)
                    {
                        if (KL.Player.CountAlliesInRange(1000) >= 1)
                            KL.Spellbook["E"].Cast(etarget.ServerPosition);

                        if (KL.RDmg(etarget, PassiveCount(etarget)) - KL.Hemorrhage(etarget, 1) >= etarget.Health)
                            KL.Spellbook["E"].Cast(etarget.ServerPosition);

                        if (KL.Spellbook["Q"].IsReady() || KL.Spellbook["W"].IsReady())
                            KL.Spellbook["E"].Cast(etarget.ServerPosition);

                        if (KL.Player.GetAutoAttackDamage(etarget) + KL.Hemorrhage(etarget, 3) * 3 >= etarget.Health)
                            KL.Spellbook["E"].Cast(etarget.ServerPosition);
                    }           
                }
            }

            if (user && KL.Spellbook["R"].IsReady())
            {
                var unit = TargetSelector.GetTarget(KL.Spellbook["E"].Range, DamageType.Physical);

                if (unit.LSIsValidTarget(KL.Spellbook["R"].Range) && !unit.IsZombie)
                {
                    if (!unit.HasBuffOfType(BuffType.Invulnerability) && !unit.HasBuffOfType(BuffType.SpellShield))
                    {
                        if (KL.RDmg(unit, PassiveCount(unit)) + RModifier >= unit.Health +
                            KL.Hemorrhage(unit, 1) + MordeShield(unit))
                        {
                            if (!TargetSelector.IsInvulnerable(unit, DamageType.True))
                            {
                                if (!unit.HasBuff("kindredrnodeathbuff"))
                                    KL.Spellbook["R"].CastOnUnit(unit);
                            }
                        }
                    }
                }
            }
        }

        internal static void Menu_OnLoad()
        {
            Config = MainMenu.AddMenu("Kurisu's Darius", "darius");

            drmenu = Config.AddSubMenu(":: Draw Settings", "drawings");
            drmenu.Add("drawe", new CheckBox("Draw E"));
            drmenu.Add("drawq", new CheckBox("Draw Q"));
            drmenu.Add("drawr", new CheckBox("Draw R"));
            drmenu.Add("drawfill", new CheckBox(":: Draw R Damage Fill"));
            drmenu.Add("drawstack", new CheckBox(":: Draw Stack Count"));


            rmenu = Config.AddSubMenu(":: Ultimate Settings", "rmenu");
            rmenu.Add("user", new CheckBox(":: Use R in combo"));
            rmenu.Add("ksr", new CheckBox(":: Use R auto (no key)"));
            rmenu.Add("ksr1", new CheckBox(":: Use R early (1v1)"));
            rmenu.AddLabel("Use early if target will bleed to death");
            rmenu.Add("rmodi", new Slider(":: Adjust R damage", 0, -250, 250));
            rmenu.AddLabel("Lower it if the target is living with a slither of health.");


            cmenu = Config.AddSubMenu(":: Darius Settings", "cmenu");
            cmenu.Add("iiii", new CheckBox(":: Hydra/Tiamat/Titanic"));
            cmenu.Add("usee", new CheckBox(":: Use E in combo"));
            cmenu.Add("useq", new CheckBox(":: Use Q in combo"));
            cmenu.Add("harassq", new CheckBox(":: Use Q in harass"));
            cmenu.Add("usew", new CheckBox(":: Use W after attack"));
            cmenu.Add("wwww", new CheckBox(":: Use W on slowed targets"));
            cmenu.Add("useeint", new CheckBox(":: Auto E interrupt"));
            cmenu.Add("useeflee", new CheckBox(":: Auto E fleeing targets"));
            cmenu.AddLabel("Will pull targets in who use a spell to flee.");
            cmenu.Add("caste", new KeyBind("Cast Assisted E [semi]", false, KeyBind.BindTypes.HoldActive, 'T'));
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
    }
}
