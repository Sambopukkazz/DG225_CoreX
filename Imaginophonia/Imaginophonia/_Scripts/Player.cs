using Microsoft.Xna.Framework;
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
    public class Player : GameObject{
        private float _moveSpeed;

        private SpriteSheet _spriteSheet;
        private AnimatedSprite _animatedSprite;

        public bool IsActive { get; set; }
        public bool IsVisible { get; set; }
        public float Alpha { get; set; }
        public float Depth { get; set; }
        public object Tag { get; set; }
        
        public SpriteEffects Effect { get; set; }

        
        public Player(){
            Texture2DAtlas atlas = Game1.Content.Load<Texture2DAtlas>("load json");
            _spriteSheet = new("", atlas);
            _spriteSheet.DefineAnimation("", builder => {
                builder.IsLooping(true)
                .AddFrame("frame name",TimeSpan.FromSeconds(0.1));
            });
        }

        public override void Update() {
            Transform.Position.Translate(InputManager.Direction.X * _moveSpeed, 0);
            base.Update();
        }

        public override void Draw() {
            base.Draw();
        }
    }
}
