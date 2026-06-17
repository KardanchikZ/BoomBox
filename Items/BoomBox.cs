using ChilliNoRules.Items.Scp3127;
using ChilliNoRules.Utils.Models;
using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using ProjectMER.Features;
using ProjectMER.Features.Objects;
using System.Linq;
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
            Exiled.Events.Handlers.Player.DroppedItem += DroppedItem;
            Exiled.Events.Handlers.Player.Died += Died;
            Exiled.Events.Handlers.Player.ChangingRole += ChangingRole;
            Exiled.Events.Handlers.Player.Left += Left;
            Exiled.Events.Handlers.Server.RestartingRound += RestartingRound;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.TogglingRadio -= OnTogglingRadio;
            Exiled.Events.Handlers.Map.PickupAdded -= PickupAdded;
            Exiled.Events.Handlers.Player.ChangingRadioPreset -= ChangingRadioPreset;
            Exiled.Events.Handlers.Player.DroppedItem -= DroppedItem;
            Exiled.Events.Handlers.Player.Died -= Died;
            Exiled.Events.Handlers.Player.ChangingRole -= ChangingRole;
            Exiled.Events.Handlers.Player.Left -= Left;
            Exiled.Events.Handlers.Server.RestartingRound -= RestartingRound;
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
            Clear(ev.Player);
            if (!Check(ev.Player.CurrentItem)) return;
            ev.IsAllowed = false;
            Create(ev.Player, Plugin.Musics[MusicId]);
        }

        private void ChangingRadioPreset(ChangingRadioPresetEventArgs ev)
        {
            Clear(ev.Player);
            if (!Check(ev.Player.CurrentItem)) return;
            ev.IsAllowed = false;
            MusicId = +1;
            if (MusicId > Plugin.Musics.Count)
            {
                MusicId = 0;
            }
            ev.Player.ShowHint($"Id: {MusicId}, Music Name: {Plugin.Musics[MusicId]}", 3);
        }

        private void Create(Player player, string nameAudio)
        {
            if (player == null) return;

            SchematicObject schematic = ObjectSpawner.SpawnSchematic(Config.SchematicName, player.Position - new Vector3(0, 0.3f, 0), new Vector3(0, player.Rotation.eulerAngles.y, 0));
            if (schematic == null)
            {
                Log.Error($"[SCP3127] Failed spawn schematic {Config.SchematicName}");
                return;
            }

            AudioPlayer audioPlayer = AudioUtility.CreateAndPlayAudio(nameAudio, $"boombox_{UnityEngine.Random.Range(0, 999999)}", false, schematic.Position, true, schematic.transform, true, 50, 5, 1f);

            if (audioPlayer == null)
            {
                Log.Error($"[SCP3127] Failed spawn audio player");
                return;
            }
            schematic.transform.parent = player.Transform;
            SchematicWithAudio schematicWithAudio = new();
            schematicWithAudio.Schematic = schematic;
            schematicWithAudio.AudioPlayer = audioPlayer;


            BoomBoxData.BoomBoxes.Add(player, schematicWithAudio);
        }
        private void Clear(Player player)
        {
            if (!BoomBoxData.BoomBoxes.ContainsKey(player)) return;

            if (!BoomBoxData.BoomBoxes.TryGetValue(player, out SchematicWithAudio schemaWithAudio)) return;

            schemaWithAudio.Schematic?.Destroy();
            schemaWithAudio.AudioPlayer?.Destroy();
            BoomBoxData.BoomBoxes.Remove(player);
        }

        private void Left(LeftEventArgs ev)
        {
            Clear(ev.Player);
        }

        private void ChangingRole(ChangingRoleEventArgs ev)
        {
            Clear(ev.Player);
        }

        private void Died(DiedEventArgs ev)
        {
            Clear(ev.Player);
        }

        private void DroppedItem(DroppedItemEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem)) return;
            Clear(ev.Player);
        }

        private void RestartingRound()
        {
            foreach (Player player in Player.List)
            {
                Clear(player);
            }
        }
    }
}
