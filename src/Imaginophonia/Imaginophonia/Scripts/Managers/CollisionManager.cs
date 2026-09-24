using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Collisions.Layers;
using MonoGame.Extended.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophobia {
    public class CollisionManager {
        private CollisionWorld2D _collisionWorld;
        private List<ICollisionActor> _colliders;

        public event Action<string> CallLoadScene;

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
                IsDynamic = true
            };

            _collisionWorld = new CollisionWorld2D(defaultLayer);
            _collisionWorld.AddLayer("walls", wallLayer);
            _collisionWorld.AddLayer("triggers", triggerLayer);
            _collisionWorld.AddLayer("enemies", enemyLayer);

            _colliders = new List<ICollisionActor>();
        }
        public void Update(Player player) {
            _collisionWorld.RebuildDynamicLayers();

            foreach (CollisionEvent2D collision in _collisionWorld.QueryCollisions(player, "walls")) {
                player.CollideWithWallMove(collision.Result.MinimumTranslationVector);
            }

            //if(_collisionWorld.)
            player.CanInteract = false;

            foreach (CollisionEvent2D collision in _collisionWorld.QueryCollisions(player, "triggers")) {
                Trigger trigger = (Trigger)collision.Other;
                if(trigger.Tag == "Locker") {
                    if(KeyboardExtended.GetState().WasKeyPressed(Keys.Space)) {
                        player.ToggleHide();
                    }
                }
                else if(trigger.Tag == "Valve") {
                    if (KeyboardExtended.GetState().WasKeyPressed(Keys.Space) && player.CanRepair) {
                        //Skill Check
                        MainGame.ScreenManager.ShowScreen(new SpinTheValveScreen());
                        player.ToggleRepair();
                    }
                    
                }
                else if (trigger.Tag == "Panel") {
                    if (KeyboardExtended.GetState().WasKeyPressed(Keys.Space) && player.CanRepair) {
                        //Connect the dot
                        MainGame.ScreenManager.ShowScreen(new SkillCheckScreen());
                        player.ToggleRepair();
                    }
                    
                }
                else if (trigger.Tag == "Door") {
                    if (KeyboardExtended.GetState().WasKeyPressed(Keys.Space)) {
                        //Load to next scene
                        CallLoadScene?.Invoke(trigger.Name);
                    }
                }

                //Make player acknowledge that they can interact
                if(trigger.Interactable)player.CanInteract = true;
            }

            foreach (CollisionEvent2D collision in _collisionWorld.QueryCollisions(player, "enemies")) {
                if (player.Active) {
                    GameObject enemy = (GameObject)collision.Other;
                    Time.AddTimer(5);
                    enemy.SetActive(false);
                    _collisionWorld.Remove(collision.Other);
                    _colliders.Remove(collision.Other);
                    //lose sanity
                }
            }

            for (int i = _colliders.Count - 1; i >= 0; i--) {
                if (player.Active) {
                    if (_colliders[i].GetType().Name == "Autophobia" && _colliders[i].Shape.Intersects(player.EyeSight)) {
                        _collisionWorld.Remove(_colliders[i]);
                        GameObject enemy = (GameObject)_colliders[i];
                        enemy.SetActive(false);
                        _colliders.RemoveAt(i);
                    }
                }
            }
        }
        public void AddCollider(ICollisionActor actor) {
            //Default Layer
            _collisionWorld.Insert(actor);
        }
        public void AddCollider(ICollisionActor actor, string layer) {
            _collisionWorld.Insert(actor, layer);
            _colliders.Add(actor);
        }

        public void RemoveCollider(ICollisionActor actor) {
            _collisionWorld.Remove(actor);
        }

        public void ClearColliders() {
            foreach(var collider in _colliders) {
                _collisionWorld.Remove(collider);
            }
            _colliders.Clear();
            
            //_collisionWorld.RemoveLayer("triggers");
            //_collisionWorld.RemoveLayer("walls");

            //Layer wallLayer = new Layer(new SpatialHash(new SizeF(64f, 64f))) {
            //    IsDynamic = false
            //};
            //Layer triggerLayer = new Layer(new SpatialHash(new SizeF(64f, 64f))) {
            //    IsDynamic = false
            //};

            //_collisionWorld.AddLayer("walls", wallLayer);
            //_collisionWorld.AddLayer("triggers", triggerLayer);
        }
    }
}
