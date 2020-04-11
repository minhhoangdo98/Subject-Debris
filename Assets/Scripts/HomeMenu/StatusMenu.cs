using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusMenu : MonoBehaviour
{
    [SerializeField]
    private Slider atkSlider, matkSlider, defSlider, mdefSlider, hpSlider, energySlider;

    private void OnEnable()
    {
        GameController gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        CharacterStat playerStat = gc.viewObj.player2d.GetComponent<CharacterStat>();
        atkSlider.value = playerStat.atk;
        matkSlider.value = playerStat.matk;
        defSlider.value = playerStat.def;
        mdefSlider.value = playerStat.mdef;
        hpSlider.value = playerStat.maxHp;
        energySlider.value = playerStat.maxEnergy;
    }
}
