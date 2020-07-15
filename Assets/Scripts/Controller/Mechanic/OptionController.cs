using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AudioConfig;

public class OptionController : MonoBehaviour
{
    public TittleController titleController;
    public Dropdown graphicDropdown;
    public Slider SoundSlider, BgmSlider;

    private void OnEnable()
    {
        SoundSlider.value = PlayerPrefs.GetInt("soundVolume");
        BgmSlider.value = PlayerPrefs.GetInt("bgmVolume");
        graphicDropdown.value = PlayerPrefs.GetInt("Quality");
    }

    public void ButtonOkClick()
    {
        QualitySettings.SetQualityLevel(graphicDropdown.value, true);
        PlayerPrefs.SetInt("Quality", graphicDropdown.value);
        SoundManager.SetSoundVolume((int)SoundSlider.value);
        BgmManager.SetBgmVolume((int)BgmSlider.value);
        SoundManager.SetSoundVolumeToObject(titleController.rainSound);
        SoundManager.SetSoundVolumeToObject(titleController.buttonClickSound);
        BgmManager.SetBgmVolumeToObject(titleController.music);
    }
}
