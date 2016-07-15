using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Color = System.Drawing.Color;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace ezEvade
{
    class SpellTester
    {
        public static Menu menu, setSpellPositionMenu, fireDummySpellMenu;
        public static Menu selectSpellMenu;

        private static AIHeroClient myHero { get { return ObjectManager.Player; } }

        private static Dictionary<string, Dictionary<string, SpellData>> spellCache
                 = new Dictionary<string, Dictionary<string, SpellData>>();

        public static Vector3 spellStartPosition = myHero.ServerPosition;
        public static Vector3 spellEndPostion = myHero.ServerPosition
                              + (myHero.Direction.LSTo2D().LSPerpendicular() * 500).To3D();

        public static float lastSpellFireTime = 0;

        public SpellTester()
        {
            menu = MainMenu.AddMenu("Spell Tester", "DummySpellTester");

            selectSpellMenu = menu.AddSubMenu("Select Spell", "SelectSpellMenu");

            setSpellPositionMenu = menu.AddSubMenu("Set Spell Position", "SetPositionMenu");
            setSpellPositionMenu.Add("SetDummySpellStartPosition", new CheckBox("Set Start Position", false));
            setSpellPositionMenu.Add("SetDummySpellEndPosition", new CheckBox("Set End Position", false));
            setSpellPositionMenu["SetDummySpellStartPosition"].Cast<CheckBox>().OnValueChange += OnSpellStartChange;
            setSpellPositionMenu["SetDummySpellEndPosition"].Cast<CheckBox>().OnValueChange += OnSpellEndChange;


            fireDummySpellMenu = menu.AddSubMenu("Fire Dummy Spell", "FireDummySpellMenu");
            fireDummySpellMenu.Add("FireDummySpell", new KeyBind("Fire Dummy Spell Key", false, KeyBind.BindTypes.HoldActive, 'O'));
            fireDummySpellMenu.Add("SpellInterval", new Slider("Spell Interval", 2500, 0, 5000));

            LoadSpellDictionary();

            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            foreach (var spell in SpellDetector.drawSpells.Values)
            {
                Vector2 spellPos = spell.currentSpellPosition;

                if (spell.heroID == myHero.NetworkId)
                {
                    if (spell.spellType == SpellType.Line)
                    {
                        if (spellPos.LSDistance(myHero) <= myHero.BoundingRadius + spell.radius
                            && EvadeUtils.TickCount - spell.startTime > spell.info.spellDelay
                            && spell.startPos.LSDistance(myHero) < spell.info.range)
                        {
                            Draw.RenderObjects.Add(new Draw.RenderCircle(spellPos, 1000, Color.Red,
                                (int)spell.radius, 10));
                            DelayAction.Add(1, () => SpellDetector.DeleteSpell(spell.spellID));
                        }
                        else
                        {
                            Render.Circle.DrawCircle(new Vector3(spellPos.X, spellPos.Y, myHero.Position.Z), (int)spell.radius, Color.White, 5);
                        }
                    }
                    else if (spell.spellType == SpellType.Circular)
                    {
                        if (myHero.ServerPosition.LSTo2D().InSkillShot(spell, myHero.BoundingRadius))
                        {

                        }
                    }
                }
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (fireDummySpellMenu["FireDummySpell"].Cast<KeyBind>().CurrentValue == true)
            {
                float interval = fireDummySpellMenu["SpellInterval"].Cast<Slider>().CurrentValue;

                if (EvadeUtils.TickCount - lastSpellFireTime > interval)
                {
                    var charName = selectSpellMenu["DummySpellHero"].Cast<ComboBox>().SelectedText;
                    var spellName = selectSpellMenu["DummySpellList"].Cast<ComboBox>().SelectedText;
                    var spellData = spellCache[charName][spellName];

                    if (ObjectCache.menuCache.cache[spellName + "DodgeSpell"] == null)
                    {
                        SpellDetector.LoadDummySpell(spellData);
                    }

                    SpellDetector.CreateSpellData(myHero, spellStartPosition, spellEndPostion, spellData);
                    lastSpellFireTime = EvadeUtils.TickCount;
                }
            }
        }

        private void OnSpellEndChange(ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
        {
            //e.Process = false;

            spellEndPostion = myHero.ServerPosition;
            Draw.RenderObjects.Add(new Draw.RenderCircle(spellEndPostion.LSTo2D(), 1000, Color.Red, 100, 20));
        }

        private void OnSpellStartChange(ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
        {
            //e.Process = false;

            spellStartPosition = myHero.ServerPosition;
            Draw.RenderObjects.Add(new Draw.RenderCircle(spellStartPosition.LSTo2D(), 1000, Color.Red, 100, 20));
        }

        private void LoadSpellDictionary()
        {
            foreach (var spell in SpellDatabase.Spells)
            {
                if (spellCache.ContainsKey(spell.charName))
                {
                    var spellList = spellCache[spell.charName];
                    if (spellList != null && !spellList.ContainsKey(spell.spellName))
                    {
                        spellList.Add(spell.spellName, spell);
                    }
                }
                else
                {
                    spellCache.Add(spell.charName, new Dictionary<string, SpellData>());
                    var spellList = spellCache[spell.charName];
                    if (spellList != null && !spellList.ContainsKey(spell.spellName))
                    {
                        spellList.Add(spell.spellName, spell);
                    }
                }
            }

            //selectSpellMenu.Add("DummySpellDescription", "    -- Select A Dummy Spell To Fire --    "));

            var heroList = spellCache.Keys.ToArray();
            selectSpellMenu.Add("DummySpellHero", new ComboBox("Hero", 0, heroList));

            var selectedHeroStr = selectSpellMenu["DummySpellHero"].Cast<ComboBox>().SelectedText;
            var selectedHero = spellCache[selectedHeroStr];
            var selectedHeroList = selectedHero.Keys.ToArray();

            selectSpellMenu.Add("DummySpellList", new ComboBox("Spell", 0, selectedHeroList));
            //selectSpellMenu["DummySpellHero"].Cast<ComboBox>().OnValueChange += OnSpellHeroChange;
        }

        private void OnSpellHeroChange(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
        {
        }
    }
}