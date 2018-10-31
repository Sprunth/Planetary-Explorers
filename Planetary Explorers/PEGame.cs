using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Planetary_Explorers.SpaceMap;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Planetary_Explorers
{
    class PEGame
    {
        public static PEGame ActiveGame;

        private GameManager GM;

        public Vector2u Screensize { get; private set; }
        public RenderWindow Window { get; private set; }

        public PEGame(Vector2u screensize)
        {
            ActiveGame = this;
            Screensize = screensize;
        }

        public void Initialize()
        {
            var contextSettings = new ContextSettings(32, 32, 8);

            Window = new RenderWindow(new VideoMode(Screensize.X, Screensize.Y), "Planetary Explorers", Styles.Close | Styles.Titlebar, contextSettings);

            var spaceMap = new SpaceMap.SpaceMap(new Vector2u(80, 30), new Vector2u(400, 300));

            GM = new GameManager(Window, spaceMap);

            Window.SetFramerateLimit(60);

            Window.Closed += window_Closed;
        }

        public void Run()
        {
           
            var spr = new Sprite();
            while (Window.IsOpen)
            {
                Window.DispatchEvents();

                Window.Clear(Color.Green);
                spr.Texture = GM.Draw();
                Window.Draw(spr);

                Window.Display();
            }
        }

        void window_Closed(object sender, EventArgs e)
        {
            Window.Close();
        }
    }
}
