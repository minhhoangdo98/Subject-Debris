using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TeleportMenu : MonoBehaviour
{
    private GameController gc;
    [SerializeField]
    private InputField inputId1, inputId2, inputId3;
    [SerializeField]
    private Text textId1, textId2, textId3;
    [SerializeField]
    private string idInput, currentLocationId;

    private void Start()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        currentLocationId = textId1.text + textId2.text + textId3.text;
    }

    public void ButtonTeleport()
    {
        //dia chi id duoc nhap vao
        idInput = inputId1.text + inputId2.text + inputId3.text;
        //neu dia chi id la noi hien tai
        if (idInput == currentLocationId)
        {
            gc.HienThongBao("The destination is the current location!");
        }
        else
        {
            //kiem tra id input de thay doi scene tuong ung
            switch (idInput)
            {
                case "198307464"://Home
                    StartCoroutine(gc.LoadingAndChangeScene(2));
                    break;
                case "198307485"://stage1
                    StartCoroutine(gc.LoadingAndChangeScene(8));
                    break;
                default:
                    gc.HienThongBao("Invalid Location");
                    break;
            }
        }       
    }
}
