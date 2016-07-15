using System;
using System.Drawing;
using DZAwarenessAIO.Utility.Extensions;
using DZAwarenessAIO.Utility.Logs;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using PortAIO.Properties;
using EloBuddy;

 namespace DZAwarenessAIO.Utility.HudUtility.HudElements
{
    /// <summary>
    /// The Hud Button class
    /// </summary>
    class HudButton : HudElement
    {
        /// <summary>
        /// The position of the button
        /// </summary>
        public Vector2 Position => new Vector2(Parent.Position.X + this.X, Parent.Position.Y + this.Y);

        /// <summary>
        /// The x position
        /// </summary>
        public int X;

        /// <summary>
        /// The y position.
        /// </summary>
        public int Y;

        /// <summary>
        /// Gets or sets the parent Hud Panel.
        /// </summary>
        /// <value>
        /// The parent Hud Panel.
        /// </value>
        public HudPanel Parent { get; set; }

        /// <summary>
        /// The OnButtonClick delegate
        /// </summary>
        public delegate void OnButtonClickDelegate();

        /// <summary>
        /// Gets or sets the delegate on button click.
        /// </summary>
        /// <value>
        /// The delegate on button click.
        /// </value>
        public OnButtonClickDelegate OnButtonClick { get; set; }

        /// <summary>
        /// Gets or sets the button sprite.
        /// </summary>
        /// <value>
        /// The button sprite.
        /// </value>
        public Render.Sprite ButtonSprite { get; set; }

        /// <summary>
        /// Gets or sets the button bitmap.
        /// </summary>
        /// <value>
        /// The button bitmap.
        /// </value>
        public Bitmap ButtonBitmap { get; set; }

        public string ButtonText { get; set; }

        public Render.Text ButtonTextObject { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HudButton"/> class.
        /// </summary>
        /// <param name="text">The text of the button</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="Parent">The parent.</param>
        /// <param name="bitmap">The Bitmap for the button</param>
        public HudButton(string text, int x, int y, HudPanel Parent, Bitmap bitmap = null)
        {
            this.ButtonText = text;
            this.Parent = Parent;
            this.ButtonBitmap = bitmap ?? Resources.Button;
            this.X = x;
            this.Y = y; 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HudButton"/> class.
        /// </summary>
        /// <param name="text">The text of the button</param>
        /// <param name="position">The position.</param>
        /// <param name="Parent">The parent.</param>
        /// <param name="bitmap">The button bitmap</param>
        public HudButton(string text, Vector2 position, HudPanel Parent, Bitmap bitmap = null)
        {
            this.ButtonText = text;
            this.Parent = Parent;
            this.ButtonBitmap = bitmap??Resources.Button;
            this.X = (int) position.X;
            this.Y = (int)position.Y;
        }

        /// <summary>
        /// Called when the instance is loaded.
        /// </summary>
        public override void OnLoad()
        {
            Game.OnWndProc += OnWndProc;
        }

        /// <summary>
        /// Gets the parent panel.
        /// </summary>
        /// <returns></returns>
        public HudPanel GetParent()
        {
            return Parent;
        }

        /// <summary>
        /// Initializes the drawings.
        /// </summary>
        public override void InitDrawings()
        {
            if (this.ButtonBitmap != null)
            {
                this.ButtonSprite = new Render.Sprite(this.ButtonBitmap, this.Position)
                {
                    PositionUpdate = () => Position,
                    VisibleCondition =
                        delegate
                        {
                            return HudVariables.ShouldBeVisible && HudVariables.CurrentStatus == SpriteStatus.Expanded;
                        }
                };
                ButtonSprite.Add(3);

                if (!string.IsNullOrEmpty(this.ButtonText))
                {
                    this.ButtonTextObject = new Render.Text(this.ButtonText,
                        new Vector2(Position.X + this.ButtonSprite.Width/2f - Helper.GetSize(this.ButtonText, 17) , Position.Y + this.ButtonSprite.Height/2f),
                        17, SharpDX.Color.White)
                    {
                        PositionUpdate = () => new Vector2(Position.X + 2, Position.Y + this.ButtonSprite.Height / 2f - 8),
                        VisibleCondition = delegate
                        {
                            return HudVariables.ShouldBeVisible && HudVariables.CurrentStatus == SpriteStatus.Expanded;
                        }
                    };

                    this.ButtonTextObject.Add(4);
                }
            }
            else
            {
                LogHelper.AddToLog(new LogItem("Hud_Button","Failed to init: Bitmap is null", LogSeverity.Warning));
            }
        }

        public override void RemoveDrawings()
        {
            //
        }

        /// <summary>
        /// Raises the events.
        /// </summary>
        public override void RaiseEvents()
        {
            OnButtonClick?.Invoke();
        }

        /// <summary>
        /// Raises the <see cref="E:WndProc" /> event.
        /// </summary>
        /// <param name="args">The <see cref="WndEventArgs"/> instance containing the event data.</param>
        private void OnWndProc(WndEventArgs args)
        {
            if (args.Msg == (uint) WindowsMessages.WM_LBUTTONUP)
            {
                if (Helper.IsInside(Utils.GetCursorPos(), (int) this.Position.X, (int) this.Position.Y, 80, 20))
                {
                    RaiseEvents();
                }
            }
        }
    }
}
