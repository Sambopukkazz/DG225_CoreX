using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tilemaps;
using Penumbra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophobia {
    public class SceneBuilder {
        public string Name { get; private set; }
        public Tilemap TileMap { get; }
        public List<SpawnPoint> PlayerSpawnPoints { get; }
        public List<Trigger> Triggers { get; }

        public SceneBuilder(string sceneName, CollisionManager collisionManager, LightManager lightManager) {
            Name = sceneName;
            TileMap = MainGame.Content.Load<Tilemap>($"Tilemaps/{sceneName}");
            PlayerSpawnPoints = new List<SpawnPoint>();
            Triggers = new List<Trigger>();
            BuildCollision(collisionManager);
            BuildLight(lightManager, collisionManager);
        }

        private void BuildCollision(CollisionManager collisionManager) {
            collisionManager.ClearColliders();

            TilemapObjectLayer objectLayer = TileMap.Layers["Collision"] as TilemapObjectLayer;

            foreach (TilemapRectangleObject entity in objectLayer.GetObjects<TilemapRectangleObject>()) {
                if (entity.Class == "wall") {
                    collisionManager.AddCollider(new Wall(entity.Id, entity.Position, entity.Size), "walls");
                }
                else if (entity.Class == "dots" || entity.Class == "hideout" || entity.Class == "skillcheck") {
                    Trigger trigger = new Trigger(entity.Id, entity.Position, entity.Size, entity.Class);
                    BuildHighlightTiles(trigger, entity);
                    collisionManager.AddCollider(trigger, "triggers");
                    Triggers.Add(trigger);
                }
                else if (entity.Class == "door") {
                    string destination = entity.Properties.GetString("Destination");
                    Trigger trigger = new Trigger(entity.Id, entity.Position, entity.Size, entity.Class, destination);
                    BuildHighlightTiles(trigger, entity);
                    collisionManager.AddCollider(trigger, "triggers");
                    Triggers.Add(trigger);
                }
                else if (entity.Class == "report") {
                    string detail = entity.Properties.GetString("Detail");
                    Trigger trigger = new Trigger(entity.Id, entity.Position, entity.Size, entity.Class, detail);
                    BuildHighlightTiles(trigger, entity);
                    collisionManager.AddCollider(trigger, "triggers");
                    Triggers.Add(trigger);
                }
                else if (entity.Class == "spawn") {
                    string origin = entity.Properties.GetString("Origin");
                    PlayerSpawnPoints.Add(new SpawnPoint(origin,entity.Position));
                }
            }
        }

        private void BuildLight(LightManager lightManager , CollisionManager collisionManager) {
            lightManager.ClearLights();

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

                        LightArea lightArea = new LightArea(pointLight, entity.Position, entity.Id, entity.Class);
                        lightManager.AddLight(lightArea);
                        collisionManager.AddCollider(lightArea, "lights");
                    }
                    else if (entity.Name == "DoorLight") {
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

        private void BuildHighlightTiles(Trigger trigger, TilemapRectangleObject entity) {
            TilemapTileLayer layer = FindHighlightLayer(entity);

            if (layer == null) {
                return;
            }

            trigger.TileLayer = layer;

            Point topLeft = TileMap.WorldToTilePosition(entity.Position);
            Point bottomRight = TileMap.WorldToTilePosition(entity.Position + entity.Size - new Vector2(1, 1));
            Rectangle colliderRect = new Rectangle(topLeft.X, topLeft.Y, Math.Max(1, bottomRight.X - topLeft.X + 1), Math.Max(1, bottomRight.Y - topLeft.Y + 1));

            foreach (TilemapTileEntry tile in layer.GetTilesInRegion(colliderRect)) {
                trigger.Tiles.Add(tile);
            }
        }

        //private void BuildHighlightTiles(Trigger trigger, TilemapRectangleObject entity) {
        //    TilemapTileLayer layer = FindHighlightLayer(entity.Name);

        //    if (layer == null) {
        //        return;
        //    }

        //    trigger.TileLayer = layer;

        //    Dictionary<(int, int), TilemapTileEntry> tileIndex = new Dictionary<(int, int), TilemapTileEntry>();

        //    foreach (TilemapTileEntry tile in layer.GetTiles()) {
        //        tileIndex[(tile.X, tile.Y)] = tile;
        //    }

        //    // The collider is smaller than the artwork, so flood fill outwards from a tile inside it
        //    Point topLeft = TileMap.WorldToTilePosition(entity.Position);
        //    Point bottomRight = TileMap.WorldToTilePosition(entity.Position + entity.Size - new Vector2(1, 1));
        //    Rectangle colliderRect = new Rectangle(topLeft.X, topLeft.Y, Math.Max(1, bottomRight.X - topLeft.X + 1), Math.Max(1, bottomRight.Y - topLeft.Y + 1));

        //    Stack<TilemapTileEntry> pending = new Stack<TilemapTileEntry>();
        //    HashSet<(int, int)> visited = new HashSet<(int, int)>();

        //    foreach (TilemapTileEntry tile in layer.GetTiles()) {
        //        if (colliderRect.Contains(tile.X, tile.Y)) {
        //            pending.Push(tile);
        //            break;
        //        }
        //    }

        //    while (pending.Count > 0) {
        //        TilemapTileEntry current = pending.Pop();

        //        if (visited.Contains((current.X, current.Y))) {
        //            continue;
        //        }

        //        visited.Add((current.X, current.Y));
        //        trigger.Tiles.Add(current);

        //        if (tileIndex.TryGetValue((current.X + 1, current.Y), out TilemapTileEntry right)) {
        //            pending.Push(right);
        //        }
        //        if (tileIndex.TryGetValue((current.X - 1, current.Y), out TilemapTileEntry left)) {
        //            pending.Push(left);
        //        }
        //        if (tileIndex.TryGetValue((current.X, current.Y + 1), out TilemapTileEntry down)) {
        //            pending.Push(down);
        //        }
        //        if (tileIndex.TryGetValue((current.X, current.Y - 1), out TilemapTileEntry up)) {
        //            pending.Push(up);
        //        }
        //    }
        //}

        // An object's entity name matches its tile layer name, except the doors
        private TilemapTileLayer FindHighlightLayer(TilemapRectangleObject entity) {
            if (string.IsNullOrEmpty(entity.Name)) {
                return null;
            }

            if (entity.Name == "LoadNextScene" && entity.Properties.GetString("Destination") == "end") {
                return FindTileLayer("Ladder");
            }
            else if (entity.Name == "LoadNextScene") {
                return FindTileLayer("Doors");
            }
            

            string wanted = entity.Name.TrimEnd('s', 'S');

            foreach (TilemapLayer layer in TileMap.Layers) {
                if (layer is TilemapTileLayer tileLayer && string.Equals(tileLayer.Name.TrimEnd('s', 'S'), wanted, StringComparison.OrdinalIgnoreCase)) {
                    return tileLayer;
                }
            }

            return null;
        }

        private TilemapTileLayer FindTileLayer(string name) {
            foreach (TilemapLayer layer in TileMap.Layers) {
                if (layer is TilemapTileLayer tileLayer && string.Equals(tileLayer.Name, name, StringComparison.OrdinalIgnoreCase)) {
                    return tileLayer;
                }
            }

            return null;
        }
    }
}
