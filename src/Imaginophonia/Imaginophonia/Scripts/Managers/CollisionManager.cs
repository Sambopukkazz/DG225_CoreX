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
        //private List<ICollisionActor> _colliders;
        private List<GameObject> _enemies;
        private Player _player;
        public event Action<string> DoorInteracted;
        public Trigger FocusedTrigger { get; private set; }

        public CollisionManager(Player player, List<GameObject> enemyList) {
            Layer defaultLayer = new Layer(new SpatialHash(new SizeF(64f, 64f))) {
                IsDynamic = true
            };

            _collisionWorld = new CollisionWorld2D(defaultLayer);

            //_colliders = new List<ICollisionActor>();

            _player = player;
            _enemies = enemyList;
        }
        public void Update() {
            _collisionWorld.RebuildDynamicLayers();

            //Player with walls
            foreach (CollisionEvent2D collision in _collisionWorld.QueryCollisions(_player, "walls")) {
                _player.CollideWithWallMove(collision.Result.MinimumTranslationVector);
            }

            //if(_collisionWorld.)
            _player.CanInteract = false;
            FocusedTrigger = null;
            //Player with Minigame triggers
            foreach (CollisionEvent2D collision in _collisionWorld.QueryCollisions(_player, "triggers")) {
                Trigger trigger = (Trigger)collision.Other;

                // Nearest wins, so overlapping triggers don't leave the highlight on the wrong object
                if (FocusedTrigger == null) {
                    FocusedTrigger = trigger;
                }
                else if ((_player.Transform.Position - trigger.Shape.BoundingBox.Center).LengthSquared() < (_player.Transform.Position - FocusedTrigger.Shape.BoundingBox.Center).LengthSquared()) {
                    FocusedTrigger = trigger;
                }

                if (trigger.CompareTag("hideout")) {
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
                    if (KeyboardExtended.GetState().WasKeyPressed(Keys.Space) && _player.CanRepair && trigger.Interactable) {
                        //Skill Check
                        Screen screen = new SpinTheValveScreen(_player, trigger);
                        MainGame.ScreenManager.ShowScreen(screen);
                        _player.ToggleRepair();
                    }
                    
                }
                else if (trigger.CompareTag("dots")) {
                    if (KeyboardExtended.GetState().WasKeyPressed(Keys.Space) && _player.CanRepair && trigger.Interactable) {
                        //Connect the dot
                        Screen screen = new ConnectTheDotsScreen(_player, trigger);
                        MainGame.ScreenManager.ShowScreen(screen);
                        AudioManager.Instance.PlaySFX(Sound.ElecRepairing);
                        _player.ToggleRepair();
                        ObjectiveManager.Complete("repair");
                    }
                    
                }
                else if (trigger.CompareTag("door")) {
                    if (KeyboardExtended.GetState().WasKeyPressed(Keys.Space)) {
                        //Load to next scene
                        //AudioManager.Instance.PlaySFX(Sound.OpenDoor);
                        //temp
                        ObjectiveManager.Complete("door");
                        //tempend
                        DoorInteracted?.Invoke(trigger.Name);
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
                        MainGame.Saturation = 0;
                        //RemoveCollider(collision.Other);
                    }
                    else if (enemy.Name == "Scopophobia" && enemy.Visible) {
                        _player.Sanity -= 25;
                        enemy.Visible = false;
                        //_colliders.Remove(collision.Other);
                    }
                    //else if (enemy.Name == "Scopophobia" && enemy.Visible) {
                    //    _player.Sanity -= 25;
                    //    enemy.Visible = false;
                    //}                               
                }
            }

            //Player's eyesight with autophobia
            if (_player.Active) {
                foreach (GameObject enemy in _enemies) {
                    if (enemy is not Autophobia autophobia) continue;
                    if (!autophobia.AllowMovement) continue;
                    if (!autophobia.Shape.Intersects(_player.EyeSight)) continue;

                    autophobia.AllowMovement = false;
                    _collisionWorld.Remove(autophobia); 
                    Time.AddTimer(autophobia.SetActive, 0.15f);
                    _player.Blink = true;
                }
            }
            //for (int i = _enemies.Count - 1; i >= 0; i--) {
            //    if (_player.Active) {
            //        if (_enemies[i].GetType().Name == "Autophobia" && _enemies[i].Shape.Intersects(_player.EyeSight)) {
            //            Autophobia enemy = (Autophobia)_enemies[i];
            //            Time.AddTimer(enemy.SetActive, 0.25f);
            //            enemy.AllowMovement = false;
            //            //Trigger Blink
            //            _player.Blink = true;
            //        }
            //    }
            //}

            _player.Nyctophobia = false;
            //Player with light
            foreach (CollisionEvent2D collision in _collisionWorld.QueryCollisions(_player, "lights")) {
                LightArea lightArea = (LightArea)collision.Other;
                if (lightArea.On == false && lightArea.IsFlickering == false) {
                    _player.Nyctophobia = true;
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
                else if (enemy is Scopophobia scopo && lightArea.On) {
                    lightArea.TurnOff();
                    scopo.LightBrokeCount++;
                }
            }
        }
        public void AddCollider(ICollisionActor actor) {
            //Default Layer
            _collisionWorld.Insert(actor);
        }
        public void AddCollider(ICollisionActor actor, string layer) {
            _collisionWorld.Insert(actor, layer);
            //_colliders.Add(actor);
        }

        public void RemoveCollider(ICollisionActor actor) {
            _collisionWorld.Remove(actor);
            //_colliders.Remove(actor);
        }

        public void ClearColliders() {
            //foreach(var collider in _colliders) {
            //    _collisionWorld.Remove(collider);
            //}
            //_colliders.Clear();
            
            ResetLayers();
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

        private void ResetLayers() {
            _collisionWorld.RemoveLayer("walls");
            _collisionWorld.RemoveLayer("triggers");
            _collisionWorld.RemoveLayer("enemies");
            _collisionWorld.RemoveLayer("lights");

            _collisionWorld.AddLayer("walls", NewLayer(dynamic: false));
            _collisionWorld.AddLayer("triggers", NewLayer(dynamic: false));
            _collisionWorld.AddLayer("enemies", NewLayer(dynamic: true));
            _collisionWorld.AddLayer("lights", NewLayer(dynamic: false));

            _collisionWorld.DisableCollisionBetweenLayers("enemies", "walls");
            _collisionWorld.DisableCollisionBetweenLayers("lights", "walls");
            _collisionWorld.EnableCollisionBetweenLayers("enemies", "lights");
        }

        private static Layer NewLayer(bool dynamic) {
            Layer layer = new Layer(new SpatialHash(new SizeF(64f, 64f))) {
                IsDynamic = dynamic
            };
            return layer;
        }
    }
}
