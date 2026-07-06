using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using MEC;
using ProjectMER.Features;
using ProjectMER.Features.Objects;
using System;
using System.Threading.Tasks;
using UnityEngine;


namespace BoomBox.Items
{
    [CustomItem(ItemType.Radio)]
    public class BoomBox : CustomItem
    {
        public override uint Id { get; set; } = Config.IdBoomBox;
        public override string Name { get; set; } = "BoomBox";
        public override string Description { get; set; } = "Play Music";
        public override float Weight { get; set; } = 1f;
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {

        };
        public override Vector3 Scale { get; set; } = new Vector3(2, 2, 2);


        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.TogglingRadio += OnTogglingRadio;
            Exiled.Events.Handlers.Map.PickupAdded += PickupAdded;
            Exiled.Events.Handlers.Player.ChangingRadioPreset += ChangingRadioPreset;
            Exiled.Events.Handlers.Player.DroppingItem += DroppingItem;
            Exiled.Events.Handlers.Player.Died += Died;
            Exiled.Events.Handlers.Player.ChangingRole += ChangingRole;
            Exiled.Events.Handlers.Player.Left += Left;
            Exiled.Events.Handlers.Server.RestartingRound += RestartingRound;
            Exiled.Events.Handlers.Player.Handcuffing += Handcuffing;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.TogglingRadio -= OnTogglingRadio;
            Exiled.Events.Handlers.Map.PickupAdded -= PickupAdded;
            Exiled.Events.Handlers.Player.ChangingRadioPreset -= ChangingRadioPreset;
            Exiled.Events.Handlers.Player.DroppingItem -= DroppingItem;
            Exiled.Events.Handlers.Player.Died -= Died;
            Exiled.Events.Handlers.Player.ChangingRole -= ChangingRole;
            Exiled.Events.Handlers.Player.Left -= Left;
            Exiled.Events.Handlers.Server.RestartingRound -= RestartingRound;
            Exiled.Events.Handlers.Player.Handcuffing -= Handcuffing;
            base.UnsubscribeEvents();
        }

        private void PickupAdded(PickupAddedEventArgs ev)
        {
            if (ev.Pickup is RadioPickup radio && Check(ev.Pickup))
            {
                radio.IsEnabled = true;
            }
        }

        private void OnTogglingRadio(TogglingRadioEventArgs ev)
        {
            ev.IsAllowed = false;

            if (Plugin.Musics.Count == 0)
            {
                Log.Warn("Нет музыки!");
                return;
            }

            if (ev.Radio.BatteryLevel < 1)
            {
                ev.Player.ShowHint("Бумбокс разрядился! :(", 3);
                return;
            }

            if (ev.Player == null || ev.Player.CurrentItem == null || !Check(ev.Player.CurrentItem))
                return;

            Timing.CallDelayed(1, () =>
            {
                Clear(ev.Player);
            });
            // Получаем индекс для игрока (если нет, то 0)
            int index = 0;
            BoomBoxData.MusicIndex.TryGetValue(ev.Player, out index);
            Timing.CallDelayed(1, () =>
            {
                Create(ev.Player, Plugin.Musics[index]);
            });

            // Расход батареи: уменьшаем на 2, но не ниже 0
            byte newBattery = (byte)Math.Max(0, ev.Radio.BatteryLevel - 2);
            ev.Radio.BatteryLevel = newBattery;
        }

        private void ChangingRadioPreset(ChangingRadioPresetEventArgs ev)
        {
            ev.IsAllowed = false;

            if (Plugin.Musics.Count == 0)
                return;

            if (ev.Player == null || ev.Player.CurrentItem == null || !Check(ev.Player.CurrentItem))
                return;

            // Получаем текущий индекс
            int currentIndex = 0;
            BoomBoxData.MusicIndex.TryGetValue(ev.Player, out currentIndex);

            // Увеличиваем, зацикливаем
            int newIndex = (currentIndex + 1) % Plugin.Musics.Count;
            BoomBoxData.MusicIndex[ev.Player] = newIndex;

            ev.Player.ShowHint($"Id: {newIndex}, Музыка: {Plugin.Musics[newIndex]}", 3);

            // Если бумбокс уже играет – обновляем аудио
            Timing.CallDelayed(1, () => 
            {
                if (BoomBoxData.BoomBoxes.TryGetValue(ev.Player, out SchematicWithAudio existing))
                {
                    // Уничтожаем старый аудиоплеер
                    existing.AudioPlayer?.Destroy();
                    // Создаём новый на той же позиции схематика
                    AudioPlayer newAudio = AudioUtility.CreateAndPlayAudio(
                        Plugin.Musics[newIndex],
                        $"boombox_{UnityEngine.Random.Range(0, 999999)}",
                        false,
                        existing.Schematic.Position,
                        true,
                        existing.Schematic.transform,
                        true,
                        50, 5, 0.67f
                    );
                    existing.AudioPlayer = newAudio;
                }
            });
        }

        private void Create(Player player, string nameAudio)
        {
            if (player == null) return;

            SchematicObject schematic = ObjectSpawner.SpawnSchematic(Config.SchematicName, player.Position - new Vector3(0, -0.35f, 0), new Vector3(0, player.Rotation.eulerAngles.y, 0));
            if (schematic == null)
            {
                Log.Error($"Failed spawn schematic {Config.SchematicName}");
                return;
            }

            AudioPlayer audioPlayer = AudioUtility.CreateAndPlayAudio(nameAudio, $"boombox_{UnityEngine.Random.Range(0, 999999)}", false, schematic.Position, true, schematic.transform, true, 50, 5, 0.67f);

            if (audioPlayer == null)
            {
                Log.Error($" Failed spawn audio player");
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
            if (BoomBoxData.BoomBoxes.TryGetValue(player, out SchematicWithAudio schemaWithAudio))
            {
                schemaWithAudio.Schematic?.Destroy();
                schemaWithAudio.AudioPlayer?.Destroy();
                BoomBoxData.BoomBoxes.Remove(player);
                BoomBoxData.MusicIndex.Remove(player);
            }
        }
        private void Handcuffing(HandcuffingEventArgs ev)
        {
            Timing.CallDelayed(1, () =>
            {
                Clear(ev.Player);
            });
        }
        private void Left(LeftEventArgs ev)
        {
            Timing.CallDelayed(1, () =>
            {
                Clear(ev.Player);
            });
        }

        private void ChangingRole(ChangingRoleEventArgs ev)
        {
            Timing.CallDelayed(1, () =>
            {
                Clear(ev.Player);
            });
        }

        private void Died(DiedEventArgs ev)
        {
            Timing.CallDelayed(1, () =>
            {
                Clear(ev.Player);
            });
        }

        private void DroppingItem(DroppingItemEventArgs ev)
        {
            if (!Check(ev.Item)) return;
            Timing.CallDelayed(1, () =>
            {
                Clear(ev.Player);
            });
        }

        private void RestartingRound()
        {
            foreach (var kvp in BoomBoxData.BoomBoxes)
            {
                kvp.Value.Schematic?.Destroy();
                kvp.Value.AudioPlayer?.Destroy();
            }
            BoomBoxData.BoomBoxes.Clear();
            BoomBoxData.MusicIndex.Clear();
        }
    }
}
