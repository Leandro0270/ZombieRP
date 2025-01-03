using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnlineGameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] OnlineMenuManager onlineMenuManager;
    private string LocalplayerNickname {set;get;}
    private string RoomCode {set;get;}
    private TypedLobby menuLobby = new TypedLobby("menu", LobbyType.Default);
    private TypedLobby inGameLobby = new TypedLobby("in-game", LobbyType.Default);

    public void connectDirectRoom()
    {
        if(!PhotonNetwork.IsConnected)
            connectToPhotonServer();
        ConnectOrCreateRoom();
    }
    
    private void connectToPhotonServer()
    {
        PhotonNetwork.LocalPlayer.NickName = LocalplayerNickname;
        PhotonNetwork.ConnectUsingSettings();
    }
    
    private void listAllRooms()
    {
        PhotonNetwork.JoinLobby(menuLobby);
        PhotonNetwork.GetCustomRoomList(menuLobby, "C0 = true AND C1 = true");
    }
    
    private void ConnectOrCreateRoom()
    {
        
        if(PhotonNetwork.InLobby)
            PhotonNetwork.LeaveLobby();
        else
        {
            PhotonNetwork.JoinLobby(inGameLobby);
        }

    }
    
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(inGameLobby);
    }
    public override void OnJoinedLobby()
    {
        if(PhotonNetwork.CurrentLobby.Equals(menuLobby))
        {
            // Código para quando se junta ao lobby do menu
        }
        else if(PhotonNetwork.CurrentLobby.Equals(inGameLobby))
        {
            onlineMenuManager.setText("Entrando na sala...");
            var roomOptions = new RoomOptions 
            {
                IsVisible = true, 
                IsOpen = true, 
                MaxPlayers = 4,
                CustomRoomProperties = new ExitGames.Client.Photon.Hashtable()
                {
                    { "C0", true }, // IsVisible
                    { "C1", true }  // IsOpen
                },
                CustomRoomPropertiesForLobby = new string[] { "C0", "C1" }
            };
            PhotonNetwork.JoinOrCreateRoom(RoomCode, roomOptions, inGameLobby);
            SceneManager.LoadScene("PlayerSetupOnline");
        }
    }
    
    

    public void setLocalPlayerNickname(string nickname)
    {
        LocalplayerNickname = nickname;
    }
    
    public void setConnectedRoomCode(string roomCode)
    {
        RoomCode = roomCode;
    }
}
