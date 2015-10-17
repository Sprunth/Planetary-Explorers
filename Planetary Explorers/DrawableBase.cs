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
    class DrawableBase : IUpdatable, IDrawable, Drawable
    {
        public RenderTexture Target;
        private readonly Sprite _targetSpr;
        public Vector2f Position
        {
            get { return _targetSpr.Position; }
            set { _targetSpr.Position = value; }
        }

        protected Color BackgroundColor;

        private readonly List<IUpdatable> toUpdate;
        /// <summary>
        /// SFML objects to draw
        /// Each contains a z-level for drawing
        /// Do not directly add to this list. Use AddItemToDraw
        /// </summary>
        private readonly Dictionary<uint, List<Drawable>> toDraw;


        public delegate void LostFocusHandler(object sender, EventArgs e);
        public event LostFocusHandler OnLostFocus;

        public delegate void KeyPressHandler(object sender, KeyEventArgs e);
        public event KeyPressHandler OnKeyPress;

        public delegate void MouseMoveHandler(object sender, MouseMoveEventArgs e, Vector2f displayCoords);
        public event MouseMoveHandler OnMouseMove;

        public delegate void MousePressHandler(object sender, MouseButtonEventArgs e, Vector2f displayCoords);
        public event MousePressHandler OnMousePress;

        public delegate void MouseReleaseHandler(object sender, MouseButtonEventArgs e, Vector2f displayCoords);
        public event MouseReleaseHandler OnMouseRelease;

        public DrawableBase(Vector2u displaySize)
        {
            toUpdate = new List<IUpdatable>();
            toDraw = new Dictionary<uint, List<Drawable>>();
            
            _targetSpr = new Sprite();

            BackgroundColor = Color.Transparent;

            Target = new RenderTexture(displaySize.X, displaySize.Y)
            {
                Smooth = true
            };
        }

        public virtual void Update()
        {
            foreach (var updatable in toUpdate)
            {
                updatable.Update();
            }
        }

        public virtual void Draw(RenderTarget sourceTarget)
        {
            Target.Clear(BackgroundColor);
            foreach (var tup in toDraw)
            {
                tup.Value.ForEach(drawable => Target.Draw(drawable));
            }
            Target.Display();
            _targetSpr.Texture = Target.Texture;

            sourceTarget.Draw(_targetSpr);
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            Draw(target);
        }

        /// <summary>
        /// Subscribes and unsubscribes to window events
        /// </summary>
        /// <param name="on"></param>
        /// <param name="window"></param>
        public void EventSubscribe(bool on, RenderWindow window)
        {
            if (on)
            {
                OnResume();
                window.LostFocus += LostFocus;
                window.KeyPressed += KeyPressed;
                window.MouseMoved += MouseMoved;
                window.MouseButtonPressed += MousePressed;
                window.MouseButtonReleased += MouseReleased;
            }
            else
            {
                OnPause();
                window.LostFocus -= LostFocus;
                window.KeyPressed -= KeyPressed;
                window.MouseMoved -= MouseMoved;
                window.MouseButtonPressed -= MousePressed;
                window.MouseButtonReleased -= MouseReleased;
            }
        }

        /// <summary>
        /// Called when display resumes
        /// </summary>
        protected virtual void OnResume()
        {

        }

        /// <summary>
        /// Called right before display is closed
        /// </summary>
        protected virtual void OnPause()
        {

        }

        public void AddItemToDraw(Drawable drawable, uint zlevel)
        {
            var exists = false;
            foreach (var tup in toDraw)
            {
                if (tup.Value.Contains(drawable))
                    exists = true;
            }
            if (exists)
                return;
            if (!toDraw.ContainsKey(zlevel))
                toDraw.Add(zlevel, new List<Drawable>());
            toDraw[zlevel].Add(drawable);
        }

        public void RemoveItemToDraw(Drawable drawable, uint zlevel)
        {
            // Could be sped up with binary search, or keep an index.
            toDraw[zlevel].Remove(drawable);
        }

        private void LostFocus(object sender, EventArgs e)
        {
            if (OnLostFocus != null)
            {
                OnLostFocus(sender, e);
            }
        }

        private void KeyPressed(object sender, KeyEventArgs e)
        {
            if (OnKeyPress != null)
            {
                OnKeyPress(sender, e);
            }
        }

        private void MouseMoved(object sender, MouseMoveEventArgs e)
        {
            if (OnMouseMove != null && DisplayContainsMouseMove(e))
            {
                OnMouseMove(sender, e, MouseCoordToDisplayCoord(e));
            }
        }

        private void MousePressed(object sender, MouseButtonEventArgs e)
        {
            if (OnMousePress != null)
            {
                OnMousePress(sender, e, MouseCoordToDisplayCoord(e));
            }
        }

        private void MouseReleased(object sender, MouseButtonEventArgs e)
        {
            if (OnMouseRelease != null)
            {
                OnMouseRelease(sender, e, MouseCoordToDisplayCoord(e));
            }
        }

        private Vector2f MouseCoordToDisplayCoord(MouseMoveEventArgs e)
        { return MouseCoordToDisplayCoord(new Vector2i(e.X, e.Y)); }
        private Vector2f MouseCoordToDisplayCoord(MouseButtonEventArgs e)
        { return MouseCoordToDisplayCoord(new Vector2i(e.X, e.Y)); }
        private Vector2f MouseCoordToDisplayCoord(Vector2i e)
        {
            //e.X *= 2;
            //e.Y *= 2;
            var rawDisplayCoord = e - new Vector2i((int)Math.Round(Position.X), (int)Math.Round(Position.Y));
            rawDisplayCoord.X = (int)Math.Round(rawDisplayCoord.X * 1 / _targetSpr.Scale.X);
            rawDisplayCoord.Y = (int)Math.Round(rawDisplayCoord.Y * 1 / _targetSpr.Scale.Y);
            return Target.MapPixelToCoords(rawDisplayCoord);
        }

        private bool DisplayContainsMouseMove(MouseMoveEventArgs e)
        {
            return (
                (Position.X <= e.X) &&
                (Position.X + Target.Size.X >= e.X) &&
                (Position.Y <= e.Y) &&
                (Position.Y + Target.Size.Y >= e.Y)
                );
        }
    }
}
