using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using UnityEngine;

namespace BoomBox.Items
{
    [CustomItem(ItemType.Radio)]
    public class BoomBox : CustomWeapon
    {
        public override uint Id { get; set; } = Config.IdBoomBox;
        public override string Name { get; set; } = "BoomBox";
        public override string Description { get; set; } = "Play Music";
        public override float Weight { get; set; } = 1f;
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            RoomSpawnPoints = new()
            {
                new RoomSpawnPoint
                {
                    Chance = 100,
                    Room = RoomType.HczNuke,
                    Offset = new Vector3(0, 0, 0)
                }
            }
        };

        int MusicId = 0;

        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.TogglingRadio += OnTogglingRadio;
            Exiled.Events.Handlers.Map.PickupAdded += PickupAdded;
            Exiled.Events.Handlers.Player.ChangingRadioPreset += ChangingRadioPreset;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.TogglingRadio -= OnTogglingRadio;
            Exiled.Events.Handlers.Map.PickupAdded -= PickupAdded;
            Exiled.Events.Handlers.Player.ChangingRadioPreset -= ChangingRadioPreset;
            base.UnsubscribeEvents();
        }

        private void PickupAdded(PickupAddedEventArgs ev)
        {
            if (ev.Pickup is RadioPickup radio)
            {
                radio.IsEnabled = false;
            }
        }

        private void OnTogglingRadio(TogglingRadioEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem)) return;
            ev.IsAllowed = false;
            
        }

        private void ChangingRadioPreset(ChangingRadioPresetEventArgs ev)
        {
            ev.IsAllowed = true;
            ev.
        }
    }
}
