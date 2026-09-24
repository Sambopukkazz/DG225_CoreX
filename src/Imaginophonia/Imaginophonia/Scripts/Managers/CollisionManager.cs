using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Collisions.Layers;
using MonoGame.Extended.Input;
using MonoGame.Extended.Screens;
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
        private Player _player;

        public event Action<string> CallLoadScene;

        public CollisionManager(Player player) {
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

            _player = player;
        }
        public void Update() {
            _collisionWorld.RebuildDynamicLayers();

            foreach (CollisionEvent2D collision in _collisionWorld.QueryCollisions(_player, "walls")) {
                _player.CollideWithWallMove(collision.Result.MinimumTranslationVector);
            }

            //if(_collisionWorld.)
            _player.CanInteract = false;

            foreach (CollisionEvent2D collision in _collisionWorld.QueryCollisions(_player, "triggers")) {
                Trigger trigger = (Trigger)collision.Other;
                if(trigger.Tag == "hideout") {
                    if(KeyboardExtended.GetState().WasKeyPressed(Keys.Space)) {
                        _player.ToggleHide();
                    }
                }
                else if(trigger.Tag == "skillcheck") {
                    if (KeyboardExtended.GetState().WasKeyPressed(Keys.Space) && player.CanRepair) {
                        //Skill Check
                        SpinTheValveScreen screen = new SpinTheValveScreen();
                        screen.ExitMiniGame += OnExitMiniGame;
                        MainGame.ScreenManager.ShowScreen(screen);
                        _player.ToggleRepair();
                    }
                    
                }
                else if (trigger.Tag == "dots") {
                    if (KeyboardExtended.GetState().WasKeyPressed(Keys.Space) && player.CanRepair) {
                        //Connect the dot
                        SkillCheckScreen screen = new SkillCheckScreen();
                        screen.ExitMiniGame += OnExitMiniGame;
                        MainGame.ScreenManager.ShowScreen(screen);
                        _player.ToggleRepair();
                    }
                    
                }
                else if (trigger.Tag == "door") {
                    if (KeyboardExtended.GetState().WasKeyPressed(Keys.Space)) {
                        //Load to next scene
                        CallLoadScene?.Invoke(trigger.Name);
                    }
                }

                //Make player acknowledge that they can interact
                if(trigger.Interactable)_player.CanInteract = true;
            }

            foreach (CollisionEvent2D collision in _collisionWorld.QueryCollisions(_player, "enemies")) {
                if (_player.Active) {
                    GameObject enemy = (GameObject)collision.Other;
                    if(enemy.Name == "Autophobia") {
                        enemy.SetActive(false);
                    }
                    _collisionWorld.Remove(collision.Other);
                    _colliders.Remove(collision.Other);
                    //lose sanity
                    _player.Sanity -= 2;
                }
            }

            for (int i = _colliders.Count - 1; i >= 0; i--) {
                if (_player.Active) {
                    if (_colliders[i].GetType().Name == "Autophobia" && _colliders[i].Shape.Intersects(_player.EyeSight)) {
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

        public void OnExitMiniGame() {
            _player.ToggleRepair();
        }
    }
}
