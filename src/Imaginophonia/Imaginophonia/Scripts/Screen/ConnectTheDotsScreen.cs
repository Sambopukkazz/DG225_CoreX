using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using MonoGame.Extended.Screens;
using RenderingLibrary.Math.Geometry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophobia {
    public class ConnectTheDotsScreen : Screen {
        private int _column = 6;
        private int _row = 6;
        private int _gridSize = 100;
        
        private Vector2 _gridPos;
        private GridCell[,] _cells;
        private List<WirePath> _completedPaths;
        private WirePath _currentPath;
        private Player _player;
        private List<Color> _colors;
        private DotsGenerator _dotsGenerator;

        public ConnectTheDotsScreen(Player player) {
            _cells = new GridCell[_column, _row];      
            _gridPos = new Vector2((MainGame.GraphicsDevice.Viewport.Width / 2) - (_gridSize * _column / 2), (MainGame.GraphicsDevice.Viewport.Height / 2) - (_gridSize * _row / 2));
            _completedPaths = new List<WirePath>();
            _colors = new List<Color>();

            CreateGrid();

            _dotsGenerator = new DotsGenerator(_row, _column, AddDot);
            _dotsGenerator.GenerateDots();

            _player = player;
        }

        public override void Update(GameTime gameTime) {
            //if (_dotsGenerator.IsGeneratingDots) {
            //    //_dotsGenerator.Update();
            //}

            MouseStateExtended mouseStateExtended = MouseExtended.GetState();
            Point? hoveredPoint = ScreenToGrid(new Vector2(mouseStateExtended.X, mouseStateExtended.Y));

            if (mouseStateExtended.IsButtonDown(MouseButton.Left)) {
                if (hoveredPoint.HasValue) {
                    Point pos = hoveredPoint.Value;

                    if (_currentPath == null) {
                        GridCell cell = _cells[pos.X, pos.Y];
                        if (cell.IsEndpoint && cell.Color != Color.White) {
                            _completedPaths.RemoveAll(path => path.Color == cell.Color);

                            _currentPath = new WirePath { Color = cell.Color };
                            _currentPath.Points.Add(pos);
                        }
                    }
                    else if (!_currentPath.IsComplete) {
                        TryExtendPath(_currentPath, pos);
                    }
                }
            }
            else if (mouseStateExtended.WasButtonReleased(MouseButton.Left)) {
                _currentPath = null;
            }
            else if (mouseStateExtended.WasButtonPressed(MouseButton.Right)) {
                _completedPaths.Clear();
            }

            if (KeyboardExtended.GetState().WasKeyPressed(Keys.Q) || _completedPaths.Count == _colors.Count) {
                this.ScreenManager.CloseScreen();
                _player.ToggleRepair();
            }
        }

        public override void Draw(GameTime gameTime) {
            MainGame.SpriteBatch.Begin();
            for (int x = 0; x < _column; x++) {
                for (int y = 0; y < _row; y++) {
                    Rectangle gridRect = new Rectangle((int)_gridPos.X + x * _gridSize,(int)_gridPos.Y + y * _gridSize, _gridSize - 1, _gridSize - 1);
                    MainGame.SpriteBatch.DrawRectangle(gridRect, Color.DarkGray);
                }
            }

            foreach (var path in _completedPaths) {

                for (int i = 0; i < path.Points.Count - 1; i++) {
                    Vector2 start = GetCellCenter(path.Points[i]);
                    Vector2 end = GetCellCenter(path.Points[i + 1]);

                    DrawLineSegment(start, end, path.Color, thickness: 12);
                }
            }

            if (_currentPath != null) {
                for (int i = 0; i < _currentPath.Points.Count - 1; i++) {
                    Vector2 start = GetCellCenter(_currentPath.Points[i]);
                    Vector2 end = GetCellCenter(_currentPath.Points[i + 1]);

                    DrawLineSegment(start, end, _currentPath.Color, thickness: 12);
                }
            }

            for (int x = 0; x < _column; x++) {
                for (int y = 0; y < _row; y++) {
                    GridCell cell = _cells[x, y];
                    if (cell.IsEndpoint) {
                        Vector2 center = GetCellCenter(new Point(x, y));
                        Rectangle dotRect = new Rectangle((int)center.X - 20, (int)center.Y - 20, 40, 40);
                        MainGame.SpriteBatch.DrawCircle(center, 20, 100, cell.Color, 5);
                    }
                }
            }
            MainGame.SpriteBatch.End();
        }

        public void DrawLineSegment(Vector2 point1, Vector2 point2, Color color, int thickness) {
            MainGame.SpriteBatch.DrawLine(point1, point2, color, thickness);
        }

        public void CreateGrid() {
            for (int x = 0; x < _column; x++) {
                for (int y = 0; y < _row; y++) {
                    _cells[x, y] = new GridCell() {
                        Position = new Point(x, y),
                        Color = Color.White
                    };
                }
            }
        }

        public void AddDot(int x, int y, Color color) {
            _cells[x, y].Color = color;
            _cells[x, y].IsEndpoint = true;
            if (_colors.Contains(color) == false) {
                _colors.Add(color);
            }
        }

        public Point? ScreenToGrid(Vector2 mousePos) {
            int x = (int)((mousePos.X - _gridPos.X) / _gridSize);
            int y = (int)((mousePos.Y - _gridPos.Y) / _gridSize);

            if (x >= 0 && x < _column && y >= 0 && y < _row)
                return new Point(x, y);

            return null;
        }

        public void TryExtendPath(WirePath path, Point nextPoint) {
            Point lastPoint = path.Points[path.Points.Count - 1];

            int distance = Math.Abs(nextPoint.X - lastPoint.X) + Math.Abs(nextPoint.Y - lastPoint.Y);
            if (distance != 1) return;

            //Remove the last wire when backtrack
            if (path.Points.Count > 1 && nextPoint == path.Points[path.Points.Count - 2]) {
                path.Points.RemoveAt(path.Points.Count - 1);
                return;
            }

            GridCell targetCell = _cells[nextPoint.X, nextPoint.Y];

            //Check if wire hit other dot color
            if (targetCell.IsEndpoint && targetCell.Color != path.Color) return;

            //Check if wire hit other  completed wire color
            foreach (var existingPath in _completedPaths) {
                if (existingPath.Points.Contains(nextPoint) && existingPath != path)
                    return;
            }

            //Check if wire hit itself color
            if (!path.Points.Contains(nextPoint)) {
                path.Points.Add(nextPoint);

                //Check if reached endpoint with same color
                if (targetCell.IsEndpoint && targetCell.Color == path.Color && path.Points.Count > 1) {
                    path.IsComplete = true;
                    _completedPaths.Add(path);
                }
            }
        }

        public Vector2 GetCellCenter(Point pos) {
            Vector2 center = new Vector2(_gridPos.X + pos.X * _gridSize + _gridSize / 2f, _gridPos.Y + pos.Y * _gridSize + _gridSize / 2f);
            return center;
        }
    }
}
