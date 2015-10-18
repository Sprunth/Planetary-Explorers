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

        protected override void OnResume()
        {
            var window = PEGame.ActiveGame.Window;
            foreach (var tup in ToDraw)
            {
                tup.Value.ForEach(drawable =>
                {
                    if (drawable is DrawableBase)
                    {
                        ((DrawableBase) drawable).EventSubscribe(true, window);
                    }
                });
            }
        }

        protected override void OnPause()
        {
            var window = PEGame.ActiveGame.Window;
            foreach (var tup in ToDraw)
            {
                tup.Value.ForEach(drawable =>
                {
                    if (drawable is DrawableBase)
                    {
                        ((DrawableBase)drawable).EventSubscribe(false, window);
                    }
                });
            }
        }
    }
}
