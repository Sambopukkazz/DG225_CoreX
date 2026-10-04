using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophobia {
    public class DotsGenerator {
        private int _column;
        private int _row;
        private int _maxDots = 4;
        public bool IsGeneratingDots { get; private set; }
        private Random _rand;
        private Action<int, int, Color> _addDot;

        private Color[] _availableColors = { Color.White, Color.Red, Color.Blue, Color.Green, Color.Yellow, Color.Purple, Color.Orange };
        public Dictionary<int, (Point start, Point end)> Dots { get; set; } = new Dictionary<int, (Point, Point)>();

        public DotsGenerator(int row, int column, Action<int, int, Color> addDot) {
            _row = row;
            _column = column;
            _addDot = addDot;

            _rand = new Random();
        }

        public void GenerateDots() {
            int[,] grid = new int[_column, _row];

            Dictionary<int, Point> heads = new Dictionary<int, Point>();

            List<int> activeColors = new List<int>();

            for (int colorId = 1; colorId <= _maxDots; colorId++) {
                Point startDot;
                do {
                    startDot = new Point(_rand.Next(_column), _rand.Next(_row));
                }
                while (grid[startDot.X, startDot.Y] != 0);

                grid[startDot.X, startDot.Y] = colorId;
                heads[colorId] = startDot;
                activeColors.Add(colorId);
                _addDot(startDot.X, startDot.Y, _availableColors[colorId]);
            }

            Point[] directions = { new Point(0, -1), new Point(1, 0), new Point(0, 1), new Point(-1, 0) };

            while (activeColors.Count > 0) {
                for (int i = activeColors.Count - 1; i >= 0; i--) {
                    int colorId = activeColors[i];
                    Point head = heads[colorId];

                    List<Point> validNeighbors = new List<Point>();
                    foreach (var dir in directions) {
                        int x = head.X + dir.X;
                        int y = head.Y + dir.Y;

                        if (x >= 0 && x < _column && y >= 0 && y < _row && grid[x, y] == 0) {
                            validNeighbors.Add(new Point(x, y));
                        }
                    }

                    if (validNeighbors.Count > 0) {
                        Point next = validNeighbors[_rand.Next(validNeighbors.Count)];
                        grid[next.X, next.Y] = colorId;
                        heads[colorId] = next;
                    }
                    else {
                        _addDot(head.X, head.Y, _availableColors[colorId]);
                        activeColors.RemoveAt(i);
                    }
                }
            }


        }
    }
}
