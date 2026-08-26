using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophonia {
    public class Player{
        private Texture2D texture;
        private Transform2 _transform;
        private float _moveSpeed;

        private SpriteSheet _spriteSheet;
        private AnimatedSprite _animatedSprite;

        public bool IsActive { get; set; }
        public bool IsVisible { get; set; }
        public float Alpha { get; set; }
        public float Depth { get; set; }
        public object Tag { get; set; }
        public Vector2 Origin { get; set; }
        public SpriteEffects Effect { get; set; }

        
        public Player(Texture2D texture){
            
        }
        //public SpriteRenderer(Texture2D texture) {
        //    this.texture = texture;

        //}

        public void Update() {
            _transform.Position.Translate(InputManager.Direction.X * _moveSpeed, 0);
        }

        public void Draw() {
            
        }
    }
}
