using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Planetary_Explorers.Displays
{
    class Display : DrawableBase
    {
        protected View DisplayView { get { return Target.GetView(); } set { Target.SetView(value); } }

        public Display(Vector2u displaySize) : base(displaySize)
        {

        }
    }
}
