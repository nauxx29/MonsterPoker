using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LobbyManagerScript : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject createRoomPanel;
    [SerializeField] GameObject roomPanel;
    [Space]
    [SerializeField] GameObject startButton;
    [Space]
    [SerializeField] TMP_Text roomName;
    [Space]
    [SerializeField] Transform playerItemParent;
    [SerializeField] Transform contentObject;
    [SerializeField] PlayerItem playerItemPrefab;
    [Space]
    [SerializeField] RoomItem roomItemPrefab;

    [Space]
    [SerializeField] GameObject allFullHint;


    List<RoomItem> roomItemsList = new List<RoomItem>();
    List<PlayerItem> playerItemsList = new List<PlayerItem>();
    Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>(); // roomName = Key, RoomInfo = Value

    public override void OnEnable()
    {
        base.OnEnable();
        if (PhotonNetwork.LocalPlayer.CustomProperties["ReturningFromGame"] == null)
            return;
        else if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["ReturningFromGame"] == true)
        {
            PhotonNetwork.CurrentRoom.IsOpen = true;
            PhotonNetwork.CurrentRoom.IsVisible = true;
            OnJoinedRoom();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
        PhotonNetwork.JoinLobby();
        
    }
    public override void OnJoinedLobby()
    {
        Debug.Log("Lobby");
        cachedRoomList.Clear();
    }

    // 1. When rejoin the lobby you will get a whole roomList
    // 2. If player stay in lobby, player will only get the update roomList info
    // If **roomList[i].RemovedFromList** represent room been remove (it will still on the list so be sure to check this statement)
    // Or simplely show the new name on the roomList, that represent a new room been created.

    // We use cacheRoomList to update the info got from roomList
    public override void OnRoomListUpdate(List<RoomInfo> roomList) 
    {
        UpdateCachedRoomList(roomList);
        UpdateRoomListItem();
    }
    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            RoomInfo info = roomList[i];
            if (info.RemovedFromList)
            {
                cachedRoomList.Remove(info.Name);
            }
            else
            {
                cachedRoomList[info.Name] = info;
            }
        }
    }
    void UpdateRoomListItem()
    {
        foreach (RoomItem item in roomItemsList)
        {
            Destroy(item.gameObject);
        }
        roomItemsList.Clear();

        foreach (RoomInfo roomInfo in cachedRoomList.Values)
        {
            RoomItem newRoom = Instantiate(roomItemPrefab, contentObject);
            newRoom.SetRoomName(roomInfo.Name, roomInfo.PlayerCount.ToString());
            roomItemsList.Add(newRoom);
        }
        allFullHint.SetActive(false);
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinRandomFailed (short returnCode,string message)	// NEW
    {
        Debug.Log(message);
        allFullHint.SetActive(true);
    }

    public override void OnJoinedRoom() // Callback to **localPlayer** joined the room
    {
        createRoomPanel.SetActive(false);
        roomPanel.SetActive(true);
        roomName.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name;
        UpdatePlayerList(); // whe someone join room renew the player list
        
        Hashtable initialProperties = new Hashtable() // default CustomProperties value
        {
            { "playerAvatar", 0 },
            { "GameTime", -1f },
            { "GameAnswer", "defult"},
            { "GameTimeForDisplay", "defult"},
            { "GameWin", "defult" },
            { "Score", 0 },
            { "ReturningFromGame", false }
        };
        PhotonNetwork.SetPlayerCustomProperties(initialProperties);
    }
    public override void OnPlayerEnteredRoom(Player newPlayer) // Callback to all the player that is **already in the room** when newPlayer enter the room
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) //  Callback to all the player that is **already in the room** when newPlayer leaveing the room
    {
        UpdatePlayerList();
    }

    void UpdatePlayerList()
    {
        playerItemsList.RemoveAll(item => item == null || item.gameObject == null);

        foreach (PlayerItem item in playerItemsList) // List to put PlayerItem ( the class we create)
        {
            Destroy(item.gameObject); // clear playerItem object tht represent playe datar
        }
        playerItemsList.Clear(); // clear the list which store all the pointer

        if (PhotonNetwork.CurrentRoom == null) // Check if player is in the room
        {
            return;
        }

        // PhotonNetwork.CurrentRoom.Players will return a dict, id is the "key", player data is the "value"
        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            PlayerItem newPlayerItem = Instantiate(playerItemPrefab, playerItemParent);
            newPlayerItem.SetPlayerInfo(player.Value); // player.Player (put Player class in )

            if (player.Value == PhotonNetwork.LocalPlayer)
            {
                newPlayerItem.ApplyLocalChaneges(); // In playerItem script we setup to highlight the player
            }
            playerItemsList.Add(newPlayerItem); // Update Pointer to list, so we can use it to Destroy() later
        }
    }

    public override void OnLeftRoom()
    {
        cachedRoomList.Clear();
        roomPanel.SetActive(false); // null but dont know why
        createRoomPanel.SetActive(true);
    }

    private void Update()
    {
        if (startButton == null) return;
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            startButton.SetActive(true);
        }
        else
        {
            startButton.SetActive(false);
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        cachedRoomList.Clear();
        Debug.Log("Local Player Disconnect");
    }

}
