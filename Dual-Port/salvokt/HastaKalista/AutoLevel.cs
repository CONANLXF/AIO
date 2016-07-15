using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp;
using LeagueSharp.Common;
using System;

using TargetSelector = PortAIO.TSManager; namespace HastaKalistaBaby
{
    internal class AutoLevel
    {
        private static Menu lvla = Program.lvl;
        static int Q, W, E, R;

        public static void Init()
        {
            Game.OnUpdate += OnUpdate;
            Obj_AI_Base.OnLevelUp += Obj_AI_Base_OnLevelUp;
            Drawing.OnDraw += OnDraw;
        }

        private static void OnUpdate(EventArgs args)
        {
            Q = lvla["1"].Cast<ComboBox>().CurrentValue;
            W = lvla["2"].Cast<ComboBox>().CurrentValue;
            E = lvla["3"].Cast<ComboBox>().CurrentValue;
            R = lvla["4"].Cast<ComboBox>().CurrentValue;
            if (SameValues())
            {
                return;
            }
        }

        private static void Obj_AI_Base_OnLevelUp(Obj_AI_Base sender, EventArgs args)
        {
            if (!sender.IsMe || !lvla["Lvlon"].Cast<CheckBox>().CurrentValue || Program.Player.Level < lvla["s"].Cast<Slider>().CurrentValue || SameValues())
            {
                return;
            }
            lvl(Q);
            lvl(W);
            lvl(E);
            lvl(R);
        }

        private static void OnDraw(EventArgs args)
        {
            if (SameValues())
            {
                Drawing.DrawText(Drawing.WorldToScreen(Program.Player.Position).X, Drawing.WorldToScreen(Program.Player.Position).Y - 10, System.Drawing.Color.OrangeRed, "Wrong Ability Sequence");
            }
        }

        private static void lvl(int i)//Inspired By seb oktw
        {
            if (Program.Player.Level < 4)
            {
                if (i == 0 && Program.Player.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
                    Program.Player.Spellbook.LevelSpell(SpellSlot.Q);
                if (i == 1 && Program.Player.Spellbook.GetSpell(SpellSlot.W).Level == 0)
                    Program.Player.Spellbook.LevelSpell(SpellSlot.W);
                if (i == 2 && Program.Player.Spellbook.GetSpell(SpellSlot.E).Level == 0)
                    Program.Player.Spellbook.LevelSpell(SpellSlot.E);
            }
            else
            {
                if (i == 0)
                    Program.Player.Spellbook.LevelSpell(SpellSlot.Q);
                if (i == 1)
                    Program.Player.Spellbook.LevelSpell(SpellSlot.W);
                if (i == 2)
                    Program.Player.Spellbook.LevelSpell(SpellSlot.E);
                if (i == 3)
                    Program.Player.Spellbook.LevelSpell(SpellSlot.R);
            }
        }

        private static bool SameValues()
        {
            if ((Q == W || Q == E || Q == R || W == E || W == R || E == R) && lvla["Lvlon"].Cast<CheckBox>().CurrentValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
