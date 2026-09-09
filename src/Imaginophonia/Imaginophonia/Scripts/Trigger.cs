using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;

namespace Imaginophonia  {
    public class Trigger : ICollisionActor {
        public int Id { get; }
        public CollisionShape2D Shape { get; }
    }
}
