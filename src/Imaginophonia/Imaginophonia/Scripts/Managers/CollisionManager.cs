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
            Layer lightLayer = new Layer(new SpatialHash(new SizeF(64f, 64f))) {
                IsDynamic = false
            };

            _collisionWorld = new CollisionWorld2D(defaultLayer);
            _collisionWorld.AddLayer("walls", wallLayer);
            _collisionWorld.AddLayer("triggers", triggerLayer);
            _collisionWorld.AddLayer("enemies", enemyLayer);
            _collisionWorld.AddLayer("lights", lightLayer);

            _collisionWorld.DisableCollisionBetweenLayers("enemies", "walls");
            _collisionWorld.DisableCollisionBetweenLayers("lights", "walls");
            _collisionWorld.EnableCollisionBetweenLayers("enemies", "lights");

            _colliders = new List<ICollisionActor>();

            _player = player;
        }
        public void Update() {
            _collisionWorld.RebuildDynamicLayers();

            //Player with walls
            foreach (CollisionEvent2D collision in _collisionWorld.QueryCollisions(_player, "walls")) {
                _player.CollideWithWallMove(collision.Result.MinimumTranslationVector);
            }

            //if(_collisionWorld.)
            _player.CanInteract = false;
            //Player with Minigame triggers
            foreach (CollisionEvent2D collision in _collisionWorld.QueryCollisions(_player, "triggers")) {
                Trigger trigger = (Trigger)collision.Other;
                if(trigger.CompareTag("hideout")) {
                    if (_player.CurrentAction != CharacterAction.Hiding) {
                        _player.CurrentAction = CharacterAction.ReadyToHide;
                    }
                    if(KeyboardExtended.GetState().WasKeyPressed(Keys.Space)) {
                        if (_player.CurrentAction != CharacterAction.Hiding && _player.CooldownTimer == null) {
                            _player.PreviousPos = _player.Transform.Position;
                            _player.Transform.Position = trigger.Shape.BoundingBox.Center;
                        }
                        _player.ToggleHide();
                    }
                }
                else if(trigger.CompareTag("skillcheck")) {
                    if (KeyboardExtended.GetState().WasKeyPressed(Keys.Space) && _player.CanRepair) {
                        //Skill Check
                        Screen screen = new SpinTheValveScreen(_player);
                        MainGame.ScreenManager.ShowScreen(screen);
                        _player.ToggleRepair();
                    }
                    
                }
                else if (trigger.CompareTag("dots")) {
                    if (KeyboardExtended.GetState().WasKeyPressed(Keys.Space) && _player.CanRepair) {
                        //Connect the dot
                        Screen screen = new ConnectTheDotsScreen(_player);
                        MainGame.ScreenManager.ShowScreen(screen);
                        AudioManager.Instance.PlaySFX(Sound.ElecRepairing);
                        _player.ToggleRepair();
                    }
                    
                }
                else if (trigger.CompareTag("door")) {
                    if (KeyboardExtended.GetState().WasKeyPressed(Keys.Space)) {
                        //Load to next scene
                        //AudioManager.Instance.PlaySFX(Sound.OpenDoor);
                        CallLoadScene?.Invoke(trigger.Name);
                    }
                }

                //Make player acknowledge that they can interact
                if(trigger.Interactable)_player.CanInteract = true;
            }

            //Player with enemies
            foreach (CollisionEvent2D collision in _collisionWorld.QueryCollisions(_player, "enemies")) {
                if (_player.Active) {
                    GameObject enemy = (GameObject)collision.Other;
                    if(enemy.Name == "Autophobia") {
                        enemy.SetActive(false);
                        _player.Sanity -= 10;
                        AudioManager.Instance.PlaySFX(Sound.AutoIndicator);
                    }
                    else {
                        _player.Sanity -= 25;
                    }
                    _collisionWorld.Remove(collision.Other);
                    _colliders.Remove(collision.Other);
                    //lose sanity
                    
                }
            }

            //Player's eyesight with autophobia
            for (int i = _colliders.Count - 1; i >= 0; i--) {
                if (_player.Active) {
                    if (_colliders[i].GetType().Name == "Autophobia" && _colliders[i].Shape.Intersects(_player.EyeSight)) {
                        _collisionWorld.Remove(_colliders[i]);
                        Autophobia enemy = (Autophobia)_colliders[i];
                        Time.AddTimer(enemy.SetActive, 0.25f);
                        enemy.AllowMovement = false;
                        _colliders.RemoveAt(i);
                        //Trigger Blink
                        _player.Blink = true;
                    }
                }
            }

            //Player with light
            foreach (CollisionEvent2D collision in _collisionWorld.QueryCollisions(_player, "lights")) {
                LightArea lightArea = (LightArea)collision.Other;
                if (lightArea.On == false && lightArea.IsFlickering == false) {
                    _player.Sanity -= 1 * Time.DeltaTime;
                }
                //LightArea lightArea = (LightArea)collision.Other;
                //lightArea.TurnOn();
            }

            //Enemy with light
            foreach (CollisionPair2D collision in _collisionWorld.QueryCollisionPairs("enemies", "lights")) {
                LightArea lightArea = (LightArea)collision.Second;
                GameObject enemy = (GameObject)collision.First;

                if (enemy.Name == "Autophobia") {
                    lightArea.StartFlicker();
                }
                else {
                    lightArea.TurnOff();
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
            _colliders.Remove(actor);
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
