using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TalkObject : MonoBehaviour
{
    [HideInInspector]
    public GameObject talkCharacter;
    private GameController gc;
    [SerializeField]
    private bool interacable = true, storyTalk = false;
    [SerializeField]
    private string interactText = "Talk";
    [SerializeField]
    private UnityEvent EventWhenTalk;

    private void Start()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        talkCharacter = gameObject.transform.parent.gameObject;
    }

    private void ActionPerform()
    {
        if (interacable)
        {
            Talk();
            EventWhenTalk.Invoke();
        }       
    }

    private void ChangeText(Text actionText)
    {
        if (interacable)
        {
            actionText.text = interactText;
        }
    }

    private void Talk()
    {
        if (talkCharacter != null)
        {
            gc.ChangeCameraToTalkObject(talkCharacter);
            gc.eve.talkCharacter = new GameObject[1];
            gc.eve.talkCharacter[0] = talkCharacter;
        }      
        gc.evc.someOneTalk = !storyTalk;
        gc.evc.PlayStory();
    }
}
