using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophobia {
    public class HighlightSettings {
        // Settings parameters
        public float ScanSpeed { get; set; } = 0.25f;
        public float ScanWidth { get; set; } = 0.05f;
        public Color ScanColor { get; set; } = Color.Transparent;
        public float PulseSpeed { get; set; } = 0.5f;
        public float PulseMin { get; set; } = 0.4f;
        public Color PulseColor { get; set; } = Color.Transparent;
        public float OutlineRadius { get; set; } = 6f;
        public Color OutlineColor { get; set; } = Color.Transparent;
    }
}
