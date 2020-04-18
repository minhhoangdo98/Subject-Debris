using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStat : MonoBehaviour
{
    public int str = 1, intl = 1, vit = 1, level;
    public float atk, matk, def, mdef, hp, energy, maxHp, maxEnergy;

    private void Start()
    {
        LevelInit();

        if (gameObject.CompareTag("Player"))
        {
            str = PlayerPrefs.GetInt("str") + level;
            intl = PlayerPrefs.GetInt("intl") + level;
            vit = PlayerPrefs.GetInt("vit") + level;
            StatInit();
        }
        else
            StatInit();
    }

    public void LevelInit()
    {
        switch (gameObject.name)
        {
            case "Main":
                level = PlayerPrefs.GetInt("Mainlevel");
                break;
            case "Lily":

                break;
        }
        if (level == 0)
        {
            level = 1;
        }
    }

    public void StatInit()
    {
        atk = (float)str * 1.5f;
        matk = (float)(str + (intl * 1.5f)) / 2;
        def = (float)((vit * 1.5f) + str) / 2;
        mdef = (float)((vit * 1.5f) + intl) / 2;
        maxHp = (float)vit * 30;
        maxEnergy = (float)intl * 15;
        hp = maxHp;
        energy = maxEnergy;
    }

    public void SaveStat()
    {
        PlayerPrefs.SetInt("str", str - level);
        PlayerPrefs.SetInt("intl", intl - level);
        PlayerPrefs.SetInt("vit", vit - level);
    }

    public void AddStatToPlayer(string statName,int numberToAdd)
    {
        switch (statName)
        {
            case "str":
                str += numberToAdd;
                break;
            case "intl":
                intl += numberToAdd;
                break;
            case "vit":
                vit += numberToAdd;
                break;
            case "level":
                level += numberToAdd;
                break;
        }
        SaveStat();
        StatInit();
    }

    public void ResetStat()
    {
        str = 1;
        intl = 1;
        vit = 1;
        level = 1;
        SaveStat();
    }
}
