using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using MonoGame.Extended.Screens;
using RenderingLibrary.Math.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophobia {
    public class ConnectTheDotsScreen : Screen {
        private int _row = 10;
        private int _col = 10;
        private int _gridSize = 64;
        private Vector2 _gridPos;
        private Player _player;
        private Cell[,] wireCell;
        public ConnectTheDotsScreen(Player player) {
            wireCell = new Cell[_row,_col];
            _gridPos = new Vector2(600,200);

            for (int i = 0; i < _row; i++) {
                for (int j = 0; j < _col; j++) {
                    wireCell[i,j] = AddCell((int)_gridPos.X + (_gridSize * i), (int)_gridPos.Y + (_gridSize * j));
                }
            }

            _player = player;
        }

        public override void Update(GameTime gameTime) {
            foreach (Cell cell in wireCell) {
                cell.Update();
            }

            if (KeyboardExtended.GetState().WasKeyPressed(Keys.Q)) {
                ScreenManager.CloseScreen();
                _player.ToggleRepair();
            }
        }

        public override void Draw(GameTime gameTime) {
            MainGame.SpriteBatch.Begin();
            foreach (Cell cell in wireCell) {
                cell.Draw();
            }
            MainGame.SpriteBatch.End();
        }

        public Cell AddCell(int posX, int posY) {
            Rectangle rect = new Rectangle(posX, posY, _gridSize, _gridSize);
            Cell cell = new Cell(rect);
            return cell;
        }

        public Point? ScreenToGrid(int posX, int posY) {
            int x = (int)((posX - _gridPos.X) / _gridSize);
            int y = (int)((posY - _gridPos.Y) / _gridSize);

            if (x >= 0 && x < _col && y >= 0 && y < _row) {
                return new Point(x, y);
            }

            return null;
        }
    }
}
