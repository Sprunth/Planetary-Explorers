using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace Planetary_Explorers
{
    interface IDrawable
    {
        void Draw(RenderTarget sourceTarget);
    }
}
