using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BoomBox
{
    public static class AudioUtility
    {
        private static Config _config => Plugin.Instance.Config;

        // Возвращает AudioPlayer
        public static AudioPlayer CreateAndPlayAudio(string FileName, string audioPlayerName, bool loop, Vector3 position, bool destroyOnEnd = false, Transform parent = null, bool isSpatial = false, float maxDistance = 5, float minDistance = 5, float volume = 1f)
        {
            AudioPlayer audioPlayer = AudioPlayer.CreateOrGet(audioPlayerName);

            FileName = FileName += ".ogg";
            string fullPath = Path.Combine(_config.AudioFolder, FileName);

            if (!audioPlayer.TryGetSpeaker(audioPlayerName, out Speaker speaker))
            {
                speaker = audioPlayer.AddSpeaker(audioPlayerName, isSpatial: isSpatial, maxDistance: maxDistance, minDistance: minDistance, volume: volume);
            }

            if (parent)
            {
                speaker.transform.SetParent(parent);
                speaker.transform.localPosition = Vector3.zero;
                speaker.transform.localRotation = Quaternion.identity;
            }
            else
                speaker.Position = position;

            if (!AudioClipStorage.AudioClips.ContainsKey(FileName))
                AudioClipStorage.LoadClip(fullPath, FileName);

            audioPlayer.AddClip(FileName, destroyOnEnd: destroyOnEnd, loop: loop);

            return audioPlayer;
        }
    }
}
