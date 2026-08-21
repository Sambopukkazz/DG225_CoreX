using MonoGame.Extended;
using MonoGame.Extended.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophonia {
    public class GameObject {
        public bool IsActive { get; set; }
        public bool IsVisible { get; set; }
        private GameObject _parent;
        private List<GameObject> _child = new();
    }
}
