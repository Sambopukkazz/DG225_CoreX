using MonoGame.Extended;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGame.Extended.Timers;

namespace Imaginophonia {
    public class GameObject {

        public Transform2 Transform { get; private set; } = new();
        public Vector2 Origin { get; set; } = Vector2.Zero;
        public Matrix WorldMatrix { get; private set; } = Matrix.Identity;

        public GameObject Parent { get; private set; }
        public readonly List<GameObject> Children = new();

        private bool _active = true;
        public bool Active => _active;
        public bool Visible { get; set; } = true;
        public string Tag { get; }
        public string Name { get; }

        public virtual void Update() {
            if (Parent != null) {
                WorldMatrix = Transform.LocalMatrix * Parent.Transform.WorldMatrix;
            }
            else {
                WorldMatrix = Transform.LocalMatrix;
            }
            //if(WorldMatrix.Decompose(out Vector3 position, out Quaternion rotation, out Vector3 scale)) {
            //    Transform.Position = new Vector2(position.X, position.Y);
            //    Transform.Rotation = rotation.Z;
            //    Transform.Scale = new Vector2(scale.X, scale.Y);
            //}
             
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
