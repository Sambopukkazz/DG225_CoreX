using MonoGame.Extended.Tilemaps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophobia {
    public class HighlightGroup {
        public TilemapTileLayer Layer { get; set; }
        public HighlightSettings Settings { get; set; }
        public List<TilemapTileEntry> FocusTiles { get; } = new List<TilemapTileEntry>();
        public List<TilemapTileEntry> ScanTiles { get; } = new List<TilemapTileEntry>();
        public List<TilemapTileEntry> PulseTiles { get; } = new List<TilemapTileEntry>();
    }
}
