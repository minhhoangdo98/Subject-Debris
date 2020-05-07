using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using FIMSpace.FLook;
using UnityEngine.UI;

public class AllyController : MonoBehaviour
{
    public CharacterObject charObj;

    void Start()
    {
        charObj = gameObject.GetComponent<CharacterObject>();
        charObj.SetValuesStart();
    }

    void Update()
    {
        charObj.SetAnimatiorAndValuesUpdate();
    }

    private void ChangeText(Text actionText)
    {
        gameObject.transform.Find("TalkObject").SendMessage("ChangeText", actionText.GetComponentInChildren<Text>(), SendMessageOptions.DontRequireReceiver);
    }

    private void ActionPerform()
    {
        gameObject.transform.Find("TalkObject").SendMessage("ActionPerform", SendMessageOptions.DontRequireReceiver);
    }
}
