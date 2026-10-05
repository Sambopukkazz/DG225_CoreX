using Gum.Forms.DefaultVisuals.V3;
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
        private int _minPathLength = 6;
        private int _minDistance = 4;
        private int _maxAttempts = 100;
        private Random _rand = new Random();
        private Action<int, int, Color> _addDot;
        private Color[] _availableColors = { Color.Red, Color.Blue, Color.Green, Color.Yellow, Color.Purple, Color.Orange };

        public DotsGenerator(int row, int column, Action<int, int, Color> addDot) {
            _row = row;
            _column = column;
            _addDot = addDot;
            GeneratedDotsMap candidate = null;

            for (int attempt = 0; attempt < _maxAttempts; attempt++) {
                candidate = GenerateDots();

                if (candidate != null && IsValidHardLevel(candidate)) {
                    break;
                }
            }

            List<Color> availableColors = _availableColors.ToList();

            foreach (var dot in candidate.Dots) {
                int colorID = _rand.Next(availableColors.Count);
                _addDot(dot.Value.start.X, dot.Value.start.Y, availableColors[colorID]);
                _addDot(dot.Value.end.X, dot.Value.end.Y, availableColors[colorID]);
                availableColors.RemoveAt(colorID);
            }
        }

        public GeneratedDotsMap GenerateDots() {
            int[,] grid = new int[_column, _row];

            Dictionary<int, List<Point>> colorPaths = new Dictionary<int, List<Point>>();

            List<int> activeColors = new List<int>();

            for (int colorId = 1; colorId <= _maxDots; colorId++) {
                Point startDot;
                do {
                    startDot = new Point(_rand.Next(_column), _rand.Next(_row));
                }
                while (grid[startDot.X, startDot.Y] != 0);

                grid[startDot.X, startDot.Y] = colorId;
                colorPaths[colorId] = new List<Point> { startDot };
                activeColors.Add(colorId);
            }

            while (activeColors.Count > 0) {
                activeColors.Sort((a, b) => colorPaths[a].Count.CompareTo(colorPaths[b].Count));

                for (int i = activeColors.Count - 1; i >= 0; i--) {
                    int colorId = activeColors[i];
                    List<Point> currentPath = colorPaths[colorId];

                    Point? nextStep = PickSmartNeighbor(grid, currentPath);

                    if (nextStep.HasValue) {
                        Point next = nextStep.Value;
                        grid[next.X, next.Y] = colorId;
                        currentPath.Add(next);
                    }
                    else {
                        activeColors.RemoveAt(i); // Stuck, stop growing this color
                    }
                }
            }

            GeneratedDotsMap map = new GeneratedDotsMap { 
                Columns = _column,
                Rows = _row
            };
            foreach (var colorPath in colorPaths) {
                int colorId = colorPath.Key;
                List<Point> path = colorPath.Value;

                map.Dots[colorId] = (path[0], path[path.Count - 1]);
                map.PathLengths[colorId] = path.Count;
            }

            return map;
        }

        private Point? PickSmartNeighbor(int[,] grid, List<Point> path) {
            Point head = path[path.Count - 1];
            Point previousDir = path.Count > 1
                ? new Point(head.X - path[path.Count - 2].X, head.Y - path[path.Count - 2].Y)
                : Point.Zero;

            Point[] directions = { new Point(0, -1), new Point(1, 0), new Point(0, 1), new Point(-1, 0) };
            List<(Point point, int weight)> candidates = new();

            foreach (var dir in directions) {
                int nx = head.X + dir.X;
                int ny = head.Y + dir.Y;

                if (nx >= 0 && nx < _column && ny >= 0 && ny < _row && grid[nx, ny] == 0) {
                    Point neighbor = new Point(nx, ny);
                    int weight = 1; // Base weight

                    // RULE A: TURN BIAS (Reward changing directions over straight lines)
                    bool isTurn = previousDir != Point.Zero && (dir.X != previousDir.X || dir.Y != previousDir.Y);
                    if (isTurn) weight += 4;

                    // RULE B: WALL-HUGGING (Reward moving adjacent to existing wires/outer walls)
                    int touchingObstacles = CountAdjacentOccupied(grid, neighbor);
                    weight += touchingObstacles * 3;

                    candidates.Add((neighbor, weight));
                }
            }

            if (candidates.Count == 0) return null;

            // Weighted Random Selection
            int totalWeight = 0;
            foreach (var c in candidates) totalWeight += c.weight;

            int roll = _rand.Next(totalWeight);
            int currentSum = 0;
            foreach (var c in candidates) {
                currentSum += c.weight;
                if (roll < currentSum) return c.point;
            }

            return candidates[0].point;
        }

        private int CountAdjacentOccupied(int[,] grid, Point p) {
            int count = 0;
            Point[] dirs = { new Point(0, -1), new Point(1, 0), new Point(0, 1), new Point(-1, 0) };

            foreach (var d in dirs) {
                int nx = p.X + d.X;
                int ny = p.Y + d.Y;

                // Out-of-bounds OR non-zero cell counts as a wall/obstacle
                if (nx < 0 || nx >= _column || ny < 0 || ny >= _row || grid[nx, ny] != 0) {
                    count++;
                }
            }
            return count;
        }

        private bool IsValidHardLevel(GeneratedDotsMap map) {
            foreach (var kvp in map.Dots) {
                int colorId = kvp.Key;
                Point start = kvp.Value.start;
                Point end = kvp.Value.end;

                // 1. Check path length
                if (map.PathLengths[colorId] < _minPathLength)
                    return false;

                // 2. Check Manhattan distance between endpoints
                int distance = Math.Abs(start.X - end.X) + Math.Abs(start.Y - end.Y);
                if (distance < _minDistance)
                    return false;
            }

            return true;
        }
    }
}
