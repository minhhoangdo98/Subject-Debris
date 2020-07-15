using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PlayerDataOnline : MonoBehaviour
{
    public int str, vit, intl, gem;
    public string ten, email;
    public int story;

    public PlayerDataOnline() { }

    //Constructor Khoi tao data de luu
    public PlayerDataOnline(int strenght, int vita, int intlligent, int eGem, 
        string tenNV, int storyNum, string emails)
    {
        str = strenght;
        vit = vita;
        intl = intlligent;
        gem = eGem;
        ten = tenNV;
        story = storyNum;
        email = emails;
    }
}
