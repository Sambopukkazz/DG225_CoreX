using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophobia {
    public class SceneData {
        public List<TriggerData> Triggers { get; set; } = new();
        public List<LightData> Lights { get; set; } = new();
    }
}
