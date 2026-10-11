using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophobia {
    public class SceneSave {
        public List<TriggerSave> Triggers { get; set; } = new();
        public List<LightSave> Lights { get; set; } = new();
    }
}
