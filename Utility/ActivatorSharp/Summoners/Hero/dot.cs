using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;

using TargetSelector = PortAIO.TSManager; namespace Activators.Summoners
{
    internal class dot : CoreSum
    {
        internal override string Name => "summonerdot";
        internal override string DisplayName => "Ignite";
        internal override string[] ExtraNames => new[] { "" };
        internal override float Range => 600f;
        internal override int Duration => 100;

        internal Spell Q => new Spell(SpellSlot.Q);
        internal Spell W => new Spell(SpellSlot.W);
        internal Spell E => new Spell(SpellSlot.E);
        internal Spell R => new Spell(SpellSlot.R);

        public override void OnTick(EventArgs args)
        {
            if (!Menu["use" + Name].Cast<CheckBox>().CurrentValue || !IsReady())
                return;

            foreach (var tar in Activator.Heroes)
            {
                if (!tar.Player.LSIsValidTarget(1500))
                {
                    continue;
                }

                if (Activator.smenu[Parent.UniqueMenuId + "useon" + tar.Player.NetworkId] == null)
                {
                    continue;
                }

                if (tar.Player.HasBuff("kindredrnodeathbuff") || tar.Player.IsZombie || tar.Player.HasBuff("summonerdot"))
                {
                    continue;
                }

                if (!Activator.smenu[Parent.UniqueMenuId + "useon" + tar.Player.NetworkId].Cast<CheckBox>().CurrentValue)
                    continue;

                // ignite damagerino
                var ignotedmg = (float) Player.GetSummonerSpellDamage(tar.Player, Damage.SummonerSpell.Ignite);

                // killsteal ignite
                if (Menu["mode" + Name].Cast<ComboBox>().CurrentValue == 0)
                {
                    if (tar.Player.Health <= ignotedmg)
                    {
                        if (tar.Player.LSDistance(Player.ServerPosition) <= 600)
                            UseSpellOn(tar.Player);
                    }
                }

                // combo ignite
                if (Menu["mode" + Name].Cast<ComboBox>().CurrentValue == 1)
                {
                    var totaldmg = 0d;
                    switch (Player.ChampionName)
                    {
                        case "Akali":
                            totaldmg += R.GetDamage(tar.Player) * R.Instance.Ammo;
                            break;
                        case "Ahri":
                            if (!tar.Player.HasBuffOfType(BuffType.Charm) &&
                                Menu["ii" + Player.ChampionName].Cast<CheckBox>().CurrentValue &&
                                E.IsReady())
                                continue;
                            break;
                        case "Cassiopeia":
                            if (!tar.Player.HasBuffOfType(BuffType.Poison) &&
                                Menu["ii" + Player.ChampionName].Cast<CheckBox>().CurrentValue &&
                                (Q.IsReady() || W.IsReady()))
                                continue;

                            var dmg = Math.Min(6, Player.Mana / E.ManaCost) * E.GetDamage(tar.Player);
                            totaldmg += tar.Player.HasBuffOfType(BuffType.Poison) ? dmg * 2 : dmg;
                            break;
                        case "Diana":
                            if (!tar.Player.HasBuff("dianamoonlight") &&
                                Menu["ii" + Player.ChampionName].Cast<CheckBox>().CurrentValue &&
                                Q.IsReady())
                                continue;

                            totaldmg += tar.Player.HasBuff("dianamoonlight")
                                ? R.GetDamage(tar.Player) * 2 : 0;
                            break;
                        case "Evelynn":
                            totaldmg += Math.Min(6, Player.Mana / Q.ManaCost) * Q.GetDamage(tar.Player);
                            break;
                    }

                    // aa dmg
                    totaldmg += Orbwalking.InAutoAttackRange(tar.Player)
                        ? Player.LSGetAutoAttackDamage(tar.Player, true) * 3
                        : 0;

                    // combo damge
                    totaldmg +=
                        Data.Somedata.DamageLib.Sum(
                            entry =>
                                Player.GetSpell(entry.Value).IsReady(2)
                                    ? entry.Key(Player, tar.Player, Player.GetSpell(entry.Value).Level - 1)
                                    : 0);

                    var finaldmg = totaldmg * Menu["idmgcheck"].Cast<Slider>().CurrentValue / 100;

                    if (Menu["idraw"].Cast<CheckBox>().CurrentValue)
                    {
                        var pdmg = finaldmg > tar.Player.Health ? 100 : finaldmg * 100 / tar.Player.Health;
                        var drawdmg = Math.Round(pdmg);
                        var pos = Drawing.WorldToScreen(tar.Player.Position);

                        Drawing.DrawText(pos[0], pos[1], System.Drawing.Color.Yellow, drawdmg + " %");
                    }

                    if (finaldmg + ignotedmg >= tar.Player.Health)
                    {
                        var nt = ObjectManager.Get<Obj_AI_Turret>().FirstOrDefault(
                                x => !x.IsDead && x.IsValid && x.Team == tar.Player.Team && tar.Player.LSDistance(x.Position) <= 1250);
                        
                        if (nt != null && Menu["itu"].Cast<CheckBox>().CurrentValue && Player.Level <= Menu["igtu"].Cast<Slider>().CurrentValue)
                        {
                            if (Player.CountAlliesInRange(750) == 0 && (totaldmg + ignotedmg / 1.85) < tar.Player.Health)
                                continue;
                        }

                        if (Orbwalking.InAutoAttackRange(tar.Player) && tar.Player.CountAlliesInRange(450) > 1)
                        {
                            if (totaldmg + ignotedmg / 2.5 >= tar.Player.Health)
                            {
                                continue;
                            }

                            if (nt != null && tar.Player.LSDistance(nt) <= 600)
                            {
                                continue;
                            }
                        }

                        if (tar.Player.Level <= 4 &&
                            tar.Player.InventoryItems.Any(item => item.Id == (ItemId) 2003 || item.Id == (ItemId) 2010))
                        {
                            continue;
                        }

                        if (tar.Player.LSDistance(Player.ServerPosition) <= 600)
                        {
                            UseSpellOn(tar.Player, true);
                        }
                    }
                }
            }
        }
    }
}
