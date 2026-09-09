using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Imaginophonia {
    public interface IMoveable {
        public float MoveSpeed { get; }
        public Vector2 Velocity { get; }
        public Vector2 Direction { get; }
    }
}
