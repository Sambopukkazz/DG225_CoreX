using MonoGame.Extended;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophonia {
    public class GameObject {

        public Transform2 Transform = new();
        public Vector2 Origin { get; set; }
        public Matrix WorldMatrix { get; private set; } = Matrix.Identity;

        public GameObject Parent;
        public readonly List<GameObject> Children = new();

        private bool _active;
        public bool Active => _active;
        public bool Visibility { get; set; }

        public virtual void Update() {
            if (Parent != null) {
                WorldMatrix = Transform.LocalMatrix * Parent.Transform.WorldMatrix;
            }
            else {
                WorldMatrix = Transform.LocalMatrix;
            }
             
            foreach (var child in Children) { 
                child.Update();
            }
        }

        public virtual void Draw() {
            foreach (var child in Children) {
                child.Draw();
            }
        }

        public void Destroy(GameObject gameObject) {
            gameObject = null;
        }
        public void Destroy(GameObject gameObject,float time) {
            gameObject = null;
        }

        public void AddChild(GameObject child) {
            child.Parent?.RemoveChild(child);
            child.Parent = this;
            Children.Add(child);
        }

        public void RemoveChild(GameObject child) {
            if (Children.Remove(child)){
                child.Parent = null;
            }
        }

        public void SetActive(bool status) { 
            _active = status;
        }
    }
}
