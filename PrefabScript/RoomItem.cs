using UnityEngine;
using TMPro;

// Attach on RoomItem Prefab

public class RoomItem : MonoBehaviour
{
    [SerializeField] TMP_Text roomName;
    [SerializeField] TMP_Text playerNumber;
    LobbyManagerScript manager;

    private void Start()
    {
        manager = FindObjectOfType<LobbyManagerScript>();
    }

    public void SetRoomName(string _roomName, string _playerNumber)
    { 
        roomName.text = _roomName;
        playerNumber.text = _playerNumber + " / 2";
    }

    public void OnClickItem()
    {
        manager.JoinRoom(roomName.text);
    }
}
