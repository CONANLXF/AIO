using System;
using System.Collections.Generic;
using System.Linq;
using Activators.Base;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;

using TargetSelector = PortAIO.TSManager; namespace Activators.Handlers
{
    struct Offset
    {
        public float X;
        public float Y;
        public int Width;
        public int Height;

        internal Offset(Vector2 vec, int width, int height)
        {
            X = vec.X;
            Y = vec.Y;
            Width = width;
            Height = height;
        }
    }

    class Drawings
    {
        private static Dictionary<string, Offset> Offsets = new Dictionary<string, Offset>
        {
            { "SRU_Blue1.1.1" , new Offset(new Vector2(-2, 23), 150, 6) },
            { "SRU_Red4.1.1" , new Offset(new Vector2(-2, 23), 150, 6) },
            { "SRU_Blue7.1.1" , new Offset(new Vector2(-2, 23), 150, 6) },
            { "SRU_Red10.1.1" , new Offset(new Vector2(-2, 23), 150, 6) },
            { "SRU_Baron12.1.1" , new Offset(new Vector2(57, 24), 165, 11) },
            { "SRU_RiftHerald17.1.1" , new Offset(new Vector2(-2, 23), 155, 6) },
            { "SRU_Dragon_Air6.1.1" , new Offset(new Vector2(1, 23), 150, 6) },
            { "SRU_Dragon_Fire6.2.1" , new Offset(new Vector2(1, 23), 150, 6) },
            { "SRU_Dragon_Water6.3.1" , new Offset(new Vector2(1, 23), 150, 6) },
            { "SRU_Dragon_Earth6.4.1" , new Offset(new Vector2(1, 23), 150, 6) },
            { "SRU_Dragon_Elder6.5.1" , new Offset(new Vector2(1, 23), 150, 7) },
        };

        public static void Init()
        {
            Drawing.OnDraw += args =>
            {
                if (Activator.zmenu["acdebug"].Cast<CheckBox>().CurrentValue)
                {
                    foreach (var hero in Activator.Allies())
                    {
                        var mpos = Drawing.WorldToScreen(hero.Player.Position);

                        if (!hero.Player.IsDead)
                        {
                            Drawing.DrawText(mpos[0] - 40, mpos[1] - 15, Color.White, "Income Damage: " + hero.IncomeDamage);
                            Drawing.DrawText(mpos[0] - 40, mpos[1] + 0, Color.White, "Income Percent: " + hero.IncomeDamage / hero.Player.MaxHealth * 100);
                            Drawing.DrawText(mpos[0] - 40, mpos[1] + 15, Color.White, "QSSBuffCount: " + hero.QSSBuffCount);
                            Drawing.DrawText(mpos[0] - 40, mpos[1] + 30, Color.White, "QSSHighestBuffTime: " + hero.QSSHighestBuffTime);
                        }


                        Drawing.DrawText(200f, 250f, Color.Wheat, "Item Priority (Debug)");
                        foreach (var item in Items.CoreItem.PriorityList().OrderByDescending(a => a.Priority))
                        {
                            for (int i = 0; i < Items.CoreItem.PriorityList().Count(); i++)
                            {
                                Drawing.DrawText(200, 265 + 5 * (i * 3), Color.White, item.DisplayName + " : " + item.Priority);
                            }
                        }

                    }
                }

                if (!Activator.SmiteInGame)
                {
                    return;
                }

                if (Activator.zmenu["drawsmitet"].Cast<CheckBox>().CurrentValue)
                {
                    var wts = Drawing.WorldToScreen(Activator.Player.Position);

                    if (Activator.smenu["usesmite"].Cast<KeyBind>().CurrentValue)
                        Drawing.DrawText(wts[0] - 35, wts[1] + 55, Color.White, "Smite: ON");

                    if (!Activator.smenu["usesmite"].Cast<KeyBind>().CurrentValue)
                        Drawing.DrawText(wts[0] - 35, wts[1] + 55, Color.Gray, "Smite: OFF");
                }

                if (Activator.zmenu["drawsmite"].Cast<CheckBox>().CurrentValue)
                {
                    if (Activator.smenu["usesmite"].Cast<KeyBind>().CurrentValue)
                        Render.Circle.DrawCircle(Activator.Player.Position, 500f, Color.White, 2);

                    if (!Activator.smenu["usesmite"].Cast<KeyBind>().CurrentValue)
                        Render.Circle.DrawCircle(Activator.Player.Position, 500f, Color.Gray, 2);
                }

                if (!Activator.Player.IsDead && Activator.zmenu["drawfill"].Cast<CheckBox>().CurrentValue)
                {
                    if (Activator.MapId != (int)MapType.SummonersRift)
                    {
                        return;
                    }

                    var smitespell = Data.Smitedata.SpellList
                        .FirstOrDefault(s => s.Name == Activator.Player.ChampionName);

                    foreach (var minion in
                        MinionManager.GetMinions(Activator.Player.Position, 1200f, MinionTypes.All, MinionTeam.Neutral)
                            .Where(th => Essentials.IsEpicMinion(th) || Essentials.IsLargeMinion(th)))
                    {
                        var yoffset = Offsets[minion.Name].Y;
                        var xoffset = Offsets[minion.Name].X;
                        var width = Offsets[minion.Name].Width;
                        var height = Offsets[minion.Name].Height;

                        if (!minion.IsHPBarRendered)
                        {
                            continue;
                        }

                        var barPos = minion.HPBarPosition;

                        var sdamage = smitespell != null && Activator.Player.GetSpell(smitespell.Slot).State == SpellState.Ready
                            ? Activator.Player.LSGetSpellDamage(minion, smitespell.Slot, smitespell.Stage)
                            : 0;

                        var smite = Activator.Player.GetSpell(Activator.Smite).State == SpellState.Ready
                            ? Activator.Player.GetSummonerSpellDamage(minion, Damage.SummonerSpell.Smite)
                            : 0;

                        var damage = smite + sdamage;
                        var pctafter = Math.Max(0, minion.Health - damage) / minion.MaxHealth;

                        var yaxis = barPos.Y + yoffset;
                        var xaxisdmg = (float)(barPos.X + xoffset + width * pctafter);
                        var xaxisnow = barPos.X + xoffset + width * minion.Health / minion.MaxHealth;

                        var ana = xaxisnow - xaxisdmg;
                        var pos = barPos.X + xoffset + 12 + (139 * pctafter);

                        for (var i = 0; i < ana; i++)
                        {
                            if (Activator.smenu["usesmite"].Cast<KeyBind>().CurrentValue)
                                Drawing.DrawLine((float)pos + i, yaxis, (float)pos + i, yaxis + height, 1,
                                    Color.White);

                            if (!Activator.smenu["usesmite"].Cast<KeyBind>().CurrentValue)
                                Drawing.DrawLine((float)pos + i, yaxis, (float)pos + i, yaxis + height, 1,
                                    Color.Gray);
                        }
                    }
                }
            };
        }
    }
}