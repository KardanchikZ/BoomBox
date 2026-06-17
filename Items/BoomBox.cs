using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomBox.Items
{
    [CustomItem(ItemType.Radio)]
    public class BoomBox : CustomWeapon
    {
        public override uint Id { get; set; } = Config.IdBoomBox;
        public override string Name { get; set; } = "BoomBox";
        public override string Description { get; set; } = "PlayMusic";
        public override float Weight { get; set; } = 1f;
        public override SpawnProperties SpawnProperties { get; set; }
    }
}
