using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TalkObject : MonoBehaviour
{
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
        gc.evc.someOneTalk = !storyTalk;
        gc.evc.PlayStory();
    }
}
