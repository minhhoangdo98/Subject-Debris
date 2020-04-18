using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusMenu : MonoBehaviour
{
    [SerializeField]
    private Slider atkSlider, matkSlider, defSlider, mdefSlider, hpSlider, energySlider;
    [SerializeField]
    private Text strNum, intNum, vitNum, energyGemNum;
    private GameController gc;
    private CharacterStat playerStat;

    private void OnEnable()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        playerStat = gc.viewObj.player2d.GetComponent<CharacterStat>();
        UpdateStat();
    }

    private void UpdateStat()
    {
        strNum.text = playerStat.str.ToString();
        intNum.text = playerStat.intl.ToString();
        vitNum.text = playerStat.vit.ToString();
        energyGemNum.text = PlayerPrefs.GetInt("BagEnergy gemcount").ToString();
        atkSlider.value = playerStat.atk;
        matkSlider.value = playerStat.matk;
        defSlider.value = playerStat.def;
        mdefSlider.value = playerStat.mdef;
        hpSlider.value = playerStat.maxHp;
        energySlider.value = playerStat.maxEnergy;
    }
    
    public void ButtonAddStat(string statName)
    {
        int gemCountInBag = PlayerPrefs.GetInt("BagEnergy gemcount");
        if (gemCountInBag >= 1)
        {
            //giam gem
            int slotUsed = PlayerPrefs.GetInt("BagslotUsed");
            gemCountInBag --;//giam so luong xuong
            //neu bang 0 thi remove item khoi bag
            if (gemCountInBag <= 0)
            {
                PlayerPrefs.SetInt("BagEnergy gemcount", 0);
                for (int j = 0; j < slotUsed; j++)
                {
                    string slotUrl = PlayerPrefs.GetString("Bag" + "slotUrl" + j);
                    if (("Prefabs/Items/Other/Energy gem") == slotUrl)
                        PlayerPrefs.SetString("Bag" + "slotUrl" + j, "");
                }
            }
            else
            {
                PlayerPrefs.SetInt("BagEnergy gemcount", gemCountInBag);
            }
            //them stat
            playerStat.AddStatToPlayer(statName, 1);
            UpdateStat();
        }
    }

    public void ButtonReset()
    {
        playerStat.ResetStat();
        UpdateStat();
    }
}
