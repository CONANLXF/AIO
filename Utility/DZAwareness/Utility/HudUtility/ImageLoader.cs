using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using DZAwarenessAIO.Utility.MenuUtility;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using Rectangle = System.Drawing.Rectangle;
using EloBuddy;
using PortAIO.Properties;
using EloBuddy.SDK.Menu.Values;

//Credits to TheSaltyWaffle - Universal Minimap Hack
//https://github.com/TheSaltyWaffle/LeagueSharp/blob/master/UniversalMinimapHack/ImageLoader.cs
//Ily Waffle

using TargetSelector = PortAIO.TSManager; namespace DZAwarenessAIO.Utility.HudUtility
{
    /// <summary>
    /// The image loading class
    /// </summary>
    public class ImageLoader
    {
        /// <summary>
        /// The dictionary of hero images added to the hud
        /// </summary>
        public static Dictionary<string, HeroHudImage> AddedHeroes = new Dictionary<string, HeroHudImage>();

        /// <summary>
        /// Initializes the sprites.
        /// </summary>
        public static void InitSprites()
        {
            RemoveSprites();

            foreach (var enemy in HeroManager.Enemies)
            {
                AddedHeroes.Add(enemy.ChampionName, new HeroHudImage(enemy.ChampionName));
            }
        }

        /// <summary>
        /// Removes the sprites.
        /// </summary>
        public static void RemoveSprites()
        {
            foreach (var hero in AddedHeroes)
            {
                hero.Value.HeroSprite.Remove();
                hero.Value.SSText.Remove();
                hero.Value.SSSprite.Remove();
            }
            AddedHeroes.Clear();
        }

        /// <summary>
        /// Called when the game updates.
        /// </summary>
        public static void OnUpdate()
        {
            var k = 0;
            foreach (var hero in AddedHeroes)
            {
                var sprite = hero.Value.HeroSprite;
                var circlePosition = new Vector2(
                        HudVariables.CurrentPosition.X + 11 + (sprite.Scale.X * (29 + (sprite.Width))) * k +
                        (sprite.Width * sprite.Scale.X) * (k) - (sprite.Width * sprite.Scale.X) / 2f,
                        HudVariables.CurrentPosition.Y + HudVariables.CroppedHeight - sprite.Height - 6); ;
                sprite.Position = circlePosition;

                var SSSprite = hero.Value.SSSprite;
                SSSprite.Position = circlePosition;

                var text = hero.Value.SSText;
                //text.X = (int) (sprite.Position.X + (sprite.Width * sprite.Scale.X)) - (int)((TextWidth(text.text, new Font("Calibri", 26)) * sprite.Scale.X) / 2f);
                //text.Y = (int)(sprite.Position.Y + (sprite.Height * sprite.Scale.Y) / 2f + 2);
                text.X = (int) (sprite.Position.X + (sprite.Width) / 2f);
                text.Y = (int)(sprite.Position.Y + (sprite.Height) / 2f);
                k++;
            }
        }

        /// <summary>
        /// Loads the bitmap for a champion name.
        /// </summary>
        /// <param name="championName">Name of the champion.</param>
        /// <returns></returns>
        public static Bitmap Load(string championName)
        {
            string cachedPath = GetCachedPath(championName);
            if (File.Exists(cachedPath))
            {
                return ChangeOpacity(new Bitmap(cachedPath));
            }
            var bitmap = Resources.ResourceManager.GetObject(championName + "_Square_0") as Bitmap;
            if (bitmap == null)
            {
                return ChangeOpacity(CreateFinalImage(Resources.empty));
            }
            Bitmap finalBitmap = CreateFinalImage(bitmap);
            //finalBitmap.Save(cachedPath);
            return finalBitmap;
        }

        /// <summary>
        /// Gets the cached path for the image.
        /// </summary>
        /// <param name="championName">Name of the champion.</param>
        /// <returns></returns>
        private static string GetCachedPath(string championName)
        {
            string path = Path.Combine(Variables.WorkingDir, "ImageCache");
            path = Path.Combine(path, Game.Version);
            return Path.Combine(path, championName + ".png");
        }

        /// <summary>
        /// Creates the final image setting it a border.
        /// </summary>
        /// <param name="srcBitmap">The source bitmap.</param>
        /// <returns></returns>
        private static Bitmap CreateFinalImage(Bitmap srcBitmap)
        {
            var img = new Bitmap(srcBitmap.Width, srcBitmap.Width);
            var cropRect = new Rectangle(0, 0, srcBitmap.Width, srcBitmap.Width);

            using (Bitmap sourceImage = srcBitmap)
            {
                using (Bitmap croppedImage = sourceImage.Clone(cropRect, sourceImage.PixelFormat))
                {
                    using (var tb = new TextureBrush(croppedImage))
                    {
                        using (Graphics g = Graphics.FromImage(img))
                        {
                            g.FillEllipse(tb, 0, 0, srcBitmap.Width, srcBitmap.Width);
                            var p = new Pen(Color.DodgerBlue, 5) { Alignment = PenAlignment.Inset };
                            g.DrawEllipse(p, 0, 0, srcBitmap.Width, srcBitmap.Width);
                        }
                    }
                }
            }
            srcBitmap.Dispose();
            return img;
        }

        /// <summary>
        /// Changes the opacity.
        /// </summary>
        /// <param name="img">The img.</param>
        /// <returns></returns>
        private static Bitmap ChangeOpacity(Bitmap img)
        {
            var bmp = new Bitmap(img.Width, img.Height); // Determining Width and Height of Source Image
            Graphics graphics = Graphics.FromImage(bmp);
            var colormatrix = new ColorMatrix { Matrix33 = 33 };
            var imgAttribute = new ImageAttributes();
            imgAttribute.SetColorMatrix(colormatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            graphics.DrawImage(
                img, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel,
                imgAttribute);
            graphics.Dispose(); // Releasing all resource used by graphics
            img.Dispose();
            return bmp;
        }

        /// <summary>
        /// Saturates the bitmap.
        /// </summary>
        /// <param name="original">The original bitmap.</param>
        /// <param name="saturation">The saturation.</param>
        /// <returns>The saturated bitmap</returns>
        private static Bitmap SaturateBitmap(Image original, float saturation)
        {
            const float rWeight = 0.3086f;
            const float gWeight = 0.6094f;
            const float bWeight = 0.0820f;

            var a = (1.0f - saturation) * rWeight + saturation;
            var b = (1.0f - saturation) * rWeight;
            var c = (1.0f - saturation) * rWeight;
            var d = (1.0f - saturation) * gWeight;
            var e = (1.0f - saturation) * gWeight + saturation;
            var f = (1.0f - saturation) * gWeight;
            var g = (1.0f - saturation) * bWeight;
            var h = (1.0f - saturation) * bWeight;
            var i = (1.0f - saturation) * bWeight + saturation;

            var newBitmap = new Bitmap(original.Width, original.Height);
            var gr = Graphics.FromImage(newBitmap);

            // ColorMatrix elements
            float[][] ptsArray =
                {
                    new[] { a, b, c, 0, 0 }, new[] { d, e, f, 0, 0 }, new[] { g, h, i, 0, 0 },
                    new float[] { 0, 0, 0, 1, 0 }, new float[] { 0, 0, 0, 0, 1 }
                };
            // Create ColorMatrix
            var clrMatrix = new ColorMatrix(ptsArray);
            // Create ImageAttributes
            var imgAttribs = new ImageAttributes();
            // Set color matrix
            imgAttribs.SetColorMatrix(clrMatrix, ColorMatrixFlag.Default, ColorAdjustType.Default);
            // Draw Image with no effects
            gr.DrawImage(original, 0, 0, original.Width, original.Height);
            // Draw Image with image attributes
            gr.DrawImage(
                original, new System.Drawing.Rectangle(0, 0, original.Width, original.Height), 0, 0, original.Width,
                original.Height, GraphicsUnit.Pixel, imgAttribs);
            gr.Dispose();

            return newBitmap;
        }

        public static Color ToGrayscaleColor(Color color)
        {
            var level = (byte) ((color.R + color.G + color.B) / 3);
            var result = Color.FromArgb(level, level, level);
            return result;
        }

        private static Bitmap ToGrayscale(Bitmap bitmap)
        {
            var result = new Bitmap(bitmap.Width, bitmap.Height);
            for (int x = 0; x < bitmap.Width; x++)
                for (int y = 0; y < bitmap.Height; y++)
                {
                    var grayColor = ToGrayscaleColor(bitmap.GetPixel(x, y));
                    result.SetPixel(x, y, grayColor);
                }
            return result;
        }

        public static int TextWidth(string text, Font f)
        {
            int textWidth;

            using (var bmp = new Bitmap(1, 1))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    textWidth = (int)g.MeasureString(text, f).Width;
                }
            }

            return textWidth;
        }
    }

    /// <summary>
    /// The Hero Hud Image class
    /// </summary>
    public class HeroHudImage
    {
        /// <summary>
        /// Gets or sets the hero sprite.
        /// </summary>
        /// <value>
        /// The hero sprite.
        /// </value>
        public Render.Sprite HeroSprite { get; set; }

        /// <summary>
        /// Gets or sets the SS sprite.
        /// </summary>
        /// <value>
        /// The ss sprite.
        /// </value>
        public Render.Sprite SSSprite { get; set; }

        /// <summary>
        /// Gets or sets the SS time text
        /// </summary>
        /// <value>
        /// The SS time text.
        /// </value>
        public Render.Text SSText { get; set; }

        public HeroHudImage(string name)
        {
            this.HeroSprite = new Render.Sprite(ImageLoader.Load(name), new Vector2(0, 0))
            {
                Scale = new Vector2(0.38f, 0.38f),
                Visible = true,
                VisibleCondition = delegate { return HudVariables.ShouldBeVisible; },
            };

            this.SSSprite = new Render.Sprite(Resources.SSCircle, new Vector2(0, 0))
            {
                Scale = new Vector2(0.38f, 0.38f),
                Visible = true,
            };

            this.SSText = new Render.Text(new Vector2(0, 0), "", 26, SharpDX.Color.White)
            {
                Visible = true,
                Centered = true
            };

            //image.GrayScale();
            HeroSprite.Add(1);
            SSSprite.Add(2);
            SSText.Add(3);
        }
    }
}