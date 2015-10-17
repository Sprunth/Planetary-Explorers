using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;

namespace Planetary_Explorers
{
    class Program
    {
        static void Main(string[] args)
        {
            var game = new PEGame(new Vector2u(900, 600));
            game.Initialize();
            game.Run();
        }
    }
}
