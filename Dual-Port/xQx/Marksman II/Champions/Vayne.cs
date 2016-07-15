#region
using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Marksman.Utils;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;

#endregion

using TargetSelector = PortAIO.TSManager; namespace Marksman.Champions
{
    internal class Vayne : Champion
    {
        public static float rqTumbleBuffEndOfTime = 0;
        public static bool VayneUltiIsActive { get; set; }

        public static LeagueSharp.Common.Spell Q, E, R;

        public Vayne()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 300f);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 650f);
            R = new LeagueSharp.Common.Spell(SpellSlot.R);

            E.SetTargetted(0.25f, 2200f);

            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;

            Utils.Utils.PrintMessage("Vayne loaded");
        }

        public override void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {

        }

        public override void Obj_AI_Base_OnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            rqTumbleBuffEndOfTime = Program.misc["Misc.R.DontAttack"].Cast<CheckBox>().CurrentValue && sender.IsMe &&
                                    args.Buff.Name.ToLower() == "vaynetumblefade"
                ? args.Buff.EndTime
                : 0;
        }

        public void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Program.misc["UseEGapcloser"].Cast<CheckBox>().CurrentValue && E.IsReady() && gapcloser.Sender.LSIsValidTarget(E.Range))
                E.CastOnUnit(gapcloser.Sender);
        }

        private void Interrupter2_OnInterruptableTarget(AIHeroClient unit, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (Program.misc["UseEInterrupt"].Cast<CheckBox>().CurrentValue && unit.LSIsValidTarget(550f))
                E.Cast(unit);
        }

        private static bool CastE(Obj_AI_Base t)
        {
            for (var i = 1; i < 8; i++)
            {
                var targetBehind = t.Position + Vector3.Normalize(t.ServerPosition - ObjectManager.Player.Position) * i * 50;

                if (targetBehind.LSIsWall() && t.LSIsValidTarget(E.Range))
                {
                    E.CastOnUnit(t);
                    return true;
                }
            }
            return false;
        }
        
        public override void Game_OnGameUpdate(EventArgs args)
        {
            PortAIO.OrbwalkerManager.SetAttack(Game.Time > rqTumbleBuffEndOfTime);

            if (JungleClearActive)
            {
                ExecJungleClear();
            }

            if ((ComboActive || HarassActive))
            {
                if (Program.combo["FocusW"].Cast<CheckBox>().CurrentValue)
                {
                    var silverBuffMarkedEnemy = VayneData.GetSilverBuffMarkedEnemy;
                    if (silverBuffMarkedEnemy != null)
                    {
                        PortAIO.OrbwalkerManager.ForcedTarget((silverBuffMarkedEnemy));
                    }
                    else
                    {
                        var attackRange = Orbwalking.GetRealAutoAttackRange(ObjectManager.Player);
                        PortAIO.OrbwalkerManager.ForcedTarget((TargetSelector.GetTarget(attackRange, DamageType.Physical)));
                    }
                }

                var useQ = Program.combo["Combo.UseQ"].Cast<ComboBox>().CurrentValue;
                var t = TargetSelector.GetTarget(Q.Range + Orbwalking.GetRealAutoAttackRange(null), DamageType.Physical);
                if (Q.IsReady() && t.LSIsValidTarget() && useQ != 0)
                {
                    switch (useQ)
                    {
                        case 1:
                            {
                                Q.Cast(Game.CursorPos);
                                break;
                            }

                        case 2:
                            {
                                var silverEnemy = VayneData.GetSilverBuffMarkedEnemy;
                                if (silverEnemy != null && t.ChampionName == silverEnemy.ChampionName &&
                                    VayneData.GetSilverBuffMarkedCount == 2)
                                {
                                    Q.Cast(Game.CursorPos);
                                    PortAIO.OrbwalkerManager.ForcedTarget((t));
                                }
                                break;
                            }

                        case 3:
                            {
                                if (t.LSDistance(ObjectManager.Player.Position) > Orbwalking.GetRealAutoAttackRange(null) && Q.IsPositionSafe(t.Position.LSTo2D()))
                                {
                                    Q.Cast(t.Position);
                                }
                                else if (Q.IsPositionSafe(Game.CursorPos.LSTo2D()))
                                {
                                    Q.Cast(Game.CursorPos);
                                }
                                PortAIO.OrbwalkerManager.ForcedTarget((t));
                                break;
                            }
                    }
                }

                var useE = Program.combo["UseEC"].Cast<ComboBox>().CurrentValue;
                if (E.IsReady() && useE != 0)
                {
                    t = TargetSelector.GetTarget(E.Range + Q.Range, DamageType.Physical);
                    if (useE == 1)
                    {
                        if (t.LSIsValidTarget())
                        {
                            CastE(t);
                        }
                    }
                    else
                    {
                        foreach (var e in HeroManager.Enemies.Where(e => e.LSIsValidTarget(E.Range) && !e.IsZombie))
                        {
                            CastE(e);
                        }
                    }
                    /*
                    foreach (var hero in
                        from hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.LSIsValidTarget(550f))
                        let prediction = E.GetPrediction(hero)
                        where
                            NavMesh.GetCollisionFlags(
                                prediction.UnitPosition.LSTo2D()
                                    .LSExtend(
                                        ObjectManager.Player.ServerPosition.LSTo2D(),
                                        -GetValue<Slider>("PushDistance").Value)
                                    .To3D()).HasFlag(CollisionFlags.Wall) ||
                            NavMesh.GetCollisionFlags(
                                prediction.UnitPosition.LSTo2D()
                                    .LSExtend(
                                        ObjectManager.Player.ServerPosition.LSTo2D(),
                                        -(GetValue<Slider>("PushDistance").Value/2))
                                    .To3D()).HasFlag(CollisionFlags.Wall)
                        select hero)
                    {
                        E.Cast(hero);
                    }
                    */
                }
            }

            if (LaneClearActive)
            {
                var useQ = Program.laneclear["UseQL"].Cast<CheckBox>().CurrentValue;

                if (Q.IsReady() && useQ)
                {
                    var vMinions = MinionManager.GetMinions(ObjectManager.Player.Position, Q.Range);
                    foreach (var minions in
                        vMinions.Where(
                            minions => minions.Health < ObjectManager.Player.LSGetSpellDamage(minions, SpellSlot.Q)))
                        Q.Cast(minions);
                }
            }
        }

        public void ExecJungleClear()
        {
            var jungleMobs =
                Marksman.Utils.Utils.GetMobs(Q.Range + Orbwalking.GetRealAutoAttackRange(null) + 65,
                    Marksman.Utils.Utils.MobTypes.All);

            if (jungleMobs != null)
            {
                switch (Program.jungleClear["UseQJ"].Cast<ComboBox>().CurrentValue)
                {
                    case 1:
                        {
                            if (!jungleMobs.BaseSkinName.ToLower().Contains("baron") ||
                                !jungleMobs.BaseSkinName.ToLower().Contains("dragon"))
                            {
                                if (jungleMobs.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65))
                                    Q.Cast(
                                        jungleMobs.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65)
                                            ? Game.CursorPos
                                            : jungleMobs.Position);
                            }
                            break;
                        }
                    case 2:
                        {
                            if (!jungleMobs.BaseSkinName.ToLower().Contains("baron") ||
                                !jungleMobs.BaseSkinName.ToLower().Contains("dragon"))
                            {
                                jungleMobs =
                                    Marksman.Utils.Utils.GetMobs(
                                        Q.Range + Orbwalking.GetRealAutoAttackRange(null) + 65,
                                        Marksman.Utils.Utils.MobTypes.BigBoys);
                                if (jungleMobs != null)
                                {
                                    Q.Cast(
                                        jungleMobs.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65)
                                            ? Game.CursorPos
                                            : jungleMobs.Position);
                                }
                            }
                            break;
                        }
                }

                switch (Program.jungleClear["UseEJ"].Cast<ComboBox>().CurrentValue)
                {
                    case 1:
                        {
                            if (jungleMobs.LSIsValidTarget(E.Range))
                                E.CastOnUnit(jungleMobs);
                            break;
                        }
                    case 2:
                        {
                            jungleMobs = Marksman.Utils.Utils.GetMobs(E.Range, Marksman.Utils.Utils.MobTypes.BigBoys);
                            if (jungleMobs != null)
                            {
                                CastE(jungleMobs);

                                if (ObjectManager.Player.LSDistance(jungleMobs) < ObjectManager.Player.AttackRange / 2)
                                {
                                    E.CastOnUnit(jungleMobs);
                                }

                            }
                            break;
                        }
                }
            }
        }

        public override void Orbwalking_AfterAttack(AfterAttackArgs args)
        {
        }

        public override bool ComboMenu(Menu config)
        {
            config.Add("Combo.UseQ", new ComboBox("Use Q", 1, "Off", "Tumble to Mouse Cursor", "Just Complete 3rd Silver Buff Mark", "Marksman Settings"));
            config.Add("UseEC", new ComboBox("Use E", 1, "Off", "On", "Just Selected Target"));
            config.Add("FocusW", new CheckBox("Force Focus Marked Enemy"));
            return true;
        }

        public override bool HarassMenu(Menu config)
        {
            config.Add("UseQH", new CheckBox("Use Q"));
            config.Add("UseEH", new CheckBox("Use E"));
            return true;
        }

        public override bool MiscMenu(Menu config)
        {
            config.Add("Misc.R.DontAttack", new CheckBox("Don't Attack If I'm visible with ulti"));
            // TODO: Add back-off option if Vayne's in dangerous
            config.Add("UseET", new KeyBind("Use E (Toggle)", false, KeyBind.BindTypes.PressToggle, 'T'));
            config.Add("UseEInterrupt", new CheckBox("Use E To Interrupt"));
            config.Add("UseEGapcloser", new CheckBox("Use E To Gapcloser"));
            config.Add("PushDistance", new Slider("E Push Distance", 425, 475, 300));
            config.Add("CompleteSilverBuff", new CheckBox("Complete Silver Buff With Q"));
            return true;
        }

        public override bool LaneClearMenu(Menu config)
        {
            config.Add("UseQL", new CheckBox("Use Q"));
            return true;
        }

        public override bool JungleClearMenu(Menu config)
        {
            config.Add("UseQJ", new ComboBox("Use Q", 2, "Off", "On", "Just big Monsters"));
            config.Add("UseEJ", new ComboBox("Use E", 2, "Off", "On", "Just big Monsters"));
            return true;
        }


        public override bool DrawingMenu(Menu config)
        {
            config.Add("DrawQ", new ComboBox("Q range", 2, "Off", "Q Range", "Q + AA Range"));
            config.Add("DrawE", new ComboBox("E range", 3, "Off", "E Range", "E Stun Status", "Both"));
            return true;
        }

        public override void Drawing_OnDraw(EventArgs args)
        {
            return;
        }

        public class VayneData
        {
            public static int GetSilverBuffMarkedCount
            {
                get
                {
                    if (GetSilverBuffMarkedEnemy == null)
                        return 0;

                    return
                        GetSilverBuffMarkedEnemy.Buffs.Where(buff => buff.Name == "vaynesilvereddebuff")
                            .Select(xBuff => xBuff.Count)
                            .FirstOrDefault();
                }
            }

            public static AIHeroClient GetSilverBuffMarkedEnemy
            {
                get
                {
                    return
                        ObjectManager.Get<AIHeroClient>()
                            .Where(
                                enemy =>
                                    !enemy.IsDead &&
                                    enemy.LSIsValidTarget(
                                        (Q.IsReady() ? Q.Range : 0) +
                                        Orbwalking.GetRealAutoAttackRange(ObjectManager.Player)))
                            .FirstOrDefault(
                                enemy => enemy.Buffs.Any(buff => buff.Name == "vaynesilvereddebuff" && buff.Count > 0));
                }
            }
        }
    }
}
