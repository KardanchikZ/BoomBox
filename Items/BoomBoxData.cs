
using Exiled.API.Features;
using System.Collections.Generic;

namespace BoomBox
{
    public static class BoomBoxData
    {
        public static Dictionary<Player, SchematicWithAudio> BoomBoxes = new Dictionary<Player, SchematicWithAudio>();
        public static Dictionary<Player, int> MusicIndex = new Dictionary<Player, int>();
    }
}
