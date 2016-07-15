using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DZAwarenessAIO.Utility.HudUtility.HudElements;
using DZAwarenessAIO.Utility.MenuUtility;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using PortAIO.Properties;

using TargetSelector = PortAIO.TSManager; namespace DZAwarenessAIO.Utility.HudUtility
{
    /// <summary>
    /// The Hud Variables class
    /// </summary>
    class HudVariables
    {

        /// <summary>
        /// The list containing the hud elements
        /// </summary>
        public static List<HudElement> HudElements = new List<HudElement>(); 

        /// <summary>
        /// Gets or sets the hud sprite.
        /// </summary>
        /// <value>
        /// The hud sprite.
        /// </value>
        public static Render.Sprite HudSprite { get; set; }

        /// <summary>
        /// Gets or sets the hud text.
        /// </summary>
        /// <value>
        /// The hud text.
        /// </value>
        public static Render.Text HudText { get; set; }
        /// <summary>
        /// Gets or sets the expand button sprite.
        /// </summary>
        /// <value>
        /// The expand button.
        /// </value>
        public static Render.Sprite ExpandShrinkButton { get; set; }

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
        /// Gets the current position of the HUD.
        /// </summary>
        /// <value>
        /// The current position.
        /// </value>
        public static Vector2 CurrentPosition => IsDragging ? DraggingPosition : new Vector2(
            getSliderItem(HudDisplay.moduleMenu, "dz191.dza.hud.x"),
            getSliderItem(HudDisplay.moduleMenu, "dz191.dza.hud.y")
            );

        /// <summary>
        /// The dragging position of the hud
        /// </summary>
        public static Vector2 DraggingPosition = new Vector2();

        /// <summary>
        /// The HUD sprite width
        /// </summary>
        public static readonly float SpriteWidth = Resources.TFHelperBG.Width;

        /// <summary>
        /// The HUD sprite height
        /// </summary>
        public static readonly float SpriteHeight = Resources.TFHelperBG.Height;

        /// <summary>
        /// Indicates whether or not the hud is being dragged
        /// </summary>
        public static bool IsDragging = false;

        /// <summary>
        /// The cropped height of the sprite
        /// </summary>
        public const int CroppedHeight = 80;

        /// <summary>
        /// Gets a value indicating whether the hud should be visible
        /// </summary>
        /// <value>
        ///   <c>true</c> if the hud should be visible; otherwise, <c>false</c>.
        /// </value>
        public static bool ShouldBeVisible => getCheckBoxItem(HudDisplay.moduleMenu, "dz191.dza.hud.show");

        /// <summary>
        /// The current status of the hud
        /// </summary>
        public static SpriteStatus CurrentStatus = SpriteStatus.Shrinked;

    }
}
