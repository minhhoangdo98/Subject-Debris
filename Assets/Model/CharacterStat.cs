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
}
