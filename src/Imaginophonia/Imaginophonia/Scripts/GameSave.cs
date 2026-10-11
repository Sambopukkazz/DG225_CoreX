using Gum.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophobia {
    public class GameSave {
        public Dictionary<string, SceneSave> Scenes { get; set; } = new();
    }
}
