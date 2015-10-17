using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Noise;
using Noise.Modules;
using Noise.Utils;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Color = SFML.Graphics.Color;

namespace Planetary_Explorers.SpaceMap
{
    class Planet : DrawableObject
    {
        private readonly CircleShape _planet;
        public Texture SurfaceTexture { get { return _planet.Texture; } }

        public Planet(Vector2u displaySize)
            : base(displaySize)
        {
            _planet = new CircleShape(0.9f*displaySize.X / 2f, 4 + 2 * (uint)Math.Sqrt(displaySize.X))
            {
                Position = new Vector2f(1.1f*displaySize.X/2f, 1.1f*displaySize.Y/2f),
                OutlineThickness = 3,
                OutlineColor = new Color(20, 20, 20)
            };
            _planet.Origin = new Vector2f(displaySize.X / 2f, displaySize.Y / 2f);
            //_planet.FillColor = SFML.Graphics.Color.Magenta;
            _planet.Texture = GeneratePlanetTexture(new Vector2u((uint)_planet.Radius, (uint)_planet.Radius));

            AddItemToDraw(_planet, 5);

            //_hoverText = new Label("Planet", FontManager.ActiveFontManager, new Vector2u(100, 40));

            OnMouseMove += Planet_OnMouseMove;

            BackgroundColor = Color.Red;
        }

        void Planet_OnMouseMove(object sender, MouseMoveEventArgs e, Vector2f displayCoords)
        {
            //Debug.WriteLine("MousePos: {0} | Planet Center: {1}", displayCoords.X + " " + displayCoords.Y, _planet.Position);
            if (ContainsVector(displayCoords))
            {
                // within planet's sprite
                _planet.FillColor = new Color(255, 255, 255);
                //_hoverText.EventSubscribe(true, GameManager.ActiveWindow);
                //AddItemToDraw(_hoverText, 30);
                //_hoverText.Position = displayCoords + new Vector2f(20, -20);
            }
            else
            {
                _planet.FillColor = new Color(200, 200, 200);
                //_hoverText.EventSubscribe(false, GameManager.ActiveWindow);
                //RemoveItemToDraw(_hoverText, 30);
            }
        }

        public override bool ContainsVector(double x, double y)
        {
            var dist = Math.Sqrt(
                Math.Pow(x - (_planet.Position.X + _planet.Origin.X), 2) +
                Math.Pow(y - (_planet.Position.Y + _planet.Origin.Y), 2));
            return (dist < _planet.Radius);
        }

        /// <summary>
        /// Change whether to draw planet as selected or not
        /// </summary>
        /// <param name="select">True for selected, False for not selected</param>
        public void Select(bool select)
        {
            _planet.OutlineColor = select ? new Color(200, 210, 40) : new Color(20, 20, 20);
            _planet.OutlineThickness = select ? 4 : 3;
        }

        #region procedural planet texture
        private static readonly Random random = new Random();
        private static NoiseMap heightMap;
        private static PlanarNoiseMapBuilder heightMapBuilder;
        private static Perlin perlin;
        private static RidgedMulti ridgedMulti;
        private static Voronoi voronoi;
        private static Select selectModule;

        public static Texture GeneratePlanetTexture(Vector2u texSize)
        {
            var imgSize = texSize;
            perlin = new Perlin(random.Next(2, 3), 0.2, NoiseQuality.Best, 4, 0.7, random.Next(0, 1024));
            ridgedMulti = new RidgedMulti(random.NextDouble() * 2, 0.3, 2, NoiseQuality.Best, random.Next(0, 1024));
            voronoi = new Voronoi(0.1, random.NextDouble() * 2, true, random.Next(0, 1024));
            selectModule = new Select(1.0, 1.0, 0.0);
            selectModule.SetSourceModule(0, perlin);
            selectModule.SetSourceModule(1, ridgedMulti);
            selectModule.SetSourceModule(2, voronoi);

            heightMapBuilder = new PlanarNoiseMapBuilder(imgSize.X, imgSize.Y, 0, selectModule, 1, 5, 1, 5, true);
            heightMap = heightMapBuilder.Build();

            var texColors = new GradientColour();
            texColors.AddGradientPoint(-1, GenerateProceduralColor());
            texColors.AddGradientPoint(-0.2 + random.NextDouble() * 0.4, GenerateProceduralColor());
            texColors.AddGradientPoint(1, GenerateProceduralColor());
            var renderer = new ImageBuilder(heightMap, texColors);
            var renderedImg = renderer.Render();
            var img = new Bitmap(renderedImg);
            var sfmlImg = new SFML.Graphics.Image(imgSize.X, imgSize.Y);

            for (uint x = 0; x < imgSize.X; x++)
            {
                for (uint y = 0; y < imgSize.Y; y++)
                {
                    var col = img.GetPixel((int)x, (int)y);
                    sfmlImg.SetPixel(x, y, new Color(col.R, col.G, col.B, col.A));
                }
            }

            var returnTex = new Texture(sfmlImg);
            return returnTex;
        }

        /// <summary>
        /// Fancy color generation that looks better than just pure random
        /// </summary>
        /// <returns></returns>
        private static System.Drawing.Color GenerateProceduralColor()
        {
            _colorGenerationCounter += random.Next(1, 6);
            // golden ratio
#if DEBUG
            var ret = HSL2RGB((double)(((Decimal)(colorOffset + (0.618033988749895f * _colorGenerationCounter))) % 1.0m), 0.5, 0.5);
            Debug.WriteLine(ret);
            return ret;
#else
            return Helper.HSL2RGB((double)(((Decimal)(colorOffset + (0.618033988749895f * _colorGenerationCounter))) % 1.0m), 0.5, 0.5);
#endif
        }
        private static readonly float colorOffset = (float)random.NextDouble();
        private static int _colorGenerationCounter = 2;

        // Given H,S,L in range of 0-1
        // Returns a Color (RGB struct) in range of 0-255
        private static System.Drawing.Color HSL2RGB(double h, double sl, double l)
        {
            double v;
            double r, g, b;

            r = l;   // default to gray
            g = l;
            b = l;
            v = (l <= 0.5) ? (l * (1.0 + sl)) : (l + sl - l * sl);
            if (v > 0)
            {
                double m;
                double sv;
                int sextant;
                double fract, vsf, mid1, mid2;

                m = l + l - v;
                sv = (v - m) / v;
                h *= 6.0;
                sextant = (int)h;
                fract = h - sextant;
                vsf = v * sv * fract;
                mid1 = m + vsf;
                mid2 = v - vsf;
                switch (sextant)
                {
                    case 0:
                        r = v;
                        g = mid1;
                        b = m;
                        break;
                    case 1:
                        r = mid2;
                        g = v;
                        b = m;
                        break;
                    case 2:
                        r = m;
                        g = v;
                        b = mid1;
                        break;
                    case 3:
                        r = m;
                        g = mid2;
                        b = v;
                        break;
                    case 4:
                        r = mid1;
                        g = m;
                        b = v;
                        break;
                    case 5:
                        r = v;
                        g = m;
                        b = mid2;
                        break;
                }
            }
            Debug.WriteLine(r + "|" + g + "|" + b + "|" + h);
            return System.Drawing.Color.FromArgb(255, (int)(r * 255), (int)(g * 255), (int)(b * 255));
        }
        #endregion
    }
}
