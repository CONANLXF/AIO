using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy.SDK.Menu;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;
using static EloBuddy.SDK.Spell;

 namespace YasuoSharpV2
{
    internal class YasuoSharp
    {


        public const string CharName = "Yasuo";


        public static Menu comboMenu, smartR, smartW, flee, lasthit, laneclear, harass, drawings, extra, debug, Config;

        public static string lastSpell = "";

        public static int afterDash = 0;

        public static bool canSave = true;
        public static bool canExport = true;
        public static bool canDelete = true;

        public static bool wasStream = false;


        public static List<Skillshot> DetectedSkillshots = new List<Skillshot>();

        public YasuoSharp()
        {

            // map = new Map();
            /* CallBAcks */
            CustomEvents.Game.OnGameLoad += onLoad;

        }

        private static void onLoad(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName != CharName)
                return;

            Yasuo.setSkillShots();
            Yasuo.setDashes();
            Yasuo.point1 = Yasuo.Player.Position;
            Chat.Print("YasuoSharpV2 by DeTuKs");

            Console.WriteLine("YasuoSharpV2 by DeTuKs");

            try
            {

                Config = MainMenu.AddMenu("YasuoSharp", "YasuoASHARP");

                //Combo
                comboMenu = Config.AddSubMenu("Combo Sharp", "combo");
                comboMenu.Add("comboItems", new CheckBox("Use Items"));
                comboMenu.Add("useEWall", new CheckBox("E behind Wall to safe"));

                //SmartR
                smartR = Config.AddSubMenu("Smart R");
                smartR.Add("smartR", new CheckBox("Use Smart R?"));
                smartR.AddGroupLabel("Settings : ");
                smartR.Add("useRHitTime", new CheckBox("Use R Delay"));
                smartR.Add("useRHit", new Slider("R if hit : ", 3, 1, 5));
                smartR.Add("useRHP", new Slider("R if Enemy HP less than : ", 25, 0, 100));

                //Flee away
                flee = Config.AddSubMenu("Flee");
                //flee.Add("flee", new KeyBind("E away", false, KeyBind.BindTypes.HoldActive, 'Z'));
                flee.Add("fleeStack", new CheckBox("Stack Q while flee"));

                //LastHit
                lasthit = Config.AddSubMenu("LastHit Sharp", "lHit");
                lasthit.Add("useQlh", new CheckBox("Use Q"));
                lasthit.Add("useElh", new CheckBox("Use E"));

                //LaneClear
                laneclear = Config.AddSubMenu("LaneClear Sharp", "lClear");
                laneclear.Add("useQlc", new CheckBox("Use Q"));
                laneclear.Add("useEmpQHit", new Slider("Emp Q Min hit", 3, 1, 6));
                laneclear.Add("useElc", new CheckBox("Use E"));

                //Harass
                harass = Config.AddSubMenu("Harass Sharp", "harass");
                harass.Add("harassTower", new CheckBox("Harass under tower", false));
                harass.Add("harassOn", new CheckBox("Harass enemies"));
                harass.Add("harQ3Only", new CheckBox("Use only Q3", false));

                //Drawings
                drawings = Config.AddSubMenu("Drawing Sharp", "drawing");
                drawings.Add("disDraw", new CheckBox("Dissabel drawing", false));
                drawings.Add("drawQ", new CheckBox("Draw Q range"));
                drawings.Add("drawE", new CheckBox("Draw E range"));
                drawings.Add("drawR", new CheckBox("Draw R range"));
                drawings.Add("drawWJ", new CheckBox("Draw Wall Jumps"));

                //Extra
                extra = Config.AddSubMenu("Extra Sharp", "extra");
                extra.Add("djTur", new CheckBox("Dont Jump turrets"));
                extra.Add("autoLevel", new CheckBox("Auto Level"));
                extra.Add("levUpSeq", new ComboBox("Sequence : ", 0, "Q E W Q start", "Q E Q W start"));

                //SmartW
                smartW = Config.AddSubMenu("Wall Usage", "aShots");
                smartW.Add("smartW", new CheckBox("Smart WW"));

                //Debug
                debug = Config.AddSubMenu("Debug", "debug");
                debug.Add("WWLast", new KeyBind("Print last ww blocked", false, KeyBind.BindTypes.HoldActive, 'T'));
                debug.Add("saveDash", new KeyBind("saveDashd", false, KeyBind.BindTypes.HoldActive, 'O'));
                debug.Add("exportDash", new KeyBind("export dashes", false, KeyBind.BindTypes.HoldActive, 'P'));
                debug.Add("deleteDash", new KeyBind("deleteLastDash", false, KeyBind.BindTypes.HoldActive, 'I'));

                Valvrave_Sharp.Program.MainA();

                Drawing.OnDraw += onDraw;
                Game.OnUpdate += OnGameUpdate;

                GameObject.OnCreate += OnCreateObject;
                GameObject.OnDelete += OnDeleteObject;
                Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
                Spellbook.OnStopCast += onStopCast;
                CustomEvents.Unit.OnLevelUp += OnLevelUp;

                Game.OnSendPacket += OnGameSendPacket;
                Game.OnProcessPacket += OnGameProcessPacket;
            }
            catch
            {
                Chat.Print("Oops. Something went wrong with Yasuo - Sharpino");
            }

        }
        
        private static void OnGameUpdate(EventArgs args)
        {
            try
            {
                Yasuo.Q.SetSkillshot(Yasuo.getNewQSpeed(), 50f, float.MaxValue, false, SkillshotType.SkillshotLine);

                if (Yasuo.startDash + 470000 / ((700 + Yasuo.Player.MoveSpeed)) < Environment.TickCount && Yasuo.isDashigPro)
                {
                    Yasuo.isDashigPro = false;
                }

                //updateSkillshots();
                //Remove the detected skillshots that have expired.

                AIHeroClient target = TargetSelector.GetTarget((Yasuo.E.IsReady()) ? 1500 : 475, DamageType.Physical);
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    Yasuo.doCombo(target);
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
                {
                    Yasuo.doLastHit(target);
                    Yasuo.useQSmart(target);
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                {
                    Yasuo.doLastHit(target);
                    Yasuo.useQSmart(target);
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                {
                    Yasuo.doLaneClear(target);
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
                {
                    Yasuo.fleeToMouse();
                    Yasuo.stackQ();
                }

                if (debug["saveDash"].Cast<KeyBind>().CurrentValue && canSave)
                {
                    Yasuo.saveLastDash();
                    canSave = false;
                }
                else
                {
                    canSave = true;
                }

                if (debug["deleteDash"].Cast<KeyBind>().CurrentValue && canDelete)
                {
                    if (Yasuo.dashes.Count > 0)
                        Yasuo.dashes.RemoveAt(Yasuo.dashes.Count - 1);
                    canDelete = false;
                }
                else
                {
                    canDelete = true;
                }
                if (debug["exportDash"].Cast<KeyBind>().CurrentValue && canExport)
                {
                    using (var file = new System.IO.StreamWriter(@"C:\YasuoDashes.txt"))
                    {

                        foreach (var dash in Yasuo.dashes)
                        {
                            string dashS = "dashes.Add(new YasDash(new Vector3(" +
                                           dash.from.X.ToString("0.00").Replace(',', '.') + "f," +
                                           dash.from.Y.ToString("0.00").Replace(',', '.') + "f," +
                                           dash.from.Z.ToString("0.00").Replace(',', '.') +
                                           "f),new Vector3(" + dash.to.X.ToString("0.00").Replace(',', '.') + "f," +
                                           dash.to.Y.ToString("0.00").Replace(',', '.') + "f," +
                                           dash.to.Z.ToString("0.00").Replace(',', '.') + "f)));";
                            //new YasDash(new Vector3(X,Y,Z),new Vector3(X,Y,Z))

                            file.WriteLine(dashS);
                        }
                        file.Close();
                    }

                    canExport = false;
                }
                else
                {
                    canExport = true;
                }

                if (debug["WWLast"].Cast<KeyBind>().CurrentValue)
                {
                    Console.WriteLine("Last WW skill blocked: " + lastSpell);
                    Chat.Print("Last WW skill blocked: " + lastSpell);
                }

                if (harass["harassOn"].Cast<CheckBox>().CurrentValue && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None))
                {
                    if (target != null)
                        Yasuo.useQSmart(target, harass["harQ3Only"].Cast<CheckBox>().CurrentValue);
                }

                if (smartR["smartR"].Cast<CheckBox>().CurrentValue && Yasuo.R.IsReady())
                    Yasuo.useRSmart();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void onDraw(EventArgs args)
        {
            if (drawings["disDraw"].Cast<CheckBox>().CurrentValue)
                return;


            foreach (Obj_AI_Base jun in MinionManager.GetMinions(Yasuo.Player.ServerPosition, 700, MinionTypes.All, MinionTeam.Neutral))
            {
                Drawing.DrawCircle(jun.Position, 70, Color.Green);
                Vector2 posAfterE = Yasuo.Player.ServerPosition.LSTo2D() + (Vector2.Normalize(jun.ServerPosition.LSTo2D() - Yasuo.Player.ServerPosition.LSTo2D()) * 475);
                // Vector2 posAfterE = Yasuo.Player.Position.LSTo2D().Extend(jun.Position.LSTo2D(), 475);//jun.ServerPosition.LSTo2D().Extend() + (Vector2.Normalize(Yasuo.Player.Position.LSTo2D() - jun.ServerPosition.LSTo2D()) * 475);
                Drawing.DrawCircle(posAfterE.To3D(), 50, Color.Violet);
                Vector3 posAfterDash = Yasuo.Player.GetPath(posAfterE.To3D()).Last();
                Drawing.DrawCircle(posAfterDash, 50, Color.DarkRed);

            }

            if (drawings["drawQ"].Cast<CheckBox>().CurrentValue)
                LeagueSharp.Common.Utility.DrawCircle(Yasuo.Player.Position, 475, (Yasuo.isDashigPro) ? Color.Red : Color.Blue, 10, 10);
            if (drawings["drawR"].Cast<CheckBox>().CurrentValue)
                LeagueSharp.Common.Utility.DrawCircle(Yasuo.Player.Position, 1200, Color.Blue);

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee) && drawings["drawWJ"].Cast<CheckBox>().CurrentValue)
            {
                LeagueSharp.Common.Utility.DrawCircle(Game.CursorPos, 350, Color.Cyan);

                LeagueSharp.Common.Utility.DrawCircle(Yasuo.lastDash.from, 60, Color.BlueViolet);
                LeagueSharp.Common.Utility.DrawCircle(Yasuo.lastDash.to, 60, Color.BlueViolet);

                foreach (Yasuo.YasDash dash in Yasuo.dashes)
                {
                    if (dash.from.LSDistance(Game.CursorPos) < 1200)
                    {
                        var SA = Drawing.WorldToScreen(dash.from);
                        var SB = Drawing.WorldToScreen(dash.to);
                        Drawing.DrawLine(SA.X, SA.Y, SB.X, SB.Y, 3, Color.Green);
                    }
                }

            }


            /*   if ((int)NavMesh.GetCollisionFlags(Game.CursorPos) == 2 || (int)NavMesh.GetCollisionFlags(Game.CursorPos) == 64)
                Drawing.DrawCircle(Game.CursorPos, 70, Color.Green);
            if (map.isWall(Game.CursorPos.LSTo2D()))
                Drawing.DrawCircle(Game.CursorPos, 100, Color.Red);

            foreach (Polygon pol in map.poligs)
            {
                pol.Draw(Color.BlueViolet, 3);
            }

            foreach(Obj_AI_Base jun in MinionManager.GetMinions(Yasuo.Player.ServerPosition,700,MinionTypes.All,MinionTeam.Neutral))
            {
                Drawing.DrawCircle(jun.Position, 70, Color.Green);
                 SharpDX.Vector2 proj = map.getClosestPolygonProj(jun.ServerPosition.LSTo2D());
                 SharpDX.Vector2 posAfterE = jun.ServerPosition.LSTo2D() + (SharpDX.Vector2.Normalize(proj - jun.ServerPosition.LSTo2D() ) * 475);
                 Drawing.DrawCircle(posAfterE.To3D(), 50, Color.Violet);
            }

            foreach (MissileClient mis in skillShots)
            {
                Drawing.DrawCircle(mis.Position, 47, Color.Orange);
                Drawing.DrawCircle(mis.EndPosition, 100, Color.BlueViolet);
               Drawing.DrawCircle(mis.SpellCaster.Position, Yasuo.Player.BoundingRadius + mis.SData.LineWidth, Color.DarkSalmon);
                Drawing.DrawCircle(mis.StartPosition, 70, Color.Green);
            }*/

        }

        private static void OnCreateObject(GameObject sender, EventArgs args)
        {
#pragma warning disable 0618
            //wall
            if (sender.IsValid<Obj_SpellLineMissile>())
            {
                if (sender is Obj_SpellLineMissile)
                {
                    Obj_SpellLineMissile missle = (Obj_SpellLineMissile)sender;
                    if (missle.SData.Name == "yasuowmovingwallmisl")
                    {
                        Yasuo.wall.setL(missle);
                    }

                    if (missle.SData.Name == "yasuowmovingwallmisr")
                    {
                        Yasuo.wall.setR(missle);
                    }
                }
            }
        }

        private static void OnDeleteObject(GameObject sender, EventArgs args)
        {
            /* int i = 0;
             foreach (var lho in skillShots)
             {
                 if (lho.NetworkId == sender.NetworkId)
                 {
                     skillShots.RemoveAt(i);
                     return;
                 }
                 i++;
             }*/
        }


        private static void onStopCast(Obj_AI_Base obj, SpellbookStopCastEventArgs args)
        {
            if (obj.IsMe)
            {
                if (obj.IsValid && args.DestroyMissile && args.StopAnimation)
                {
                    Yasuo.isDashigPro = false;
                }
            }
        }

        internal static void OnProcessSpell(Obj_AI_Base obj, GameObjectProcessSpellCastEventArgs arg)
        {
            if (obj.IsMe)
            {
                if (arg.SData.Name == "YasuoDashWrapper")//start dash
                {
                    Console.WriteLine("--- DAhs started---");
                    Yasuo.lastDash.from = Yasuo.Player.Position;
                    Yasuo.isDashigPro = true;
                    Yasuo.castFrom = Yasuo.Player.Position;
                    Yasuo.startDash = Environment.TickCount;
                }
            }
        }

        internal static void OnLevelUp(Obj_AI_Base sender, LeagueSharp.Common.CustomEvents.Unit.OnLevelUpEventArgs args)
        {
            if (sender.NetworkId == Yasuo.Player.NetworkId)
            {
                if (!extra["autoLevel"].Cast<CheckBox>().CurrentValue)
                    return;
                if (extra["levUpSeq"].Cast<ComboBox>().CurrentValue == 0)
                    Yasuo.sBook.LevelUpSpell(Yasuo.levelUpSeq[args.NewLevel - 1].Slot);
                else if (extra["levUpSeq"].Cast<ComboBox>().CurrentValue == 1)
                    Yasuo.sBook.LevelUpSpell(Yasuo.levelUpSeq2[args.NewLevel - 1].Slot);
            }
        }



        private static void OnGameProcessPacket(GamePacketEventArgs args)
        {//28 16 176 ??184
            if (args.PacketData[0] == 41)//135no 100no 183no 34no 101 133 56yesss? 127 41yess
            {
                GamePacket gp = new GamePacket(args.PacketData);
                //Console.WriteLine(Encoding.UTF8.GetString(args.PacketData, 0, args.PacketData.Length));
                gp.Position = 1;
                if (gp.ReadInteger() == Yasuo.Player.NetworkId /*&&  Encoding.UTF8.GetString(args.PacketData, 0, args.PacketData.Length).Contains("Spell3")*/)
                {
                    Console.WriteLine("----");
                    Yasuo.lastDash.to = Yasuo.Player.Position;
                    Yasuo.isDashigPro = false;
                    Yasuo.time = Game.Time - Yasuo.startDash;
                }
                /* for (int i = 1; i < gp.Size() - 4; i++)
                 {
                     gp.Position = i;
                     if (gp.ReadInteger() == Yasuo.Player.NetworkId)
                     {
                         Console.WriteLine("Found: "+i);
                     }
                 }

                 Console.WriteLine("End dash");
                 Yasuo.Q.Cast(Yasuo.Player.Position);*/
            }

            /*if (args.PacketData[0] == 176) //135no 100no 183no 34no 101 133 56yesss? 127 41yess
            {
                GamePacket gp = new GamePacket(args.PacketData);
                //Console.WriteLine(Encoding.UTF8.GetString(args.PacketData, 0, args.PacketData.Length));
                gp.Position = 1;
                if (gp.ReadInteger() == Yasuo.Player.NetworkId)
                {
                    Console.WriteLine("--- DAhs started Packets---");
                    Yasuo.lastDash.from = Yasuo.Player.Position;
                    Yasuo.isDashigPro = true;
                    Yasuo.castFrom = Yasuo.Player.Position;
                    Yasuo.startDash = Game.Time;
                }
            }*/
        }

        private static void OnGameSendPacket(GamePacketEventArgs args)
        {
            /*if (args.PacketData[0] == 154) //135no 100no 183no 34no 101 133 56yesss? 127 41yess
            {
                var spell = Packet.C2S.Cast.Decoded(args.PacketData);
                if (spell.Slot == Yasuo.E.Slot)
                {
                    Console.WriteLine("--- DAhs started Packets---");
                    Yasuo.lastDash.from = Yasuo.Player.Position;
                    Yasuo.isDashigPro = true;
                    Yasuo.castFrom = Yasuo.Player.Position;
                    Yasuo.startDash = Game.Time;
                }
            }*/
        }
    }
}
