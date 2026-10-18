using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Tilemaps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophobia {
    public class TileHighlightRenderer {
        public HighlightGroup ValveGroup { get; } = new HighlightGroup();
        public HighlightGroup LockerGroup { get; } = new HighlightGroup();
        public HighlightGroup PanelGroup { get; } = new HighlightGroup();
        public HighlightGroup DoorGroup { get; } = new HighlightGroup();

        private Effect _maskShader;
        private Effect _compositeShader;
        private RenderTarget2D _maskTarget;
        private RenderTarget2D _highlightTarget;
        private Tilemap _map;
        private float _time;

        public TileHighlightRenderer() {
            _maskShader = MainGame.Content.Load<Effect>("Shaders/Silhouette");
            _maskShader.CurrentTechnique = _maskShader.Techniques["HighlightMask"];

            _compositeShader = MainGame.Content.Load<Effect>("Shaders/HighlightComposite");
            _compositeShader.CurrentTechnique = _compositeShader.Techniques["Highlight"];
        }

        public void LoadScene(Tilemap map) {
            _map = map;
            Clear();
        }

        public void Clear() {
            ClearGroup(ValveGroup);
            ClearGroup(LockerGroup);
            ClearGroup(PanelGroup);
            ClearGroup(DoorGroup);
        }

        public void Update() {
            _time += Time.DeltaTime;
            EnsureTargets();
        }

        // Builds the highlight into its own target. Call this before the scene is drawn
        public void Render(OrthographicCamera camera) {
            GraphicsDevice device = MainGame.GraphicsDevice;
            SpriteBatch spriteBatch = MainGame.SpriteBatch;

            RenderTargetBinding[] previous = device.GetRenderTargets();

            device.SetRenderTarget(_highlightTarget);
            device.Clear(Color.Transparent);

            DrawLayer(device, spriteBatch, camera, ValveGroup);
            DrawLayer(device, spriteBatch, camera, LockerGroup);
            DrawLayer(device, spriteBatch, camera, PanelGroup);
            DrawLayer(device, spriteBatch, camera, DoorGroup);

            if (previous.Length > 0) {
                device.SetRenderTargets(previous);
            }
            else {
                device.SetRenderTarget(null);
            }
        }

        // Puts the highlight on screen. Call this after the scene is drawn - inside the lighting
        // capture to sit behind the player, or after it to sit on top
        public void Draw() {
            SpriteBatch spriteBatch = MainGame.SpriteBatch;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp);
            spriteBatch.Draw(_highlightTarget, MainGame.GraphicsDevice.Viewport.Bounds, Color.White);
            spriteBatch.End();
        }

        private void DrawLayer(GraphicsDevice device, SpriteBatch spriteBatch, OrthographicCamera camera, HighlightGroup group) {
            if (group.Layer == null || group.Settings == null || _map == null) {
                return;
            }

            if (group.FocusTiles.Count == 0 && group.ScanTiles.Count == 0 && group.PulseTiles.Count == 0) {
                return;
            }

            device.SetRenderTarget(_maskTarget);
            device.Clear(Color.Transparent);

            DrawMaskTiles(spriteBatch, camera, group, group.FocusTiles, new Vector4(1, 0, 0, 0));
            DrawMaskTiles(spriteBatch, camera, group, group.ScanTiles, new Vector4(0, 1, 0, 0));
            DrawMaskTiles(spriteBatch, camera, group, group.PulseTiles, new Vector4(0, 0, 1, 0));

            device.SetRenderTarget(_highlightTarget);
            ApplySettings(group.Settings);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, _compositeShader);
            spriteBatch.Draw(_maskTarget, device.Viewport.Bounds, Color.White);
            spriteBatch.End();
        }

        private void DrawMaskTiles(SpriteBatch spriteBatch, OrthographicCamera camera, HighlightGroup group, List<TilemapTileEntry> tiles, Vector4 channel) {
            if (tiles.Count == 0) {
                return;
            }

            _maskShader.Parameters["MaskChannel"].SetValue(channel);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, _maskShader, camera.GetViewMatrix());

            foreach (TilemapTileEntry tile in tiles) {
                DrawTile(spriteBatch, group.Layer, tile);
            }

            spriteBatch.End();
        }

        private void DrawTile(SpriteBatch spriteBatch, TilemapTileLayer layer, TilemapTileEntry tile) {
            int localId = tile.Tile.GetLocalId(_map.Tilesets, out TilemapTileset tileset);

            if (tileset == null) {
                return;
            }

            tileset.GetRenderSource(localId, out Texture2D texture, out Rectangle source);

            if (texture == null) {
                return;
            }

            Point world = _map.TileToWorldPosition(tile.X, tile.Y);
            Vector2 position = new Vector2(world.X, world.Y) + layer.Offset + tileset.TileOffset;
            position.Y += layer.TileHeight - source.Height;
            spriteBatch.Draw(texture, position, source, Color.White);
        }

        private void ApplySettings(HighlightSettings settings) {
            _compositeShader.Parameters["MaskTexel"].SetValue(new Vector2(1f / _maskTarget.Width, 1f / _maskTarget.Height));
            _compositeShader.Parameters["Time"].SetValue(_time);
            _compositeShader.Parameters["OutlineRadius"].SetValue(settings.OutlineRadius);
            _compositeShader.Parameters["ScanSpeed"].SetValue(settings.ScanSpeed);
            _compositeShader.Parameters["ScanWidth"].SetValue(settings.ScanWidth);
            _compositeShader.Parameters["PulseSpeed"].SetValue(settings.PulseSpeed);
            _compositeShader.Parameters["PulseMin"].SetValue(settings.PulseMin);
            _compositeShader.Parameters["OutlineColor"].SetValue(settings.OutlineColor.ToVector4());
            _compositeShader.Parameters["ScanColor"].SetValue(settings.ScanColor.ToVector4());
            _compositeShader.Parameters["PulseColor"].SetValue(settings.PulseColor.ToVector4());
        }

        private void EnsureTargets() {
            Viewport viewport = MainGame.GraphicsDevice.Viewport;

            if (_maskTarget != null && _maskTarget.Width == viewport.Width && _maskTarget.Height == viewport.Height) {
                return;
            }

            _maskTarget?.Dispose();
            _highlightTarget?.Dispose();

            _maskTarget = new RenderTarget2D(MainGame.GraphicsDevice, viewport.Width, viewport.Height, false, SurfaceFormat.Color, DepthFormat.None);

            // We come back to this target after every mask pass, so it must keep its contents
            _highlightTarget = new RenderTarget2D(MainGame.GraphicsDevice, viewport.Width, viewport.Height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
        }

        private void ClearGroup(HighlightGroup group) {
            group.FocusTiles.Clear();
            group.ScanTiles.Clear();
            group.PulseTiles.Clear();
        }
    }
}