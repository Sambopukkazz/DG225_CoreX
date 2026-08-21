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
    public class SpriteRenderer{
        private Texture2D texture;
        private Transform2 _transform;
        public bool IsActive { get; set; }
        public bool IsVisible { get; set; }
        public float Alpha { get; set; }
        public float Depth { get; set; }
        public object Tag { get; set; }
        public Vector2 Origin { get; set; }
        public SpriteEffects Effect { get; set; }
        
        public SpriteRenderer(Texture2D texture){
            this.texture = texture;
            IsActive = true;
            IsVisible = true;
            Alpha = 1.0f;
            Depth = 0f;
            
            
            
        }
        //public SpriteRenderer(Texture2D texture) {
        //    this.texture = texture;

        //}

        public void Update() {
            
        }

        public void Draw() {
            
        }
    }
}
