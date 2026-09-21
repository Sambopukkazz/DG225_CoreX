using Microsoft.Xna.Framework;
using MonoGame.Extended.Tilemaps;
using Penumbra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophobia {
    public class Scene {
        public string Name { get; private set; }
        public Tilemap TileMap { get; }
        public List<SpawnPoint> PlayerSpawnPoints { get; }

        public Scene(string sceneName, CollisionManager collisionManager, LightManager lightManager) {
            Name = sceneName;
            TileMap = MainGame.Content.Load<Tilemap>($"Tilemaps/{sceneName}");
            PlayerSpawnPoints = new List<SpawnPoint>();
            BuildCollision(collisionManager);
            BuildLight(lightManager);
        }

        private void BuildCollision(CollisionManager collisionManager) {
            collisionManager.ClearCollider();

            TilemapObjectLayer objectLayer = TileMap.Layers["Collision"] as TilemapObjectLayer;

            foreach (TilemapRectangleObject entity in objectLayer.GetObjects<TilemapRectangleObject>()) {
                if (entity.Class == "wall") {
                    collisionManager.AddCollider(new Wall(entity.Id, entity.Position, entity.Size), "walls");
                }
                else if (entity.Class == "dots" || entity.Class == "hideout") {
                    collisionManager.AddCollider(new Trigger(entity.Id, entity.Position, entity.Size, entity.Class), "triggers");
                }
                else if (entity.Class == "door") {
                    string destination = entity.Properties.GetString("Destination");
                    collisionManager.AddCollider(new Trigger(entity.Id, entity.Position, entity.Size, entity.Class, destination), "triggers");
                }
                else if (entity.Class == "spawn") {
                    string origin = entity.Properties.GetString("Origin");
                    PlayerSpawnPoints.Add(new SpawnPoint(origin,entity.Position));
                }
            }
        }

        private void BuildLight(LightManager lightManager) {
            lightManager.ClearLight();

            TilemapObjectLayer objectLayer = TileMap.Layers["Lighting"] as TilemapObjectLayer;

            foreach (TilemapRectangleObject entity in objectLayer.GetObjects<TilemapRectangleObject>()) {
                if (entity.Class == "light") {
                    if (entity.Name == "PointLight") {
                        PointLight pointLight = new PointLight();
                        pointLight.Position = entity.Position;
                        pointLight.Intensity = entity.Properties.GetFloat("Intensity");
                        float scale = entity.Properties.GetFloat("Scale");
                        //float scaleX = entity.Properties.GetFloat("ScaleX");
                        //float scaleY = entity.Properties.GetFloat("ScaleY");
                        pointLight.Scale = new Vector2(scale, scale);
                        pointLight.Color = entity.Properties.GetColor("Color");
                        lightManager.AddLight(pointLight);
                    }
                    else if (entity.Name == "SpotLight") {
                        Spotlight spotlight = new Spotlight();
                        spotlight.Rotation = MathHelper.ToRadians(entity.Properties.GetFloat("Rotation"));
                        spotlight.Position = entity.Position;
                        spotlight.Intensity = entity.Properties.GetFloat("Intensity");
                        float scaleX = entity.Properties.GetFloat("ScaleX");
                        float scaleY = entity.Properties.GetFloat("ScaleY");
                        spotlight.Scale = new Vector2(scaleX, scaleY);
                        spotlight.Color = entity.Properties.GetColor("Color");
                        lightManager.AddLight(spotlight);
                    }
                }
            }
        }
    }
}
