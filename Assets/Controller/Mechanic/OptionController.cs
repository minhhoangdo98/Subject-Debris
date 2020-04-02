using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AudioConfig;

public class OptionController : MonoBehaviour
{
    public TittleController titleController;
    public Slider SoundSlider, BgmSlider;

    private void Start()
    {
        SoundSlider.value = PlayerPrefs.GetInt("soundVolume");
        BgmSlider.value = PlayerPrefs.GetInt("bgmVolume");
    }

    public void ButtonOkClick()
    {
        SoundManager.SetSoundVolume((int)SoundSlider.value);
        BgmManager.SetBgmVolume((int)BgmSlider.value);
        SoundManager.SetSoundVolumeToObject(titleController.rainSound);
        SoundManager.SetSoundVolumeToObject(titleController.buttonClickSound);
        BgmManager.SetBgmVolumeToObject(titleController.music);
    }
}
