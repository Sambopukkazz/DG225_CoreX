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
    public class CollisionManager : IDisposable {
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
                        AudioManager.Instance.PlaySFX(SoundEffectName.ElecRepairing);
                        _player.ToggleRepair();
                        ObjectiveManager.Complete("repair");
                    }
                    
                }
                else if (trigger.CompareTag("door")) {
                    if (KeyboardExtended.GetState().WasKeyPressed(Keys.Space)) {
                        ObjectiveManager.Complete("door");
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
                        MainGame.Saturation = 0;
                        //RemoveCollider(collision.Other);
                        AudioManager.Instance.PlaySFX(SoundEffectName.PhobiaHit);
                    }
                    else if (enemy.Name == "Scopophobia" && enemy.Visible) {
                        _player.Sanity -= 25;
                        enemy.Visible = false;
                        AudioManager.Instance.PlaySFX(SoundEffectName.PhobiaHit);
                    }                       
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

            _player.Nyctophobia = false;
            //Player with light
            foreach (CollisionEvent2D collision in _collisionWorld.QueryCollisions(_player, "lights")) {
                LightArea lightArea = (LightArea)collision.Other;
                if (lightArea.On == false && lightArea.IsFlickering == false) {
                    AudioManager.Instance.PlaySFX(SoundEffectName.Darkness, playAfterLastCompleted: true);
                    _player.Nyctophobia = true;
                }
            }

            //Enemy with light
            foreach (CollisionPair2D collision in _collisionWorld.QueryCollisionPairs("enemies", "lights")) {
                LightArea lightArea = (LightArea)collision.Second;
                GameObject enemy = (GameObject)collision.First;

                if (enemy is Scopophobia scopo && lightArea.ScopoHasPassed == false) {
                    lightArea.TurnOff();
                    lightArea.ScopoHasPassed = true;
                    scopo.LightBrokeCount++;
                    AudioManager.Instance.Play3DSFX(lightArea.Shape.BoundingBox.Center, SoundEffectName.LightBreak, 50, 1800);
                }
                else if (enemy.Name == "Autophobia") {
                    lightArea.StartFlicker();
                }
            }
        }

        public void AddCollider(ICollisionActor actor) {
            //Default Layer
            _collisionWorld.Insert(actor);
        }

        public void AddCollider(ICollisionActor actor, string layer) {
            _collisionWorld.Insert(actor, layer);
        }

        public void RemoveCollider(ICollisionActor actor) {
            _collisionWorld.Remove(actor);
        }

        public void ClearColliders() {          
            ResetLayers();
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

        public void Dispose() {
            FocusedTrigger = null;
            ClearColliders();
        }
    }
}
