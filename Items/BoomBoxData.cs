
using Exiled.API.Features;
using System.Collections.Generic;

namespace BoomBox
{
    public static class BoomBoxData
    {
        public static Dictionary<Player, SchematicWithAudio> BoomBoxes { get; set; } = new();
    }
}
