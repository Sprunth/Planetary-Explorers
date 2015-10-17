using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;

namespace Planetary_Explorers
{
    abstract class DrawableObject : DrawableBase
    {
        public DrawableObject(Vector2u displaySize) : base(displaySize)
        {
            
        }

        public virtual bool ContainsVector(Vector2f vec)
        { return ContainsVector(vec.X, vec.Y); }
        public abstract bool ContainsVector(double x, double y);
    }
}
