using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Collisions.Layers;
using MonoGame.Extended.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophobia {
    public class CollisionManager {
        private CollisionWorld2D _collisionWorld;

        public CollisionManager() {
            Layer defaultLayer = new Layer(new SpatialHash(new SizeF(64f, 64f))) {
                IsDynamic = true
            };
            Layer wallLayer = new Layer(new SpatialHash(new SizeF(64f, 64f))) {
                IsDynamic = false
            };
            Layer triggerLayer = new Layer(new SpatialHash(new SizeF(64f, 64f))) {
                IsDynamic = false
            };
            Layer enemyLayer = new Layer(new SpatialHash(new SizeF(64f, 64f))) {
                IsDynamic = false
            };

            _collisionWorld = new CollisionWorld2D(defaultLayer);
            _collisionWorld.AddLayer("walls", wallLayer);
            _collisionWorld.AddLayer("triggers", triggerLayer);
            _collisionWorld.AddLayer("enemies", enemyLayer);
        }
        public void Update(Player player) {
            //_collisionWorld.RebuildDynamicLayers();

            foreach (CollisionEvent2D collision in _collisionWorld.QueryCollisions(player, "walls")) {
                player.CollideWithWallMove(collision.Result.MinimumTranslationVector);
            }

            
            //if(_collisionWorld.)
            foreach (CollisionEvent2D collision in _collisionWorld.QueryCollisions(player, "triggers")) {
                Trigger trigger = (Trigger)collision.Other;
                if(trigger.Tag == "Locker") {
                    if(KeyboardExtended.GetState().WasKeyPressed(Keys.Space)) {
                        player.ToggleHide();
                    }
                }
            }
        }
        public void AddCollision(ICollisionActor actor) {
            //Default Layer
            _collisionWorld.Insert(actor);
        }
        public void AddCollision(ICollisionActor actor, string layer) {
            _collisionWorld.Insert(actor, layer);
        }
    }
}
