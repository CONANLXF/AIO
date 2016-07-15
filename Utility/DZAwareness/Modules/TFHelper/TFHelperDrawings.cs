using System;
using System.Runtime.Remoting.Messaging;
using DZAwarenessAIO.Utility.HudUtility;
using DZAwarenessAIO.Utility.HudUtility.HudElements;
using DZAwarenessAIO.Utility.Logs;
using DZAwarenessAIO.Utility.MenuUtility;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using PortAIO.Properties;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;
using EloBuddy;

using TargetSelector = PortAIO.TSManager; namespace DZAwarenessAIO.Modules.TFHelper
{
    class TFHelperDrawings
    {
        /// <summary>
        /// The previous state of the text
        /// </summary>
        private static string PrevState = "";

        /// <summary>
        /// The last tick the calculation was made
        /// </summary>
        private static float LastTick = 0f;

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

        /// <summary>
        /// Called when the module is loaded.
        /// </summary>
        public static void OnLoad()
        {
            try
            {
                var hudPanel = new HudPanel("Teamfight Helper", 10, 10, 250, 200);
                {
                    TFHelperVariables.AllyBarSprite = new Render.Sprite(Resources.AllyTeamStrength,
                        new Vector2(hudPanel.Position.X + 3, hudPanel.Position.Y + 15))
                    {
                        PositionUpdate = () => new Vector2(hudPanel.Position.X + 5, hudPanel.Position.Y + 18),
                        Visible = true,
                        Scale = new Vector2(0.95f, 0.95f),
                        VisibleCondition = delegate
                        {
                            return getCheckBoxItem(TFHelperBase.moduleMenu, "dz191.dza.tf.enabled") && HudVariables.ShouldBeVisible && HudVariables.CurrentStatus == SpriteStatus.Expanded;
                        }
                    };

                    TFHelperVariables.EnemyBarSprite = new Render.Sprite(Resources.EnemyTeamStength,
                        new Vector2(hudPanel.Position.X + 3, hudPanel.Position.Y + 36))
                    {
                        PositionUpdate = () => new Vector2(hudPanel.Position.X + 5, hudPanel.Position.Y + 48),
                        Visible = true,
                        Scale = new Vector2(0.95f, 0.95f),
                        VisibleCondition = delegate
                        {
                            return getCheckBoxItem(TFHelperBase.moduleMenu, "dz191.dza.tf.enabled") && HudVariables.ShouldBeVisible && HudVariables.CurrentStatus == SpriteStatus.Expanded;
                        }
                    };

                    TFHelperVariables.AllyStrengthText = new Render.Text(
                        "",
                        new Vector2(0, 0), 19, Color.White)
                    {
                        Centered = true,
                        PositionUpdate =
                            () =>
                                new Vector2(
                                    TFHelperVariables.AllyBarSprite.Position.X +
                                    TFHelperVariables.AllyBarSprite.Width/2f,
                                    TFHelperVariables.AllyBarSprite.Position.Y +
                                    TFHelperVariables.AllyBarSprite.Height/2f),
                        TextUpdate = () => $"{TFHelperCalculator.GetAllyStrength()*100} %",
                        VisibleCondition = delegate
                        {
                            return getCheckBoxItem(TFHelperBase.moduleMenu, "dz191.dza.tf.enabled") && HudVariables.ShouldBeVisible && HudVariables.CurrentStatus == SpriteStatus.Expanded;
                        }
                    };

                    TFHelperVariables.EnemyStrengthText = new Render.Text(
                        "",
                        new Vector2(0, 0), 19, Color.White)
                    {
                        Centered = true,
                        PositionUpdate =
                            () =>
                                new Vector2(
                                    TFHelperVariables.EnemyBarSprite.Position.X +
                                    TFHelperVariables.EnemyBarSprite.Width/2f,
                                    TFHelperVariables.EnemyBarSprite.Position.Y +
                                    TFHelperVariables.EnemyBarSprite.Height/2f),
                        TextUpdate = () => $"{TFHelperCalculator.GetEnemyStrength()*100} %",
                        VisibleCondition = delegate
                        {
                            return getCheckBoxItem(TFHelperBase.moduleMenu, "dz191.dza.tf.enabled") && HudVariables.ShouldBeVisible && HudVariables.CurrentStatus == SpriteStatus.Expanded;
                        }
                    };

                    TFHelperVariables.TeamsVSText = new Render.Text("", new Vector2(0, 0), 19, Color.White)
                    {
                        Centered = true,
                        TextUpdate = () => TFHelperCalculator.GetText(),
                        PositionUpdate =
                            () =>
                                new Vector2(hudPanel.Position.X + hudPanel.Width/2f,
                                    hudPanel.Position.Y + hudPanel.Height/2f - 12),
                        VisibleCondition = delegate
                        {
                            return getCheckBoxItem(TFHelperBase.moduleMenu, "dz191.dza.tf.enabled") && HudVariables.ShouldBeVisible && HudVariables.CurrentStatus == SpriteStatus.Expanded;
                        }
                    };

                    TFHelperVariables.AllyBarSprite.Add(3);
                    TFHelperVariables.EnemyBarSprite.Add(3);
                    TFHelperVariables.AllyStrengthText.Add(4);
                    TFHelperVariables.EnemyStrengthText.Add(4);
                    TFHelperVariables.TeamsVSText.Add(4);

                    Game.OnUpdate += OnUpdate;
                }
            }
            catch (Exception e)
            {
                LogHelper.AddToLog(new LogItem("TFHelper_Drawings", e, LogSeverity.Error));
            }

        }

        /// <summary>
        /// Raises the <see cref="E:Update" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs"/> instance containing the event data.</param>
        private static void OnUpdate(EventArgs args)
        {
            try
            {
                if ((Environment.TickCount - LastTick < 2000) || !(HudVariables.ShouldBeVisible && HudVariables.CurrentStatus == SpriteStatus.Expanded) || !getCheckBoxItem(TFHelperBase.moduleMenu, "dz191.dza.tf.enabled"))
                {
                    return;
                }

                LastTick = Environment.TickCount;
                var currentState = TFHelperCalculator.GetText();
                
                if (currentState != PrevState)
                {
                    PrevState = currentState;
                    var allyStrength = TFHelperCalculator.GetAllyStrength();
                    var enemyStrength = TFHelperCalculator.GetEnemyStrength();

                    TFHelperVariables.AllyBarSprite.Crop(
                        0, 0, (int) (Resources.AllyTeamStrength.Width*allyStrength),
                        (int) TFHelperVariables.AllyBarSprite.Height);

                    TFHelperVariables.EnemyBarSprite.Crop(
                        0, 0, (int) (Resources.EnemyTeamStength.Width*enemyStrength),
                        (int) TFHelperVariables.EnemyBarSprite.Height);
                }
            }
            catch (Exception e)
            {
                LogHelper.AddToLog(new LogItem("TFHelper_Drawings", e, LogSeverity.Severe));
            }
        }

        /// <summary>
        /// Rounds the specified number.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns></returns>
        private static float Round(float number)
        {
            return (float) Math.Ceiling(number + 0.5);
        }
    }
}
