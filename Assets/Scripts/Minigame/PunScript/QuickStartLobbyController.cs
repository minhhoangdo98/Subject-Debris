using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickStartLobbyController : MonoBehaviourPunCallbacks
{
    public static QuickStartLobbyController lobby;
    [SerializeField]
    private int RoomSize; //Manual set the number of player in the room at one time.
    public Button buttonOnline;
    public bool online = false;

    private void Awake()
    {
        lobby = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.Disconnect();
        PhotonNetwork.ConnectUsingSettings();//Connect to master Photon server
    }


    public override void OnConnectedToMaster() //Callback function for when the first connection is established successfully.
    {
        Debug.Log("OnConnectedToMaster");
        buttonOnline.interactable = true;
        online = true;
        PhotonNetwork.AutomaticallySyncScene = true; //Makes it so whatever scene the master client has loaded is the scene all other clients will load
    }

    public void QuickStart() //Paired to the Quick Start button
    {
        PhotonNetwork.JoinRandomRoom(); //First tries to join an existing room
        Debug.Log("Quick start");
    }
    public override void OnJoinRandomFailed(short returnCode, string message) //Callback function for if we fail to join a rooom
    {
        Debug.Log("Failed to join a room");
        CreateRoom();
    }
    void CreateRoom() //trying to create our own room
    {
        Debug.Log("Creating room now");
        int randomRoomNumber = Random.Range(0, 10000); //creating a random name for the room
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)RoomSize };
        PhotonNetwork.CreateRoom("Room" + randomRoomNumber, roomOps); //attempting to create a new room
        Debug.Log(randomRoomNumber);
    }
    public override void OnCreateRoomFailed(short returnCode, string message) //callback function for if we fail to create a room. Most likely fail because room name was taken.
    {
        Debug.Log("Failed to create room... trying again");
        CreateRoom(); //Retrying to create a new room with a different name.
    }
    public void QuickCancel() //Paired to the cancel button. Used to stop looking for a room to join.
    {
        PhotonNetwork.LeaveRoom();
    }
}