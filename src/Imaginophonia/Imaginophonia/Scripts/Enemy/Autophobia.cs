using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophobia {
    public class Autophobia : GameObject, IMoveable, ICollisionActor {
        public Vector2 Direction { get; private set; }
        public Vector2 Velocity { get; private set; }
        public float MoveSpeed { get; private set; } = 200f;
        public int Id { get; }
        public CollisionShape2D Shape { get; private set; }
        private readonly Vector2 _size;

        private AnimatedSprite _animatedSprite;

        public Autophobia(Vector2 pos, GameObject player) : base("Scopophobia", "Enemy") {
            Texture2DAtlas atlas = MainGame.Content.Load<Texture2DAtlas>("Character/spitesheet_player");
            SpriteSheet spriteSheet = new("player", atlas);

            spriteSheet.DefineAnimation("walk-forward", builder => {
                builder.IsLooping(true);
                for (int i = 1; i < 8; i++) {
                    if (i == 7) builder.AddFrame($"sprite_walk_0{i}", TimeSpan.FromSeconds(0));
                    else builder.AddFrame($"sprite_walk_0{i}", TimeSpan.FromSeconds(0.3));
                }
            });

            Transform.Position = pos;
            if (Transform.Position.X - player.Transform.Position.X < 0) {
                Direction = Vector2.UnitX * -1f;
            }
            else {
                Direction = Vector2.UnitX;
            }
        }
    }
}
