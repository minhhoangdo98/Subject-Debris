using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AudioConfig
{
    public static class SoundManager
    {
        public static int minSoundVolume = 0, maxSoundVolume = 10;

        public static int SoundVolume { get; set; } = PlayerPrefs.GetInt("soundVolume");

        public static void SetSoundVolume(int volume)
        {
            if (volume < minSoundVolume)
                volume = minSoundVolume;
            if (volume > maxSoundVolume)
                volume = maxSoundVolume;
            SoundVolume = volume;
            PlayerPrefs.SetInt("soundVolume", SoundVolume);
        }

        public static void SetSoundVolumeToObject(GameObject obj)
        {
            if (obj.GetComponent<AudioSource>() != null)
            {
                obj.GetComponent<AudioSource>().volume = (float)SoundVolume / 10;
            }
        }

        public static void PlayRandomSound(GameObject objToPlay, List<AudioClip> soundClipList)
        {
            SetSoundVolumeToObject(objToPlay);
            objToPlay.GetComponent<AudioSource>().clip = soundClipList[Random.Range(0, soundClipList.Count)];
            objToPlay.GetComponent<AudioSource>().Play();
        }

        public static void PlaySound(GameObject objToPlay, AudioClip soundClip)
        {
            SetSoundVolumeToObject(objToPlay);
            objToPlay.GetComponent<AudioSource>().clip = soundClip;
            objToPlay.GetComponent<AudioSource>().Play();
        }
    }
}

