using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophonia {
    public class Player : GameObject,IMoveable{

        private SpriteSheet _spriteSheet;
        private AnimatedSprite _animatedSprite;
        public static AudioListener Listener { get; }
        private BoundingCapsule2D _bounds;
        public Vector2 Direction { get; private set; }
        public Vector2 Velocity { get; private set; }
        public float MoveSpeed { get; private set; } = 100f;

        public float Alpha { get; set; }

        public SpriteEffects Effect { get; set; }

        
        public Player(){
            //Set up animation
            Texture2DAtlas atlas = Game1.Content.Load<Texture2DAtlas>("Character/spitesheet_player");
            _spriteSheet = new("player", atlas);

            _spriteSheet.DefineAnimation("walk", builder => {
                builder.IsLooping(true);
                for(int i = 1; i < 8; i++) {
                    builder.AddFrame($"sprite_walk_0{i}", TimeSpan.FromSeconds(0.2));
                }
            });

            _spriteSheet.DefineAnimation("idle", builder => {
                builder.IsLooping(false)
                .AddFrame("sprite_idle", TimeSpan.FromSeconds(0));
            });

            _animatedSprite = new AnimatedSprite(_spriteSheet,"idle");
        }

        public void Update(GameTime gameTime) {
            Direction = InputManager.Direction;
            Velocity = MoveSpeed * InputManager.Direction;
            Transform.Position = Transform.Position.Translate(Velocity.X * (float)gameTime.ElapsedGameTime.TotalSeconds, 0);
            //Transform.Position += new Vector2(InputManager.Direction.X * _moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds, InputManager.Direction.Y * _moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);

            _animatedSprite.Update(gameTime);
            //base.Update();
        }

        public override void Draw() {
            if (Visible) {
                Game1.SpriteBatch.Draw(_animatedSprite, Transform.Position,0,Transform.Scale * 4);
            }

            base.Draw();
        }
    }
}
