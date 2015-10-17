using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Planetary_Explorers.Displays;
using SFML.Graphics;

namespace Planetary_Explorers
{
    class GameManager
    {
        private RenderTexture tex;

        private RenderWindow window;

        /// <summary>
        /// Internal. Do not set this variable! Use the public property
        /// </summary>
        private Display activeDisplay;
        public Display ActiveDisplay
        {
            get { return activeDisplay; }
            set
            {
                if (ActiveDisplay != null)
                    ActiveDisplay.EventSubscribe(false, window);
                //if (!allPageDisplays.Contains(value))
                //{ allPageDisplays.Add(value); }
                value.EventSubscribe(true, window);

                activeDisplay = value;
            }
        }

        public GameManager(RenderWindow window, Display startingDisplay)
        {
            this.window = window;

            ActiveDisplay = startingDisplay;
            tex = new RenderTexture(window.Size.X, window.Size.Y);
        }

        public void Update()
        {
            ActiveDisplay.Update();
        }

        public Texture Draw()
        {
            tex.Clear(Color.Cyan);
            ActiveDisplay.Draw(tex);
            tex.Display();
            return tex.Texture;
        }

    }
}
