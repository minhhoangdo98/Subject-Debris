using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingMenu : MonoBehaviour
{
    public GameObject multiplayController;
    private GameController gc;
    public int gameScene;
    public string mode;

    private void Start()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    public void SetGame(int sceneId)
    {
        gameScene = sceneId;
    }

    public void SetMode(string modeName)
    {
        mode = modeName;
    }

    public void ButtonStart()
    {
        multiplayController.GetComponent<QuickStartRoomController>().multiplayerSceneIndex = gameScene;
        switch (mode)
        {
            case "Offline":
                multiplayController.GetComponent<QuickStartLobbyController>().online = false;
                break;
            case "Online":
                multiplayController.GetComponent<QuickStartLobbyController>().online = true;
                break;
        }
        StartCoroutine(StartTheTraining());
    }

    IEnumerator StartTheTraining()
    {
        StartCoroutine(gc.StartLoading());
        yield return new WaitForSeconds(1f);
        if (!multiplayController.GetComponent<QuickStartLobbyController>().online)
        {
            PhotonNetwork.Disconnect();
            yield return new WaitForSeconds(0.5f);
            PhotonNetwork.OfflineMode = true;
        }
        multiplayController.GetComponent<QuickStartLobbyController>().QuickStart();
    }
}
