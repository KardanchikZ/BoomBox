using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BoomBox
{
    public class Plugin : Plugin<Config>
    {
        public override string Name => "BoomBox";

        public override string Prefix => "BoomBox";

        public override string Author => "Kardanchik";

        public static Plugin Instance { get; private set; }

        public static List<String> Musics = null;

        public override void OnEnabled()
        {
            Instance = this;

            Exiled.Events.Handlers.Map.Generated += MapGenerated;
            CustomItem.RegisterItems();

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Instance = null;

            Exiled.Events.Handlers.Map.Generated -= MapGenerated;
            CustomItem.UnregisterItems();

            base.OnDisabled();
        }
        private void MapGenerated()
        {
            LoadMusics();
        }

        private void LoadMusics()
        {
            string audioFolderPath = Config.AudioFolder;

            if (Directory.Exists(audioFolderPath))
            {
                string[] musicFiles = Directory.GetFiles(audioFolderPath, "*.ogg");

                foreach (string filePath in musicFiles)
                {
                    Musics.Add(Path.GetFileName(filePath));
                }
            }
            if (Musics.Count > 0)
            {
                string allMusicsFile = string.Join(Environment.NewLine, Musics.Select(e => e.ToString()));
                Log.Debug($"список музыки: {allMusicsFile}");
            }
            else
            {
                Log.Warn("Музыки нет!");
            }
        }
    }
}
