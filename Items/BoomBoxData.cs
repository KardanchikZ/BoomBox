using ChilliNoRules.Utils.Models;
using Exiled.API.Features;
using System.Collections.Generic;

namespace ChilliNoRules.Items.Scp3127
{
    public static class BoomBoxData
    {
        public static Dictionary<Player, SchematicWithAudio> BoomBoxes { get; set; } = new();
    }
}
