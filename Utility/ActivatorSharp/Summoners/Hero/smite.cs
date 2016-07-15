using System;
using System.Linq;
using Activators.Base;
using Activators.Data;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;

using TargetSelector = PortAIO.TSManager; namespace Activators.Summoners
{
    internal class smite : CoreSum
    {
        internal override string Name => "summonersmite";
        internal override string DisplayName => "Smite";
        internal override float Range => 500f;
        internal override int Duration => 0;
        internal override string[] ExtraNames => new[]
        {
            "s5_summonersmiteplayerganker", "s5_summonersmiteduel",
            "s5_summonersmitequick", "itemsmiteaoe"
        };

        internal static int Limiter;
        internal static void L33TSmite(Obj_AI_Base unit, float smitedmg)
        {
            foreach (var hero in Smitedata.SpellList.Where(x => x.Name == Activator.Player.ChampionName))
            {
                if (Activator.Player.LSGetSpellDamage(unit, hero.Slot, hero.Stage) + smitedmg >= unit.Health && Activator.Player.GetSpell(Activator.Smite).IsReady())
                {
                    if (unit.LSDistance(Activator.Player.ServerPosition) <= hero.CastRange + 
                        unit.BoundingRadius + Activator.Player.BoundingRadius)
                    {
                        if (hero.HeroReqs(unit))
                        {
                            switch (hero.Type)
                            {
                                case SpellDataTargetType.Location:
                                    Activator.Player.Spellbook.CastSpell(hero.Slot, unit.ServerPosition);
                                    break;
                                case SpellDataTargetType.Unit:
                                    Activator.Player.Spellbook.CastSpell(hero.Slot, unit);
                                    break;
                                case SpellDataTargetType.Self:
                                    Activator.Player.Spellbook.CastSpell(hero.Slot);
                                    break;
                                case SpellDataTargetType.SelfAndUnit:
                                    Activator.Player.Spellbook.CastSpell(hero.Slot);
                                    if (Utils.GameTimeTickCount - Limiter >= 200 && Orbwalking.InAutoAttackRange(unit))
                                    {
                                        LeagueSharp.Common.Utility.DelayAction.Add(Game.Ping, () =>
                                        {
                                            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, unit);
                                            Limiter = Utils.GameTimeTickCount;
                                        });
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }

        public override void OnTick(EventArgs args)
        {
            if (!Menu["usesmite"].Cast<KeyBind>().CurrentValue || !Player.GetSpell(Activator.Smite).IsReady())
                return;

            foreach (var minion in MinionManager.GetMinions(900f, MinionTypes.All, MinionTeam.Neutral))
            {
                var damage = Player.Spellbook.GetSpell(Activator.Smite).State == SpellState.Ready
                    ? (float) Player.GetSummonerSpellDamage(minion, Damage.SummonerSpell.Smite)
                    : 0;

                if (minion.LSDistance(Player.ServerPosition) <= 500 + minion.BoundingRadius + Player.BoundingRadius)
                {
                    if (Essentials.IsLargeMinion(minion) && Menu["smitelarge"].Cast<CheckBox>().CurrentValue)
                    {
                        if (Menu["smiteskill"].Cast<CheckBox>().CurrentValue)
                            L33TSmite(minion, damage);

                        if (damage >= minion.Health && IsReady())
                        {
                            Player.Spellbook.CastSpell(Activator.Smite, minion);
                        }
                    }

                    if (Essentials.IsSmallMinion(minion) && Menu["smitesmall"].Cast<CheckBox>().CurrentValue)
                    {
                        if (Menu["smiteskill"].Cast<CheckBox>().CurrentValue)
                            L33TSmite(minion, damage);

                        if (damage >= minion.Health && IsReady())
                        {
                            Player.Spellbook.CastSpell(Activator.Smite, minion);
                        }
                    }

                    if (Essentials.IsEpicMinion(minion) && Menu["smitesuper"].Cast<CheckBox>().CurrentValue && Player.GetSpell(Activator.Smite).IsReady())
                    {
                        if (Menu["smiteskill"].Cast<CheckBox>().CurrentValue)
                            L33TSmite(minion, damage);

                        if (damage >= minion.Health && IsReady())
                        {
                            Player.Spellbook.CastSpell(Activator.Smite, minion);
                        }
                    }
                }
            }

            // smite hero blu/red
            if (Player.GetSpell(Activator.Smite).Name.ToLower() == "s5_summonersmiteduel" ||
                Player.GetSpell(Activator.Smite).Name.ToLower() == "s5_summonersmiteplayerganker" && Player.GetSpell(Activator.Smite).IsReady())
            {
                if (!Menu["savesmite"].Cast<CheckBox>().CurrentValue ||
                     Menu["savesmite"].Cast<CheckBox>().CurrentValue && Player.GetSpell(Activator.Smite).Ammo > 1)
                {
                    // KS Smite
                    if (Menu["smitemode"].Cast<ComboBox>().CurrentValue == 0 &&
                        Player.GetSpell(Activator.Smite).Name.ToLower() == "s5_summonersmiteplayerganker" && Player.GetSpell(Activator.Smite).IsReady())
                    {
                        foreach (
                            var hero in
                                Activator.Heroes.Where(
                                    h =>
                                        h.Player.LSIsValidTarget(500) && !h.Player.IsZombie &&
                                        h.Player.Health <= 20 + 8 * Player.Level))
                        {
                            Player.Spellbook.CastSpell(Activator.Smite, hero.Player);
                        }
                    }

                    // Combo Smite
                    if (Menu["smitemode"].Cast<ComboBox>().CurrentValue == 1 &&
                        Player.GetSpell(Activator.Smite).Name.ToLower() == "s5_summonersmiteduel" && Player.GetSpell(Activator.Smite).IsReady())
                    {
                        if (Activator.Origin["usecombo"].Cast<KeyBind>().CurrentValue)
                        {
                            foreach (
                                var hero in
                                    Activator.Heroes
                                        .Where(h => h.Player.LSIsValidTarget(1200) && !h.Player.IsZombie)
                                        .OrderBy(h => h.Player.LSDistance(Game.CursorPos)))
                            {
                                if (hero.Player.LSDistance(Player.ServerPosition) <= 500)
                                    Player.Spellbook.CastSpell(Activator.Smite, hero.Player);
                            }
                        }
                    }
                }
            }
        }
    }
}
