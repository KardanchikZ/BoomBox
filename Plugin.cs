using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using System;
using System.Collections.Generic;
using System.IO;

namespace BoomBox
{
    public class Plugin : Plugin<Config>
    {
        public string Name => "BoomBox";

        public string Prefix => "BoomBox";

        public string Author => "Kardanchik";

        public static Plugin Instance { get; private set; }

        public static List<String> Musics = null;

        public void OnEnabled()
        {
            Instance = this;
            LoadMusics();
            CustomItem.RegisterItems();

            base.OnEnabled();
        }

        public void OnDisabled()
        {
            Instance = null;
            CustomItem.UnregisterItems();
            base.OnDisabled();
        }

        private void LoadMusics()
        {
            string audioFolderPath = "Audios";

            if (Directory.Exists(audioFolderPath))
            {
                string[] musicFiles = Directory.GetFiles(audioFolderPath, "*.ogg");

                foreach (string filePath in musicFiles)
                {
                    Musics.Add(Path.GetFileName(filePath));
                }
            }
        }
    }
}
