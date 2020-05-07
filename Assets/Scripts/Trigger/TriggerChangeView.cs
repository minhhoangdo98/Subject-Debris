using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerChangeView : MonoBehaviour
{
    private GameController gc;
    [SerializeField]
    [Header("Disable ButtonChangeView after change view")]
    private bool disableButton = true;

    private void Start()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    //Auto change view from 3d to 2d
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player3d"))
        {
            gc.touchButton.button3DView.SetActive(!disableButton);
            if (disableButton)
            {
                gc.viewObj.ChangeView();
                gc.viewObj.player2d.GetComponent<CharacterObject>().DisableChangeView();
            }
            else
                gc.viewObj.player2d.GetComponent<CharacterObject>().EnableChangeView();
        }  
    }

    //Enable button Change View when in 2d
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerCollider"))
        {
            gc.touchButton.button3DView.SetActive(!disableButton);
            if (disableButton)
                gc.viewObj.player2d.GetComponent<CharacterObject>().DisableChangeView();
            else
                gc.viewObj.player2d.GetComponent<CharacterObject>().EnableChangeView();
        }          
    }
}
