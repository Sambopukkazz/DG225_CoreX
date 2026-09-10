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

        public Scene(string name, CollisionManager collisionManager, LightManager lightManager) {
            Name = name;
            TileMap = MainGame.Content.Load<Tilemap>($"Tilemap/{name}");
            BuildCollision(collisionManager);
            BuildLight(lightManager);
        }

        private void BuildCollision(CollisionManager collisionManager) {
            TilemapObjectLayer objectLayer = TileMap.Layers["Collision"] as TilemapObjectLayer;

            foreach (TilemapRectangleObject rect in objectLayer.GetObjects<TilemapRectangleObject>()) {
                // Use as collision zones, trigger areas, etc.
                if (rect.Name == "InvisWall") {
                    collisionManager.AddCollision(new Wall(rect.Position, rect.Size), "walls");
                }
                else if (rect.Name == "ElectricalPanel" || rect.Name == "Locker") {
                    collisionManager.AddCollision(new Trigger(rect.Position, rect.Size, rect.Class), "triggers");
                }
            }
        }

        private void BuildLight(LightManager lightManager) {
            TilemapObjectLayer objectLayer = TileMap.Layers["Lighting"] as TilemapObjectLayer;

            foreach (TilemapRectangleObject rect in objectLayer.GetObjects<TilemapRectangleObject>()) {
                // Use as collision zones, trigger areas, etc.
                if (rect.Name == "PointLight") {
                    PointLight pointLight = new PointLight();
                    pointLight.Position = rect.Position;
                    pointLight.Intensity = rect.Properties.GetFloat("Intensity");
                    float scale = rect.Properties.GetFloat("Scale");
                    pointLight.Scale = new Vector2(scale, scale);
                    pointLight.Color = rect.Properties.GetColor("Color");
                    lightManager.AddLight(pointLight);
                }
                else if (rect.Name == "SpotLight") {
                    Spotlight spotlight = new Spotlight();
                    spotlight.Rotation = MathHelper.ToRadians(90);
                    spotlight.Position = rect.Position;
                    spotlight.Intensity = rect.Properties.GetFloat("Intensity");
                    float scale = rect.Properties.GetFloat("Scale");
                    spotlight.Scale = new Vector2(scale, scale);
                    spotlight.Color = rect.Properties.GetColor("Color");
                    lightManager.AddLight(spotlight);
                }
            }
        }
    }
}
