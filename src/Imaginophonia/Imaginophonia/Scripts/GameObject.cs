using MonoGame.Extended;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGame.Extended.Timers;

namespace Imaginophobia {
    public class GameObject {

        public Transform2 Transform { get; private set; }
        public Vector2 Origin { get; set; }
        //public Matrix WorldMatrix { get; private set; }
        public GameObject Parent { get; private set; }
        public readonly List<GameObject> Children;

        private bool _active;
        public bool Active => _active;
        public bool Visible { get; set; }
        public string Tag { get; protected set; }
        public string Name { get; }

        public GameObject(string name, string tag) {
            Transform = new();
            Origin = Vector2.Zero;
            //WorldMatrix = Matrix.Identity;
            Children = new();
            _active = true;
            Visible = true;
            Name = name;
            Tag = tag;
        }

        public virtual void Update() {
            if (Parent != null) {
                //WorldMatrix = Transform.LocalMatrix * Parent.Transform.WorldMatrix;
                Transform = Parent.Transform;
            }
            else {
                //WorldMatrix = Transform.LocalMatrix;
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
