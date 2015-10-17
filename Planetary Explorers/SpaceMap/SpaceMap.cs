using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Planetary_Explorers.Displays;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Planetary_Explorers.SpaceMap
{
    class SpaceMap : Display
    {
        private View _view;
        private RenderTexture _gridTexture;
        private Vertex[] _gridlines;
        private Sprite _grid;

        private bool _dragging;
        private Vector2i _mousePrevDragPos;

        public SpaceMap(Vector2u mapSize, Vector2u displaySize) : base(displaySize)
        {
            SetupGrid(mapSize);

            OnLostFocus += SpaceGrid_OnLostFocus;
            OnMouseMove += SpaceGrid_OnMouseMove;
            OnMousePress += SpaceGrid_OnMousePress;
            OnMouseRelease += SpaceGrid_OnMouseRelease;
            OnKeyPress += SpaceGrid_OnKeyPress;

            var planet = new Planet(new Vector2u(80, 80));
            planet.Position = new Vector2f(40, 120);
            planet.EventSubscribe(true, PEGame.ActiveGame.Window);
            
            AddItemToDraw(planet, 1);
        }

        public override void Update()
        {
            _gridTexture.Clear(new Color(190, 190, 190));
            _gridTexture.Draw(_gridlines, PrimitiveType.Lines);
            _gridTexture.Display();
            _grid.Texture = _gridTexture.Texture;
            base.Update();
        }

        private void SetupGrid(Vector2u mapSize)
        {
            const int gridSize = 16;
            _gridTexture = new RenderTexture(2000, 2000);
            var col = new Color(120, 120, 120);
            var verticies = new List<Vertex>();
            for (int x = 0; x < mapSize.X; x += gridSize)
            {
                verticies.Add(new Vertex(new Vector2f(x, 0), col));
                verticies.Add(new Vertex(new Vector2f(x, mapSize.Y), col));
            }
            for (int y = 0; y < mapSize.Y; y += gridSize)
            {
                verticies.Add(new Vertex(new Vector2f(0, y), col));
                verticies.Add(new Vertex(new Vector2f(mapSize.X, y), col));
            }
            _gridlines = verticies.ToArray();

            _gridTexture.Clear(new Color(190, 190, 190));
            //_view = new View(new FloatRect(0,0,displaySize.X, displaySize.Y));
            _view = new View(new FloatRect(0, 0, DisplayView.Size.X, DisplayView.Size.Y));
            DisplayView = _view;
            //_gridTexture.SetView(_view);
            _gridTexture.Draw(_gridlines, PrimitiveType.Lines);
            _gridTexture.Display();

            _grid = new Sprite(_gridTexture.Texture);
            AddItemToDraw(_grid, 0);
        }

        private void SpaceGrid_OnLostFocus(object sender, EventArgs e)
        {
            _dragging = false;
        }

        void SpaceGrid_OnKeyPress(object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.Right)
            {
                _view.Move(new Vector2f(10, 0));
                DisplayView = _view;
            }
        }

        void SpaceGrid_OnMouseMove(object sender, MouseMoveEventArgs e, Vector2f displayCoords)
        {
            if (_dragging)
            {
                var mousePos = new Vector2i(e.X, e.Y);
                var diffVec = mousePos - _mousePrevDragPos;
                _view.Move(new Vector2f(-diffVec.X / 1f, -diffVec.Y / 1f));
                DisplayView = _view;
                //_gridTexture.SetView(_view);
                _mousePrevDragPos = mousePos;
            }
        }

        void SpaceGrid_OnMousePress(object sender, MouseButtonEventArgs e, Vector2f displayCoords)
        {
            switch (e.Button)
            {
                case Mouse.Button.Left:
                    {
                        _dragging = true;
                        _mousePrevDragPos = new Vector2i(e.X, e.Y);
                        break;
                    }
                case Mouse.Button.Middle:
                    break;
                case Mouse.Button.Right:
                    break;
            }
        }

        void SpaceGrid_OnMouseRelease(object sender, MouseButtonEventArgs e, Vector2f displayCoords)
        {
            switch (e.Button)
            {
                case Mouse.Button.Left:
                    {
                        _dragging = false;
/*                        allPlanets.ForEach(
                            planet => planet.Select(
                                        planet.ContainsVector(displayCoords.X, displayCoords.Y)
                                        ));*/
                        break;
                    }
                case Mouse.Button.Middle:
                    break;
                case Mouse.Button.Right:
                    break;
                default:
                    break;
            }
        }
    }
}
