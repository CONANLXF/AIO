using System;
using DZAwarenessAIO.Utility.Extensions;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;
using EloBuddy;

using TargetSelector = PortAIO.TSManager; namespace DZAwarenessAIO.Utility.HudUtility.HudElements
{
    /// <summary>
    /// The Parent Hud Panel class
    /// </summary>
    class HudPanel : HudElement
    {
        /// <summary>
        /// Gets or sets the position of the Panel.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public Vector2 Position
            =>
                IsDragging
                    ? DraggingPosition
                    : new Vector2(
                        HudVariables.CurrentPosition.X + this.X,
                        HudVariables.CurrentPosition.Y + HudVariables.CroppedHeight + this.Y);

        public int X;

        public int Y;

        public int savedX;

        public int savedY;
                
        private Vector2 InitialPosition = new Vector2();

        /// <summary>
        /// Gets or sets the name of the panel.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the panel border rectangle.
        /// </summary>
        /// <value>
        /// The rectangle.
        /// </value>
        public Rectangle_Ex Rectangle { get; set; }

        /// <summary>
        /// Gets or sets the panel header render object.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public Render.Text Text { get; set; }

        /// <summary>
        /// Tells whether or not the panel is being dragged
        /// </summary>
        public bool IsDragging = false;


        /// <summary>
        /// The dragging position
        /// </summary>
        public Vector2 DraggingPosition = new Vector2();
        /// <summary>
        /// Initializes a new instance of the <see cref="HudPanel"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public HudPanel(string name, int x, int y, int width, int height)
        {
            this.Name = name;
            this.Width = width;
            this.Height = height;
            this.X = (int) (x);
            this.Y = (int) (y);
            this.savedX = (int) x;
            this.savedY = (int) y;

            InitialPosition = new Vector2(
                        HudVariables.CurrentPosition.X + x,
                        HudVariables.CurrentPosition.Y + HudVariables.CroppedHeight + y);
            HudVariables.HudElements.Add(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HudPanel"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="Position">The position.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public HudPanel(string name, Vector2 Position, int width, int height)
        {
            this.Name = name;
            this.Width = width;
            this.Height = height;
            this.X = (int) (Position.X);
            this.Y = (int) (Position.Y);
            this.savedX = (int) (Position.X);
            this.savedY = (int) (Position.Y);
            InitialPosition = new Vector2(
                        HudVariables.CurrentPosition.X + Position.X,
                        HudVariables.CurrentPosition.Y + HudVariables.CroppedHeight +  Position.Y);
            HudVariables.HudElements.Add(this);
        }

        /// <summary>
        /// Called when panel is loaded.
        /// </summary>
        public override void OnLoad()
        {
            Game.OnUpdate += OnUpdate;
            Game.OnWndProc += OnWndProc;
        }

        /// <summary>
        /// Raises the <see cref="E:Update"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnUpdate(System.EventArgs args)
        {
            if (this.Rectangle != null)
            {
                this.Rectangle.X = (int) this.Position.X;
                this.Rectangle.Y = (int) this.Position.Y;
            }

            if (this.Text != null)
            {
                this.Text.X = (int) (this.Position.X + this.Width / 2f);
                this.Text.Y = (int) this.Position.Y + 9;
            }
        }
        
        private void OnWndProc(WndEventArgs args)
        {
            //Fuck this shit.
            return;
            /*
            if (HudVariables.CurrentStatus != SpriteStatus.Expanded || !HudVariables.ShouldBeVisible || HudVariables.IsDragging)
            {
                return;
            }
                                    //Console.WriteLine("B4 : " + this.X + " "+ this.Y);
            if (this.IsDragging)
            {
                this.DraggingPosition.X = (int)(Utils.GetCursorPos().X - XDistanceFromEdge);
                this.DraggingPosition.Y = (int)(Utils.GetCursorPos().Y - YDistanceFromEdge);
                this.X = (int) this.DraggingPosition.X;
                this.Y = (int) this.DraggingPosition.Y;
            }

            if (Helper.IsInside(Utils.GetCursorPos(), (int) this.Position.X, (int) this.Position.Y, this.Width, this.Height) && args.Msg == (uint)WindowsMessages.WM_LBUTTONDOWN)
            {
                if (!this.IsDragging)
                {
                    if (InitialDragPoint == new Vector2())
                    {
                        InitialDragPoint = new Vector2(
                        HudVariables.CurrentPosition.X + this.X,
                        HudVariables.CurrentPosition.Y + HudVariables.CroppedHeight + this.Y);

                        XDistanceFromEdge = Math.Abs(InitialDragPoint.X - Utils.GetCursorPos().X);
                        YDistanceFromEdge = Math.Abs(InitialDragPoint.Y - Utils.GetCursorPos().Y);

                        this.DraggingPosition.X = (int)(Utils.GetCursorPos().X - XDistanceFromEdge);
                        this.DraggingPosition.Y = (int)(Utils.GetCursorPos().Y - YDistanceFromEdge);
                    }

                    this.IsDragging = true;
                }
            }
            else if (this.IsDragging && args.Msg == (uint)WindowsMessages.WM_LBUTTONUP)
            {
                this.IsDragging = false;
                if (IsInside(new Vector2(this.X, this.Y)))
                {
                    Inside = true;
                    var position = new Vector2(this.X, this.Y);
                    var distanceX = position.X - InitialDragPoint.X;
                    var distanceY = position.Y - InitialDragPoint.Y;
                    this.X = (int) distanceX;
                    this.Y = (int) distanceY;
                    Console.WriteLine("X: {0} Y: {1}", this.X, this.Y);
                }
                else
                {
                    Inside = false;
                }

                InitialDragPoint = new Vector2();
                XDistanceFromEdge = 0;
                YDistanceFromEdge = 0;
                this.DraggingPosition = new Vector2();
                //Console.WriteLine("After : " + this.X + " "+ this.Y);
            }
            */
        }

        public override void RaiseEvents()
        {
            //
        }

        /// <summary>
        /// Initializes the drawings.
        /// </summary>
        public override void InitDrawings()
        {

            Rectangle = new Rectangle_Ex((int)this.Position.X, (int)this.Position.Y, (int)this.Width, (int)this.Height, Color.DodgerBlue)
            {
                VisibleCondition = delegate
                { return HudVariables.ShouldBeVisible && HudVariables.CurrentStatus == SpriteStatus.Expanded; }
            };

            Text = new Render.Text(
                (int) (this.Position.X + this.Width / 2f - Helper.GetSize(this.Name, 17)), (int) this.Position.Y - 9,
                this.Name, 17, Color.White)
            {
                VisibleCondition = delegate
                {
                    return HudVariables.ShouldBeVisible && HudVariables.CurrentStatus == SpriteStatus.Expanded;
                },
                Centered = true
            };

            Rectangle.Add(2);
            Text.Add(2);
        }

        public override void RemoveDrawings()
        {
            //
        }

        public bool IsInside(Vector2 EndPos)
        {
            if ((EndPos.X + this.Width) < (HudVariables.CurrentPosition.X + HudVariables.SpriteWidth) &&
               (EndPos.X) > (HudVariables.CurrentPosition.X) && (EndPos.Y) > (HudVariables.CurrentPosition.Y) &&
               (HudVariables.CurrentPosition.Y + HudVariables.SpriteHeight) > (EndPos.Y + this.Height))
           {
                return true;
           }
            return false;
        }
    }

    public class Rectangle_Ex : Render.RenderObject
        {
            public delegate Vector2 PositionDelegate();
            public delegate bool VisibleDelegate();

            private readonly SharpDX.Direct3D9.Line _line;
            public ColorBGRA Color;
            public float border;

            public Rectangle_Ex(int x, int y, int width, int height, ColorBGRA color, float border = 2)
            {
                X = x;
                Y = y;
                Width = width;
                Height = height;
                Color = color;
                this.border = border;
                _line = new SharpDX.Direct3D9.Line(Drawing.Direct3DDevice) { Width = border};
                Game.OnUpdate += Game_OnUpdate;
            }

            private void Game_OnUpdate(EventArgs args)
            {
                if (PositionUpdate != null)
                {
                    Vector2 pos = PositionUpdate();
                    X = (int) pos.X;
                    Y = (int) pos.Y;
                }
            }

            public int X { get; set; }

            public int Y { get; set; }

            public int Width { get; set; }
            public int Height { get; set; }
            public PositionDelegate PositionUpdate { get; set; }

            public override void OnEndScene()
            {
                try
                {
                    if (_line.IsDisposed)
                    {
                        return;
                    }

                    _line.Begin();
                    _line.Draw(new[] { new Vector2(X, Y), new Vector2(X + Width, Y) }, Color);
                    _line.Draw(new[] { new Vector2(X, Y + Height / 2), new Vector2(X + Width, Y + Height / 2) }, Color);
                    _line.Draw(new[] { new Vector2(X, Y), new Vector2(X, Y + Height / 2) }, Color);
                    _line.Draw(new[] { new Vector2(X + Width, Y), new Vector2(X + Width, Y + Height / 2) }, Color);
                    _line.End();
                }
                catch (Exception e)
                {
                    Console.WriteLine(@"Common.Render.Rectangle.OnEndScene: " + e);
                }
            }

            public override void OnPreReset()
            {
                _line.OnLostDevice();
            }

            public override void OnPostReset()
            {
                _line.OnResetDevice();
            }

            public override void Dispose()
            {
                if (!_line.IsDisposed)
                {
                    _line.Dispose();
                }
                Game.OnUpdate -= Game_OnUpdate;
            }
        }
}
