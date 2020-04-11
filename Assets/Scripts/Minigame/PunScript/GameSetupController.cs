using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameSetupController : MonoBehaviour
{
    public Transform[] spawnPoints;
    private int spawnPicker;
    // This script will be added to any multiplayer scene
    void Start()
    {
        spawnPicker = Random.Range(0, spawnPoints.Length);
        CreatePlayer(); //Create a networked player object for each player that loads into the multiplayer scenes.
    }
    private void CreatePlayer()
    {
        Debug.Log("Creating Player");
        PhotonNetwork.Instantiate(Path.Combine("Prefabs", "Minigame", "Descend", "DescendPlayer"), spawnPoints[spawnPicker].position, Quaternion.identity);
    }
}