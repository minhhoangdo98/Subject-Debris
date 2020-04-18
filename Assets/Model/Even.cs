using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Even : MonoBehaviour
{
    public GameObject hoiThoaiPanel, talkPanel, luaChonPanel, manHinh, warnMessPanel;
    public Text ten, talk, luaChon1, luaChon2, messText;
    public int story, textNum, choiceNumAdd = 0;
    public bool enableNext = false;
    public GameObject[] talkCharacter;
}
