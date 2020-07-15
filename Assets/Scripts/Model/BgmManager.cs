using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AudioConfig
{
    public static class BgmManager
    {
        public static int minBgmVolume = 0, maxBgmVolume = 10;

        public static int BgmVolume { get; set; } = PlayerPrefs.GetInt("bgmVolume");

        public static void SetBgmVolume(int volume)
        {
            if (volume < minBgmVolume)
                volume = minBgmVolume;
            if (volume > maxBgmVolume)
                volume = maxBgmVolume;
            BgmVolume = volume;
            PlayerPrefs.SetInt("bgmVolume", BgmVolume);
        }

        public static void SetBgmVolumeToObject(GameObject obj)
        {
            if (obj.GetComponent<AudioSource>() != null)
            {
                obj.GetComponent<AudioSource>().volume = (float)BgmVolume / 10;
            }
        }

        public static void PlayBgm(GameObject objToPlay, AudioClip soundClip)
        {
            SetBgmVolumeToObject(objToPlay);
            objToPlay.GetComponent<AudioSource>().Stop();
            objToPlay.GetComponent<AudioSource>().clip = soundClip;
            objToPlay.GetComponent<AudioSource>().Play();
        }
    }
}
